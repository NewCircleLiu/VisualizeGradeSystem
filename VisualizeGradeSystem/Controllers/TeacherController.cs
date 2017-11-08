using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VisualizeGradeSystem.Utils.filters;
using VisualizeGradeSystem.Models.User;
using VisualizeGradeSystem.Utils;
using VisualizeGradeSystem.Models.Score;
using VisualizeGradeSystem.Models.Puzzle;
using VisualizeGradeSystem.Models.Knowledge;

namespace VisualizeGradeSystem.Controllers
{
    public class TeacherController : Controller
    {
        //
        // GET: /Teacher/
        private ScoreContext sc = new ScoreContext();
        private PuzzleContext pc = new PuzzleContext();
        private KnowledgeContext kc = new KnowledgeContext();
        private static string filePath;
        //首页
        public ActionResult Index()
        {
            User u = (User)Session["User"];
            if (u != null)
            {
                ViewBag.account = u.user_account;
            }
            return View();
        }
        //知识点展示页面
        [UserAuthorize]
        public ActionResult KnowledgePage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            return View();
        }
        //上传成绩单页面
        [UserAuthorize]
        public ActionResult UploadFilesPage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            return View();
        }
        //选择科目页面
        public ActionResult ChooseSubjectPage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            return View();
        }
        //对于每个题，选择知识点的页面
        public ActionResult ChooseKnowledgePage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            return View(pc.PuzzleList.ToList());
        }
        //全校成绩页面
        [UserAuthorize]
        public ActionResult GradeSchollChartPage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            return View();
        }
        [HttpPost]
        public JsonResult GetKnowledge_X(string subject, string Class, string knowledge)
        {
            User u = (User)Session["User"];
            Knowledge[] kList;
            List<double> scoreList = new List<double>();
            if (u.user_kind == "admin")
            {
                kList = kc.KnowledgeList.Where(k => k.subject == subject && k.stu_class == Class && k.knowledge_name == knowledge).ToArray();
            }
            else
            {
                kList = kc.KnowledgeList.Where(k => k.subject == subject && k.stu_class == Class && k.knowledge_name == knowledge && k.uploader == u.user_account).ToArray();
            }
            List<string> time = new List<string>();
            for (int i = 0; i < kList.Length; i++)
            {
                if (!time.Contains(kList[i].uploadtime))
                {
                    time.Add(kList[i].uploadtime); //有多少次考试
                }

            }
            List<string> x = new List<string>();
            int j=1;
            foreach (string t in time)
            {
                x.Add("第" + j + "次考试");
                j++;
            }
            return Json(x);
        }
        [HttpPost]
        //某个班级对于某个知识点掌握的情况
        public JsonResult GetKnowledge(string subject,string Class,string knowledge)
        {
            User u = (User)Session["User"];
            Knowledge[] kList;
            List<double> scoreList = new List<double>();
            if (u.user_kind == "admin")
            {
                kList = kc.KnowledgeList.Where(k => k.subject == subject && k.stu_class == Class && k.knowledge_name==knowledge).ToArray();
            }
            else
            {
                kList = kc.KnowledgeList.Where(k => k.subject == subject && k.stu_class == Class && k.knowledge_name == knowledge && k.uploader == u.user_account).ToArray();
            }
            List<string> time = new List<string>();
            for (int i = 0; i < kList.Length; i++)
            {
                if (!time.Contains(kList[i].uploadtime))
                {
                    time.Add(kList[i].uploadtime); //有多少次考试
                }

            }
            foreach (string t in time)
            {
                double temp = 0.0;
                int l = 0;
                for (int i = 0; i < kList.Length; i++)
                {
                    if (kList[i].uploadtime.Equals(t))
                    {
                        temp += kList[i].score;
                        l++;
                    }
                }
                if (l != 0)
                {
                    temp = temp / l;
                    scoreList.Add(Math.Round(temp, 2));
                }
                
            }
            return Json(scoreList);
        }
        [HttpPost]
        public ActionResult ChooseKnowledge_Access()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            Puzzle[] pList = pc.PuzzleList.Where(p => p.uploadtime==today).ToArray();
            Knowledge[] kList;
            User u = (User)Session["User"];
            ReadExcel re = new ReadExcel();
            for (int i = 0; i < pList.Length; i++)
            {
                Puzzle p = pList[i];
                string knowledge = (string)Request.Form[(string)("knowledge1st-"+i)]+"-"+(string)Request.Form[(string)("knowledge2nd-"+i)];
                p.puzzle_knowledge = knowledge;
                p.uploader = u.user_account;
                pc.PuzzleList.Attach(p);
                pc.Entry(p).State = System.Data.EntityState.Modified;
            }
            pc.SaveChanges();
            kList = re.readKowledge(filePath, pList.Length, pList);
            for (int i = 0; i < kList.Length; i++)
            {
                Knowledge k = kList[i];
                Score s = sc.ScoreList.Where(score => score.stu_id == k.stu_id).First();
                k.stu_class = s.stu_class;
                k.uploader = u.user_account;
                kc.KnowledgeList.Add(k);
                //kc.Entry(k).State = System.Data.EntityState.Modified;
            }
            kc.SaveChanges();
            return RedirectToAction("Index", "Teacher");
        }
        //获得全校成绩：1.如果是admin，那么全校的都可以
        //2.否则，只能看自己上传的
        [HttpPost]
        public JsonResult GradeSchollChartPage(string subject, string depart, string Class)
        {
            User u = (User)Session["User"];
            Score[] list;
            if (u.user_kind == "admin")
            {
                list = sc.ScoreList.Where(s => s.subject == subject && s.stu_depart == depart && s.stu_class == Class).ToArray();
            }
            else
            {
                list = sc.ScoreList.Where(s => s.subject == subject && s.stu_depart == depart && s.stu_class == Class && s.uploader==u.user_account).ToArray();
            }
            List<string> time = new List<string>();
            for (int i = 0; i < list.Length; i++)
            {
                if (!time.Contains(list[i].uploadtime))
                {
                    time.Add(list[i].uploadtime); //有多少次考试
                }

            }
            List<double> scoreList = new List<double>();
            foreach (string t in time)
            {
                double temp = 0.0;
                int l = 0;
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].uploadtime.Equals(t))
                    {
                        temp += list[i].score;
                        l++;
                    }
                }
                temp = temp / l;
                scoreList.Add(Math.Round(temp, 2));
            }
            return Json(scoreList);
        }
        //获得全校成绩的x轴：1.如果是admin，那么全校的都可以2.否则，只能看自己上传的
        [HttpPost]
        public JsonResult GradeSchollChartPaget_X(string subject, string depart, string Class)
        {
            User u = (User)Session["User"];
            Score[] list;
            if (u.user_kind == "admin")
            {
                list = sc.ScoreList.Where(s => s.subject == subject && s.stu_depart == depart && s.stu_class == Class).ToArray();
            }
            else
            {
                list = sc.ScoreList.Where(s => s.subject == subject && s.stu_depart == depart && s.stu_class == Class && s.uploader==u.user_account).ToArray();
            }
            List<string> time = new List<string>();
            for (int i = 0; i < list.Length; i++)
            {
                if (!time.Contains(list[i].uploadtime))
                {
                    time.Add(list[i].uploadtime); //有多少次考试
                }
            }
            List<string> x = new List<string>();
            int j = 1;
            foreach (string t in time)
            {
                x.Add("第" + j + "次考试");
                j++;
            }
            return Json(x);
        }
        //更新这张成绩单是哪门课
        [HttpPost]
        public ActionResult ChooseSubject(string subject)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            Score[] array = sc.ScoreList.Where(s => s.uploadtime.Contains(today)).ToArray();
            Puzzle[] pArray = pc.PuzzleList.Where(p => p.uploadtime==today).ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                Score s = array[i];
                s.subject = subject;
                sc.ScoreList.Attach(s);
                sc.Entry(s).State = System.Data.EntityState.Modified;
            }
            sc.SaveChanges();
            for (int i = 0; i < pArray.Length; i++)
            {
                Puzzle p = pArray[i];
                p.subject = subject;
                pc.PuzzleList.Attach(p);
                pc.Entry(p).State = System.Data.EntityState.Modified;
            }
            pc.SaveChanges();
            return RedirectToAction("ChooseKnowledgePage", "Teacher");
        }
        //上传成绩单文件
        [HttpPost]
        public ActionResult UploadFiles()
        {
            HttpPostedFileBase file = Request.Files[0];
            string fileName = file.FileName;
            string saveUrl = Server.MapPath("/") + "UploadFiles/GradeExcel/";
            Uploader up = new Uploader();
            ReadExcel re = new ReadExcel();
            up.UploadExcel(file, saveUrl);//上传文件到服务器上
            filePath = saveUrl + file.FileName;
            Score[] scoreArray = re.Read(filePath); //读取成绩
            Puzzle[] puzzleArray = re.readPuzzle(filePath);//读取题型
            User u = (User)Session["User"];

            for (int i = 0; i < scoreArray.Length; i++)
            {
                scoreArray[i].uploadtime = DateTime.Now.ToString("yyyy-MM-dd");
                scoreArray[i].uploader = u.user_account;
                sc.ScoreList.Add(scoreArray[i]);
            }
            sc.SaveChanges();
            for (int i = 0; i < puzzleArray.Length; i++)
            {
                puzzleArray[i].uploader = u.user_account;
                pc.PuzzleList.Add(puzzleArray[i]);
            }
            pc.SaveChanges();
            return RedirectToAction("ChooseSubjectPage", "Teacher");
        }
        //登出
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("", "Student");
        }

    }
}

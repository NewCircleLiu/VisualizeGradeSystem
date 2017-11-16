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
        private UserContext uc = new UserContext();

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
            Score[] scoreList = sc.ScoreList.Where(s => s.uploader == u.user_account).ToArray();
            List<string> time = new List<string>();
            for (int i = 0; i < scoreList.Length; i++)
            {
                if (!time.Contains(scoreList[i].uploadtime))
                {
                    time.Add(scoreList[i].uploadtime); //有多少次考试
                }
            }
            return View(time);
        }
        [HttpPost]
        public ActionResult DeleteLog(string time) //把上传的某一次删除
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            Score[] sList = sc.ScoreList.Where(s => s.uploader == u.user_account && s.uploadtime == time).ToArray();
            Puzzle[] pList = pc.PuzzleList.Where(p => p.uploader == u.user_account && p.uploadtime == time).ToArray();
            Knowledge[] kList = kc.KnowledgeList.Where(k => k.uploader == u.user_account && k.uploadtime == time).ToArray();
            for (int i = 0; i < sList.Length; i++)
            {
                if (ModelState.IsValid)
                {
                    sc.ScoreList.Remove(sList[i]);
                    sc.SaveChanges();
                }
                else
                {
                    return Content("error");
                }
            }
            for (int i = 0; i < kList.Length; i++)
            {
                if (ModelState.IsValid)
                {
                    kc.KnowledgeList.Remove(kList[i]);
                    kc.SaveChanges();
                }
                else
                {
                    return Content("error");
                }
            }
            for (int i = 0; i < pList.Length; i++)
            {
                if (ModelState.IsValid)
                {
                    pc.PuzzleList.Remove(pList[i]);
                    pc.SaveChanges();
                }
                else
                {
                    return Content("error");
                }
            }
            return Content("ok");
        }
        public ActionResult ModifyPassword()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            return View();
        }
        [HttpPost]
        public ActionResult Modify(string old_pass, string new_pass)
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            User[] exsit = uc.UserList.Where(user => user.user_account == u.user_account && user.user_password == old_pass).ToArray();
            if (exsit.Length == 0)
            {
                return Content("error");
            }
            else
            {
                User temp = exsit[0];
                temp.user_password = new_pass;
                uc.UserList.Attach(temp);
                uc.Entry(temp).State = System.Data.EntityState.Modified;
                uc.SaveChanges();
                FormsAuthentication.SignOut();
                Session.Clear();
                return RedirectToAction("Index", "Student");
            }
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
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.account = u.user_account;
            return View(pc.PuzzleList.Where(p=> p.uploadtime==today).ToList());
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
        public JsonResult GetKnowledge_X(string subject, string Class, string knowledge, string year)
        {
            User u = (User)Session["User"];
            Knowledge[] kList;
            List<double> scoreList = new List<double>();
            if (u.user_kind == "admin")
            {
                kList = kc.KnowledgeList.Where(k => k.subject == subject && k.stu_class == Class && k.knowledge_name == knowledge && k.uploadtime.StartsWith(year)).ToArray();
            }
            else
            {
                kList = kc.KnowledgeList.Where(k => k.subject == subject && k.stu_class == Class && k.knowledge_name == knowledge && k.uploader == u.user_account && k.uploadtime.StartsWith(year)).ToArray();
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
        public JsonResult GetKnowledge(string subject,string Class,string knowledge, string year)
        {
            User u = (User)Session["User"];
            Knowledge[] kList;
            List<double> scoreList = new List<double>();
            if (u.user_kind == "admin")
            {
                kList = kc.KnowledgeList.Where(k => k.subject == subject && k.stu_class == Class && k.knowledge_name==knowledge && k.uploadtime.StartsWith(year)).ToArray();
            }
            else
            {
                kList = kc.KnowledgeList.Where(k => k.subject == subject && k.stu_class == Class && k.knowledge_name == knowledge && k.uploader == u.user_account && k.uploadtime.StartsWith(year)).ToArray();
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
        public JsonResult GradeSchollChartPage(string subject, string depart, string Class, string year)
        {
            User u = (User)Session["User"];
            Score[] list;
            if (u.user_kind == "admin")
            {
                list = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.uploadtime.StartsWith(year)).ToArray();
            }
            else
            {
                list = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.uploader==u.user_account && s.uploadtime.StartsWith(year)).ToArray();
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
        public JsonResult GradeSchollChartPaget_X(string subject, string depart, string Class, string year)
        {
            User u = (User)Session["User"];
            Score[] list;
            if (u.user_kind == "admin")
            {
                list = sc.ScoreList.Where(s => s.subject == subject  && s.stu_class == Class && s.uploadtime.StartsWith(year)).ToArray();
            }
            else
            {
                list = sc.ScoreList.Where(s => s.subject == subject  && s.stu_class == Class && s.uploader==u.user_account && s.uploadtime.StartsWith(year)).ToArray();
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
            if (file.FileName != "")
            {
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
            else
            {
                return RedirectToAction("UploadFilesPage", "Teacher", new { errorMsg = "未选择文件上传" });
            }
        }
        public ActionResult Guide()
        {
            if (Session["User"] != null)
            {
                User u = (User)Session["User"];
                ViewBag.account = u.user_account;
            }
            return View();
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

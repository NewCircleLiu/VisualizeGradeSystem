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
using VisualizeGradeSystem.Models.Class;


namespace VisualizeGradeSystem.Controllers
{
    public class HeadController : Controller
    {
        //
        // GET: /Head/
        //课题组负责人，负责上传成绩，也可看所有班级成绩
        private ScoreContext sc = new ScoreContext();
        private PuzzleContext pc = new PuzzleContext();
        private KnowledgeContext kc = new KnowledgeContext();
        private UserContext uc = new UserContext();
        private ClassContext cc = new ClassContext();
        private static string filePath;
        private static string time;

        public ActionResult Index()
        {
            return View();
        }
        //选择这是第几次考试页面
        public ActionResult ChooseWhichTimePage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            return View();
        }
        //选择这是第几次考试
        [HttpPost]
        public ActionResult ChooseWhichTime(string whichTime)
        {
            time = whichTime;
            Score[] scoreList = sc.ScoreList.Where(s=> s.uploadtime == whichTime).ToArray();
            if (scoreList.Length != 0)
            {
                return RedirectToAction("ChooseWhichTimePage", "Head", new { errorMsg = "此次考试已经上传" });
            }
            else
            {
                return RedirectToAction("UploadFilesPage", "Head");
            }
        }
        //上传成绩的页面
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
        //把上传的某一次删除，不留空
        [HttpPost]
        public ActionResult DeleteLog_withoutspace(string time) 
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
            sList = sc.ScoreList.ToArray();
            int max_index = Convert.ToInt32(time);
            for (int i = 0; i < sList.Length; i++)
            {
                if (Convert.ToInt32(sList[i].updatetime) > max_index) { 
                    max_index=(Convert.ToInt32(sList[i].updatetime));
                }
            }
            for(int i=Convert.ToInt32(time)+1;i<=max_index;i++)
            {
                string temp=i.ToString();
                sList = sc.ScoreList.Where(s=>s.updatetime==temp).ToArray();
                for(int j=0;j<sList.Length;j++)
                {
                    Score s = sList[j];
                    s.updatetime=(i-1).ToString();
                    sc.ScoreList.Attach(s);
                    sc.Entry(s).State = System.Data.EntityState.Modified;
                }
                sc.SaveChanges();
            }
            return Content("ok");
        }
        //删除上传记录,把那一次空下来
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
                    scoreArray[i].uploadtime = time;
                    scoreArray[i].updatetime = DateTime.Now.ToString("yyyy-MM-dd");
                    scoreArray[i].uploader = u.user_account;
                    sc.ScoreList.Add(scoreArray[i]);
                }
                sc.SaveChanges();
                for (int i = 0; i < puzzleArray.Length; i++)
                {
                    puzzleArray[i].uploader = u.user_account;
                    puzzleArray[i].uploadtime = time;
                    puzzleArray[i].updatetime = DateTime.Now.ToString("yyyy-MM-dd");
                    pc.PuzzleList.Add(puzzleArray[i]);
                }
                pc.SaveChanges();
                return RedirectToAction("ChooseSubjectPage", "Head");
            }
            else
            {
                return RedirectToAction("UploadFilesPage", "Head", new { errorMsg = "未选择文件上传" });
            }
        }
        //选择科目页面
        public ActionResult ChooseSubjectPage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            return View();
        }
        //更新这张成绩单是哪门课
        [HttpPost]
        public ActionResult ChooseSubject(string subject)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            Score[] array = sc.ScoreList.Where(s => s.uploadtime==time).ToArray();
            Puzzle[] pArray = pc.PuzzleList.Where(p => p.uploadtime == time).ToArray();
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
            if (subject == "C")
            {
                return RedirectToAction("ChooseKnowledgePage_C", "Head");
            }
            else 
            {
                return RedirectToAction("ChooseKnowledgePage", "Head");
            }
        }
        //Access对于每个题，选择知识点的页面
        public ActionResult ChooseKnowledgePage(string time)
        {
            User u = (User)Session["User"];
            //string today = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.account = u.user_account;
            if (time == null)
            {
                return View(pc.PuzzleList.Where(p => p.uploadtime == time).ToList());
            }
            else {
                return View(pc.PuzzleList.Where(p => p.uploadtime == time && p.uploadtime == time).ToList());
            }
        }
        //C语言选择知识点的页面
        public ActionResult ChooseKnowledgePage_C()
        {
            User u = (User)Session["User"];
            //string today = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.account = u.user_account;
            return View(pc.PuzzleList.Where(p => p.uploadtime == time).ToList());
        }
        //更新知识点的表
        [HttpPost]
        public ActionResult ChooseKnowledge()
        {
            string today = time;
            Puzzle[] pList = pc.PuzzleList.Where(p => p.uploadtime == today).ToArray();
            Knowledge[] kList;
            User u = (User)Session["User"];
            ReadExcel re = new ReadExcel();
            for (int i = 0; i < pList.Length; i++)
            {
                Puzzle p = pList[i];
                string knowledge = (string)Request.Form[(string)("knowledge1st-" + i)] + "-" + (string)Request.Form[(string)("knowledge2nd-" + i)];
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
                Score s = sc.ScoreList.Where(score => score.stu_id == k.stu_id).FirstOrDefault();
                if (s != null)
                {
                    k.stu_class = s.stu_class;
                    k.uploader = u.user_account;
                    kc.KnowledgeList.Add(k);
                }
                //kc.Entry(k).State = System.Data.EntityState.Modified;
            }
            kc.SaveChanges();
            return RedirectToAction("Index", "Head");
        }
        //管理自己科目下面的老师所带的班级
        public ActionResult ManageTeacherPage()
        {
            User u = (User)Session["User"];
            User[] userList = uc.UserList.Where(user => user.user_subject == u.user_subject).ToArray();
            return View(userList);
        }
        [HttpPost]
        public ActionResult EditClass(int id, string newClass)
        {
            User u = uc.UserList.Where(user => user.user_id == id).First();
            u.user_class = newClass;
            uc.UserList.Attach(u);
            uc.Entry(u).State = System.Data.EntityState.Modified;
            uc.SaveChanges();
            return RedirectToAction("ManageTeacherPage", "Admin");
        }
        public ActionResult SchoolGradePage()
        {
            User u = (User)Session["User"];
            Score[] scoreList = sc.ScoreList.Where(s => s.subject==u.user_subject).ToArray();
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
        public ActionResult SchoolKnowledgePage()
        {
            return View();
        }
        public ActionResult ManageClassPage()
        {
            Class[] cList = cc.ClassList.ToArray();
            return View(cList);
        }
        public ActionResult DeleteClass(int id, string newName, string newDepart)
        {
            Class C = cc.ClassList.Where(c => c.class_id == id).FirstOrDefault();
            C.class_name = newName;
            C.class_depart = newDepart;
            cc.ClassList.Attach(C);
            cc.Entry(C).State = System.Data.EntityState.Modified;
            cc.SaveChanges();
            return RedirectToAction("ManageClassPage", "Head");
        }
        public ActionResult DeleteClass(int id)
        {
            Class C = cc.ClassList.Where(c => c.class_id == id).FirstOrDefault();
            cc.ClassList.Remove(C);
            cc.SaveChanges();
            return Content("ok");
        }
        //自己所带课题组知识点页面
        [HttpGet]
        public ActionResult GetStuKnowledgePage(string stu_id, string time, string subject)
        {
            ViewBag.stu_id = stu_id;
            return View();
        }
        //自己所带课题组成绩页面
        [HttpGet]
        public ActionResult GetStuGradePage(string stu_id, string subject)
        {
            ViewBag.stu_id = stu_id;
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

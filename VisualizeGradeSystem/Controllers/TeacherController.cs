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
        public ActionResult SchoolKnowledgePage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            return View();
        }
        //改密码页面
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
        //自己所带班级知识点页面
        [HttpGet]
        public ActionResult GetStuKnowledgePage(string stu_id, string time, string subject)
        {
            ViewBag.stu_id = stu_id;
            return View();
        }
        //自己所带班级成绩页面
        [UserAuthorize]
        public ActionResult SchoolGradePage()
        {

            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            Score[] scoreList = sc.ScoreList.Where(s=>u.user_class.Contains(s.stu_class) && u.user_subject==s.subject).ToArray();
            List<string> time = new List<string>();
            for (int i = 0; i < scoreList.Length; i++)
            {
                if (!time.Contains(scoreList[i].uploadtime))
                {
                    time.Add(scoreList[i].uploadtime); //有多少次考试
                }
            }
            return View();
        }
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

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
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        //管理员：负责管理教师账号、数据的年份进行管理
        private ScoreContext sc = new ScoreContext();
        private PuzzleContext pc = new PuzzleContext();
        private KnowledgeContext kc = new KnowledgeContext();
        private UserContext uc = new UserContext();

        public ActionResult Index()
        {
            return View();
        }
        //添加账号页面
        public ActionResult AddUserPage()
        {
            User u = (User)Session["User"];
            return View();
        }
        //添加账号
        [HttpPost]
        public ActionResult AddUser(string account, string kind, string name, string depart, string Class, string subject)
        {
            User u = (User)Session["User"];
            User user = new User();
            user.user_account = account;
            user.user_depart = depart;
            user.user_class = Class;
            user.user_kind = kind;
            user.user_password = md5_pass.GetMD5(account);
            user.user_subject = subject;
            uc.UserList.Add(user);
            uc.SaveChanges();
            return RedirectToAction("AddUserPage", "Admin");
        }
        [HttpGet]
        //管理账号
        public ActionResult ManageUserPage(int Page = 1,string kind = "all")
        {
            User user = (User)Session["User"];
            Session["kind"] = kind;
            Paging p = new Paging();
            IEnumerable<User> List = uc.UserList;
            if (kind.Equals("teacher"))
            {
                List = uc.UserList.Where(u => u.user_kind == "teacher" || u.user_kind == "head");
            }
            if (kind.Equals("stu"))
            {
                List = uc.UserList.Where(u => u.user_kind == "stu");
            }
            //TempData["userCount"] = List.Count();
            ViewBag.searchAction = "/Admin/ManageUserPage?Page=";
            p.GetCurrentPageData(List, Page);
            return View(p);
        }

        [HttpPost]
        public ActionResult SetHead(int id)
        {
            User u = uc.UserList.Where(user => user.user_id == id).First();
            u.user_kind = "head";
            uc.UserList.Attach(u);
            uc.Entry(u).State = System.Data.EntityState.Modified;
            uc.SaveChanges();
            return RedirectToAction("ManageUserPage", "Admin");
        }
        [HttpPost]
        public ActionResult CancelHead(int id)
        {
            User u = uc.UserList.Where(user => user.user_id == id).First();
            u.user_kind = "teacher";
            uc.UserList.Attach(u);
            uc.Entry(u).State = System.Data.EntityState.Modified;
            uc.SaveChanges();
            return RedirectToAction("ManageUserPage", "Admin");
        }
        [HttpPost]
        public ActionResult EditUser(int id,string newName, string newDepart, string newClass)
        {
            User user = uc.UserList.Where(u => u.user_id == id).FirstOrDefault();
            user.user_name = newName;
            user.user_depart = newDepart;
            user.user_class = newClass;
            uc.UserList.Attach(user);
            uc.Entry(user).State = System.Data.EntityState.Modified;
            uc.SaveChanges();
            return RedirectToAction("ManageUserPage", "Admin");
        }
        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            User user = uc.UserList.Where(u => u.user_id == id).First();
            uc.UserList.Remove(user);
            uc.SaveChanges();
            return RedirectToAction("ManageUserPage", "Admin");
        }
        public ActionResult SchoolGradePage()
        {
            Score[] scoreList = sc.ScoreList.ToArray();
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
        //全校知识点页面
        [HttpGet]
        public ActionResult GetStuKnowledgePage(string stu_id, string time, string subject)
        {
            ViewBag.stu_id = stu_id;
            return View();
        }
        //全校成绩页面
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VisualizeGradeSystem.Utils.filters;
using VisualizeGradeSystem.Models.User;
using System.Web.Security;
using VisualizeGradeSystem.Models.Score;

namespace VisualizeGradeSystem.Controllers
{
    public class StudentController : Controller
    {
        //
        // GET: /Student/
        ScoreContext sc = new ScoreContext();
        public ActionResult Index()
        {
            User u = (User)Session["User"];
            if (u != null)
            {
                ViewBag.account = u.user_account;
                ViewBag.name = u.user_name;
            }
            return View();
        }
        [UserAuthorize]
        public ActionResult PersonalLinePage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            return View();
        }
        //获得当前学号所在班级的成绩变化曲线
        [UserAuthorize]
        public ActionResult GradeClassChartPage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            ViewBag.userclass=u.user_class;
            return View();
        }
        //获得当前学号成绩
        [HttpPost]
        public JsonResult PersonalLine(string subject)
        {
            if (subject == null)
            {

                subject = "Access";
            }
            User u = (User)Session["User"];
            Score[] list = sc.ScoreList.Where(s => s.stu_id.Contains(u.user_account) && s.subject == subject).ToArray();
            List<double> scoreList = new List<double>();
            for (int i = 0; i < list.Length; i++)
            {
                scoreList.Add(list[i].score);
            }
            return Json(scoreList);
        }
        //获得当前学号班级的每个考试平均成绩
        [HttpPost]
        public JsonResult GradeClassChart(string subject)
        {
            if (subject == null)
            {

                subject = "Access";
            }
            User u = (User)Session["User"];
            Score[] list = sc.ScoreList.Where(s => s.stu_class==u.user_class && s.subject == subject).ToArray();
            List<string> time = new List<string>();
            for (int i = 0; i < list.Length; i++)
            {
                if (!time.Contains(list[i].uploadtime))
                {
                    time.Add(list[i].uploadtime); //有多少次考试
                }

            }
            List<double> scoreList = new List<double>();
            foreach(string t in time)
            {
                double temp=0.0;
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
                scoreList.Add(Math.Round(temp,2));
            }
            return Json(scoreList);
        }
        [HttpPost]
        public JsonResult PersonalLine_X(string subject)
        {
            User u = (User)Session["User"];
            Score[] list = sc.ScoreList.Where(s => s.stu_id.Contains(u.user_account) && s.subject==subject).ToArray();
            List<string> x = new List<string>();
            for (int i = 1; i <= list.Length; i++)
            {
                x.Add("第" + i + "次考试");
            }
            return Json(x);
        }
        //获得当前学号班级的每个考试平均成绩
        [HttpPost]
        public JsonResult GradeClassChart_X(string subject)
        {
            if (subject == null)
            {

                subject = "Access";
            }
            User u = (User)Session["User"];
            Score[] list = sc.ScoreList.Where(s => s.stu_class == u.user_class && s.subject == subject).ToArray();
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
        //退出登录
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("", "Student");
        }
    }
}

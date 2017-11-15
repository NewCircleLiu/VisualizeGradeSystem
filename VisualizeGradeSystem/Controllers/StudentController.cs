using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VisualizeGradeSystem.Utils.filters;
using VisualizeGradeSystem.Models.User;
using System.Web.Security;
using VisualizeGradeSystem.Models.Score;
using VisualizeGradeSystem.Models.Knowledge;

namespace VisualizeGradeSystem.Controllers
{
    public class StudentController : Controller
    {
        //
        // GET: /Student/
        ScoreContext sc = new ScoreContext();
        KnowledgeContext kc = new KnowledgeContext();
        UserContext uc = new UserContext();
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
            var exsit = uc.UserList.Where(user => user.user_account == u.user_account && user.user_password == old_pass).ToList();
            if (exsit == null)
            {
                return Content("error");
            }
            else
            {
                u.user_account = new_pass;
                uc.UserList.Attach(u);
                uc.Entry(u).State = System.Data.EntityState.Modified;
                uc.SaveChanges();
                FormsAuthentication.SignOut();
                Session.Clear();
                return RedirectToAction("", "Student");
            }
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
        [UserAuthorize]
        public ActionResult KnowledgePage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            ViewBag.userclass = u.user_class;
            return View();
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
        [HttpPost]
        public JsonResult GetSomeData(string subject,string time)
        {
            User u = (User)Session["User"];
            Score[] list = sc.ScoreList.Where(s=>s.subject==subject).ToArray();
            List<string> times = new List<string>();
            List<string> data = new List<string>();
            double user_score = 0;//它的分数
            int higer_5 = 0; //比他低5分的人数
            int lower_5 = 0; //比他高5分的人数
            int equal = 0;
            int max_i=0;
            int min_i=0;
            double avg = 0.0;
            for (int i = 0; i < list.Length; i++)
            {
                if (!times.Contains(list[i].uploadtime))
                {
                    times.Add(list[i].uploadtime); //有多少次考试
                }
            }
            int start = time.IndexOf("第")+1;
            int end = time.IndexOf("次") - 1;
            time = time.Substring(start, end - start + 1);
            int index = Convert.ToInt32(time)-1; //第一次考试index为0
            if (index == -1)
            {
                data.Add("0");
                data.Add(user_score.ToString());
                data.Add(higer_5.ToString());
                data.Add(lower_5.ToString());
                data.Add(equal.ToString());
                data.Add(list[max_i].score.ToString());
                data.Add(list[max_i].stu_class);
                data.Add(list[min_i].score.ToString());
                data.Add(list[min_i].stu_class);
                data.Add(avg.ToString());
                return Json(data);
            }
            string n = times[index];
            list = sc.ScoreList.Where(s => s.uploadtime == n && s.subject==subject).ToArray();
            //这次考试的情况
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].stu_id == u.user_account) //用户的分数
                {
                    user_score = list[i].score;
                }
                avg += list[i].score;
            }
            int total = list.Length;//总人数
            avg=avg/total;//平均分
            Math.Round(avg, 2);
            for (int i = 0; i < list.Length; i++)
            {
                double temp=list[i].score-user_score; //分差
                if (list[i].stu_id != u.user_account && (temp <= 5.5 && temp >= 4.5)) //高于5分的人数
                {
                    higer_5++;
                }
                if (list[i].stu_id != u.user_account && (temp >= -5.5 && temp <= -4.5)) //低于5分的人数
                {
                    lower_5++;
                }
                if (list[i].stu_id != u.user_account && temp == 0)
                {
                    equal++;
                }
            }
            for (int i = 0; i < list.Length; i++)
            {
                if (list[max_i].score < list[i].score)
                {
                    max_i = i;
                }
                if (list[min_i].score > list[i].score)
                {
                    min_i = i;
                }
            }
            data.Add(total.ToString());
            data.Add(user_score.ToString());
            data.Add(higer_5.ToString());
            data.Add(lower_5.ToString());
            data.Add(equal.ToString());
            data.Add(list[max_i].score.ToString());
            data.Add(list[max_i].stu_class);
            data.Add(list[min_i].score.ToString());
            data.Add(list[min_i].stu_class);
            data.Add(avg.ToString());
            return Json(data);
        }
        [HttpPost]
        public JsonResult GetKnowledge_X(string subject, string knowledge)
        {
            User u = (User)Session["User"];
            Knowledge[] list = kc.KnowledgeList.Where(k => k.stu_id == u.user_account && k.subject == subject && k.knowledge_name == knowledge).ToArray();
            List<string> time = new List<string>();
            for (int i = 0; i < list.Length; i++)
            {
                if (!time.Contains(list[i].uploadtime))
                {
                    time.Add(list[i].uploadtime); //有多少次考试
                }
            }
            List<string> x = new List<string>();
            int j=1;
            foreach(string t in time)
            {
                x.Add("第" + j + "次考试");
                j++;
            }
            return Json(x);
        }
        [HttpPost]
        public JsonResult GetKnowledge(string subject,string knowledge) 
            //哪一门课的,某一个知识点的各次的得分
        {
            User u = (User)Session["User"];
            Knowledge[] list = kc.KnowledgeList.Where(k => k.stu_id == u.user_account && k.subject == subject && k.knowledge_name == knowledge).ToArray();
            List<double> scoreList = new List<double>();
            List<string> time = new List<string>();
            for (int i = 0; i < list.Length; i++)
            {
                if (!time.Contains(list[i].uploadtime))
                {
                    time.Add(list[i].uploadtime); //有多少次考试
                }

            }
            //因为同一次考试，可能有好几个题，涉及到这个知识点，所有需要取平均：
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
                if (l != 0)
                {
                    temp = temp / l;
                    scoreList.Add(Math.Round(temp, 2));
                }
            }
            return Json(scoreList);
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
            Score[] list = sc.ScoreList.Where(s => s.stu_id == u.user_account && s.subject == subject).ToArray();
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
            Score[] list = sc.ScoreList.Where(s => s.stu_id==u.user_account && s.subject==subject).ToArray();
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

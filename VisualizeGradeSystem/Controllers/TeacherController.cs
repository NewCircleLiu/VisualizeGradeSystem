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

namespace VisualizeGradeSystem.Controllers
{
    public class TeacherController : Controller
    {
        //
        // GET: /Teacher/
        ScoreContext sc = new ScoreContext();
        public ActionResult Index()
        {
            User u = (User)Session["User"];
            if (u != null)
            {
                ViewBag.account = u.user_account;
            }
            return View();
        }

        [UserAuthorize]
        public ActionResult UploadFilesPage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            return View();
        }
        public ActionResult ChooseSubjectPage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            return View();
        }
        [UserAuthorize]
        public ActionResult GradeSchollChartPage()
        {
            User u = (User)Session["User"];
            ViewBag.account = u.user_account;
            return View();
        }
        [HttpPost]
        public JsonResult GradeSchollChartPage(string subject, string depart, string Class)
        {
            Score[] list = sc.ScoreList.Where(s => s.subject == subject && s.stu_depart == depart && s.stu_class == Class).ToArray();
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
        [HttpPost]
        public JsonResult GradeSchollChartPaget_X(string subject, string depart, string Class)
        {
            Score[] list = sc.ScoreList.Where(s => s.subject == subject && s.stu_depart == depart && s.stu_class == Class).ToArray();
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
        [HttpPost]
        public ActionResult ChooseSubject(string subject)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            Score[] array = sc.ScoreList.Where(s => s.uploadtime.Contains(today)).ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                Score s = array[i];
                s.subject = subject;
                sc.ScoreList.Attach(s);
                sc.Entry(s).State = System.Data.EntityState.Modified;
                sc.SaveChanges();
                //sc. = subject;
                //sc.SaveChanges();
            }
            return RedirectToAction("UploadFiles", "Teacher");
        }
        [HttpPost]
        public ActionResult UploadFiles()
        {
            HttpPostedFileBase file = Request.Files[0];
            string fileName = file.FileName;
            string saveUrl = Server.MapPath("/") + "UploadFiles/GradeExcel/";
            Uploader up = new Uploader();
            ReadExcel re = new ReadExcel();
            up.UploadExcel(file, saveUrl);//上传了
            Score[] scoreArray = re.Read(saveUrl + file.FileName);

            for (int i = 0; i < scoreArray.Length; i++)
            {
                scoreArray[i].uploadtime = DateTime.Now.ToString("yyyy-MM-dd");
                sc.ScoreList.Add(scoreArray[i]);
                sc.SaveChanges();
            }
            return RedirectToAction("ChooseSubjectPage", "Teacher");
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("", "Student");
        }

    }
}

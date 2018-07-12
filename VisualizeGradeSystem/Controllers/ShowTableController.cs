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
    public class ShowTableController : Controller
    {
        //
        // GET: /ShowTable/
        //show各种表格的(只针对教师端的三种角色)
        private ScoreContext sc = new ScoreContext();
        private PuzzleContext pc = new PuzzleContext();
        private KnowledgeContext kc = new KnowledgeContext();
        private UserContext uc = new UserContext();
        private ClassContext cc = new ClassContext();

        public ActionResult Index()
        {
            return View();
        }
        //获得全校成绩
        [HttpPost]
        public JsonResult GradeSchollChartPage(string subject, string depart, string Class, string year)
        {
            User u = (User)Session["User"];
            Score[] list=sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.updatetime.StartsWith(year)).ToArray();
            if (u.user_kind == "haed")
            {
                list = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject==subject &&s.updatetime.StartsWith(year)).ToArray();
            }
            if (u.user_kind == "teacher") 
            {
                list = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && u.user_class.Contains(Class) && s.updatetime.StartsWith(year)).ToArray();
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
        //获得全校成绩的x轴
        [HttpPost]
        public JsonResult GradeSchollChartPaget_X(string subject, string depart, string Class, string year)
        {
            User u = (User)Session["User"];
            Score[] list = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.updatetime.StartsWith(year)).ToArray();
            if (u.user_kind == "haed")
            {
                list = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && s.updatetime.StartsWith(year)).ToArray();
            }
            if (u.user_kind == "teacher")
            {
                list = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && u.user_class.Contains(Class) && s.updatetime.StartsWith(year)).ToArray();
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
                x.Add("第" + t + "次考试");
                j++;
            }
            return Json(x);
        }

        //获取全校的知识点
        [HttpPost]
        public JsonResult GetKnowledge(string subject, string Class, string knowledge, string year)
        {
            User u = (User)Session["User"];
            Knowledge[] kList = kc.KnowledgeList.Where(k => k.subject == subject && k.stu_class == Class && k.knowledge_name == knowledge && k.updatetime.StartsWith(year)).ToArray();
            if (u.user_kind == "head")
            {
                kList = kc.KnowledgeList.Where(k => k.subject == subject && u.user_subject==subject && k.stu_class == Class && k.knowledge_name == knowledge && k.updatetime.StartsWith(year)).ToArray();
            }
            if (u.user_kind == "teacher")
            {
                kList = kc.KnowledgeList.Where(k => k.subject == subject && u.user_subject == subject && k.stu_class == Class && k.knowledge_name == knowledge && k.updatetime.StartsWith(year) && u.user_class.Contains(Class)).ToArray();
            }
            List<double> scoreList = new List<double>();
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
        //获取知识点的X轴
        [HttpPost]
        public JsonResult GetKnowledge_X(string subject, string Class, string knowledge, string year)
        {
            User u = (User)Session["User"];
            
            Knowledge[] kList = kc.KnowledgeList.Where(k => k.subject == subject && k.stu_class == Class && k.knowledge_name == knowledge).ToArray();
            if (u.user_kind == "head")
            {
                kList = kc.KnowledgeList.Where(k => k.subject == subject && u.user_subject == subject && k.stu_class == Class && k.knowledge_name == knowledge && k.updatetime.StartsWith(year)).ToArray();
            }
            if (u.user_kind == "teacher")
            {
                kList = kc.KnowledgeList.Where(k => k.subject == subject && u.user_subject == subject && k.stu_class == Class && k.knowledge_name == knowledge && k.updatetime.StartsWith(year) && u.user_class.Contains(Class)).ToArray();
            }
            List<double> scoreList = new List<double>();
            kList = kc.KnowledgeList.Where(k => k.subject == subject && k.stu_class == Class && k.knowledge_name == knowledge).ToArray();
            List<string> time = new List<string>();
            for (int i = 0; i < kList.Length; i++)
            {
                if (!time.Contains(kList[i].uploadtime))
                {
                    time.Add(kList[i].uploadtime); //有多少次考试
                }

            }
            List<string> x = new List<string>();
            int j = 1;
            foreach (string t in time)
            {
                x.Add("第" + t + "次考试");
                j++;
            }
            return Json(x);
        }
        //获得分段成绩
        [HttpPost]
        public JsonResult GetSomeData(string subject = "Access", string depart = "生态学院", string Class = "环境生态15", string year = "20", string time="1")
        {
            User u = (User)Session["User"];
            Score[] sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.updatetime.StartsWith(year) && s.uploadtime==time && s.score>=90).ToArray();
            if (u.user_kind == "haed")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score >= 90).ToArray();
            }
            if (u.user_kind == "teacher")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && u.user_class.Contains(Class) && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score >= 90).ToArray();
            }
            List<PartClass> partList = new List<PartClass>();
            //90分以上的
            PartClass p1 = new PartClass();
            p1.name = "grade90";
            p1.stu_num = sList.Length;
            p1.stu_id = new string[p1.stu_num];
            p1.stu_name = new string[p1.stu_num];
            for (int i = 0; i < sList.Length; i++)
            {
                p1.stu_id[i] = sList[i].stu_id;
                p1.stu_name[i] = sList[i].stu_name;
            }
            partList.Add(p1);
            //
            sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 90 && s.score>=80).ToArray();
            if (u.user_kind == "haed")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 90 && s.score >= 80).ToArray();
            }
            if (u.user_kind == "teacher")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && u.user_class.Contains(Class) && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 90 && s.score >= 80).ToArray();
            }
            PartClass p2 = new PartClass();
            p2.name = "grade80";
            p2.stu_num = sList.Length;
            p2.stu_id = new string[p2.stu_num];
            p2.stu_name = new string[p2.stu_num];
            for (int i = 0; i < sList.Length; i++)
            {
                p2.stu_id[i] = sList[i].stu_id;
                p2.stu_name[i] = sList[i].stu_name;
            }
            partList.Add(p2);
            sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 80 && s.score >= 70).ToArray();
            if (u.user_kind == "haed")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 80 && s.score >= 70).ToArray();
            }
            if (u.user_kind == "teacher")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && u.user_class.Contains(Class) && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 80 && s.score >= 70).ToArray();
            }
            PartClass p3 = new PartClass();
            p3.name = "grade70";
            p3.stu_num = sList.Length;
            p3.stu_id = new string[p3.stu_num];
            p3.stu_name = new string[p3.stu_num];
            for (int i = 0; i < sList.Length; i++)
            {
                p3.stu_id[i] = sList[i].stu_id;
                p3.stu_name[i] = sList[i].stu_name;
            }
            partList.Add(p3);
            sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 70 && s.score >= 60).ToArray();
            if (u.user_kind == "haed")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 70 && s.score >= 60).ToArray();
            }
            if (u.user_kind == "teacher")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && u.user_class.Contains(Class) && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 70 && s.score >= 60).ToArray();
            }
            PartClass p4 = new PartClass();
            p4.name = "grade60";
            p4.stu_num = sList.Length;
            p4.stu_id = new string[p4.stu_num];
            p4.stu_name = new string[p4.stu_num];
            for (int i = 0; i < sList.Length; i++)
            {
                p4.stu_id[i] = sList[i].stu_id;
                p4.stu_name[i] = sList[i].stu_name;
            }
            partList.Add(p4);
            sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 60 && s.score >= 50).ToArray();
            if (u.user_kind == "haed")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 60 && s.score >= 50).ToArray();
            }
            if (u.user_kind == "teacher")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && u.user_class.Contains(Class) && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 60 && s.score >= 50).ToArray();
            }
            PartClass p5 = new PartClass();
            p5.name = "grade50";
            p5.stu_num = sList.Length;
            p5.stu_id = new string[p5.stu_num];
            p5.stu_name = new string[p5.stu_num];
            for (int i = 0; i < sList.Length; i++)
            {
                p5.stu_id[i] = sList[i].stu_id;
                p5.stu_name[i] = sList[i].stu_name;
            }
            partList.Add(p5);
            sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 50 && s.score >= 40).ToArray();
            if (u.user_kind == "haed")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 50 && s.score >= 40).ToArray();
            }
            if (u.user_kind == "teacher")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && u.user_class.Contains(Class) && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 50 && s.score >= 40).ToArray();
            }
            PartClass p6 = new PartClass();
            p6.name = "grade40";
            p6.stu_num = sList.Length;
            p6.stu_id = new string[p6.stu_num];
            p6.stu_name = new string[p6.stu_num];
            for (int i = 0; i < sList.Length; i++)
            {
                p6.stu_id[i] = sList[i].stu_id;
                p6.stu_name[i] = sList[i].stu_name;
            }
            partList.Add(p6);
            sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 40 && s.score >= 30).ToArray();
            if (u.user_kind == "haed")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 40 && s.score >= 30).ToArray();
            }
            if (u.user_kind == "teacher")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && u.user_class.Contains(Class) && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 40 && s.score >= 30).ToArray();
            }
            PartClass p7 = new PartClass();
            p7.name = "grade30";
            p7.stu_num = sList.Length;
            p7.stu_id = new string[p7.stu_num];
            p7.stu_name = new string[p7.stu_num];
            for (int i = 0; i < sList.Length; i++)
            {
                p7.stu_id[i] = sList[i].stu_id;
                p7.stu_name[i] = sList[i].stu_name;
            }
            partList.Add(p7);
            sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 30 && s.score >= 20).ToArray();
            if (u.user_kind == "haed")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 30 && s.score >= 20).ToArray();
            }
            if (u.user_kind == "teacher")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && u.user_class.Contains(Class) && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 30 && s.score >= 20).ToArray();
            }
            PartClass p8 = new PartClass();
            p8.name = "grade20";
            p8.stu_num = sList.Length;
            p8.stu_id = new string[p8.stu_num];
            p8.stu_name = new string[p8.stu_num];
            for (int i = 0; i < sList.Length; i++)
            {
                p8.stu_id[i] = sList[i].stu_id;
                p8.stu_name[i] = sList[i].stu_name;
            }
            partList.Add(p8);
            sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 20 && s.score >= 10).ToArray();
            if (u.user_kind == "haed")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 20 && s.score >= 10).ToArray();
            }
            if (u.user_kind == "teacher")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && u.user_class.Contains(Class) && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 20 && s.score >= 10).ToArray();
            }
            PartClass p9 = new PartClass();
            p9.name = "grade10";
            p9.stu_num = sList.Length;
            p9.stu_id = new string[p9.stu_num];
            p9.stu_name = new string[p9.stu_num];
            for (int i = 0; i < sList.Length; i++)
            {
                p9.stu_id[i] = sList[i].stu_id;
                p9.stu_name[i] = sList[i].stu_name;
            }
            partList.Add(p9);
            sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 10 ).ToArray();
            if (u.user_kind == "haed")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 10).ToArray();
            }
            if (u.user_kind == "teacher")
            {
                sList = sc.ScoreList.Where(s => s.subject == subject && s.stu_class == Class && u.user_subject == subject && u.user_class.Contains(Class) && s.updatetime.StartsWith(year) && s.uploadtime == time && s.score < 10 ).ToArray();
            }
            PartClass p10 = new PartClass();
            p10.name = "grade0";
            p10.stu_num = sList.Length;
            p10.stu_id = new string[p10.stu_num];
            p10.stu_name = new string[p10.stu_num];
            for (int i = 0; i < sList.Length; i++)
            {
                p10.stu_id[i] = sList[i].stu_id;
                p10.stu_name[i] = sList[i].stu_name;
            }
            partList.Add(p10);
            return Json(partList);
        }

        [HttpPost]
        public JsonResult GetStuKnowledge(string stu_id, string subject, string knowledge)
        {
            Knowledge[] list = kc.KnowledgeList.Where(k => k.stu_id == stu_id && k.subject == subject && k.knowledge_name == knowledge).ToArray();
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
        public JsonResult GetStuKnowledge_X(string stu_id, string subject, string knowledge)
        {
            Knowledge[] list = kc.KnowledgeList.Where(k => k.stu_id == stu_id && k.subject == subject && k.knowledge_name == knowledge).ToArray();
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
                x.Add("第" + t + "次考试");
                j++;
            }
            return Json(x);
        }
        [HttpPost]
        public JsonResult GetStuGrade(string stu_id, string subject)
        {
            if (subject == null)
            {

                subject = "Access";
            }
            Score[] list = sc.ScoreList.Where(s => s.stu_id == stu_id && s.subject == subject).ToArray();
            List<double> scoreList = new List<double>();
            for (int i = 0; i < list.Length; i++)
            {
                scoreList.Add(list[i].score);
            }
            return Json(scoreList);
        }
        [HttpPost]
        public JsonResult GetStuGrade_X(string stu_id, string subject)
        {
            Score[] list = sc.ScoreList.Where(s => s.stu_id == stu_id && s.subject == subject).ToArray();
            List<string> x = new List<string>();
            for (int i = 1; i <= list.Length; i++)
            {
                x.Add("第" + list[i] + "次考试");
            }
            return Json(x);
        }
        [HttpPost]
        public JsonResult GetClassInDepart(string depart)
        {
            Class[] cArrary = cc.ClassList.Where(c => c.class_depart == depart).ToArray();
            List<string> classList = new List<string>();
            for (int i = 0; i < cArrary.Length; i++)
            {
                classList.Add(cArrary[i].class_name);
            }
            return Json(classList);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VisualizeGradeSystem.Models.User;

namespace VisualizeGradeSystem.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        private UserContext uc = new UserContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CheckUser(string account, string password, string verifycode)
        {
            User currentUser = null;
            //string returnInfo = managerService.checkManager(account, password, out currentManager);
            var exsit = uc.UserList.Where(u => u.user_account == account && u.user_password == password).ToList();
            if (Session["ValidateCode"] != null)
            {
                if (Session["ValidateCode"].ToString() != verifycode)
                {
                    return Content("ErrorCode");
                }
                else
                {
                    if (exsit.Count() <= 0)
                    {
                        return Content("error");
                    }
                    else
                    {
                        currentUser = exsit[0];
                        Session["User"] = currentUser;
                        return Content(currentUser.user_kind);
                    }
                }
            }
            else
            {
                ViewBag.error = "请重新填写验证码";
                return RedirectToAction("Index", "Login");
            }
        }

    }
}

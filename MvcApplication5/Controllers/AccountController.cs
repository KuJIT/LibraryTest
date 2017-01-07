using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication5.Models;
using System.Web.Security;

namespace MvcApplication5.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public JsonResult Login(LogOnModel model)
        {
            if (Membership.ValidateUser(model.UserName, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.UserName, true);
                return Json(new { success = true, message = "Добро пожаловать, "  /*Membership.GetUser().UserName /*model.UserName*/ });
            }
            else
            {
                return Json(new { success = false, message = "Неправильный логин или пароль"});
            }

        }

        [HttpGet]
        public JsonResult IsAuth()
        {
            if (Membership.GetUser() == null)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            return Json(new { success = true, UserName = Membership.GetUser().UserName }, JsonRequestBehavior.AllowGet);
        }

        public void LogOff()
        {
            FormsAuthentication.SignOut();
        }

        public JsonResult Register(RegiserModel model)
        {
            MembershipCreateStatus createStatus;
            Membership.CreateUser(model.UserName, model.Password, email: null, passwordQuestion: null, passwordAnswer: null,
                isApproved: true, status: out createStatus);
            if (createStatus == MembershipCreateStatus.Success)
            {
                FormsAuthentication.SetAuthCookie(model.UserName, true);
                return Json(new { success = true });
            }
            else
                return Json(new { success = false });
        }

    }
}

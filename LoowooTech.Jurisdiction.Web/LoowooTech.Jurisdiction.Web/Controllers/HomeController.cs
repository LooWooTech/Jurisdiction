using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoowooTech.Jurisdiction.Common;
using System.DirectoryServices;
using LoowooTech.Jurisdiction.Models;


namespace LoowooTech.Jurisdiction.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (Identity.IsAuthenticated)
            {
                switch (Identity.Type)
                {
                    case GroupType.Administrator:
                        return Redirect("/Admin/Index");
                    case GroupType.Manager:
                        return Redirect("/Manager/Index");
                    case GroupType.Member:
                        return Redirect("/Member/Index");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string Name, string Password)
        {
            LoowooTech.Jurisdiction.Models.User user = Core.UserManager.Login(Name, Password);
            if (user == null)
            {
                throw new ArgumentException("登录失败");
            }
            HttpContext.SaveAuth(Password,user);
            if (user.Group.Contains("Administrators"))
            {
                return Redirect("/Admin/Index");
            }
            return Redirect("/User/Index");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            HttpContext.ClearAuth();
            return RedirectToAction("Index");
        }

    }
}

using LoowooTech.Jurisdiction.Common;
using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoowooTech.Jurisdiction.WindowsWeb.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            var user = Core.UserManager.GetWindowsAccount(sAMAccountName);
            if (user == null)
            {
                throw new ArgumentException("当前操作用户无法识别！");
            }
            switch (user.Type)
            { 
                case GroupType.Administrator:
                    return Redirect("/Admin");
                case GroupType.Manager:
                    return Redirect("/Manager");
                case GroupType.Member:
                    return Redirect("/Member");
            }
            return View();
        }
    }
}

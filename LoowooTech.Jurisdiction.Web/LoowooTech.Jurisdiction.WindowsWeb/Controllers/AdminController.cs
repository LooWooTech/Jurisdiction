using LoowooTech.Jurisdiction.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoowooTech.Jurisdiction.WindowsWeb.Controllers
{
    public class AdminController : ControllerBase
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserList(string key = null)
        {
            ViewBag.Users = ADController.GetUserDict(key);
            return View();
        }

    }
}

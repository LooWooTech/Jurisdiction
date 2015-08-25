using LoowooTech.Jurisdiction.Common;
using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoowooTech.Jurisdiction.Web.Controllers
{
    [UserAuthorize]
    [UserRole(groupType=GroupType.Member)]
    public class MemberController : ControllerBase
    {
        //
        // GET: /Member/

        public ActionResult Index()
        {
            return View();
        }

    }
}

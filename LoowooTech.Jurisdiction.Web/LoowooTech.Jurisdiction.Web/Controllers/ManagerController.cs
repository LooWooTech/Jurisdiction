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
    [UserRole(groupType=GroupType.Manager)]
    public class ManagerController : ControllerBase
    {
        //
        // GET: /Manager/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Manager()
        {
            ViewBag.ManageGroup = Core.ADManager.GetManageGroup(Identity.Name);
            //待审批列表
            ViewBag.List = Core.DataBookManager.GetWait(Identity);
            //审批列表
            ViewBag.Finishs = Core.DataBookManager.GetFinish(Identity.Name);
            //我的申请
            ViewBag.Mines = Core.DataBookManager.GetMine(Identity.Name);
            return View();
        }
        [HttpPost]
        public ActionResult Manager(int ID, string Reason, bool? Check)
        {
            Core.DataBookManager.Check(ID, Reason, Identity.Name, Check);
            return View();
        }


        

    }
}

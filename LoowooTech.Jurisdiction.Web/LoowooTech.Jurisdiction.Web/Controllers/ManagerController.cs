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
            //管理组以及对应组里面的成员
            ViewBag.DICT = Core.ADManager.GetManager(Identity.Name);
            //审批列表
            ViewBag.Finishs = Core.DataBookManager.GetFinish(Identity.Name);
            //我的申请
            ViewBag.Mines = Core.DataBookManager.GetMine(Identity.Name);
            string error = string.Empty;
            Core.DataBookManager.Examine(Identity.Name, out error);
            if (!string.IsNullOrEmpty(error))
            {
                throw new ArgumentException("更新权限列表失败："+error);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Manager(int ID, string Reason, bool? Check,int? Day,int ?Month,int ? Year)
        {
            Core.DataBookManager.Check(ID, Reason, Identity.Name, Check,Day,Month,Year);
            ViewBag.ManageGroup = Core.ADManager.GetManageGroup(Identity.Name);
            //待审批列表
            //审批列表
            ViewBag.Finishs = Core.DataBookManager.GetFinish(Identity.Name);
            //我的申请
            ViewBag.Mines = Core.DataBookManager.GetMine(Identity.Name);
            ViewBag.DICT = Core.ADManager.GetManager(Identity.Name);
            return View();
        }


        

    }
}

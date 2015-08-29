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
            //获取我管理的组
            var groups = Core.AuthorizeManager.GetList(ADController.GetNameBysAMAccountName(Identity.Name));
            //获取当前管理组的权限审核列表
            ViewBag.Wait = Core.DataBookManager.Get(groups,CheckStatus.Wait);
            //获取我审核的列表（通过Checker==Identity.Name）


            //管理组以及对应组里面的成员
            //ViewBag.DICT = Core.ADManager.GetManager(Identity.Name);
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
        public ActionResult Manager(int ID, string Reason, int? Day,int ?Month,int ? Year,CheckStatus status=CheckStatus.Wait)
        {
            Core.DataBookManager.Check(ID, Reason, Identity.Name,Day,Month,Year,status);

            ViewBag.ManageGroup = Core.AuthorizeManager.GetList(Identity.Name);

            //待审批列表
            var groups = Core.AuthorizeManager.GetList(ADController.GetNameBysAMAccountName(Identity.Name));
            ViewBag.Wait = Core.DataBookManager.Get(groups,CheckStatus.Wait);

            //管理组以及对应组里面的成员
            //ViewBag.DICT = Core.ADManager.GetManager(Identity.Name);
            //审批列表
            ViewBag.Finishs = Core.DataBookManager.GetFinish(Identity.Name);
            //我的申请
            ViewBag.Mines = Core.DataBookManager.GetMine(Identity.Name);
            string error = string.Empty;
            Core.DataBookManager.Examine(Identity.Name, out error);
            if (!string.IsNullOrEmpty(error))
            {
                throw new ArgumentException("更新权限列表失败：" + error);
            }
            return View();
        }


        

    }
}

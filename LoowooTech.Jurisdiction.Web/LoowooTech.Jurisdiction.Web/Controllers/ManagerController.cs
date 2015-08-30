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

            ViewBag.DGroups = ADController.GetGroupDict(groups);

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

            ViewBag.DGroups = ADController.GetGroupDict(groups);
            return View();
        }



        public ActionResult CHistory(int page=1) 
        {
            var filter = new DataBookFilter
            {
                Status = CheckStatus.All,
                Checker=Identity.Name,
                Page = new Page(1)
            };
            ViewBag.List = Core.DataBookManager.Get(filter);
            ViewBag.Page = filter.Page;
            return View();
        }


        

    }
}

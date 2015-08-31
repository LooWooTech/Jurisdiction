﻿using LoowooTech.Jurisdiction.Common;
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
            ViewBag.User = Core.UserManager.Get(Identity);
            return View();
        }


        public ActionResult Manager()
        {
            //获取我管理的组
            var groups = Core.AuthorizeManager.GetList(Identity.Name);
            //获取当前管理组的权限审核列表
            ViewBag.Wait = Core.DataBookManager.Get(groups,CheckStatus.Wait);

            ViewBag.DGroups = ADController.GetUserDict(groups);

            return View();
        }
        [HttpPost]
        public ActionResult Manager(int ID, string Reason, int? Day,bool?Check,CheckStatus status=CheckStatus.Wait)
        {
            Core.DataBookManager.Check(ID, Reason, Identity.Name,Day,Check,status);
            //待审批列表
            var groups = Core.AuthorizeManager.GetList(Identity.Name);
            ViewBag.Wait = Core.DataBookManager.Get(groups,CheckStatus.Wait);

            ViewBag.DGroups = ADController.GetUserDict(groups);
            return View();
        }



        public ActionResult CHistory(bool?Label,CheckStatus status=CheckStatus.All, string Name=null,string GroupName=null,int page=1) 
        {
            var filter = new DataBookFilter
            {
                Status = status,
                Checker=Identity.Name,
                Name=Name,
                GroupName=GroupName,
                Label=Label,
                Page = new Page(page)
            };
            ViewBag.List = Core.DataBookManager.Get(filter);
            ViewBag.Page = filter.Page;
            var list=Core.DataBookManager.GetList(Identity.Name);
            ViewBag.NList = list.GroupBy(e => e.Name).Select(e => e.Key).ToList();
            ViewBag.GList = list.GroupBy(e => e.GroupName).Select(e => e.Key).ToList();
            return View();
        }

        public ActionResult Apply()
        {
            ViewBag.ManagerList = Core.AuthorizeManager.GetAllManager();
            ViewBag.User = Core.UserManager.Get(Identity);
            return View();
        }

        [HttpPost]
        public ActionResult Apply(string Boss)
        {
            var groups = HttpContext.GetValue("Group");
            List<string> None;
            List<string> Have;
            List<int> Indexs;
            Core.AuthorizeManager.Screen(groups, Identity.Name, out None, out Have);
            try
            {
                Indexs = Core.DataBookManager.Add(None, Identity.Name);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            ViewBag.Have = Have;
            ViewBag.Book = Core.DataBookManager.Get(Indexs);
            return View("MSuccess");
        }

        public ActionResult Gain()
        {
            var list = Core.ADManager.GetListGroup().Select(e => e.Name).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult History(CheckStatus status=CheckStatus.All,int page=1)
        {
            var filter = new DataBookFilter
            {
                Name = Identity.Name,
                Status = status,
                Page = new Page(page)
            };
            ViewBag.List = Core.DataBookManager.Get(filter);
            ViewBag.Page = filter.Page;
            return View();
        }


        

    }
}

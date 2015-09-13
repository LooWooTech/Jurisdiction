using LoowooTech.Jurisdiction.Common;
using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoowooTech.Jurisdiction.WindowsWeb.Controllers
{
    public class ManagerController : ControllerBase
    {
        //
        // GET: /Manager/

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.User = LUser;
            return View();
        }

        
        public ActionResult Manager()
        {
            List<string> groups;
            //获取我管理的组
            if (ADController.IsManager(LUser))
            {
                groups = ADController.GetGroupList();
            }
            else
            {
                groups = Core.AuthorizeManager.GetList(ADController.GetNameBysAMAccountName(sAMAccountName));
            }
            //获取当前管理组的权限审核列表
            ViewBag.Wait = Core.DataBookManager.Get(groups, CheckStatus.Wait);

            ViewBag.DGroups = ADController.GetUserDict(groups);

            return View();
        }
        [HttpPost]
        public ActionResult Manager(int ID, string Reason, int? Day, bool? Check, CheckStatus status = CheckStatus.Wait)
        {
            Core.DataBookManager.Check(ID, Reason, sAMAccountName, Day, Check, status);
            Core.MessageManager.Add(Core.DataBookManager.Get(ID), ADController.GetNameBysAMAccountName(sAMAccountName));
            //待审批列表
            List<string> groups;
            if (ADController.IsManager(LUser))
            {
                groups = ADController.GetGroupList();
            }
            else
            {
                groups = Core.AuthorizeManager.GetList(ADController.GetNameBysAMAccountName(sAMAccountName));
            }

            
            ViewBag.Wait = Core.DataBookManager.Get(groups, CheckStatus.Wait);

            ViewBag.DGroups = ADController.GetUserDict(groups);
            return View();
        }


        //审批历史
        public ActionResult CHistory(bool? Label, CheckStatus status = CheckStatus.All, string Name = null, string GroupName = null, int page = 1)
        {
            var filter = new DataBookFilter
            {
                Status = status,
                Checker = sAMAccountName,
                Name = Name,
                GroupName = GroupName,
                Label = Label,
                Page = new Page(page)
            };
            ViewBag.List = Core.DataBookManager.Get(filter);
            ViewBag.Page = filter.Page;
            var list = Core.DataBookManager.GetList(sAMAccountName);
            ViewBag.NList = list.GroupBy(e => e.Name).Select(e => e.Key).ToList();
            ViewBag.GList = list.GroupBy(e => e.GroupName).Select(e => e.Key).ToList();
            return View();
        }

        public ActionResult Apply()
        {
            ViewBag.ManagerList = Core.AuthorizeManager.GetAllManager();
            ViewBag.User = LUser;
            return View();
        }

        [HttpPost]
        public ActionResult Apply(string Boss)
        {
            var groups = HttpContext.GetValue("Group");
            List<string> None;
            List<string> Have;
            List<int> Indexs;
            Core.AuthorizeManager.Screen(groups, sAMAccountName, out None, out Have);
            try
            {
                Indexs = Core.DataBookManager.Add(None, sAMAccountName);
                Core.MessageManager.Add(None, ADController.GetNameBysAMAccountName(sAMAccountName), Boss);
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

        public ActionResult History(CheckStatus status = CheckStatus.All, int page = 1)
        {
            var filter = new DataBookFilter
            {
                Name = sAMAccountName,
                Status = status,
                Page = new Page(page)
            };
            ViewBag.List = Core.DataBookManager.Get(filter);
            ViewBag.Page = filter.Page;
            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (LUser.Type != GroupType.Manager)
            {
                throw new HttpException(401, "你没有权限查看此页面");
            }
            base.OnActionExecuting(filterContext);
        }

    }
}

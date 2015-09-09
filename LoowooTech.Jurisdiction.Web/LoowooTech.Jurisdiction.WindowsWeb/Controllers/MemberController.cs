using LoowooTech.Jurisdiction.Common;
using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoowooTech.Jurisdiction.WindowsWeb.Controllers
{
    public class MemberController : ControllerBase
    {
        //
        // GET: /Member/

        public ActionResult Index()
        {
            ViewBag.User = Core.UserManager.GetWindowsAccount(sAMAccountName);
            return View();
        }

        /// <summary>
        /// 用户申请组权限
        /// </summary>
        /// <returns></returns>
        public ActionResult Apply()
        {
            ViewBag.ManagerList = Core.AuthorizeManager.GetAllManager();
            ViewBag.User = Core.UserManager.GetWindowsAccount(sAMAccountName);
            return View();
        }
        public ActionResult GetGroupList(string Boss)
        {
            var html = Core.AuthorizeManager.GetList(Boss);
            return HtmlResult(html);
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
                Indexs = Core.DataBookManager.Add(None,sAMAccountName);
                Core.MessageManager.Add(None, ADController.GetNameBysAMAccountName(sAMAccountName), Boss);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            ViewBag.Have = Have;
            ViewBag.Book = Core.DataBookManager.Get(Indexs);
            return View("ASuccess");
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

    }
}

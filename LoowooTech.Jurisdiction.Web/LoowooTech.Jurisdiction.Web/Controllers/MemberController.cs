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

        /// <summary>
        /// 用户申请组权限
        /// </summary>
        /// <returns></returns>
        public ActionResult Apply()
        {
            ViewBag.ManagerList = Core.AuthorizeManager.GetAllManager();
            ViewBag.User = Core.UserManager.Get(Identity);
            ViewBag.ListGroup = Core.ADManager.GetListGroup();
            ViewBag.Mine = Core.DataBookManager.GetMine(Identity.Name);
            return View();
        }
        public ActionResult GetGroupList(string Boss)
        {
            var html = Core.AuthorizeManager.GetList(Boss);
            return HtmlResult(html);
        }


        [HttpPost]
        public ActionResult Apply()
        {
            var groups = HttpContext.GetValue("Group");
            var ID = Core.DataBookManager.Add(Book, Identity.Name);
            if (ID == 0)
            {
                throw new ArgumentException("填写申请权限失败，请重新填写！");
            }
            ViewBag.Book = Core.DataBookManager.Get(ID);
            return View("ASuccess");
        }

        public ActionResult Gain()
        {
            var list = Core.ADManager.GetListGroup().Select(e => e.Name).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


       

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoowooTech.Jurisdiction.Common;
using LoowooTech.Jurisdiction.Models;

namespace LoowooTech.Jurisdiction.Web.Controllers
{
    [UserAuthorize]
    [UserRole(groupType=GroupType.Administrator)]

    public class AdminController : ControllerBase
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            ViewBag.Groups = Core.GroupManager.GetListGroupExcept(null);
            ViewBag.Users = Core.ADManager.GetListUser(null);
            return View();
        }

        /// <summary>
        /// 查看当前域中所有的用户
        /// </summary>
        /// <returns></returns>
        public ActionResult User(string Key=null)
        {
            ViewBag.Users = Core.ADManager.GetListUser(Key);
            return View();
        }

        /// <summary>
        /// 查看当前域中所有的组
        /// </summary>
        /// <returns></returns>
        public ActionResult Group(string Key=null)
        {
            ViewBag.Groups = Core.GroupManager.GetListGroupExcept(Key);
            return View();
        }


        [HttpPost]
        public ActionResult CreateUser(AUser AdUser)
        {
            if (string.IsNullOrEmpty(AdUser.sAMAccountName))
            {
                throw new ArgumentException("账户名不能为空");
            }
            try
            {
                Core.ADManager.Create(AdUser);
                string[] groups = HttpContext.GetValue("Group");
                foreach (var item in groups)
                {
                    Core.ADManager.AddUserToGroup(AdUser.sAMAccountName, item);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return RedirectToAction("User");
        }


        public ActionResult Delete(string Name,bool Flag)
        { 
            Core.ADManager.Delete(Name,Flag);
            return Flag ? RedirectToAction("User") : RedirectToAction("Group");
        }

        [HttpPost]
        public ActionResult CreateGroup(Group group)
        {
            Core.ADManager.Create(group);
            return RedirectToAction("Group");
        }


    }
}

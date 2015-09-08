using LoowooTech.Jurisdiction.Common;
using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoowooTech.Jurisdiction.WindowsWeb.Controllers
{
    public class AdminController : ControllerBase
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserList(bool ? IsActive=null, string key = null)
        {
            ViewBag.Users = ADController.GetUserDict(IsActive,key);
            ViewBag.Organization = ADController.GetOrganizations(System.Configuration.ConfigurationManager.AppSettings["PEOPLE"]);
            return View();
        }

        public ActionResult Group()
        {
            ViewBag.DICT = Core.ADManager.Gain();
            var list = Core.ADManager.GetAllOrganization();
            list.Remove("内部人员");
            ViewBag.List = list;
            ViewBag.Tree = Core.ADManager.GetTree();
            return View();
        }


        [HttpPost]
        public ActionResult CreateUser(string Name,string sAMAccountName,string Organization,string FirstPassword)
        {
            if (string.IsNullOrEmpty(sAMAccountName)||string.IsNullOrEmpty(Name)||string.IsNullOrEmpty(Organization)||string.IsNullOrEmpty(FirstPassword))
            {
                throw new ArgumentException("姓名、账户名或者初始密码不能为空");
            }
            try
            {
                ADController.CreateUser(Name, sAMAccountName, Organization,FirstPassword);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return RedirectToAction("UserList");
        }


        public ActionResult DisableUser(string sAMAccountName)
        {
            try
            {
                ADController.DisableAccount(sAMAccountName);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            
            return RedirectToAction("UserList");
        }

        public ActionResult ActiveUser(string sAMAccountName)
        {
            try
            {
                ADController.ActiveAccount(sAMAccountName);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            
            return RedirectToAction("UserList");
        }

        public ActionResult Delete(string Name, bool Flag)
        {
            Core.ADManager.Delete(Name, Flag);
            return Flag ? RedirectToAction("UserList") : RedirectToAction("Group");
        }

        /// <summary>
        /// 管理员查看申请权限记录以及进行审核
        /// </summary>
        /// <returns></returns>
        public ActionResult Manager()
        {
            var groups = ADController.GetGroupList();
            ViewBag.Wait = Core.DataBookManager.Get(groups, CheckStatus.Wait);
            ViewBag.DGroups = ADController.GetUserDict(groups);
            return View();
        }

        [HttpPost]
        public ActionResult Manager(int ID, string Reason, int? Day, bool? Check, CheckStatus status = CheckStatus.Wait)
        {
            Core.DataBookManager.Check(ID, Reason, sAMAccountName, Day, Check, status);
            var groups = ADController.GetGroupList();
            ViewBag.Wait = Core.DataBookManager.Get(groups, CheckStatus.Wait);
            ViewBag.DGroups = ADController.GetUserDict(groups);
            return View();
        }

        public ActionResult History(bool? Label, CheckStatus status = CheckStatus.All, string Checker = null, string Name = null, string GroupName = null, int page = 1)
        {
            var filter = new DataBookFilter
            {
                Status = status,
                Checker = Checker,
                Name = Name,
                GroupName = GroupName,
                Label = Label,
                Page = new Page(page)
            };
            ViewBag.List = Core.DataBookManager.Get(filter);
            ViewBag.Page = filter.Page;
            var list = Core.DataBookManager.GetList();
            ViewBag.NList = list.GroupBy(e => e.Name).Select(e => e.Key).ToList();
            ViewBag.GList = list.GroupBy(e => e.GroupName).Select(e => e.Key).ToList();
            ViewBag.CList = list.GroupBy(e => e.Checker).Select(e => e.Key).ToList();
            return View();
        }


        [HttpPost]
        public ActionResult CreateUserGroup(string Name, string Description)
        {
            try
            {
                Core.ADManager.AddUserGroup(Name, Description);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return RedirectToAction("Group");
        }


        public ActionResult Impower()
        {
            ViewBag.List = Core.AuthorizeManager.GetList();
            ViewBag.Groups = ADController.GetGroupDict().DictToTable();
            return View();
        }

        public ActionResult Gain()
        {
            var list = ADController.GetUserList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Impower(Authorize authorize)
        {
            try
            {
                Core.AuthorizeManager.Add(Core.AuthorizeManager.Get(HttpContext));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            ViewBag.List = Core.AuthorizeManager.GetList();
            ViewBag.Groups = ADController.GetGroupList().ListToTable();
            return View();
        }
        [HttpPost]
        public ActionResult ImpowerEdit(int ID)
        {
            try
            {
                Core.AuthorizeManager.Edit(Core.AuthorizeManager.Get(HttpContext, ID));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            return RedirectToAction("Impower");
        }

        public ActionResult GJson()
        {
            var treeObject = ADController.GetTreeObject();
            return Json(treeObject, JsonRequestBehavior.AllowGet);
        }

    }
}

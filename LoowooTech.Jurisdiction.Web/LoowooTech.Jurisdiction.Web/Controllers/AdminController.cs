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
            return View();
        }

        /// <summary>
        /// 查看当前域中所有的用户
        /// </summary>
        /// <returns></returns>
        public  ActionResult  User(string Key=null)
        {
            ViewBag.Users = Core.ADManager.GetListUser(Key);
            ViewBag.Organization = Core.ADManager.GetOrganizations("内部人员");
            return View();
        }

        /// <summary>
        /// 查看当前域中所有的组
        /// </summary>
        /// <returns></returns>
        public ActionResult Group(string Key=null)
        {
            ViewBag.DICT = Core.ADManager.Gain();
            var list = Core.ADManager.GetAllOrganization();
            list.Remove("内部人员");
            ViewBag.List = list;
            ViewBag.Tree = Core.ADManager.GetTree();
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
        public ActionResult CreateGroup(Group group,string Position,Category category)
        {
            Core.ADManager.Create(group,Position,category);
            return RedirectToAction("Group");
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
            Core.DataBookManager.Check(ID, Reason, Identity.Name, Day, Check, status);
            var groups = ADController.GetGroupList();
            ViewBag.Wait = Core.DataBookManager.Get(groups, CheckStatus.Wait);
            ViewBag.DGroups = ADController.GetUserDict(groups);
            return View();
        }

        public ActionResult History(bool?Label,CheckStatus status=CheckStatus.All,string Checker=null, string Name=null,string GroupName=null,int page=1)
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
        public ActionResult CreateUserGroup(string Name,string Description)
        {
            try
            {
                Core.ADManager.AddUserGroup(Name,Description);
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
            //return Content(treeObject.ToJson());
            return Json(treeObject, JsonRequestBehavior.AllowGet);
            //var group = ADController.GetTree();
            //return Content(group.ToJson());
            //return JsonGroup(group);
        }


    }
}

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
    public class SafeController : ControllerBase
    {

        public ActionResult ChangeCode()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangeCode(string Old, string Word)
        {
            if (Old != Identity.Password)
            {
                throw new ArgumentException("输入的原始密码不正确");
            }
            string message = string.Empty;
            Core.ADManager.SetUserPassword(Identity.Name, Old, Word, out message);
            if (!string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(message);
            }
            return Redirect("/Home/Index");
        }


        public ActionResult Message()
        {
            //待审批列表
            ViewBag.List = Core.DataBookManager.GetWait(Identity);
            //审批列表
            ViewBag.Finishs = Core.DataBookManager.GetFinish(Identity.Name);
            //我的申请
            ViewBag.Mines = Core.DataBookManager.GetMine(Identity.Name);
            return View();
        }

        [HttpPost]
        public ActionResult Message(int ID,string Reason,bool? Check)
        {
            Core.DataBookManager.Check(ID, Reason, Identity.Name, Check);
            ViewBag.List = Core.DataBookManager.GetWait(Identity);
            ViewBag.Finishs = Core.DataBookManager.GetFinish(Identity.Name);
            ViewBag.Mines = Core.DataBookManager.GetMine(Identity.Name);
            return View();
        }
    }
}

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
            if (!ADController.SetUserPassword(Identity.Name, Old, Word, out message))
            {
                throw new ArgumentException("修改密码失败！1、密码最少六位，推荐使用八位以上密码；2、密码包括大写字符、小写字符、数字、符号中三类；"+message);
            }
            return Redirect("/Home/Index");
        }
    }
}

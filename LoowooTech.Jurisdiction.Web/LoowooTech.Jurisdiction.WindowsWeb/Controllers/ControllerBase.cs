using LoowooTech.Jurisdiction.Common;
using LoowooTech.Jurisdiction.Manager;
using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoowooTech.Jurisdiction.WindowsWeb.Controllers
{
    public class ControllerBase : AsyncController
    {
        //public ControllerBase()
        //{
        //    this.Name = WindowsHelper.GetUserName(Request, HttpContext);
        //}
        protected ManagerCore Core = new ManagerCore();
        protected User User { get; set; }
        protected string sAMAccountName
        {
            get
            {
                return WindowsHelper.GetUserName(Request, HttpContext);
            }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.Controller = RouteData.Values["Controller"];
            ViewBag.Action = RouteData.Values["action"];
            ViewBag.Name = sAMAccountName;
            base.OnActionExecuting(filterContext);
        }

        private Exception GetException(Exception ex)
        {
            var innerEx = ex.InnerException;
            if (innerEx != null)
            {
                return GetException(innerEx);
            }
            return ex;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled) return;
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = 500;
            ViewBag.Exception = GetException(filterContext.Exception);
            filterContext.Result = View("Error");
        }

        protected ActionResult HtmlResult(List<string> html)
        {
            var values = html.ListToTable();

            string str = string.Empty;
            foreach (var item in values)
            {
                string st = string.Empty;
                st += "<tr>";
                foreach (var entry in item)
                {
                    if (string.IsNullOrEmpty(entry))
                    {
                        continue;
                    }
                    st += "<td><label class='checkbox-inline'><input type='checkbox' name='Group' value='" + entry + "' />" + entry + "</label></td>";
                }
                st += "</tr>";
                str += st;
            }
            return Content(str);
        }

    }
}

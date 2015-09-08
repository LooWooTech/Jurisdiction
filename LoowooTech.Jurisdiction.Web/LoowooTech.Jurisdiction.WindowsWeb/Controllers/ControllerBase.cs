using LoowooTech.Jurisdiction.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoowooTech.Jurisdiction.WindowsWeb.Controllers
{
    public class ControllerBase : AsyncController
    {
        protected string Name { get; set; }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.Controller = RouteData.Values["Controller"];
            ViewBag.Action = RouteData.Values["action"];
            ViewBag.Name = WindowsHelper.GetUserName(Request, HttpContext);
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

    }
}

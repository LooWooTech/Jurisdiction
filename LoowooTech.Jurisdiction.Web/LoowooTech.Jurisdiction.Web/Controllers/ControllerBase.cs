using LoowooTech.Jurisdiction.Common;
using LoowooTech.Jurisdiction.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoowooTech.Jurisdiction.Web.Controllers
{
    public class ControllerBase : AsyncController
    {
        protected ManagerCore Core = new ManagerCore();
        protected UserIdentity Identity
        {
            get
            {
                return (UserIdentity)HttpContext.User.Identity;
            }
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.Controller = RouteData.Values["Controller"];
            ViewBag.Action = RouteData.Values["action"];
            if (!string.IsNullOrEmpty(Identity.Name))
            {
                string cn = Core.ADManager.GetCn(Identity.Name);
                if (!string.IsNullOrEmpty(cn))
                {
                    if (Core.GroupManager.IsAdministrator(Identity.Groups))
                    {
                        ViewBag.BCount = Core.DataBookManager.Wait(null);
                    }
                    else
                    {
                        ViewBag.BCount = Core.DataBookManager.Wait(cn);
                    }
                }
                else
                {
                    ViewBag.BCount = 0;
                }
            }
           
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

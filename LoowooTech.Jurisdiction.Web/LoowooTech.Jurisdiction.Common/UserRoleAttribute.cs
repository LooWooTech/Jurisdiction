using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace LoowooTech.Jurisdiction.Common
{
    public class UserRoleAttribute:System.Web.Mvc.ActionFilterAttribute
    {
        public UserRoleAttribute()
        {
            groupType = GroupType.Member;
        }

        public GroupType groupType { get; set; }

        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            if (groupType == GroupType.Member)
            {
                return;
            }

            var currentUser = (UserIdentity)Thread.CurrentPrincipal.Identity;

            if (currentUser.Type != groupType)
            {
                throw new HttpException(401, "你没有权限查看此页面");
            }
        }
    }
}

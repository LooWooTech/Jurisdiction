using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LoowooTech.Jurisdiction.Common
{
    public static class WindowsHelper
    {
        public static string GetUserName(HttpRequestBase request,HttpContextBase context)
        {
            //return "poweradmin";
            //return "Administrator";
            //return "wjl";
            return request.IsAuthenticated ? context.User.Identity.Name.GetDomainName() : string.Empty;
        }

        public static string GetDomainName(this string FullName)
        {
            var array = FullName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            return array.Count() == 2 ? array[1] : string.Empty;
        }
    }
}

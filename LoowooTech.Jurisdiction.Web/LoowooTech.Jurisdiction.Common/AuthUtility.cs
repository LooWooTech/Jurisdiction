using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace LoowooTech.Jurisdiction.Common
{
    public static class AuthUtility
    {
        private const string _cookieName = "Jurisdiction_user";
        public static void SaveAuth(this HttpContextBase context, string Password,LoowooTech.Jurisdiction.Models.User user)
        {
            var ticket = new FormsAuthenticationTicket(user.Name+ "|" + Password +"|"+StringHelper.ToStr(user.Group), true, 60);
            var cookieValue = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(_cookieName, cookieValue);
            context.Response.Cookies.Remove(_cookieName);
            context.Response.Cookies.Add(cookie);
        }

        public static UserIdentity GetCurrentUser(this HttpContextBase context)
        {
            var cookie = context.Request.Cookies.Get(_cookieName);
            if (cookie != null)
            {
                if (string.IsNullOrEmpty(cookie.Value))
                {
                    return UserIdentity.Guest;
                }
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                if (ticket != null && !string.IsNullOrEmpty(ticket.Name))
                {
                    var values = ticket.Name.Split('|');
                    if (values.Length == 3)
                    {
                        return new UserIdentity
                        {
                            UserName = values[0],
                            Password=values[1],
                            Groups=values[2]
                        };
                    }
                }
            }
            return UserIdentity.Guest;
        }

        public static void ClearAuth(this HttpContextBase context)
        {
            var cookie = context.Request.Cookies.Get(_cookieName);
            if (cookie == null) return;
            cookie.Value = null;
            cookie.Expires = DateTime.Now.AddDays(-1);
            context.Response.SetCookie(cookie);
        }

       
    }
}

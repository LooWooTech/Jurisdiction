using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LoowooTech.Jurisdiction.Common
{
    public class AuthenticateModule:IHttpModule
    {
        public void Dispose() { }

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += new EventHandler((obj, ea) =>
            {
                var currentUser = new HttpContextWrapper(context.Context).GetCurrentUser();
                context.Context.User = new UserPrincipal(currentUser);
            });
        }
    }
}

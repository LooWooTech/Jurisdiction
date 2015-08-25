using LoowooTech.Jurisdiction.Common;
using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Web;

namespace LoowooTech.Jurisdiction.Manager
{
    public class UserManager:ManagerBase
    {
        private User Get(string Name)
        {
            SearchResult searchResult = Core.ADManager.SearchOne("(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + Name + "))", null);
            return new User()
            {
                Name = Core.ADManager.GetProperty(searchResult, "sAMAccountName"),
                Group = Core.ADManager.Tranlate(Core.ADManager.GetAllProperty(searchResult, "memberOf"),"group"),
                Managers=Core.ADManager.Tranlate(Core.ADManager.GetAllProperty(searchResult,"managedObjects"),"group")
            };
        }

        
        public User Get(UserIdentity Identity)
        {
            return new User()
            {
                Name = Identity.Name,
                Group = Core.ADManager.Tranlate(Identity.Groups.StrToList(';'),"group")
            };
        }
        public User Login(string Name, string Password)
        {
            DirectoryEntry user =Core.ADManager.GetUser(Name, Password);
            if (user == null)
            {
                throw new ArgumentException("当前域中不存在改用户或者密码不正确");
            }
            return Get(Name);
            //DirectoryEntry Admin = Core.ADManager.GetDirectoryObject();
            //SearchResult searchResult = Core.ADManager.SearchOne("(&(objectClass=user)(cn=" + Name + "))", Admin);
            //if (searchResult != null)
            //{
            //    return new User
            //    {
            //        Name = Core.ADManager.GetProperty(searchResult, "cn"),
            //        Group = Core.ADManager.Tranlate(Core.ADManager.GetAllProperty(searchResult, "memberOf"))
            //    };
            //}
            //return null;
        }
        public void Get(HttpContextBase context)
        {
            //if(context.Request.Form[""])
        }
    }
}

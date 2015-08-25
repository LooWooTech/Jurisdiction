﻿using LoowooTech.Jurisdiction.Common;
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
            User user = new User()
            {
                Name = Core.ADManager.GetProperty(searchResult, "sAMAccountName"),
                Group = Core.ADManager.Tranlate(Core.ADManager.GetAllProperty(searchResult, "memberOf"), "group"),
                Managers = Core.ADManager.Tranlate(Core.ADManager.GetAllProperty(searchResult, "managedObjects"), "group")
            };
            if (user.Group.Contains("Administrators"))
            {
                user.Type = GroupType.Administrator;
            }
            else
            {
                if (user.Managers.Count != 0)
                {
                    user.Type = GroupType.Manager;
                }
                else
                {
                    user.Type = GroupType.Member;
                }
            }
            return user;
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
        }
        public void Get(HttpContextBase context)
        {
            //if(context.Request.Form[""])
        }
    }
}

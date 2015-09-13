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
            var user = ADController.GetUser(Name);
            user.Managers = Core.AuthorizeManager.GetList(user.Name);
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
                MGroup=ADController.GetGroupList(Identity.Name)
            };
        }
        public User Login(string Name, string Password)
        {
            if (!ADController.Login(Name,Password))
            {
                throw new ArgumentException("当前域中不存在该用户或者密码不正确");
            }

            return Get(Name);
        }


        public User GetWindowsAccount(string sAMAccountName)
        {
            if (string.IsNullOrEmpty(sAMAccountName))
            {
                return null;
            }
            var user = ADController.GetUser(sAMAccountName);
            if (user.Type == GroupType.Guest)
            {
                return user;
            }
            user.Managers = Core.AuthorizeManager.GetList(user.Name);
            user.MGroup = ADController.GetGroupList(sAMAccountName);
            if (ADController.IsAdministrator(user))
            {
                user.Type = GroupType.Administrator;
            }
            else if (ADController.IsManager(user))
            {
                user.Type = GroupType.Manager;
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
    }
}

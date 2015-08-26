using LoowooTech.Jurisdiction.Models;
using LoowooTech.Jurisdiction.Common;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Manager
{

    public partial class ADManager
    {
        /// <summary>
        /// 通过用户名以及密码获取用户DirectoryEntry
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public DirectoryEntry GetUser(string Name, string Password)
        {
            DirectoryEntry user = null;
            DirectoryEntry entry = GetDirectoryObject(Name, Password);
            SearchResult searchResult = SearchOne("(&(objectClass=user)(sAMAccountName=" + Name + "))", entry);
            if (searchResult != null)
            {
                user = new DirectoryEntry(searchResult.Path, ADName, ADPassword, AuthenticationTypes.Secure);
            }

            return user;
        }
        /// <summary>
        /// 通过用户名获取DirectoryEntry
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public DirectoryEntry GetUser(string Name)
        {
            return Get("(&(objectCategory=person)(objectClass=user)(cn=" + Name + "))");
        }

        /// <summary>
        /// 获取用户列表（可通过关键字搜素）
        /// </summary>
        /// <param name="Key">关键字</param>
        /// <returns></returns>
        public List<User> GetListUser(string Key)
        {
            List<User> list = new List<User>();
            SearchResultCollection collection = SearchAll("(&(objectCategory=person)(objectClass=user))", null);
            foreach (SearchResult result in collection)
            {
                string Name = GetProperty(result, "cn");
                string Account = GetProperty(result, "sAMAccountName");
                List<string> Groups = Tranlate(GetAllProperty(result, "memberOf"),"group");
                long accountExpires ;
                string Expires=GetProperty(result, "accountExpires");
                if (long.TryParse(Expires, out accountExpires))
                {
                    try
                    {
                        Expires = DateTime.FromFileTime(accountExpires).ToString();
                    }
                    catch
                    {
                        Expires = "从不";
                    }
                    
                }

                if (!string.IsNullOrEmpty(Key))
                {
                    if (Name == Key || Account == Key)
                    {
                        list.Add(new User
                        {
                            Name = Name,
                            Account = Account,
                            Group = Groups,
                            AccountExpires=Expires
                        });
                    }
                }
                else
                {
                    list.Add(new User
                    {
                        Name = Name,
                        Account = Account,
                        Group = Groups,
                        AccountExpires=Expires
                    });
                }
               
            }
            return list;
        }
        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="OldPassword"></param>
        /// <param name="NewPassword"></param>
        /// <param name="Message"></param>
        public void SetUserPassword(string Name, string OldPassword, string NewPassword, out string Message)
        {
            DirectoryEntry Entry = GetUser(Name, OldPassword);
            if (Entry == null)
            {
                throw new ArgumentException("输入的原始密码错误,请核对");
            }
            try
            {
                Entry.Invoke("SetPassword", new object[] { NewPassword });
                Entry.CommitChanges();
                Entry.Close();
                Message = "";
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }

        /// <summary>
        /// 将用户添加到组
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="GroupName"></param>
        public void AddUserToGroup(string Name, string GroupName)
        {
            string value = GetDistinguishedName(Name);
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("未找到申请用户信息");
            }
            DirectoryEntry Group = GetGroup(GroupName);
            if (Group == null)
            {
                throw new ArgumentException("未找到当前组");
            }
            Group.Properties["member"].Add(value);
            Group.CommitChanges();
            Group.Close();
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="AdUser"></param>
        public void Create(AUser AdUser)
        {
            DirectoryEntry UserGroup = Get("(&(OU="+AdUser.Orginzation+"))");
            if (UserGroup == null)
            {
                throw new ArgumentException("未找到创建用户的组织单元");
            }
            DirectoryEntry user = UserGroup.Children.Add("CN=" + AdUser.Initial, "user");
            UserGroup.Close();
            user.Properties["sAMAccountName"].Value = AdUser.sAMAccountName;
            user.Properties["sn"].Value = AdUser.Sn;
            user.Properties["givenName"].Value = AdUser.GivenName;
            if (AdUser.Day != 0 || AdUser.Month != 0 || AdUser.Year != 0)
            {
                if (user.Properties.Contains("accountExpires"))
                {
                    user.Properties["accountExpires"].Value = DateTime.Now.ToString();
                }
            }
            
            user.CommitChanges();
            user.Close();
            
        }

        /// <summary>
        /// 删除用户、组
        /// </summary>
        /// <param name="Name">删除的用户名或者组名</param>
        /// <param name="Flag">区分用户和组</param>
        public void Delete(string Name,bool Flag)
        {
            DirectoryEntry entry = null;
            if (Flag)
            {
                entry = GetUser(Name);
            }
            else
            {
                entry = GetGroup(Name);
            }
            if (entry == null) 
            {
                throw new ArgumentException("未找到需要删除的用户或者组");
            }
            string parentPath = entry.Parent.Path;
            DirectoryEntry parent = new DirectoryEntry(parentPath, ADName, ADPassword, AuthenticationTypes.Secure);
            parent.Children.Remove(entry);
            parent.CommitChanges();
            parent.Close();
        }


    }


}

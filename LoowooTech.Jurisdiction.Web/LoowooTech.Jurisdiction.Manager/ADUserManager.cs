using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Manager
{

    public partial class ADManager
    {
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

        public DirectoryEntry GetUser(string Name)
        {
            return Get("(&(objectCategory=person)(objectClass=user)(cn=" + Name + "))");
            //SearchResult searchResult = SearchOne("(&(objectCategory=person)(objectClass=user)(cn=" + Name + "))", null);
            //if (searchResult != null)
            //{
            //    return new DirectoryEntry(searchResult.Path, ADName, ADPassword, AuthenticationTypes.Secure);
            //}
            //return null;
        }
        public List<User> GetListUser(string Key)
        {
            List<User> list = new List<User>();
            SearchResultCollection collection = SearchAll("(&(objectCategory=person)(objectClass=user))", null);
            foreach (SearchResult result in collection)
            {
                string Name = GetProperty(result, "cn");
                string Account = GetProperty(result, "sAMAccountName");
                List<string> Groups = Tranlate(GetAllProperty(result, "memberOf"),"group");
                if (!string.IsNullOrEmpty(Key))
                {
                    if (Name == Key || Account == Key)
                    {
                        list.Add(new User
                        {
                            Name = Name,
                            Account = Account,
                            Group = Groups
                        });
                    }
                }
                else
                {
                    list.Add(new User
                    {
                        Name = Name,
                        Account = Account,
                        Group = Groups
                    });
                }
               
            }
            return list;
        }
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

        public void Create(AUser AdUser)
        {
            DirectoryEntry UserGroup = Get("(&(cn=Users))");
            if (UserGroup == null)
            {
                throw new ArgumentException("未找到创建用户的组织单元");
            }
            DirectoryEntry user = UserGroup.Children.Add("CN=" + AdUser.Initial, "user");
            user.Properties["sAMAccountName"].Value = AdUser.sAMAccountName;
            user.Properties["sn"].Value = AdUser.Sn;
            user.Properties["givenName"].Value = AdUser.GivenName;
            user.CommitChanges();
            user.Close();
            UserGroup.Close();
        }

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

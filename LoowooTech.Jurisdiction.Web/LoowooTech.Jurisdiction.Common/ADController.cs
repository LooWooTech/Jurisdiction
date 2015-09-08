using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Xml;

namespace LoowooTech.Jurisdiction.Common
{
    public static class ADController
    {
        private static  string ADServer { get; set; }
        private static  string ADName { get; set; }
        private static  string ADPassword { get; set; }
        private static XmlDocument configXml { get; set; }
        private static List<string> IgnoresList { get; set; }
        private static List<string> AdminList { get; set; }
        static ADController()
        {
            ADServer = System.Configuration.ConfigurationManager.AppSettings["Server"];
            ADName = System.Configuration.ConfigurationManager.AppSettings["Name"];
            ADPassword = System.Configuration.ConfigurationManager.AppSettings["Password"];
            Init();
        }
        private static void Init()
        {
            configXml = new XmlDocument();
            configXml.Load(System.Configuration.ConfigurationManager.AppSettings["IGNORE"]);

            IgnoresList = new List<string>();
            var nodes = configXml.SelectNodes("/Composes/Compose");
            if (nodes != null)
            {
                for (var i = 0; i < nodes.Count; i++)
                {
                    IgnoresList.Add(nodes[i].Attributes["Name"].Value);
                }
            }
            AdminList = new List<string>();
            nodes = configXml.SelectNodes("/Composes/Administrators/Administrator");
            if (nodes != null)
            {
                for (var i = 0; i < nodes.Count; i++)
                {
                    AdminList.Add(nodes[i].Attributes["Name"].Value);
                }
            }
        }

#region Common  通用
        //通过用户名和密码获取DirectoryEntry
        private static DirectoryEntry GetDirectoryObject(string Name, string Password)
        {
            try
            {
                return new DirectoryEntry(ADServer, Name,Password, AuthenticationTypes.Secure);
            }
            catch
            {
                return null;
            }
        }
        //获取超级管理员DirectoryEntry
        private static DirectoryEntry GetDirectoryObject()
        {
            return GetDirectoryObject(ADName, ADPassword);
        }
        //通过筛选器获取DirectoryEntry
        private static DirectoryEntry Get(string Filter)
        {
            var searchResult = SearchOne(Filter);
            if (searchResult != null)
            {
                return searchResult.GetDirectoryEntry();
            }
            return null;
        }
        //通过名称name获取DirectoryEntry
        private static DirectoryEntry GetDirectoryObject(string Name)
        {
            return Get("(&(name=" + Name + "))"); 
        }
        private static SearchResult SearchOne(string Filter, DirectoryEntry Entry=null)
        {
            if (Entry == null)
            {
                Entry = GetDirectoryObject();
            }
            using (var searcher = new DirectorySearcher(Entry))
            {
                searcher.Filter = Filter;
                searcher.SearchScope = SearchScope.Subtree;
                return searcher.FindOne();
            }
        }
        private static SearchResultCollection SearchAll(string Filter, DirectoryEntry Entry=null)
        {
            if (Entry == null)
            {
                Entry = GetDirectoryObject();
            }
            using (var searcher = new DirectorySearcher(Entry))
            {
                searcher.Filter = Filter;
                searcher.SearchScope = SearchScope.Subtree;
                return searcher.FindAll();
            }
        }
        //获取DirectoryEntry中属性propertyName的值【0】
        private static string GetProperty(DirectoryEntry Entry, string PropertyName)
        {
            if (Entry.Properties.Contains(PropertyName))
            {
                return Entry.Properties[PropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        //获取DirectoryEntry中属性PropertyName所有的值
        private static List<string> GetAllProperty(DirectoryEntry Entry, string PropertyName)
        {
            var list = new List<string>();
            if (Entry.Properties.Contains(PropertyName))
            {
                
                foreach (var m in Entry.Properties[PropertyName])
                {
                    list.Add(m.ToString());
                }
                
            }
            return list;
        }
        //属性值中提取group或者用户
        private static List<string> Extract(List<string> Origin, string Category)
        {
            var results = new List<string>();
            foreach (var item in Origin)
            {
                var entry = Get("(&(objectCategory=" + Category + ")(cn=" + item.Extract() + "))");
                if (entry == null)
                {
                    continue;
                }
                results.Add(item.Extract());
            }
            return results;
        }
        //通过SearchResult获取属性PropertyName的值
        private static string GetProperty(SearchResult SearchResult, string PropertyName)
        {
            if (SearchResult.Properties.Contains(PropertyName))
            {
                return SearchResult.Properties[PropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        //通过筛选器获取符合条件的名称列表
        private  static List<string> GetList(string Filter)
        {
            var list = new List<string>();
            var collection = SearchAll(Filter);
            foreach (SearchResult item in collection)
            {
                if (!IsIgnore(GetDistinguishedName(item)))
                {
                    list.Add(GetProperty(item, "name"));
                }
            }
            return list;
        }
        private static Dictionary<string, List<string>> GetDict(string Filter)
        {
            var dict = new Dictionary<string, List<string>>();
            var collection = SearchAll(Filter);
            foreach (SearchResult item in collection)
            {
                var distinguishedName = GetDistinguishedName(item);
                if (!IsIgnore(distinguishedName))
                {
                    var ou = distinguishedName.Extract(1, "OU=");
                    if (dict.ContainsKey(ou))
                    {
                        dict[ou].Add(GetProperty(item, "name"));
                    }
                    else
                    {
                        var list = new List<string>();
                        list.Add(GetProperty(item,"name"));
                        dict.Add(ou, list);
                    }
                }
            }
            return dict;
        }
        //获取对象Seachresult的DistinguishedName值
        private static string GetDistinguishedName(SearchResult result)
        {
            return GetProperty(result, "distinguishedName");
        }
        //获取对象DirectoryEntry的DistinguishedName值
        private static string GetDistinguishedName(DirectoryEntry Entry)
        {
            return GetProperty(Entry, "distinguishedName");
        }
        private static string GetDistinguishedName(string sAMAccountName)
        {
            var user = GetUserObject(sAMAccountName);
            if (user == null)
            {
                return null;
            }
            return GetDistinguishedName(user);
        }
        private static bool IsIgnore(string DistinguishedName)
        {
            var str=DistinguishedName.Split(',');
            for (var i = 1; i < str.Count(); i++)
            {
                if (IgnoresList.Contains(str[i].Replace("OU=","").Replace("CN=","")))
                {
                    return true;
                }
            }
            return false;
        }
        private static DirectoryEntries GetChildren(string OU)
        {
            var DirectoryEntry = GetOrganizationObject(OU);
            if (DirectoryEntry == null)
            {
                throw new ArgumentException("无法获取DirectoryEntry对象");
            }
            return DirectoryEntry.Children;
        }
        public static string GetDomainName()
        {
            var admin = GetDirectoryObject();
            var sb = new StringBuilder();
            sb.Append(GetProperty(admin, "name"));
            sb.Append(".com");
            return sb.ToString();
        }
        

#endregion

#region User 用户
        private static DirectoryEntry GetUserObject(string sAMAccountName)
        {
            return Get("(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + sAMAccountName + "))");
        }
        private static DirectoryEntry GetUserObject(string sAMAccountName, string Password)
        {
            var entry = GetDirectoryObject(sAMAccountName, Password);
            var result = SearchOne("(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + sAMAccountName + "))", entry);
            if (result != null)
            {
                return new DirectoryEntry(result.Path, ADName, ADPassword, AuthenticationTypes.Secure);
            }
            return null;
        }
        public static bool Login(string Name, string Password)
        {
            var user = GetDirectoryObject(Name, Password);
            var result = SearchOne("(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + Name + "))", user);
            return result == null ? false : true;
        }
        public static User GetUser(string sAMAccountName)
        {
            var user = GetUserObject(sAMAccountName);
            if (user == null)
            {
                return new User()
                {
                    Name = sAMAccountName,
                    Type = GroupType.Guest
                };
            }
            return new User()
            {
                Name = GetProperty(user, "name"),
                Account=GetProperty(user,"sAMAccountName"),
                Group = Extract(GetAllProperty(user, "memberOf"), "group").OrderBy(e=>e).ToList()
            };
        }

        private static bool IsActive(DirectoryEntry Entry)
        {
            int iUserAccountControl = Convert.ToInt32(GetProperty(Entry, "userAccountControl"));
            int iUserAccountControl_Disabled = Convert.ToInt32(ADAccountOptions.UF_ACCOUNTDISABLE);
            int iFlagExists = iUserAccountControl & iUserAccountControl_Disabled;
            return iFlagExists > 0 ? false : true;
        }
        public static bool IsAdministrator(User user)
        {
            if (user.Group == null || user.Group.Count == 0)
            {
                return false;
            }
            foreach (var item in AdminList)
            {
                if (user.Group.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static void DisableAccount(string sAMAccountName)
        {
            if(string.IsNullOrEmpty(sAMAccountName))
            {
                return;
            }
            var user = GetUserObject(sAMAccountName);
            user.Properties["userAccountControl"][0] = ADAccountOptions.UF_NORMAL_ACCOUNT | ADAccountOptions.UF_DONT_EXPIRE_PASSWD | ADAccountOptions.UF_ACCOUNTDISABLE;
            user.CommitChanges();
            user.Close();
        }

        public static void ActiveAccount(string sAMAccountName)
        {
            if (string.IsNullOrEmpty(sAMAccountName))
            {
                return;
            }
            var user = GetUserObject(sAMAccountName);
            user.Properties["userAccountControl"][0] = ADAccountOptions.UF_NORMAL_ACCOUNT;
            user.CommitChanges();
            user.Close();
        }

        private static List<User> GetUserList(DirectoryEntry Parent,string Key=null)
        {
            var list = new List<User>();
            foreach (DirectoryEntry child in Parent.Children)
            {
                var name = GetProperty(child, "name");
                var account = GetProperty(child, "sAMAccountName");
                var flag = IsActive(child);
                if (string.IsNullOrEmpty(Key))
                {
                    list.Add(new User()
                    {
                        Name = name,
                        Account = account,
                        Group = Extract(GetAllProperty(child, "memberOf"), "group").OrderBy(e=>e).ToList(),
                        IsActive=flag
                    });
                }else{
                    if (name.Contains(Key) || account.Contains(Key))
                    {
                        list.Add(new User()
                        {
                            Name = name,
                            Account = account,
                            Group = Extract(GetAllProperty(child, "memberOf"), "group").OrderBy(e=>e).ToList(),
                            IsActive=flag
                        });
                    }
                }
                
            }
            return list;
        }
        public static List<string> GetUserList()
        {
            return GetList("(&(objectCategory=person)(objectClass=user))");
        }
        public static string GetNameBysAMAccountName(string sAMAccountName)
        {
            return GetProperty(Get("(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + sAMAccountName + "))"), "name");
        }
        private  static string GetsAMAccountByName(string Name)
        {
            return GetProperty(Get("(&(objectCategory=person)(objectClass=user)(name=" + Name + "))"), "sAMAccountName");
        }
        /// <summary>
        /// 通过组名获取当前组中的用户
        /// </summary>
        /// <param name="GroupName">组名</param>
        /// <returns></returns>
        public static List<User> GetUserList(string GroupName)
        {
            var list = new List<User>();
            var groupEntry = GetGroupObject(GroupName);
            var userlist = Extract(GetAllProperty(groupEntry, "member"), "person");
            foreach (var item in userlist)
            {
                list.Add(GetUser(GetsAMAccountByName(item)));
            }
            return list;
        }  
        /// <summary>
        /// 获取组里面包含的用户
        /// </summary>
        /// <param name="Groups"></param>
        /// <returns></returns>
        public static Dictionary<string, List<User>> GetUserDict(List<string> Groups)
        {
            var dict = new Dictionary<string, List<User>>();
            foreach (var item in Groups)
            {
                if (dict.ContainsKey(item))
                {
                    continue;
                }
                dict.Add(item, GetUserList(item));
            }
            return dict;
        }
        public static bool SetUserPassword(string sAMAccountName, string OldPassword, string NewPassword, out string Error)
        {
            if (string.IsNullOrEmpty(NewPassword))
            {
                throw new ArgumentException("输入的密码不能为空");
            }
            DirectoryEntry userEntry = GetUserObject(sAMAccountName, OldPassword);
            if (userEntry == null)
            {
                throw new ArgumentException("输入的原始密码不正确，请核对");
            }
            try
            {
                userEntry.Invoke("SetPassword", new object[] { NewPassword });
                userEntry.CommitChanges();
                userEntry.Close();
                Error = string.Empty;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return false;
            }
            return true;
        }
        public static Dictionary<string, List<User>> GetUserDict(bool? IsActive=null,string Key=null)
        {
            var dict = new Dictionary<string, List<User>>();
            foreach (DirectoryEntry child in GetChildren(System.Configuration.ConfigurationManager.AppSettings["PEOPLE"]))
            {
                var name = GetProperty(child, "name");
                if (string.IsNullOrEmpty(name) || dict.ContainsKey(name))
                {
                    continue;
                }
                var query = GetUserList(child, Key).AsQueryable();
                if (IsActive.HasValue)
                {
                    query = query.Where(e => e.IsActive == IsActive.Value);
                }
                dict.Add(name, query.ToList());
            }
            return dict;
        }

        public static void CreateUser(string Name, string sAMAccountName, string Organization,string FirstPassword)
        {
            var organizationEntry = GetOrganizationObject(Organization);
            if (organizationEntry == null)
            {
                throw new ArgumentException("未找到创建用户的组织单元");
            }
            var user = organizationEntry.Children.Add("CN=" + Name, "user");
            organizationEntry.Close();
            user.Properties["sAMAccountName"].Value = sAMAccountName;
            user.CommitChanges();
            user.Invoke("SetPassword", new object[] { FirstPassword });
            user.CommitChanges();
            user.Close();
            ActiveAccount(sAMAccountName);
        }

#endregion

#region Group 组

        private static DirectoryEntry GetGroupObject(string GroupName)
        {
            return Get("(&(objectCategory=group)(objectClass=group)(cn=" + GroupName + "))");
        }
        private static List<string> GetGroupListBysAMAccountName(string sAMAccountName)
        {
            var userEntry = GetUserObject(sAMAccountName);
            if (userEntry == null)
            {
                return null;
            }
            return Extract(GetAllProperty(userEntry, "memberOf"), "group");
        }

        public static List<string> GetGroupList()
        {
            return GetList("(&(objectCategory=group)(objectClass=group))");
        }

        public static Dictionary<string, List<Group>> GetGroupDict()
        {
            var dict = new Dictionary<string, List<Group>>();
            var collection = SearchAll("(&(objectCategory=group)(objectClass=group))");
            foreach (SearchResult item in collection)
            {
                var distinguishedName = GetDistinguishedName(item);
                if (!IsIgnore(distinguishedName))
                {
                    var ou = distinguishedName.Extract(1, "OU=");
                    if (dict.ContainsKey(ou))
                    {
                        dict[ou].Add(new Group()
                        {
                            Name = GetProperty(item, "name"),
                            Descriptions = GetProperty(item, "description")
                        });
                    }
                    else
                    {
                        var list = new List<Group>();
                        list.Add(new Group()
                        {
                            Name = GetProperty(item, "name"),
                            Descriptions = GetProperty(item, "description")
                        });
                        dict.Add(ou, list);
                    }
                }
            }
            return dict;
        }

        /// <summary>
        /// 判断用户Name是否在组GroupName中
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static bool IsMember(string GroupName, string sAMAccountName)
        {
            var GEntry = GetDirectoryObject(GroupName);
            if (GEntry == null)
            {
                throw new ArgumentException("未找到相关的组信息");
            }
            string UserDistinguishedName = GetDistinguishedName(sAMAccountName);
            return GetAllProperty(GEntry, "member").Contains(UserDistinguishedName) ? true : false;
        }
        public static Group GetGroup(string GroupName)
        {
            var Group = GetGroupObject(GroupName);
            return new Group()
            {
                Name = GetProperty(Group, "name"),
                Descriptions = GetProperty(Group, "description")
            };
        }
        public static List<Group> GetGroupList(string sAMAccountName)
        {
            var GroupsNames = GetGroupListBysAMAccountName(sAMAccountName);
            var list = new List<Group>();
            if (GroupsNames != null)
            {
                foreach (var item in GroupsNames)
                {
                    list.Add(GetGroup(item));
                }
            }
            return list.OrderBy(e=>e.Name).ToList();
        }

        public static Group GetTree()
        {
            var group = new Group();
            var admin = GetDirectoryObject();
            group.Name = GetProperty(admin, "name");
            group.Descriptions = GetProperty(admin, "description");
            DateTime time;
            DateTime.TryParse(GetProperty(admin, "whenCreated"), out time);
            group.CreateTime = time;
            foreach(DirectoryEntry child in  admin.Children)
            {
                var temp = GetTree(child);
                if (temp != null)
                {
                    group.Children.Add(temp);
                }
            }
            return group;
        }

        private static Group GetTree(DirectoryEntry Entry)
        {
            var group = new Group();
            var name = GetProperty(Entry, "name");
            if (string.IsNullOrEmpty(name) || IgnoresList.Contains(name))
            {
                return null;
            }
            group.Name = name;
            group.Descriptions = GetProperty(Entry, "description");
            DateTime time;
            DateTime.TryParse(GetProperty(Entry, "whenCreated"), out time);
            group.CreateTime = time;
            foreach (DirectoryEntry item in Entry.Children)
            {
                var temp = GetTree(item);
                if (temp != null)
                {
                    group.Children.Add(temp);
                }
            }
            return group;
        }

        private static TreeObject GetTreeObject(DirectoryEntry Entry)
        {
            var tree = new TreeObject();
            tree.label = GetProperty(Entry, "name");
            if (string.IsNullOrEmpty(tree.label) || IgnoresList.Contains(tree.label))
            {
                return null;
            }
            foreach (DirectoryEntry item in Entry.Children)
            {
                var temp = GetTreeObject(item);
                if (temp != null)
                {
                    tree.children.Add(temp);
                }
            }
            return tree;
        }

        public static TreeObject GetTreeObject()
        {
            var tree = new TreeObject();
            var admin = GetDirectoryObject();
            tree.label = GetProperty(admin, "name");
            foreach (DirectoryEntry child in admin.Children)
            {
                var temp = GetTreeObject(child);
                if (temp != null) 
                {
                    tree.children.Add(temp);
                }
            }
            return tree;
            
        }
        
#endregion

        #region 组织单元

        private static DirectoryEntry GetOrganizationObject(string OU)
        {
            return Get("(&(OU=" + OU + "))");
        }
        public static List<string> GetOrganizations(string OU)
        {
            var list = new List<string>();
            foreach (DirectoryEntry item in GetChildren(OU))
            {
                var name = GetProperty(item, "name");
                if (!string.IsNullOrEmpty(name))
                {
                    list.Add(name);
                }
            }
            return list;
        }

        #endregion



        #region Operation  组和用户之间操作
        public static bool AddUserToGroup(string sAMAccountName, string GroupName,out string Error)
        {
            Error = string.Empty;
            if (IsMember(GroupName, sAMAccountName))
            {
                Error = "当前组中包括当前用户";
                return true;
            }
            var UserDistinguishedName = GetDistinguishedName(sAMAccountName);
            if (string.IsNullOrEmpty(UserDistinguishedName))
            {
                Error += "未找到相关用户" + sAMAccountName + "信息,添加用户失败";
                return false;
            }
            var GroupEntry = GetGroupObject(GroupName);
            if (GroupEntry == null)
            {
                Error += "未找到需要添加到的组"+GroupName+"的信息";
                return false;
            }
            try
            {
                GroupEntry.Properties["member"].Add(UserDistinguishedName);
                GroupEntry.CommitChanges();
                GroupEntry.Close();
            }
            catch (Exception ex)
            {
                Error += ex.Message;
                return false;
            }

            return true;
        }

        public static bool DeleteUserFromGroup(string Name, string GroupName, out string Error)
        {
            Error = string.Empty;
            if (!IsMember(GroupName, Name))
            {
                Error = "当前组中不包含该成员";
                return true;
            }
            var UserDistinguishedName = GetDistinguishedName(Name);
            if (string.IsNullOrEmpty(UserDistinguishedName))
            {
                Error += "未找到相关用户" + Name + "信息,添加用户失败";
                return false;
            }
            var GroupEntry = GetGroupObject(GroupName);
            if (GroupEntry == null)
            {
                Error += "未找到需要添加到的组" + GroupName + "的信息";
                return false;
            } try
            {
                GroupEntry.Properties["member"].Remove(UserDistinguishedName);
                GroupEntry.CommitChanges();
                GroupEntry.Close();
            }
            catch (Exception ex)
            {
                Error += ex.Message;
                return false;
            }
            return true;
        }
        #endregion










    }

    public enum ADAccountOptions
    {
        UF_TEMP_DUPLICATE_ACCOUNT = 0x0100,
        UF_NORMAL_ACCOUNT = 0x0200,
        UF_INTERDOMAIN_TRUST_ACCOUNT = 0x0800,
        UF_WORKSTATION_TRUST_ACCOUNT = 0x1000,
        UF_SERVER_TRUST_ACCOUNT = 0x2000,
        UF_DONT_EXPIRE_PASSWD = 0x10000,
        UF_SCRIPT = 0x0001,
        UF_ACCOUNTDISABLE = 0x0002,
        UF_HOMEDIR_REQUIRED = 0x0008,
        UF_LOCKOUT = 0x0010,
        UF_PASSWD_NOTREQD = 0x0020,
        UF_PASSWD_CANT_CHANGE = 0x0040,
        UF_ACCOUNT_LOCKOUT = 0X0010,
        UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0X0080,
        UF_EXPIRE_USER_PASSWORD = 0x800000,
    } 
}

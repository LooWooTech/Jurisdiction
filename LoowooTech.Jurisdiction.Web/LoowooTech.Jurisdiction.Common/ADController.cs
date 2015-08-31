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
        }

#region Common  通用
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
        private static DirectoryEntry GetDirectoryObject()
        {
            return GetDirectoryObject(ADName, ADPassword);
        }
        private static DirectoryEntry Get(string Filter)
        {
            var searchResult = SearchOne(Filter);
            if (searchResult != null)
            {
                return searchResult.GetDirectoryEntry();
            }
            return null;
        }
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
        private static string GetDistinguishedName(SearchResult result)
        {
            return GetProperty(result, "distinguishedName");
        }
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
        

#endregion

#region User 用户
        private static DirectoryEntry GetUserObject(string sAMAccountName)
        {
            return Get("(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + sAMAccountName + "))");
        }
        public static bool Login(string Name, string Password)
        {
            var user = GetDirectoryObject(Name, Password);
            var result = SearchOne("(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + Name + "))", user);
            return result == null ? false : true;
        }
        public static User GetUser(string Name)
        {
            var user = GetUserObject(Name);
            return new User()
            {
                Name = GetProperty(user, "name"),
                Account=GetProperty(user,"sAMAccountName"),
                Group = Extract(GetAllProperty(user, "memberOf"), "group")
            };
        }
        public static List<string> GetUserList()
        {
            return GetList("(&(objectCategory=person)(objectClass=user))");
        }
        public static string GetNameBysAMAccountName(string sAMAccountName)
        {
            return GetProperty(Get("(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + sAMAccountName + "))"), "name");
        }
        private static string GetsAMAccountByName(string Name)
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

        

#endregion

#region Group 组

        private static DirectoryEntry GetGroupObject(string GroupName)
        {
            return Get("(&(objectCategory=group)(objectClass=group)(cn=" + GroupName + "))");
        }

        public static List<string> GetGroupList()
        {
            return GetList("(&(objectCategory=group)(objectClass=group))");
        }

        /// <summary>
        /// 判断用户Name是否在组GroupName中
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static bool IsMember(string GroupName, string Name)
        {
            var GEntry = GetDirectoryObject(GroupName);
            if (GEntry == null)
            {
                throw new ArgumentException("未找到相关的组信息");
            }
            string UserDistinguishedName = GetDistinguishedName(Name);
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
        public static List<Group> GetGroupList(List<string> GroupsNames)
        {
            var list = new List<Group>();
            foreach (var item in GroupsNames)
            {
                list.Add(GetGroup(item));
            }
            return list;

        }
        public static Dictionary<string, List<User>> GetGroupDict(List<string> Groups)
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
#endregion

        #region Operation  组和用户之间操作
        public static bool AddUserToGroup(string Name, string GroupName,out string Error)
        {
            Error = string.Empty;
            if (IsMember(GroupName, Name))
            {
                Error = "当前组中包括当前用户";
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
}

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
        private static DirectoryEntry GetUserObject(string sAMAccountName)
        {
            return Get("(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + sAMAccountName + "))");
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
        public static List<string> GetGroupList()
        {
            return GetList("(&(objectCategory=group)(objectClass=group))");
        }
        public static List<string> GetUserList()
        {
            return GetList("(&(objectCategory=person)(objectClass=user))");
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
                Name = GetProperty(user, "sAMAccountName"),
                Group = Extract(GetAllProperty(user, "memberOf"), "group")
            };
        }
        public static string GetNameBysAMAccountName(string sAMAccountName)
        {
            return GetProperty(Get("(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + sAMAccountName + "))"), "name");
        }

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
    }
}

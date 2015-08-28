﻿using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using LoowooTech.Jurisdiction.Common;

namespace LoowooTech.Jurisdiction.Manager
{
    public partial class ADManager:ManagerBase
    {
        private static readonly string ADServer = "LDAP://10.22.102.19";
        private static readonly string ADName = "Administrator";
        private static readonly string ADPassword = "L0owo0Tech";
        static ADManager()
        {
            ADServer = System.Configuration.ConfigurationManager.AppSettings["Server"];
            ADName = System.Configuration.ConfigurationManager.AppSettings["Name"];
            ADPassword = System.Configuration.ConfigurationManager.AppSettings["Password"];
        }

        public  DirectoryEntry GetDirectoryObject(string Name, string Password)
        {
            
            try
            {
                return new DirectoryEntry(ADServer, ADName, ADPassword, AuthenticationTypes.Secure);
            }
            catch
            {
                return null;
            }
            
        }

        public  DirectoryEntry GetDirectoryObject()
        {
            return GetDirectoryObject(ADName, ADPassword);
        }

        public SearchResultCollection SearchAll(string Filter, DirectoryEntry Entry)
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

        public  SearchResult SearchOne(string Filter, DirectoryEntry Entry)
        {
            if (Entry == null)
            {
                Entry = GetDirectoryObject();
            }
            using (DirectorySearcher searcher = new DirectorySearcher(Entry))
            {
                searcher.Filter = Filter;
                searcher.SearchScope = SearchScope.Subtree;
                return searcher.FindOne();
            }
        }

        public DirectoryEntry Get(string Filter)
        {
            SearchResult result = SearchOne(Filter, null);
            if (result != null)
            {
                return result.GetDirectoryEntry();
            }
            else
            {
                return null;
            }
        }

        public  string GetProperty(SearchResult result, string PropertyName)
        {
            if (result.Properties.Contains(PropertyName))
            {
                return result.Properties[PropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetProperty(DirectoryEntry Entry, string PropertyName)
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

        public List<string> GetAllProperty(SearchResult result, string PropertyName)
        {
            List<string> Values = new List<string>();
            if (result.Properties.Contains(PropertyName))
            {
                foreach (var m in result.Properties[PropertyName])
                {
                    Values.Add(m.ToString());
                }
            }
            return Values;
        }

        public List<string> GetAllProperty(DirectoryEntry Entry, string PropertyName)
        {
            List<string> Values = new List<string>();
            if (Entry.Properties.Contains(PropertyName))
            {
                foreach (var m in Entry.Properties[PropertyName])
                {
                    Values.Add(m.ToString());
                }
            }
            return Values;
        }

        /// <summary>
        /// 提取组或者用户
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="Category"></param>
        /// <returns></returns>
        public  List<string> Tranlate(List<string> origin,string Category)
        {
            List<string> TheNew = new List<string>();
            foreach (var item in origin)
            {
                SearchResult searchResult = SearchOne("(&(objectCategory="+Category+")(cn="+item.Extract()+"))", null);
                if (searchResult == null)
                {
                    continue;
                }
                TheNew.Add(item.Extract());
            }
            return TheNew;
        }
        /// <summary>
        /// 通过用户名
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        private string GetPropertyValueByName(string Name, string PropertyName)
        {
            SearchResult searchResult = SearchOne("(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + Name + "))", null);
            if (searchResult != null)
            {
                return GetProperty(searchResult, PropertyName);
            }
            return string.Empty;
        }
        /// <summary>
        /// 获得属性distinguishedName的值
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>

        public string GetDistinguishedName(string Name)
        {
            return GetPropertyValueByName(Name, "distinguishedName");
        }
        /// <summary>
        /// 获得属性cn的值
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string GetCn(string Name)
        {
            return GetPropertyValueByName(Name, "cn");
        }

        /// <summary>
        /// 获取当前用户所管理的组
        /// </summary>
        /// <param name="Name">用户名</param>
        /// <returns></returns>
        public List<string> GetManageGroup(string Name)
        {
            SearchResult searchResult = SearchOne("(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + Name + "))", null);
            return Tranlate(GetAllProperty(searchResult, "managedObjects"),"person");
        }
    }
}

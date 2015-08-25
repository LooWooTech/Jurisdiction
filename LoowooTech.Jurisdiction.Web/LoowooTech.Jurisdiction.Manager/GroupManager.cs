using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using LoowooTech.Jurisdiction.Common;

namespace LoowooTech.Jurisdiction.Manager
{
    public class GroupManager:ManagerBase
    {
        public List<Group> GetListGroup()
        {
            List<Group> list = new List<Group>();
            SearchResultCollection collection = Core.ADManager.SearchAll("(&(objectClass=group))", null);
            foreach (SearchResult result in collection)
            {
                string str = Core.ADManager.GetProperty(result, "createTimeStamp");
                DateTime time = DateTime.Now;
                if (!string.IsNullOrEmpty(str))
                {
                    Convert.ToDateTime(str);
                }

                list.Add(new Group
                {
                    Name = Core.ADManager.GetProperty(result, "name"),
                    CreateTime = time,
                    Descriptions = Core.ADManager.GetProperty(result, "description")
                });
            }
            return list;
        }


        public List<Group> GetListGroupExcept()
        {
            List<Group> list = new List<Group>();
            SearchResultCollection collection = Core.ADManager.SearchAll("(&(objectClass=group))", null);
            foreach (SearchResult result in collection)
            {
                string str = Core.ADManager.GetProperty(result, "createTimeStamp");
                DateTime time = DateTime.Now;
                if (!string.IsNullOrEmpty(str))
                {
                    Convert.ToDateTime(Core.ADManager.GetProperty(result, "createTimeStamp"));
                }
                string distinguishedName = Core.ADManager.GetProperty(result, "distinguishedName");
                if (!string.IsNullOrEmpty(distinguishedName))
                {
                    string cn = distinguishedName.Extract(1);
                    if (cn == "Builtin")
                    {
                        continue;
                    }
                }
                list.Add(new Group
                {
                    Name = Core.ADManager.GetProperty(result, "name"),
                    CreateTime = time,
                    Descriptions = Core.ADManager.GetProperty(result, "description")
                });

            }
            return list;
        }

        public string GetAdministrator(string GroupName)
        {
            SearchResult searchResult = Core.ADManager.SearchOne("(&(objectClass=group)(name=" + GroupName + "))", null);
            string manager = string.Empty;
            if (searchResult != null)
            {
                manager = Core.ADManager.GetProperty(searchResult, "managedBy"); 
            }
            if (string.IsNullOrEmpty(manager))
            {
                manager = "Administrator";
            }
            else
            {
                manager = manager.Extract();
            }

            return manager;
        }

        public bool IsAdministrator(string Groups)
        {
            var list = Groups.StrToList(';');
            if (list != null)
            {
                if (list.Contains("Administrators"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

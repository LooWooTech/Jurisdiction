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

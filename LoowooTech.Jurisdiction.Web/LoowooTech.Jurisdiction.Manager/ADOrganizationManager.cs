using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using LoowooTech.Jurisdiction.Common;

namespace LoowooTech.Jurisdiction.Manager
{
    public partial class ADManager
    {
        public List<string> GetOrganizations(string OU)
        {
            List<string> List = new List<string>();
            SearchResultCollection collection = SearchAll("(&(objectCategory=organizationalUnit))", null);
            foreach (SearchResult result in collection)
            {
                string distinguishedName = GetProperty(result, "distinguishedName");
                string mou = distinguishedName.Extract(1, "OU=");
                if (OU == mou)
                {
                    List.Add(GetProperty(result, "name"));
                }
            }

            return List;
        }
    }
}

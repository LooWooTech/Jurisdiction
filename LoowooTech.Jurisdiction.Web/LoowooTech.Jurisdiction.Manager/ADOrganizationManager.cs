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

        public Dictionary<string, List<string>> Gain()
        {
            Dictionary<string, List<string>> DICT = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> DICTO = Gain("(&(objectCategory=organizationalUnit))");
            Dictionary<string, List<string>> DICTG = Gain("(&(objectCategory=group))");
            foreach (var item in DICTO.Keys)
            {
                if (DICT.ContainsKey(item))
                {
                    foreach (var value in DICTO[item])
                    {
                        DICT[item].Add(value);
                    }
                }
                else
                {
                    DICT.Add(item, DICTO[item]);
                }
                
            }

            foreach (var item in DICTG.Keys) 
            {
                if (DICT.ContainsKey(item))
                {
                    foreach (var value in DICTG[item])
                    {
                        DICT[item].Add(value);
                    }
                }
                else
                {
                    DICT.Add(item, DICTG[item]);
                }
            }
            return DICT;
        }

        public Dictionary<string,List<string>> Gain(string Filter)
        {
            Dictionary<string, List<string>> Dict = new Dictionary<string, List<string>>();
            SearchResultCollection collection = SearchAll(Filter, null);
            foreach (SearchResult result in collection)
            {
                string distingusihedName = GetProperty(result, "distinguishedName");
                string[] values = distingusihedName.Split(',');
                string name = GetProperty(result, "name");
                if (values.Count() == 3)
                {
                    if (name.IsChinese())
                    {
                        if (Dict.ContainsKey(name))
                        {
                            continue;
                        }
                        else
                        {
                            Dict.Add(name, new List<string>());
                        }
                    }

                }
                else if (values.Count() == 4)
                {
                    string parent = values[1].Replace("OU=", "");
                    if (parent.IsChinese())
                    {
                        if (Dict.ContainsKey(parent))
                        {
                            Dict[parent].Add(name);
                        }
                        else
                        {
                            List<string> Keyvalue = new List<string>();
                            Keyvalue.Add(name);
                            Dict.Add(parent, Keyvalue);
                        }
                    }
                }
            }
            return Dict;
            
        }

        public void AddUserGroup(string Name, string Description)
        {
            DirectoryEntry Entry = Get("(&(OU=内部人员))");
            if (Entry == null)
            {
                throw new ArgumentException("未找到创建用户组的组织单元！");
            }
            DirectoryEntry UserGroup = Entry.Children.Add("OU=" + Name, "organizationalUnit");
            Entry.Close();
            if (!string.IsNullOrEmpty(Description))
            {
                UserGroup.Properties["description"].Value = Description;
            }
            UserGroup.CommitChanges();
            UserGroup.Close();
        }

        public List<string> GetAllOrganization()
        {
            return Gain("(&(objectCategory=organizationalUnit))").Select(e => e.Key).ToList();
        }
    }
}

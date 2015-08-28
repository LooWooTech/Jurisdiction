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
        /// 通过组名获取DirectoryEntry
        /// </summary>
        /// <param name="GroupName"></param>
        /// <returns></returns>
        public DirectoryEntry GetGroup(string GroupName)
        {
            return Get("(&(objectCategory=group)(objectClass=group)(cn=" + GroupName + "))");
        }

        public void Create(Group group,string Position,Category category)
        {
            if (string.IsNullOrEmpty(group.Name))
            {
                return;
            }
            DirectoryEntry Entry = Get("(&(OU="+Position+"))");
            string nn = "CN=";
            if (category == Category.organizationalUnit)
            {
                Entry = GetDirectoryObject();
                nn = "OU=";
            }
            if (Entry == null)
            {
                throw new ArgumentException("未找到创建组的组织单元");
            }
            DirectoryEntry groupEntry = Entry.Children.Add(nn+ group.Name, category.ToString());
            if (!string.IsNullOrEmpty(group.Descriptions))
            {
                groupEntry.Properties["description"].Add(group.Descriptions);
            }
            
            groupEntry.CommitChanges();
            groupEntry.Close();
            Entry.Close();
        }


        public List<Group> GetListGroup()
        {
            List<Group> list = new List<Group>();
            SearchResultCollection collection = SearchAll("(&(objectClass=group))", null);
            foreach (SearchResult result in collection)
            {
                string str = GetProperty(result, "createTimeStamp");
                DateTime time = DateTime.Now;
                if (!string.IsNullOrEmpty(str))
                {
                    Convert.ToDateTime(str);
                }
                string orginzation = GetProperty(result, "distinguishedName").Extract(1, "OU=");
                string Name =GetProperty(result, "name");

                if (orginzation.IsChinese())
                {
                    list.Add(new Group
                    {
                        Name = Name,
                        CreateTime = time,
                        Descriptions = GetProperty(result, "description")
                    });
                }


            }
            return list;
        }

        public List<Group> GetListGroupExcept(string Key)
        {
            List<Group> list = new List<Group>();
            SearchResultCollection collection = SearchAll("(&(objectClass=group))", null);
            foreach (SearchResult result in collection)
            {
                string str = GetProperty(result, "createTimeStamp");
                DateTime time = DateTime.Now;
                if (!string.IsNullOrEmpty(str))
                {
                    Convert.ToDateTime(GetProperty(result, "createTimeStamp"));
                }
                string distinguishedName = GetProperty(result, "distinguishedName");
                if (!string.IsNullOrEmpty(distinguishedName))
                {
                    string cn = distinguishedName.Extract(1);
                    if (cn == "Builtin")
                    {
                        continue;
                    }
                }
                string Name = GetProperty(result, "name");

                if (Name.IsChinese())
                {

                    if (!string.IsNullOrEmpty(Key))
                    {
                        if (Key == Name)
                        {
                            list.Add(new Group
                            {
                                Name = Name,
                                CreateTime = time,
                                Descriptions = GetProperty(result, "description")
                            });
                        }
                    }
                    else
                    {
                        list.Add(new Group
                        {
                            Name = Name,
                            CreateTime = time,
                            Descriptions = GetProperty(result, "description")
                        });
                    }

                }


            }
            return list;
        }


        public Group GetTree()
        {
            Group group = new Group();
            DirectoryEntry Admin = GetDirectoryObject();
            group.Name = GetProperty(Admin, "name");
            group.Descriptions = GetProperty(Admin, "description");
            DateTime time;
            DateTime.TryParse(GetProperty(Admin, "whenCreated"), out time);
            group.CreateTime = time;
            List<string> ignores = Core.IgnoreManager.GetCompose();
            foreach (DirectoryEntry child in Admin.Children)
            {
                Group value = GetTree(child, ignores);
                if (value != null)
                {
                    group.Children.Add(value);
                }
            }
            return group;
        }


        public Group GetTree(DirectoryEntry Entry,List<string> Ignores)
        {
            
            Group group = new Group();
            string name = GetProperty(Entry, "name");
            if (string.IsNullOrEmpty(name)||Ignores.Contains(name))
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
                Group temp = GetTree(item, Ignores);
                if (temp != null)
                {
                    group.Children.Add(temp);
                }
                
            }
            return group;
        }

        public List<string> GetManagerObject(string Name)
        {
            DirectoryEntry user = GetUser(Name);
            if (user == null)
            {
                return null;
            }
            var list = GetAllProperty(user, "managedObjects");
            return Tranlate(list, "group");
        }


        public IDictionary<string, List<User>> GetManager(string Name)
        {
            var list = GetManagerObject(Name);
            Dictionary<string, List<User>> DICT = new Dictionary<string, List<User>>();
            foreach (var item in list)
            {
                if (DICT.ContainsKey(item))
                {
                    continue;
                }
                DICT.Add(item, GetListByGroupName(item));
            }
            return DICT;
        }

        


        
    }
}

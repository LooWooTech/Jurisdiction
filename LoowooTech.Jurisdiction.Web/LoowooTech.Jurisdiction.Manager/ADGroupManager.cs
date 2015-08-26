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
        public DirectoryEntry GetGroup(string GroupName)
        {
            return Get("(&(objectCategory=group)(objectClass=group)(cn=" + GroupName + "))");
        }

        public void Create(Group group)
        {
            DirectoryEntry Entry = Get("(&(OU=内部人员))");
            if (Entry == null)
            {
                throw new ArgumentException("未找到创建组的组织单元");
            }
            DirectoryEntry groupEntry = Entry.Children.Add("CN=" + group.Name, "group");
            groupEntry.Properties["description"].Add(group.Descriptions);
            groupEntry.CommitChanges();
            groupEntry.Close();
            Entry.Close();
        }
        

        public List<Group> GetListGroup()
        {
            List<Group> list = new List<Group>();
            SearchResultCollection collection = SearchAll("(&(objectCategory=group)(objectClass=group))", null);
            foreach (SearchResult result in collection)
            {
                string str = GetProperty(result, "createTimeStamp");
                DateTime time = DateTime.Now;
                if (!string.IsNullOrEmpty(str))
                {
                    Convert.ToDateTime(str);
                }

                string Name = GetProperty(result, "name");
                if (Name.IsChinese())
                {
                    list.Add(new Group()
                    {
                        Name = Name,
                        CreateTime = time,
                        Descriptions = GetProperty(result, "description"),
                        Ou = GetProperty(result, "ou")
                    });
                }
            }
            return list;
        }

        public List<Group> GetListGroupByKey(string OU)
        {
            return GetListGroup().Where(e => e.Ou == OU).ToList();
        }
    }
}

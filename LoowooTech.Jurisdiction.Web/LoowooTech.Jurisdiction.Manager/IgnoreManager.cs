using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

namespace LoowooTech.Jurisdiction.Manager
{
    public class IgnoreManager:ManagerBase
    {
        private XmlDocument configXml;
        public IgnoreManager()
        {
            configXml = new XmlDocument();
            configXml.Load(ConfigurationManager.AppSettings["IGNORE"]);
        }

        public List<string> GetCompose()
        {
            var nodes = configXml.SelectNodes("/Composes/Compose");
            if (nodes == null)
            {
                return null;
            }
            List<string> list = new List<string>();
            for (var i = 0; i < nodes.Count; i++)
            {
                list.Add(nodes[i].Attributes["Name"].Value);
            }
            return list;
        }

    }
}

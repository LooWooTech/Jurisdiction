using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Models
{
    public class TreeObject
    {
        public TreeObject()
        {
            children = new List<TreeObject>();
        }
        public string label { get; set; }
        public List<TreeObject> children { get; set; }
    }
}

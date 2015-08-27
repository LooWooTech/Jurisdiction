﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Models
{
    public class Group
    {
        public Group()
        {
            Children = new List<Group>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public string Descriptions { get; set; }
        public string Ou { get; set; }
        public List<Group> Children { get; set; }
    }



}

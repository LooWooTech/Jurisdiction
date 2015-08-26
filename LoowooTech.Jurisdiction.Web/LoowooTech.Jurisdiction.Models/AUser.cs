using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Models
{
    public class AUser
    {
        public string Sn { get; set; }
        public string GivenName { get; set; }
        public string Initial { get; set; }
        public string sAMAccountName { get; set; }

    }

    public enum Category
    {
        Group,
        User
    }
}

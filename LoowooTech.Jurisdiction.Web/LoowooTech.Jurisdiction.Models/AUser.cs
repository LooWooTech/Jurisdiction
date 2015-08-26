using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Models
{
    public class AUser
    {
        /// <summary>
        /// 姓
        /// </summary>
        public string Sn { get; set; }
        /// <summary>
        /// 名
        /// </summary>
        public string GivenName { get; set; }
        /// <summary>
        /// 缩写
        /// </summary>
        public string Initial { get; set; }
        /// <summary>
        /// 账户名
        /// </summary>
        public string sAMAccountName { get; set; }
        public bool Flag { get; set; }
        public string Orginzation { get; set; }
        /// <summary>
        /// 主要设置账户使用期限
        /// </summary>
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

    }

    public enum Category
    {
        [Description("组")]
        group,
        [Description("用户")]
        user,
        [Description("组织单元")]
        organizationalUnit
    }
}

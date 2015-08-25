using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Models
{
    public class User
    {
        public int ID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 隶属组
        /// </summary>
        public List<string> Group { get; set; }
        /// <summary>
        /// 管理哪些组
        /// </summary>
        public List<string> Managers { get; set; }

    }
}

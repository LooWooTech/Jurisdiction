using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace LoowooTech.Jurisdiction.Models
{
    [Table("databooks")]
    public class DataBook
    {
        public DataBook()
        {
            CreateTime = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 申请的组名
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string Checker { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        [Column(TypeName="int")]
        public CheckStatus Status { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime CheckTime { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime MaturityTime { get; set; }
        /// <summary>
        /// 原因 备注
        /// </summary>
        public string Reason { get; set; }
        public bool Label { get; set; }
        [NotMapped]
        public TimeSpan Span
        {
            get
            {
                return MaturityTime.Subtract(DateTime.Now);
            }
        }
        [NotMapped]
        public string DateDiff
        {
            get
            {
                return Span.Days.ToString() + "天"+Span.Hours.ToString()+"小时"+Span.Minutes.ToString()+"分钟"+Span.Seconds.ToString()+"秒";
            }
        }
    }

    public class DataBookFilter
    {
        public string  Name { get; set; }
        public string Checker { get; set; }
        public string GroupName { get; set; }
        public bool? Label { get; set; }
        public CheckStatus Status { get; set; }
        public Page Page { get; set; }
    }

    public enum CheckStatus
    {
        [Description("等待审核")]
        Wait,
        [Description("同意")]
        Agree,
        [Description("不同意")]
        Disagree,
        [Description("全部")]
        All
    }
}

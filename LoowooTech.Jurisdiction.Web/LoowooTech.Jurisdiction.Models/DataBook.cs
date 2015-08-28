using System;
using System.Collections.Generic;
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
        public string Name { get; set; }
        public string GroupName { get; set; }
        public DateTime CreateTime { get; set; }
        public string Checker { get; set; }
        public bool? Check { get; set; }
        public DateTime CheckTime { get; set; }
        public DateTime MaturityTime { get; set; }
        public string Reason { get; set; }

        
        [NotMapped]
        public TimeSpan Span
        {
            get
            {
                return SpareTime();
            }
        }
        [NotMapped]
        public string DateDiff
        {
            get
            {
                TimeSpan span = SpareTime();
                return span.Days.ToString() + "天"+span.Hours.ToString()+"小时"+span.Minutes.ToString()+"分钟"+span.Seconds.ToString()+"秒";
            }
        }

        public TimeSpan SpareTime()
        {
            TimeSpan temp = time.Subtract(CheckTime);
            if (temp.Days == 0 && temp.Hours == 0 && temp.Minutes == 0 && temp.Seconds == 0)
            {
                return temp;
            }
            return time.Subtract(DateTime.Now);
        }
    }
}

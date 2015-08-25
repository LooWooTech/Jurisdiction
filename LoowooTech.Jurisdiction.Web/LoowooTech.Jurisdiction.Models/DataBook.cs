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
        public string Manager { get; set; }
        public DateTime CreateTime { get; set; }
        public string Checker { get; set; }
        public bool? Check { get; set; }
        public DateTime CheckTime { get; set; }
        public string Reason { get; set; }
    }
}

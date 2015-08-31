using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Models
{
    [Table("record")]
    public class Record
    {
        public Record()
        {
            Time = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime Time { get; set; }
        public bool Flag { get; set; }
        public string Mark { get; set; }
        public int DID { get; set; }
        [NotMapped]
        public DataBook Book { get; set; }
    }
}

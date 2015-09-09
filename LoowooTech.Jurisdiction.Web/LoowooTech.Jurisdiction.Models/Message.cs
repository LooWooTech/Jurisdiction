using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Models
{
    [Table("messages")]
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Sender { get; set; }
        [MaxLength(1023)]
        public string Info { get; set; }
        public string Receiver { get; set; }
        public bool Check { get; set; }

    }
}

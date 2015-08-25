using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Manager
{
    public class JURDbContext:DbContext
    {
        public JURDbContext() : base("name=JUR") { }
        public JURDbContext(string connectionString) : base(connectionString) { }
        public DbSet<DataBook> DataBooks { get; set; }
    }
}

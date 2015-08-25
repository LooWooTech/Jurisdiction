using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Manager
{
    public class ManagerBase
    {
        protected ManagerCore Core = new ManagerCore();

        protected JURDbContext GetJURDataContext()
        {
            var db = new JURDbContext();
            db.Database.Connection.Open();
            return db;
        }
    }
}

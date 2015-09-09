using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Manager
{
    public static class MessageHelper
    {
        private static JURDbContext GetJURDataContext()
        {
            var db = new JURDbContext();
            db.Database.Connection.Open();
            return db;
        }

        public static Message Get(string sAMAccountName)
        {
            using (var db = GetJURDataContext())
            {
                return  db.Messages.FirstOrDefault(e => e.Receiver == sAMAccountName&&e.Check==false);
            }
        }

        public static List<Message> GetList(string sAMAccountName)
        {
            using (var db = GetJURDataContext())
            {
                return db.Messages.Where(e => e.Receiver == sAMAccountName && e.Check == false).ToList();
            }
        }

        public static void Click(int ID)
        {
            using (var db = GetJURDataContext())
            {
                var entry = db.Messages.Find(ID);
                if (entry != null)
                {
                    entry.Check = true;
                    db.SaveChanges();
                }
            }
        }
    }
}

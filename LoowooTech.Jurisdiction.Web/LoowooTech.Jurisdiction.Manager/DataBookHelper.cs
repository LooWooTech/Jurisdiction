using LoowooTech.Jurisdiction.Models;
using LoowooTech.Jurisdiction.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Manager
{
    public static class DataBookHelper
    {
        private static JURDbContext GetJURDataContext()
        {
            var db = new JURDbContext();
            db.Database.Connection.Open();
            return db;
        }

        private  static List<DataBook> GetList()
        {
            using (var db = GetJURDataContext())
            {
                return db.DataBooks.Where(e => e.Label == false).ToList();
            }
        }

        public static List<DataBook> GetCheckList()
        {
            var list = GetList();
            var TheList = new List<DataBook>();
            foreach (var item in list)
            {
                if (item.CheckTime > DateTime.MinValue && item.MaturityTime > DateTime.MinValue)
                {
                    if (item.Span.Days < 0 || item.Span.Hours < 0 || item.Span.Minutes < 0 || item.Span.Seconds < 0)
                    {
                        TheList.Add(item);
                    }
                }
               
            }
            return TheList;
        }

        public static void Edit(DataBook Book)
        {
            using (var db = GetJURDataContext())
            {
                var entry = db.DataBooks.Find(Book.ID);
                if (entry != null)
                {
                    db.Entry(entry).CurrentValues.SetValues(Book);
                    db.SaveChanges();
                }
            }
        }
        public static void Records(Record record)
        {
            using (var db = GetJURDataContext())
            {
                var entry = db.Records.FirstOrDefault(e => e.DID == record.DID);
                if (entry != null)
                {
                    record.ID = entry.ID;
                    db.Entry(entry).CurrentValues.SetValues(record);
                }else
                {
                    db.Records.Add(record);
                }

                db.SaveChanges();
            }
        }

        public static bool Examine(DataBook book,out string Error)
        {
            Error = string.Empty;
            if (book.Span.Days > 0 || book.Span.Hours > 0 || book.Span.Minutes > 0 || book.Span.Seconds > 0)
            {
                return false;
            }
            string str = string.Empty;
            if (ADController.DeleteUserFromGroup(book.Name, book.GroupName, out str))
            {
                book.Label = true;
                try
                {
                    Edit(book);
                }catch(Exception ex){
                    Error = ex.Message;
                    return false;
                }
            }
            else
            {
                Error = str;
                return false;
            }
            return true;

        }
    }
}

using LoowooTech.Jurisdiction.Common;
using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Manager
{
    public class DataBookManager:ManagerBase
    {
        public int Add(DataBook Book)
        {
            using (var db = GetJURDataContext())
            {
                db.DataBooks.Add(Book);
                db.SaveChanges();
                return Book.ID;
            }
        }
        public List<int> Add(List<string> Groups, string Name)
        {
            var list = new List<int>();
            foreach (var group in Groups)
            {
                list.Add(Add(new DataBook() { Name = Name, GroupName = group }));
            }
            return list;
        }

        public void Edit(DataBook Book)
        {
            using (var db = GetJURDataContext())
            {
                var entry = db.DataBooks.Find(Book.ID);
                if (entry != null)
                {
                    //entry.Reason = Book.Reason;
                    //entry.Checker = Book.Checker;
                    //entry.Check = Book.Check;
                    Book.ID=entry.ID;
                    db.Entry(entry).CurrentValues.SetValues(Book);
                    db.SaveChanges();
                }
            }
        }
        public DataBook Get(string GroupName)
        {
            using (var db = GetJURDataContext())
            {
                return db.DataBooks.FirstOrDefault(e => e.GroupName == GroupName);
            }
        }
        public List<DataBook> Get(List<int> Indexs)
        {
            var list = new List<DataBook>();
            foreach (var item in Indexs)
            {
                list.Add(Get(item));
            }
            return list;
        }
        public List<DataBook> Get(List<string> GroupNames)
        {
            var list = new List<DataBook>();
            foreach (var item in GroupNames)
            {
                var entry = Get(item);
                if (entry != null)
                {
                    list.Add(entry);
                }
                //list.Add(Get(item));
            }
            return list;
        }

        public DataBook Get(int ID)
        {
            using (var db = GetJURDataContext())
            {
                return db.DataBooks.Find(ID);
            }
        }

        public List<DataBook> GetFinish(string Name)
        {
            using (var db = GetJURDataContext())
            {
                return db.DataBooks.Where(e => e.Checker == Name).OrderByDescending(e => e.ID).ToList();
            }
        }

        public List<DataBook> GetMine(string Name)
        {
            using (var db = GetJURDataContext())
            {
                return db.DataBooks.Where(e => e.Name == Name).OrderByDescending(e => e.ID).ToList();
            }
        }
 
        public void Check(int ID, string Reason,string Checker, bool? Check,int? Day,int? Month,int ?Year)
        {
            if (!Check.HasValue||string.IsNullOrEmpty(Checker))
            {
                return;
            }
            DataBook Book = Get(ID);
            if (Book == null)
            {
                throw new ArgumentException("内部服务器错误!");
            }
            if (Check.Value == true)
            {
                try
                {
                    Core.ADManager.AddUserToGroup(Book.Name, Book.GroupName);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(ex.Message);
                }
            }
            Book.Checker = Checker;
            Book.Reason = Reason;
            Book.CheckTime = DateTime.Now;
            try
            {
                Edit(Book);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
           


        }

        public void Examine(string Name,out string error)
        {
            var list = GetMine(Name);
            error = string.Empty;
            foreach (var item in list)
            {
                if (item.Span.Days < 0 || item.Span.Hours < 0 || item.Span.Minutes < 0 || item.Span.Seconds < 0)
                {
                    try
                    {
                        Core.ADManager.DeleteUserFromGroup(item.Name, item.GroupName);
                    }
                    catch (Exception ex)
                    {
                        error += ex.Message;
                    }
                }
            }
        }

    }
}

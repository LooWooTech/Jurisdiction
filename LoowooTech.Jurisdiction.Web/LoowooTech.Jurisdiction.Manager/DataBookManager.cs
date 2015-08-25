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
        public int Add(DataBook Book,string Name)
        {
            Book.Name = Name;
            Book.Manager = Core.GroupManager.GetAdministrator(Book.GroupName);
            using (var db = GetJURDataContext())
            {
                db.DataBooks.Add(Book);
                db.SaveChanges();
                return Book.ID;
            }
        }

        public void Edit(DataBook Book)
        {
            using (var db = GetJURDataContext())
            {
                var entry = db.DataBooks.Find(Book.ID);
                if (entry != null)
                {
                    entry.Reason = Book.Reason;
                    entry.Checker = Book.Checker;
                    entry.Check = Book.Check;
                    db.SaveChanges();
                }
            }
        }

        public DataBook Get(int ID)
        {
            using (var db = GetJURDataContext())
            {
                return db.DataBooks.Find(ID);
            }
        }

        public int Wait(string Name)
        {
            using (var db = GetJURDataContext())
            {
                return db.DataBooks.Where(e => e.Manager == Name && e.Check == null).Count();
            }
        }

        public List<DataBook> GetWait(UserIdentity Identity)
        {
            List<DataBook> List = null;
            if (Core.GroupManager.IsAdministrator(Identity.Groups))
            {
                List = GetList(null);
            }
            else
            {
                List = GetList(Identity.Name);
            }
            return List;
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


        public List<DataBook> GetList(string Name)
        {
            using (var db = GetJURDataContext())
            {
                return db.DataBooks.Where(e => e.Manager == Name && e.Check == null).OrderByDescending(e=>e.ID).ToList();
            }
        }

        

        public void Check(int ID, string Reason,string Checker, bool? Check)
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
            Book.Check = Check;
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
    }
}

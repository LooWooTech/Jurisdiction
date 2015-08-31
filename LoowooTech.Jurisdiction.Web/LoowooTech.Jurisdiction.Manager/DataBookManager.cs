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
        public List<DataBook> GetListByGroupName(string GroupName)
        {
            using (var db = GetJURDataContext())
            {
                return db.DataBooks.Where(e => e.GroupName == GroupName).ToList();
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
        public List<DataBook> Get(List<string> GroupNames,CheckStatus status)
        {
            var list = new List<DataBook>();
            foreach (var item in GroupNames)
            {
                var glist = GetListByGroupName(item).Where(e=>e.Status==status).ToList();
                if (glist != null)
                {
                    foreach (var entry in glist)
                    {
                        list.Add(entry);
                    }
                }
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

        public List<DataBook> Get(DataBookFilter Filter)
        {
            using (var db = GetJURDataContext())
            {
                var query = db.DataBooks.AsQueryable();
                switch (Filter.Status)
                {
                    case CheckStatus.Agree:
                    case CheckStatus.Disagree:
                    case CheckStatus.Wait:
                        query = query.Where(e => e.Status == Filter.Status);
                        break;
                    case CheckStatus.All:
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(Filter.Name))
                {
                    query = query.Where(e => e.Name == Filter.Name);
                }
                if (!string.IsNullOrEmpty(Filter.Checker))
                {
                    query = query.Where(e => e.Checker == Filter.Checker);
                }
                if (!string.IsNullOrEmpty(Filter.GroupName))
                {
                    query = query.Where(e => e.GroupName == Filter.GroupName);
                }
                if (Filter.Label.HasValue)
                {
                    query = query.Where(e => e.Label == Filter.Label.Value);
                }

                if (Filter.Page != null)
                {
                    Filter.Page.RecordCount = query.Count();
                    query = query.OrderBy(e => e.ID).Skip(Filter.Page.PageSize * (Filter.Page.PageIndex - 1)).Take(Filter.Page.PageSize);
                }
                return query.ToList();
            }
        }


        public List<DataBook> GetList(string Name)
        {
            using (var db = GetJURDataContext())
            {
                return db.DataBooks.Where(e => e.Checker == Name).ToList();
            }
        }


        
 
        public void Check(int ID, string Reason,string Checker,int? Day,bool?Check,CheckStatus status)
        {
            if (string.IsNullOrEmpty(Checker))
            {
                return;
            }
            DataBook Book = Get(ID);
            if (Book == null)
            {
                throw new ArgumentException("内部服务器错误!");
            }
            if (status==CheckStatus.Agree)
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
            if (status != CheckStatus.Wait)
            {
                Book.Checker = Checker;
                Book.Reason = Reason;
                Book.CheckTime = DateTime.Now;
                Book.Status = status;
                DateTime time = Book.CheckTime;
                if (Day.HasValue)
                {
                    time = time.AddDays(Day.Value);
                }
                TimeSpan span = time.Subtract(Book.CheckTime);
                if (span.Days == 0 && span.Minutes == 0 && span.Hours == 0 && span.Seconds == 0)
                {
                    time = new DateTime(9999, 12, 31, 12, 00, 00);
                }
                if (Check.HasValue)
                {
                    if (Check.Value)
                    {
                        time = new DateTime(9999, 12, 31, 12, 00, 00);
                    }
                    
                }
                Book.MaturityTime = time;
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


        /// <summary>
        /// 更新权限列表
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="error"></param>
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

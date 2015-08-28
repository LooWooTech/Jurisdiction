using LoowooTech.Jurisdiction.Models;
using LoowooTech.Jurisdiction.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LoowooTech.Jurisdiction.Manager
{
    public class AuthorizeManager:ManagerBase
    {
        public List<Authorize> GetList()
        {
            using (var db = GetJURDataContext())
            {
                return db.Authorizes.ToList();
            }
        }

        public List<string> GetList(string Name)
        {
            var authorize = GetList().FirstOrDefault(e => e.Manager == Name);
            if (authorize == null)
            {
                return new List<string>();
            }
            var array=authorize.GroupName.Split(',');
            var list = new List<string>();
            foreach (var item in array)
            {
                list.Add(item);
            }
            return list;
        }
        public List<string> GetAllManager()
        {
            return GetList().Select(e => e.Manager).ToList();
        }

        public int Add(Authorize authorize)
        {
            using (var db = GetJURDataContext())
            {
                var entry = db.Authorizes.FirstOrDefault(e => e.Manager == authorize.Manager);
                if (entry != null)
                {
                    throw new ArgumentException("需要修改"+authorize.Manager+"的管理组，请去编辑权限列表");
                }
                db.Authorizes.Add(authorize);
                db.SaveChanges();
                return authorize.ID;
            }
        }

        public Authorize Get(HttpContextBase context)
        {
            return new Authorize()
            {
                GroupName = context.Request.Form["GroupName"],
                Manager = context.Request.Form["Manager"]
            };
        }

        public void Screen(string[] Origin,string sAMAccountName, out List<string> None, out List<string> Have)
        {
            None = new List<string>();
            Have = new List<string>();
            foreach (var item in Origin)
            {
                if (ADController.IsMember(item, sAMAccountName))
                {
                    Have.Add(item);
                }
                else
                {
                    None.Add(item);
                }
            }
        }
    }
}

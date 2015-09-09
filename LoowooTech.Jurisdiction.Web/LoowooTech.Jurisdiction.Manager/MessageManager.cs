using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Manager
{
    public class MessageManager:ManagerBase
    {
        public void Add(Message message)
        {
            using (var db = GetJURDataContext())
            {
                db.Messages.Add(message);
                db.SaveChanges();
            }
        }
        public void Add(List<string> Groups, string Sender,string Receiver)
        {
            var sb = new StringBuilder();
            foreach (var item in Groups)
            {
                sb.Append(item + "、");
            }
            Add(new Message()
            {
                Sender = Sender,
                Info = String.Format("申请{0}的权限", sb.ToString()),
                Receiver = Receiver
            });
        }

        public void Add(DataBook book, string Sender)
        {
            Add(new Message()
            {
                Sender = Sender,
                Info = String.Format("申请{0}的权限已经确认！", book.GroupName),
                Receiver = book.Name
            });
        }
    }
}

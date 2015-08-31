using LoowooTech.Jurisdiction.Manager;
using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Jurisdiction.Service
{
    public static class Operate
    {
        public static void  Process()
        {
            var list = DataBookHelper.GetCheckList();
            string error;
            foreach (var item in list)
            {
                error = string.Empty;
                var record = new Record()
                {
                    DID = item.ID
                };
                if (DataBookHelper.Examine(item, out error))
                {
                    record.Flag = true;
                }
                else
                {
                    record.Flag = false;
                }
                record.Mark = error;
                DataBookHelper.Records(record);
            }
        }
    }
}

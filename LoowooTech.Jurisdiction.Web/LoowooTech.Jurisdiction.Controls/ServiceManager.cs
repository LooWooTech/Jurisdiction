using LoowooTech.Jurisdiction.Manager;
using LoowooTech.Jurisdiction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LoowooTech.Jurisdiction.Controls
{
    public class ServiceManager
    {
        private Thread thread { get; set; }
        private bool signal { get; set; }
        public void Start()
        {
            signal = true;
            thread = new Thread(MainLoop);
            thread.Start();
        }

        public void Stop()
        {
            signal = false;
            thread.Join();
        }
        public void MainLoop()
        {
            while (true)
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
                    try
                    {
                        DataBookHelper.Records(record);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                Thread.Sleep(500);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace LoowooTech.Jurisdiction.Service
{
    public class ServiceManager
    {
        private Thread thread;
        private bool signal = false;
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
            try
            {
                var loopInterval = int.Parse(ConfigurationManager.AppSettings["LoopInterval"]);
                while (signal)
                {
                    Operate.Process();
                    Thread.Sleep(loopInterval);
                }
            }
            catch (Exception ex)
            {
                var Loggerror = log4net.LogManager.GetLogger("logerror");
                Loggerror.ErrorFormat("服务遍历操作时发生错误：{0}", ex);
            }
            

        }
    }
}

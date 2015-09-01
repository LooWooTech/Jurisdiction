using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoowooTech.Jurisdiction.Manager;
using System.Threading;

namespace LoowooTech.Jurisdiction.Controls
{
    class Program
    {
        static void Main(string[] args)
        {
            string name=System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            Console.WriteLine(name);
            Console.Read();
            //var manager = new ServiceManager();
            //manager.MainLoop();
        }
    }
}

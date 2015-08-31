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
            var manager = new ServiceManager();
            manager.MainLoop();
        }
    }
}

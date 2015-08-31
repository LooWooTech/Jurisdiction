using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace LoowooTech.Jurisdiction.Service
{
    public partial class Jurisdiction : ServiceBase
    {
        private ServiceManager manager = new ServiceManager();
        public Jurisdiction()
        {
            InitializeComponent();
            this.ServiceName = "LoowooTech_Jurisdiction";
        }
        public void DebugOnStart()
        {
            this.OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            manager.Start();
        }

        protected override void OnStop()
        {
            manager.Stop();
        }
    }
}

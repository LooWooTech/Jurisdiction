using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace LoowooTech.Jurisdiction.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        ServiceProcessInstaller processInstall;
        ServiceInstaller serviceInstall;
        public ProjectInstaller()
        {
            this.processInstall = new ServiceProcessInstaller();
            this.serviceInstall = new ServiceInstaller();

            processInstall.Account = ServiceAccount.LocalSystem;
            this.serviceInstall.ServiceName = "LoowooTech-Jurisdiction";
            this.serviceInstall.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            this.Installers.Add(this.serviceInstall);
            this.Installers.Add(this.processInstall);
        }
    }
}

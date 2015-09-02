using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Jurisdiction.WinForm
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            this.ComputerName.Text = Environment.UserDomainName;
            this.LoginName.Text = Environment.UserName;
            this.WindowsName.Text = GetOSName();
            this.DomainName.Text = GetDomainName();
        }
        private string GetOSName()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT Caption FROM Win32_operatingSystem");
            foreach (ManagementObject queryobj in searcher.Get())
            {
                return queryobj["Caption"] as string;
            }
            return null;
        }
        private string GetDomainName()
        {
            SelectQuery query = new SelectQuery("Win32_ComputerSystem");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject mo in searcher.Get())
                {
                    if ((bool)mo["partofdomain"])
                    {
                        return mo["domain"].ToString();
                    }
                }
            }
            return string.Empty;
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("您确定退出程序", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}

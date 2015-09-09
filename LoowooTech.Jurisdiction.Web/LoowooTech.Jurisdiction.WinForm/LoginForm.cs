using LoowooTech.Jurisdiction.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LoowooTech.Jurisdiction.WinForm
{
    public partial class LoginForm : Form
    {
        private Thread Thread { get; set; }
        private string sAMAccountName { get; set; }
        public LoginForm()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            this.sAMAccountName = "wjl";
            this.ComputerName.Text = Environment.UserDomainName;
            this.LoginName.Text = Environment.UserName;
            this.WindowsName.Text = GetOSName();
            this.DomainName.Text = GetDomainName();
            this.Thread = new Thread(Detect);
            this.Thread.Start();
        }
        /// <summary>
        /// 获取客户端操作系统
        /// </summary>
        /// <returns></returns>
        private string GetOSName()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT Caption FROM Win32_operatingSystem");
            foreach (ManagementObject queryobj in searcher.Get())
            {
                return queryobj["Caption"] as string;
            }
            return null;
        }
        /// <summary>
        /// 获取域名
        /// </summary>
        /// <returns></returns>
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
                this.Thread.Join();
                e.Cancel = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoowooTech.Jurisdiction.Models.Message entry = MessageHelper.Get(sAMAccountName);
            var box = new Infomation(string.Format("{0}{1}",entry.Sender,entry.Info),entry.ID);
            box.Show();
        }

        private void Detect()
        {
            while (true)
            {
                foreach (var item in MessageHelper.GetList(sAMAccountName))
                {
                    var box = new Infomation(string.Format("{0}{1}", item.Sender, item.Info), item.ID);
                    box.ShowDialog();
                    Thread.Sleep(50000);
                }
                Thread.Sleep(50000);
            }
        }
    }
}

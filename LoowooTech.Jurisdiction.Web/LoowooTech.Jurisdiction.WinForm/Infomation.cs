using LoowooTech.Jurisdiction.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Jurisdiction.WinForm
{
    public partial class Infomation : Form
    {
        private string Message { get; set; }
        private int ID { get; set; }

        [DllImport("user32")]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);

        /// <summary>
        /// 自左向右显示窗口，该标志可以在滚动动画和滑动动画中使用。当使用AW_CENTER标志时，该标志将被忽略
        /// </summary>
        public const Int32 AW_HOR_POSITIVE = 0x00000001;
        /// <summary>
        /// 自右向左显示窗口。当使用了AW_CENTER标志时该标志被忽略
        /// </summary>
        public const Int32 AW_HOR_NEGATIVE = 0x00000002;
        /// <summary>  
        /// 自顶向下显示窗口。该标志可以在滚动动画和滑动动画中使用。当使用AW_CENTER标志时，该标志将被忽略  
        /// </summary>
        public const Int32 AW_VER_POSITIVE = 0x00000004;
        /// <summary>  
        /// 自下向上显示窗口。该标志可以在滚动动画和滑动动画中使用。当使用AW_CENTER标志时，该标志将被忽略  
        /// </summary>  
        public const Int32 AW_VER_NEGATIVE = 0x00000008;
        /// <summary>  
        /// 若使用了AW_HIDE标志，则使窗口向内重叠；若未使用AW_HIDE标志，则使窗口向外扩展  
        /// </summary>  
        public const Int32 AW_CENTER = 0x00000010;
        /// <summary>  
        /// 隐藏窗口，缺省则显示窗口  
        /// </summary>  
        public const Int32 AW_HIDE = 0x00010000;
        /// <summary>  
        /// 激活窗口。在使用了AW_HIDE标志后不要使用这个标志  
        /// </summary>  
        public const Int32 AW_ACTIVATE = 0x00020000;
        /// <summary>  
        /// 使用滑动类型。缺省则为滚动动画类型。当使用AW_CENTER标志时，这个标志就被忽略  
        /// </summary>
        public const Int32 AW_SLIDE = 0x00040000;
        /// <summary>  
        /// 使用淡入效果。只有当hWnd为顶层窗口的时候才可以使用此标志  
        /// </summary>  
        public const Int32 AW_BLEND = 0x00080000;  
        public Infomation(string Message,int ID)
        {
            this.ID = ID;
            this.Message = Message;
            InitializeComponent();
        }

        private void Infomation_Load(object sender, EventArgs e)
        {
            this.label1.Text = Message;
            this.linkLabel1.Text = "打开网页";
            this.linkLabel1.Links.Add(0, 1023, "http://10.22.102.3:9999");
            int x = Screen.PrimaryScreen.WorkingArea.Right - this.Width;
            int y = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;
            this.Location = new Point(x, y);
            AnimateWindow(this.Handle, 1000, AW_SLIDE | AW_ACTIVATE | AW_VER_NEGATIVE);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.Links[linkLabel1.Links.IndexOf(e.Link)].Visited = true;
            string targetUrl = e.Link.LinkData as string;
            if (string.IsNullOrEmpty(targetUrl))
            {
                MessageBox.Show("没有连接地址！");
            }
            else
            {
                System.Diagnostics.Process.Start(targetUrl);
            }
            try 
            {
                MessageHelper.Click(ID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            
        }

        private void Infomation_FormClosing(object sender, FormClosingEventArgs e)
        {
            AnimateWindow(this.Handle, 1000, AW_BLEND | AW_HIDE);
        }
    }

    public enum LoadMode
    {
        /// <summary>  
        /// 警告  
        /// </summary>  
        Warning,


        /// <summary>  
        /// 错误  
        /// </summary>  
        Error,


        /// <summary>  
        /// 提示  
        /// </summary>  
        Prompt  
    }
}

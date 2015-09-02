namespace LoowooTech.Jurisdiction.WinForm
{
    partial class LoginForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ComputerName = new System.Windows.Forms.Label();
            this.LoginName = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.WindowsName = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.DomainName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("华文楷体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "计算机名:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("华文楷体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(49, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "当前Windows信息";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("华文楷体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(12, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "登录名:";
            // 
            // ComputerName
            // 
            this.ComputerName.AutoSize = true;
            this.ComputerName.Font = new System.Drawing.Font("华文楷体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComputerName.Location = new System.Drawing.Point(99, 71);
            this.ComputerName.Name = "ComputerName";
            this.ComputerName.Size = new System.Drawing.Size(51, 18);
            this.ComputerName.TabIndex = 3;
            this.ComputerName.Text = "label4";
            // 
            // LoginName
            // 
            this.LoginName.AutoSize = true;
            this.LoginName.Font = new System.Drawing.Font("华文楷体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LoginName.Location = new System.Drawing.Point(99, 114);
            this.LoginName.Name = "LoginName";
            this.LoginName.Size = new System.Drawing.Size(51, 18);
            this.LoginName.TabIndex = 4;
            this.LoginName.Text = "label5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "当前操作系统：";
            // 
            // WindowsName
            // 
            this.WindowsName.AutoSize = true;
            this.WindowsName.Location = new System.Drawing.Point(116, 153);
            this.WindowsName.Name = "WindowsName";
            this.WindowsName.Size = new System.Drawing.Size(35, 13);
            this.WindowsName.TabIndex = 6;
            this.WindowsName.Text = "label7";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 193);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "域名：";
            // 
            // DomainName
            // 
            this.DomainName.AutoSize = true;
            this.DomainName.Location = new System.Drawing.Point(102, 193);
            this.DomainName.Name = "DomainName";
            this.DomainName.Size = new System.Drawing.Size(35, 13);
            this.DomainName.TabIndex = 8;
            this.DomainName.Text = "label9";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(267, 262);
            this.Controls.Add(this.DomainName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.WindowsName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.LoginName);
            this.Controls.Add(this.ComputerName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "LoginForm";
            this.Text = "杭州智拓域信息";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoginForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label ComputerName;
        private System.Windows.Forms.Label LoginName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label WindowsName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label DomainName;
    }
}


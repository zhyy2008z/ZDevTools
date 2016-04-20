namespace ZDevTools.ServiceConsole
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lbConsole = new System.Windows.Forms.ListBox();
            this.pDown = new System.Windows.Forms.Panel();
            this.bInstall = new System.Windows.Forms.Button();
            this.bConfigOneKeyStart = new System.Windows.Forms.Button();
            this.bOneKeyStart = new System.Windows.Forms.Button();
            this.lService = new System.Windows.Forms.Label();
            this.bStopAll = new System.Windows.Forms.Button();
            this.niMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.cms4Ni = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.bRefreshStatus = new System.Windows.Forms.Button();
            this.pDown.SuspendLayout();
            this.cms4Ni.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbConsole
            // 
            this.lbConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbConsole.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbConsole.FormattingEnabled = true;
            this.lbConsole.ItemHeight = 12;
            this.lbConsole.Location = new System.Drawing.Point(12, 12);
            this.lbConsole.Name = "lbConsole";
            this.lbConsole.Size = new System.Drawing.Size(900, 160);
            this.lbConsole.TabIndex = 1;
            this.lbConsole.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbConsole_DrawItem);
            this.lbConsole.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbConsole_KeyDown);
            // 
            // pDown
            // 
            this.pDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pDown.AutoScroll = true;
            this.pDown.Controls.Add(this.bRefreshStatus);
            this.pDown.Controls.Add(this.bInstall);
            this.pDown.Controls.Add(this.bConfigOneKeyStart);
            this.pDown.Controls.Add(this.bOneKeyStart);
            this.pDown.Controls.Add(this.lService);
            this.pDown.Controls.Add(this.bStopAll);
            this.pDown.Location = new System.Drawing.Point(12, 189);
            this.pDown.Name = "pDown";
            this.pDown.Size = new System.Drawing.Size(900, 330);
            this.pDown.TabIndex = 5;
            // 
            // bInstall
            // 
            this.bInstall.Location = new System.Drawing.Point(575, 9);
            this.bInstall.Name = "bInstall";
            this.bInstall.Size = new System.Drawing.Size(75, 23);
            this.bInstall.TabIndex = 6;
            this.bInstall.Text = "安装服务";
            this.bInstall.UseVisualStyleBackColor = true;
            this.bInstall.Click += new System.EventHandler(this.bInstall_Click);
            // 
            // bConfigOneKeyStart
            // 
            this.bConfigOneKeyStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bConfigOneKeyStart.Location = new System.Drawing.Point(656, 9);
            this.bConfigOneKeyStart.Name = "bConfigOneKeyStart";
            this.bConfigOneKeyStart.Size = new System.Drawing.Size(75, 23);
            this.bConfigOneKeyStart.TabIndex = 5;
            this.bConfigOneKeyStart.Text = "设置";
            this.bConfigOneKeyStart.UseVisualStyleBackColor = true;
            this.bConfigOneKeyStart.Click += new System.EventHandler(this.bConfigOneKeyStart_Click);
            // 
            // bOneKeyStart
            // 
            this.bOneKeyStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bOneKeyStart.Location = new System.Drawing.Point(737, 9);
            this.bOneKeyStart.Name = "bOneKeyStart";
            this.bOneKeyStart.Size = new System.Drawing.Size(75, 23);
            this.bOneKeyStart.TabIndex = 4;
            this.bOneKeyStart.Text = "一键启动";
            this.bOneKeyStart.UseVisualStyleBackColor = true;
            this.bOneKeyStart.Click += new System.EventHandler(this.bOneKeyStart_Click);
            // 
            // lService
            // 
            this.lService.AutoSize = true;
            this.lService.Location = new System.Drawing.Point(18, 14);
            this.lService.Name = "lService";
            this.lService.Size = new System.Drawing.Size(53, 12);
            this.lService.TabIndex = 3;
            this.lService.Text = "全部服务";
            // 
            // bStopAll
            // 
            this.bStopAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bStopAll.Location = new System.Drawing.Point(818, 9);
            this.bStopAll.Name = "bStopAll";
            this.bStopAll.Size = new System.Drawing.Size(75, 23);
            this.bStopAll.TabIndex = 2;
            this.bStopAll.Text = "全部停用";
            this.bStopAll.UseVisualStyleBackColor = true;
            this.bStopAll.Click += new System.EventHandler(this.bDisableAll_Click);
            // 
            // niMain
            // 
            this.niMain.ContextMenuStrip = this.cms4Ni;
            this.niMain.Icon = ((System.Drawing.Icon)(resources.GetObject("niMain.Icon")));
            this.niMain.Visible = true;
            this.niMain.DoubleClick += new System.EventHandler(this.niMain_DoubleClick);
            // 
            // cms4Ni
            // 
            this.cms4Ni.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiExit});
            this.cms4Ni.Name = "cms4Ni";
            this.cms4Ni.Size = new System.Drawing.Size(101, 26);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(100, 22);
            this.tsmiExit.Text = "退出";
            this.tsmiExit.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // bRefreshStatus
            // 
            this.bRefreshStatus.Location = new System.Drawing.Point(494, 9);
            this.bRefreshStatus.Name = "bRefreshStatus";
            this.bRefreshStatus.Size = new System.Drawing.Size(75, 23);
            this.bRefreshStatus.TabIndex = 7;
            this.bRefreshStatus.Text = "刷新状态";
            this.bRefreshStatus.UseVisualStyleBackColor = true;
            this.bRefreshStatus.Click += new System.EventHandler(this.bRefreshStatus_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(924, 531);
            this.Controls.Add(this.pDown);
            this.Controls.Add(this.lbConsole);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.pDown.ResumeLayout(false);
            this.pDown.PerformLayout();
            this.cms4Ni.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListBox lbConsole;
        private System.Windows.Forms.Panel pDown;
        private System.Windows.Forms.Button bStopAll;
        private System.Windows.Forms.NotifyIcon niMain;
        private System.Windows.Forms.ContextMenuStrip cms4Ni;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.Label lService;
        private System.Windows.Forms.Button bConfigOneKeyStart;
        private System.Windows.Forms.Button bOneKeyStart;
        private System.Windows.Forms.Button bInstall;
        private System.Windows.Forms.Button bRefreshStatus;
    }
}


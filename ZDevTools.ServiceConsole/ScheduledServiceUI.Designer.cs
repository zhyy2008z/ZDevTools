namespace ZDevTools.ServiceConsole
{
    partial class ScheduledServiceUI
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lServiceName = new System.Windows.Forms.Label();
            this.lStatus = new System.Windows.Forms.Label();
            this.bOperation = new System.Windows.Forms.Button();
            this.cbImmediately = new System.Windows.Forms.CheckBox();
            this.lDescription = new System.Windows.Forms.Label();
            this.bManageSchedule = new System.Windows.Forms.Button();
            this.ttMain = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lServiceName
            // 
            this.lServiceName.AutoEllipsis = true;
            this.lServiceName.Location = new System.Drawing.Point(24, 26);
            this.lServiceName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lServiceName.Name = "lServiceName";
            this.lServiceName.Size = new System.Drawing.Size(340, 24);
            this.lServiceName.TabIndex = 0;
            this.lServiceName.Text = "服务名称";
            this.lServiceName.Click += new System.EventHandler(this.lServiceName_Click);
            // 
            // lStatus
            // 
            this.lStatus.AutoSize = true;
            this.lStatus.Location = new System.Drawing.Point(376, 26);
            this.lStatus.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(82, 24);
            this.lStatus.TabIndex = 1;
            this.lStatus.Text = "已停止";
            this.lStatus.Click += new System.EventHandler(this.lServiceName_Click);
            // 
            // bOperation
            // 
            this.bOperation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bOperation.Location = new System.Drawing.Point(1598, 16);
            this.bOperation.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.bOperation.Name = "bOperation";
            this.bOperation.Size = new System.Drawing.Size(150, 46);
            this.bOperation.TabIndex = 5;
            this.bOperation.Text = "启用";
            this.bOperation.UseVisualStyleBackColor = true;
            this.bOperation.Click += new System.EventHandler(this.bOperation_Click);
            // 
            // cbImmediately
            // 
            this.cbImmediately.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbImmediately.AutoSize = true;
            this.cbImmediately.Checked = true;
            this.cbImmediately.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbImmediately.Location = new System.Drawing.Point(1456, 24);
            this.cbImmediately.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cbImmediately.Name = "cbImmediately";
            this.cbImmediately.Size = new System.Drawing.Size(138, 28);
            this.cbImmediately.TabIndex = 13;
            this.cbImmediately.Text = "立即执行";
            this.cbImmediately.UseVisualStyleBackColor = true;
            // 
            // lDescription
            // 
            this.lDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lDescription.AutoEllipsis = true;
            this.lDescription.Location = new System.Drawing.Point(590, 26);
            this.lDescription.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lDescription.Name = "lDescription";
            this.lDescription.Size = new System.Drawing.Size(734, 30);
            this.lDescription.TabIndex = 14;
            this.lDescription.Text = "没有计划";
            this.lDescription.Click += new System.EventHandler(this.lServiceName_Click);
            // 
            // bManageSchedule
            // 
            this.bManageSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bManageSchedule.Location = new System.Drawing.Point(1336, 16);
            this.bManageSchedule.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.bManageSchedule.Name = "bManageSchedule";
            this.bManageSchedule.Size = new System.Drawing.Size(102, 46);
            this.bManageSchedule.TabIndex = 15;
            this.bManageSchedule.Text = "管理";
            this.bManageSchedule.UseVisualStyleBackColor = true;
            this.bManageSchedule.Click += new System.EventHandler(this.bManageSchedule_Click);
            // 
            // ScheduledServiceUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bManageSchedule);
            this.Controls.Add(this.lDescription);
            this.Controls.Add(this.cbImmediately);
            this.Controls.Add(this.bOperation);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.lServiceName);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "ScheduledServiceUI";
            this.Size = new System.Drawing.Size(1754, 78);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lServiceName;
        private System.Windows.Forms.Label lStatus;
        private System.Windows.Forms.Button bOperation;
        private System.Windows.Forms.CheckBox cbImmediately;
        private System.Windows.Forms.Label lDescription;
        private System.Windows.Forms.Button bManageSchedule;
        private System.Windows.Forms.ToolTip ttMain;
    }
}

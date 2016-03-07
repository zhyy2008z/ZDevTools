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
            this.lServiceName = new System.Windows.Forms.Label();
            this.lStatus = new System.Windows.Forms.Label();
            this.bOperation = new System.Windows.Forms.Button();
            this.cbImmediately = new System.Windows.Forms.CheckBox();
            this.lDescription = new System.Windows.Forms.Label();
            this.bManageSchedule = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lServiceName
            // 
            this.lServiceName.AutoEllipsis = true;
            this.lServiceName.Location = new System.Drawing.Point(12, 13);
            this.lServiceName.Name = "lServiceName";
            this.lServiceName.Size = new System.Drawing.Size(170, 12);
            this.lServiceName.TabIndex = 0;
            this.lServiceName.Text = "服务名称";
            // 
            // lStatus
            // 
            this.lStatus.AutoSize = true;
            this.lStatus.Location = new System.Drawing.Point(188, 13);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(41, 12);
            this.lStatus.TabIndex = 1;
            this.lStatus.Text = "已停止";
            // 
            // bOperation
            // 
            this.bOperation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bOperation.Location = new System.Drawing.Point(799, 8);
            this.bOperation.Name = "bOperation";
            this.bOperation.Size = new System.Drawing.Size(75, 23);
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
            this.cbImmediately.Location = new System.Drawing.Point(725, 12);
            this.cbImmediately.Name = "cbImmediately";
            this.cbImmediately.Size = new System.Drawing.Size(72, 16);
            this.cbImmediately.TabIndex = 13;
            this.cbImmediately.Text = "立即执行";
            this.cbImmediately.UseVisualStyleBackColor = true;
            // 
            // lDescription
            // 
            this.lDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lDescription.AutoEllipsis = true;
            this.lDescription.Location = new System.Drawing.Point(295, 13);
            this.lDescription.Name = "lDescription";
            this.lDescription.Size = new System.Drawing.Size(367, 15);
            this.lDescription.TabIndex = 14;
            this.lDescription.Text = "没有计划";
            // 
            // bManageSchedule
            // 
            this.bManageSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bManageSchedule.Location = new System.Drawing.Point(668, 8);
            this.bManageSchedule.Name = "bManageSchedule";
            this.bManageSchedule.Size = new System.Drawing.Size(51, 23);
            this.bManageSchedule.TabIndex = 15;
            this.bManageSchedule.Text = "管理";
            this.bManageSchedule.UseVisualStyleBackColor = true;
            this.bManageSchedule.Click += new System.EventHandler(this.bManageSchedule_Click);
            // 
            // ScheduledServiceUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bManageSchedule);
            this.Controls.Add(this.lDescription);
            this.Controls.Add(this.cbImmediately);
            this.Controls.Add(this.bOperation);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.lServiceName);
            this.Name = "ScheduledServiceUI";
            this.Size = new System.Drawing.Size(877, 39);
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
    }
}

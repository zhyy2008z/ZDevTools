namespace ZDevTools.ServiceConsole
{
    partial class HostedServiceUI
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
            this.bOperation = new System.Windows.Forms.Button();
            this.lStatus = new System.Windows.Forms.Label();
            this.lServiceName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bOperation
            // 
            this.bOperation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bOperation.Location = new System.Drawing.Point(1598, 16);
            this.bOperation.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.bOperation.Name = "bOperation";
            this.bOperation.Size = new System.Drawing.Size(150, 46);
            this.bOperation.TabIndex = 17;
            this.bOperation.Text = "启动";
            this.bOperation.UseVisualStyleBackColor = true;
            this.bOperation.Click += new System.EventHandler(this.bOperation_Click);
            // 
            // lStatus
            // 
            this.lStatus.AutoSize = true;
            this.lStatus.Location = new System.Drawing.Point(376, 26);
            this.lStatus.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(82, 24);
            this.lStatus.TabIndex = 15;
            this.lStatus.Text = "已停止";
            this.lStatus.Click += new System.EventHandler(this.lServiceName_Click);
            // 
            // lServiceName
            // 
            this.lServiceName.AutoSize = true;
            this.lServiceName.Location = new System.Drawing.Point(24, 26);
            this.lServiceName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lServiceName.Name = "lServiceName";
            this.lServiceName.Size = new System.Drawing.Size(106, 24);
            this.lServiceName.TabIndex = 14;
            this.lServiceName.Text = "服务名称";
            this.lServiceName.Click += new System.EventHandler(this.lServiceName_Click);
            // 
            // HostedServiceUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bOperation);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.lServiceName);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "HostedServiceUI";
            this.Size = new System.Drawing.Size(1754, 78);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bOperation;
        private System.Windows.Forms.Label lStatus;
        private System.Windows.Forms.Label lServiceName;
    }
}

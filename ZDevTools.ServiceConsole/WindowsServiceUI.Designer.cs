namespace ZDevTools.ServiceConsole
{
    partial class WindowsServiceUI
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
            this.bApply = new System.Windows.Forms.Button();
            this.lStartupType = new System.Windows.Forms.Label();
            this.cbStartupType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // bApply
            // 
            this.bApply.Location = new System.Drawing.Point(489, 8);
            this.bApply.Name = "bApply";
            this.bApply.Size = new System.Drawing.Size(75, 23);
            this.bApply.TabIndex = 18;
            this.bApply.Text = "应用";
            this.bApply.UseVisualStyleBackColor = true;
            this.bApply.Click += new System.EventHandler(this.bApply_Click);
            // 
            // lStartupType
            // 
            this.lStartupType.AutoSize = true;
            this.lStartupType.Location = new System.Drawing.Point(295, 13);
            this.lStartupType.Name = "lStartupType";
            this.lStartupType.Size = new System.Drawing.Size(65, 12);
            this.lStartupType.TabIndex = 19;
            this.lStartupType.Text = "启动类型：";
            this.lStartupType.Click += new System.EventHandler(this.lStartupType_Click);
            // 
            // cbStartupType
            // 
            this.cbStartupType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStartupType.FormattingEnabled = true;
            this.cbStartupType.Items.AddRange(new object[] {
            "自动（延迟启动）",
            "自动",
            "手动",
            "禁用"});
            this.cbStartupType.Location = new System.Drawing.Point(361, 9);
            this.cbStartupType.Name = "cbStartupType";
            this.cbStartupType.Size = new System.Drawing.Size(121, 20);
            this.cbStartupType.TabIndex = 20;
            // 
            // WindowsServiceUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbStartupType);
            this.Controls.Add(this.lStartupType);
            this.Controls.Add(this.bApply);
            this.Name = "WindowsServiceUI";
            this.Controls.SetChildIndex(this.bApply, 0);
            this.Controls.SetChildIndex(this.lStartupType, 0);
            this.Controls.SetChildIndex(this.cbStartupType, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bApply;
        private System.Windows.Forms.Label lStartupType;
        private System.Windows.Forms.ComboBox cbStartupType;
    }
}

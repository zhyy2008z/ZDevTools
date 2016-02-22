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
            this.label2 = new System.Windows.Forms.Label();
            this.bOperation = new System.Windows.Forms.Button();
            this.cbAutoRun = new System.Windows.Forms.CheckBox();
            this.tJob = new System.Windows.Forms.Timer(this.components);
            this.rbRecycle = new System.Windows.Forms.RadioButton();
            this.rbOnTime = new System.Windows.Forms.RadioButton();
            this.mtbOntime = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mtbIntervalTime = new System.Windows.Forms.MaskedTextBox();
            this.cbImmediately = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lServiceName
            // 
            this.lServiceName.AutoSize = true;
            this.lServiceName.Location = new System.Drawing.Point(12, 13);
            this.lServiceName.Name = "lServiceName";
            this.lServiceName.Size = new System.Drawing.Size(53, 12);
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(466, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "执行一次";
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
            // cbAutoRun
            // 
            this.cbAutoRun.AutoSize = true;
            this.cbAutoRun.Checked = true;
            this.cbAutoRun.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoRun.Location = new System.Drawing.Point(281, 11);
            this.cbAutoRun.Name = "cbAutoRun";
            this.cbAutoRun.Size = new System.Drawing.Size(48, 16);
            this.cbAutoRun.TabIndex = 6;
            this.cbAutoRun.Text = "自动";
            this.cbAutoRun.UseVisualStyleBackColor = true;
            this.cbAutoRun.CheckedChanged += new System.EventHandler(this.cbAutoRun_CheckedChanged);
            // 
            // tJob
            // 
            this.tJob.Interval = 10000;
            this.tJob.Tick += new System.EventHandler(this.tJob_Tick);
            // 
            // rbRecycle
            // 
            this.rbRecycle.AutoSize = true;
            this.rbRecycle.Checked = true;
            this.rbRecycle.Location = new System.Drawing.Point(334, 11);
            this.rbRecycle.Name = "rbRecycle";
            this.rbRecycle.Size = new System.Drawing.Size(47, 16);
            this.rbRecycle.TabIndex = 7;
            this.rbRecycle.TabStop = true;
            this.rbRecycle.Text = "每隔";
            this.rbRecycle.UseVisualStyleBackColor = true;
            this.rbRecycle.CheckedChanged += new System.EventHandler(this.rbRecycle_CheckedChanged);
            // 
            // rbOnTime
            // 
            this.rbOnTime.AutoSize = true;
            this.rbOnTime.Location = new System.Drawing.Point(527, 11);
            this.rbOnTime.Name = "rbOnTime";
            this.rbOnTime.Size = new System.Drawing.Size(47, 16);
            this.rbOnTime.TabIndex = 8;
            this.rbOnTime.TabStop = true;
            this.rbOnTime.Text = "每天";
            this.rbOnTime.UseVisualStyleBackColor = true;
            // 
            // mtbOntime
            // 
            this.mtbOntime.Location = new System.Drawing.Point(573, 9);
            this.mtbOntime.Mask = "90:00";
            this.mtbOntime.Name = "mtbOntime";
            this.mtbOntime.Size = new System.Drawing.Size(38, 21);
            this.mtbOntime.TabIndex = 10;
            this.mtbOntime.Text = "1000";
            this.mtbOntime.ValidatingType = typeof(System.DateTime);
            this.mtbOntime.ModifiedChanged += new System.EventHandler(this.mtbOntime_ModifiedChanged);
            this.mtbOntime.Validating += new System.ComponentModel.CancelEventHandler(this.mtbOntime_Validating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(614, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "执行一次";
            // 
            // mtbIntervalTime
            // 
            this.mtbIntervalTime.Location = new System.Drawing.Point(382, 10);
            this.mtbIntervalTime.Mask = "90时90分90秒";
            this.mtbIntervalTime.Name = "mtbIntervalTime";
            this.mtbIntervalTime.Size = new System.Drawing.Size(80, 21);
            this.mtbIntervalTime.TabIndex = 12;
            this.mtbIntervalTime.Text = "003000";
            this.mtbIntervalTime.ModifiedChanged += new System.EventHandler(this.mtbIntervalTime_ModifiedChanged);
            this.mtbIntervalTime.Validating += new System.ComponentModel.CancelEventHandler(this.mtbIntervalTime_Validating);
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
            // ScheduledServiceUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbImmediately);
            this.Controls.Add(this.mtbIntervalTime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mtbOntime);
            this.Controls.Add(this.rbOnTime);
            this.Controls.Add(this.rbRecycle);
            this.Controls.Add(this.cbAutoRun);
            this.Controls.Add(this.bOperation);
            this.Controls.Add(this.label2);
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bOperation;
        private System.Windows.Forms.CheckBox cbAutoRun;
        private System.Windows.Forms.Timer tJob;
        private System.Windows.Forms.RadioButton rbRecycle;
        private System.Windows.Forms.RadioButton rbOnTime;
        private System.Windows.Forms.MaskedTextBox mtbOntime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox mtbIntervalTime;
        private System.Windows.Forms.CheckBox cbImmediately;
    }
}

namespace ZDevTools.ServiceConsole
{
    partial class ScheduleForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbPerWeek = new System.Windows.Forms.RadioButton();
            this.rbPerDay = new System.Windows.Forms.RadioButton();
            this.rbOneTime = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lWeek = new System.Windows.Forms.Label();
            this.nudWeeks = new System.Windows.Forms.NumericUpDown();
            this.clbWeekDays = new System.Windows.Forms.CheckedListBox();
            this.lDay = new System.Windows.Forms.Label();
            this.nudDays = new System.Windows.Forms.NumericUpDown();
            this.lPer = new System.Windows.Forms.Label();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mtbRepeatUntil = new System.Windows.Forms.MaskedTextBox();
            this.dtpUntilTime = new System.Windows.Forms.DateTimePicker();
            this.dtpUntilDate = new System.Windows.Forms.DateTimePicker();
            this.cbUntil = new System.Windows.Forms.CheckBox();
            this.cbEnabled = new System.Windows.Forms.CheckBox();
            this.cbRepeatUntil = new System.Windows.Forms.CheckBox();
            this.cbRepeat = new System.Windows.Forms.CheckBox();
            this.mtbRepeatInterval = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bOK = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeeks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDays)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbPerWeek);
            this.groupBox1.Controls.Add(this.rbPerDay);
            this.groupBox1.Controls.Add(this.rbOneTime);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.dtpStartTime);
            this.groupBox1.Controls.Add(this.dtpStartDate);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(634, 173);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "设置";
            // 
            // rbPerWeek
            // 
            this.rbPerWeek.AutoSize = true;
            this.rbPerWeek.Location = new System.Drawing.Point(30, 84);
            this.rbPerWeek.Name = "rbPerWeek";
            this.rbPerWeek.Size = new System.Drawing.Size(47, 16);
            this.rbPerWeek.TabIndex = 7;
            this.rbPerWeek.TabStop = true;
            this.rbPerWeek.Text = "每周";
            this.rbPerWeek.UseVisualStyleBackColor = true;
            this.rbPerWeek.CheckedChanged += new System.EventHandler(this.rbPerWeek_CheckedChanged);
            // 
            // rbPerDay
            // 
            this.rbPerDay.AutoSize = true;
            this.rbPerDay.Location = new System.Drawing.Point(30, 57);
            this.rbPerDay.Name = "rbPerDay";
            this.rbPerDay.Size = new System.Drawing.Size(47, 16);
            this.rbPerDay.TabIndex = 6;
            this.rbPerDay.TabStop = true;
            this.rbPerDay.Text = "每天";
            this.rbPerDay.UseVisualStyleBackColor = true;
            this.rbPerDay.CheckedChanged += new System.EventHandler(this.rbPerDay_CheckedChanged);
            // 
            // rbOneTime
            // 
            this.rbOneTime.AutoSize = true;
            this.rbOneTime.Location = new System.Drawing.Point(30, 28);
            this.rbOneTime.Name = "rbOneTime";
            this.rbOneTime.Size = new System.Drawing.Size(47, 16);
            this.rbOneTime.TabIndex = 0;
            this.rbOneTime.TabStop = true;
            this.rbOneTime.Text = "一次";
            this.rbOneTime.UseVisualStyleBackColor = true;
            this.rbOneTime.CheckedChanged += new System.EventHandler(this.rbOneTime_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lWeek);
            this.groupBox2.Controls.Add(this.nudWeeks);
            this.groupBox2.Controls.Add(this.clbWeekDays);
            this.groupBox2.Controls.Add(this.lDay);
            this.groupBox2.Controls.Add(this.nudDays);
            this.groupBox2.Controls.Add(this.lPer);
            this.groupBox2.Location = new System.Drawing.Point(119, 43);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(503, 115);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            // 
            // lWeek
            // 
            this.lWeek.AutoSize = true;
            this.lWeek.Location = new System.Drawing.Point(108, 22);
            this.lWeek.Name = "lWeek";
            this.lWeek.Size = new System.Drawing.Size(17, 12);
            this.lWeek.TabIndex = 5;
            this.lWeek.Text = "周";
            // 
            // nudWeeks
            // 
            this.nudWeeks.Location = new System.Drawing.Point(60, 20);
            this.nudWeeks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudWeeks.Name = "nudWeeks";
            this.nudWeeks.Size = new System.Drawing.Size(39, 21);
            this.nudWeeks.TabIndex = 4;
            this.nudWeeks.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // clbWeekDays
            // 
            this.clbWeekDays.CheckOnClick = true;
            this.clbWeekDays.FormattingEnabled = true;
            this.clbWeekDays.Location = new System.Drawing.Point(21, 47);
            this.clbWeekDays.MultiColumn = true;
            this.clbWeekDays.Name = "clbWeekDays";
            this.clbWeekDays.Size = new System.Drawing.Size(470, 52);
            this.clbWeekDays.TabIndex = 3;
            // 
            // lDay
            // 
            this.lDay.AutoSize = true;
            this.lDay.Location = new System.Drawing.Point(108, 22);
            this.lDay.Name = "lDay";
            this.lDay.Size = new System.Drawing.Size(65, 12);
            this.lDay.TabIndex = 2;
            this.lDay.Text = "天发生一次";
            // 
            // nudDays
            // 
            this.nudDays.Location = new System.Drawing.Point(60, 20);
            this.nudDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudDays.Name = "nudDays";
            this.nudDays.Size = new System.Drawing.Size(39, 21);
            this.nudDays.TabIndex = 1;
            this.nudDays.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lPer
            // 
            this.lPer.AutoSize = true;
            this.lPer.Location = new System.Drawing.Point(19, 22);
            this.lPer.Name = "lPer";
            this.lPer.Size = new System.Drawing.Size(41, 12);
            this.lPer.TabIndex = 0;
            this.lPer.Text = "每隔：";
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.CustomFormat = "";
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpStartTime.Location = new System.Drawing.Point(281, 16);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.ShowUpDown = true;
            this.dtpStartTime.Size = new System.Drawing.Size(75, 21);
            this.dtpStartTime.TabIndex = 4;
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.CustomFormat = "";
            this.dtpStartDate.Location = new System.Drawing.Point(152, 16);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(123, 21);
            this.dtpStartDate.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(117, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "开始";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(107, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1, 147);
            this.panel1.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.mtbRepeatUntil);
            this.groupBox3.Controls.Add(this.dtpUntilTime);
            this.groupBox3.Controls.Add(this.dtpUntilDate);
            this.groupBox3.Controls.Add(this.cbUntil);
            this.groupBox3.Controls.Add(this.cbEnabled);
            this.groupBox3.Controls.Add(this.cbRepeatUntil);
            this.groupBox3.Controls.Add(this.cbRepeat);
            this.groupBox3.Controls.Add(this.mtbRepeatInterval);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(12, 191);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(634, 144);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "附加设置";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(336, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 24;
            this.label3.Text = "后不再重复";
            // 
            // mtbRepeatUntil
            // 
            this.mtbRepeatUntil.Enabled = false;
            this.mtbRepeatUntil.Location = new System.Drawing.Point(248, 29);
            this.mtbRepeatUntil.Mask = "90时90分90秒";
            this.mtbRepeatUntil.Name = "mtbRepeatUntil";
            this.mtbRepeatUntil.Size = new System.Drawing.Size(80, 21);
            this.mtbRepeatUntil.TabIndex = 23;
            this.mtbRepeatUntil.Text = "040000";
            this.mtbRepeatUntil.Enter += new System.EventHandler(this.mtbRepeatInterval_Enter);
            this.mtbRepeatUntil.Validating += new System.ComponentModel.CancelEventHandler(this.mtbRepeatInterval_Validating);
            // 
            // dtpUntilTime
            // 
            this.dtpUntilTime.CustomFormat = "";
            this.dtpUntilTime.Enabled = false;
            this.dtpUntilTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpUntilTime.Location = new System.Drawing.Point(227, 67);
            this.dtpUntilTime.Name = "dtpUntilTime";
            this.dtpUntilTime.ShowUpDown = true;
            this.dtpUntilTime.Size = new System.Drawing.Size(75, 21);
            this.dtpUntilTime.TabIndex = 22;
            // 
            // dtpUntilDate
            // 
            this.dtpUntilDate.CustomFormat = "";
            this.dtpUntilDate.Enabled = false;
            this.dtpUntilDate.Location = new System.Drawing.Point(98, 67);
            this.dtpUntilDate.Name = "dtpUntilDate";
            this.dtpUntilDate.Size = new System.Drawing.Size(123, 21);
            this.dtpUntilDate.TabIndex = 21;
            // 
            // cbUntil
            // 
            this.cbUntil.AutoSize = true;
            this.cbUntil.Location = new System.Drawing.Point(17, 71);
            this.cbUntil.Name = "cbUntil";
            this.cbUntil.Size = new System.Drawing.Size(84, 16);
            this.cbUntil.TabIndex = 20;
            this.cbUntil.Text = "到期日期：";
            this.cbUntil.UseVisualStyleBackColor = true;
            this.cbUntil.CheckedChanged += new System.EventHandler(this.cbUntil_CheckedChanged);
            // 
            // cbEnabled
            // 
            this.cbEnabled.AutoSize = true;
            this.cbEnabled.Checked = true;
            this.cbEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbEnabled.Location = new System.Drawing.Point(17, 112);
            this.cbEnabled.Name = "cbEnabled";
            this.cbEnabled.Size = new System.Drawing.Size(60, 16);
            this.cbEnabled.TabIndex = 19;
            this.cbEnabled.Text = "已启用";
            this.cbEnabled.UseVisualStyleBackColor = true;
            // 
            // cbRepeatUntil
            // 
            this.cbRepeatUntil.AutoSize = true;
            this.cbRepeatUntil.Enabled = false;
            this.cbRepeatUntil.Location = new System.Drawing.Point(227, 31);
            this.cbRepeatUntil.Name = "cbRepeatUntil";
            this.cbRepeatUntil.Size = new System.Drawing.Size(15, 14);
            this.cbRepeatUntil.TabIndex = 18;
            this.cbRepeatUntil.UseVisualStyleBackColor = true;
            this.cbRepeatUntil.CheckedChanged += new System.EventHandler(this.cbRepeatUntil_CheckedChanged);
            // 
            // cbRepeat
            // 
            this.cbRepeat.AutoSize = true;
            this.cbRepeat.Location = new System.Drawing.Point(17, 31);
            this.cbRepeat.Name = "cbRepeat";
            this.cbRepeat.Size = new System.Drawing.Size(108, 16);
            this.cbRepeat.TabIndex = 17;
            this.cbRepeat.Text = "重复任务，每隔";
            this.cbRepeat.UseVisualStyleBackColor = true;
            this.cbRepeat.CheckedChanged += new System.EventHandler(this.cbRepeat_CheckedChanged);
            // 
            // mtbRepeatInterval
            // 
            this.mtbRepeatInterval.Enabled = false;
            this.mtbRepeatInterval.Location = new System.Drawing.Point(128, 29);
            this.mtbRepeatInterval.Mask = "90时90分90秒";
            this.mtbRepeatInterval.Name = "mtbRepeatInterval";
            this.mtbRepeatInterval.Size = new System.Drawing.Size(80, 21);
            this.mtbRepeatInterval.TabIndex = 13;
            this.mtbRepeatInterval.Text = "003000";
            this.mtbRepeatInterval.Enter += new System.EventHandler(this.mtbRepeatInterval_Enter);
            this.mtbRepeatInterval.Validating += new System.ComponentModel.CancelEventHandler(this.mtbRepeatInterval_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 12);
            this.label2.TabIndex = 0;
            // 
            // bOK
            // 
            this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOK.Location = new System.Drawing.Point(490, 347);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 2;
            this.bOK.Text = "确定";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // bCancel
            // 
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(571, 347);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 3;
            this.bCancel.Text = "取消";
            this.bCancel.UseVisualStyleBackColor = true;
            // 
            // ScheduleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 382);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScheduleForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "计划设置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeeks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDays)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbOneTime;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbPerWeek;
        private System.Windows.Forms.RadioButton rbPerDay;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox mtbRepeatInterval;
        private System.Windows.Forms.DateTimePicker dtpUntilTime;
        private System.Windows.Forms.DateTimePicker dtpUntilDate;
        private System.Windows.Forms.CheckBox cbUntil;
        private System.Windows.Forms.CheckBox cbEnabled;
        private System.Windows.Forms.CheckBox cbRepeatUntil;
        private System.Windows.Forms.CheckBox cbRepeat;
        private System.Windows.Forms.Label lWeek;
        private System.Windows.Forms.NumericUpDown nudWeeks;
        private System.Windows.Forms.CheckedListBox clbWeekDays;
        private System.Windows.Forms.Label lDay;
        private System.Windows.Forms.NumericUpDown nudDays;
        private System.Windows.Forms.Label lPer;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MaskedTextBox mtbRepeatUntil;
    }
}
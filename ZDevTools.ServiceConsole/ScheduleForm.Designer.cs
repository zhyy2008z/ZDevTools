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
            this.gbPerMonth = new System.Windows.Forms.GroupBox();
            this.bWeeksDropDown = new System.Windows.Forms.Button();
            this.tbWeeks = new System.Windows.Forms.TextBox();
            this.bOrderDropDown = new System.Windows.Forms.Button();
            this.tbOrder = new System.Windows.Forms.TextBox();
            this.rbOn = new System.Windows.Forms.RadioButton();
            this.bDaysDropDown = new System.Windows.Forms.Button();
            this.tbDays = new System.Windows.Forms.TextBox();
            this.rbDays = new System.Windows.Forms.RadioButton();
            this.bMonthDropDown = new System.Windows.Forms.Button();
            this.tbMonth = new System.Windows.Forms.TextBox();
            this.lMonth = new System.Windows.Forms.Label();
            this.rbPerMonth = new System.Windows.Forms.RadioButton();
            this.rbPerWeek = new System.Windows.Forms.RadioButton();
            this.rbPerDay = new System.Windows.Forms.RadioButton();
            this.rbOneTime = new System.Windows.Forms.RadioButton();
            this.gbMain = new System.Windows.Forms.GroupBox();
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
            this.clbMonth = new System.Windows.Forms.CheckedListBox();
            this.clbDays = new System.Windows.Forms.CheckedListBox();
            this.clbOrder = new System.Windows.Forms.CheckedListBox();
            this.clbWeeks = new System.Windows.Forms.CheckedListBox();
            this.groupBox1.SuspendLayout();
            this.gbPerMonth.SuspendLayout();
            this.gbMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeeks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDays)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gbPerMonth);
            this.groupBox1.Controls.Add(this.rbPerMonth);
            this.groupBox1.Controls.Add(this.rbPerWeek);
            this.groupBox1.Controls.Add(this.rbPerDay);
            this.groupBox1.Controls.Add(this.rbOneTime);
            this.groupBox1.Controls.Add(this.gbMain);
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
            // gbPerMonth
            // 
            this.gbPerMonth.Controls.Add(this.bWeeksDropDown);
            this.gbPerMonth.Controls.Add(this.tbWeeks);
            this.gbPerMonth.Controls.Add(this.bOrderDropDown);
            this.gbPerMonth.Controls.Add(this.tbOrder);
            this.gbPerMonth.Controls.Add(this.rbOn);
            this.gbPerMonth.Controls.Add(this.bDaysDropDown);
            this.gbPerMonth.Controls.Add(this.tbDays);
            this.gbPerMonth.Controls.Add(this.rbDays);
            this.gbPerMonth.Controls.Add(this.bMonthDropDown);
            this.gbPerMonth.Controls.Add(this.tbMonth);
            this.gbPerMonth.Controls.Add(this.lMonth);
            this.gbPerMonth.Location = new System.Drawing.Point(119, 43);
            this.gbPerMonth.Name = "gbPerMonth";
            this.gbPerMonth.Size = new System.Drawing.Size(503, 115);
            this.gbPerMonth.TabIndex = 9;
            this.gbPerMonth.TabStop = false;
            // 
            // bWeeksDropDown
            // 
            this.bWeeksDropDown.Location = new System.Drawing.Point(456, 86);
            this.bWeeksDropDown.Margin = new System.Windows.Forms.Padding(0);
            this.bWeeksDropDown.Name = "bWeeksDropDown";
            this.bWeeksDropDown.Size = new System.Drawing.Size(34, 19);
            this.bWeeksDropDown.TabIndex = 17;
            this.bWeeksDropDown.Text = "▼";
            this.bWeeksDropDown.UseVisualStyleBackColor = true;
            this.bWeeksDropDown.Click += new System.EventHandler(this.bMonthDropDown_Click);
            // 
            // tbWeeks
            // 
            this.tbWeeks.Location = new System.Drawing.Point(232, 85);
            this.tbWeeks.Name = "tbWeeks";
            this.tbWeeks.Size = new System.Drawing.Size(259, 21);
            this.tbWeeks.TabIndex = 16;
            this.tbWeeks.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbMonth_KeyPress);
            // 
            // bOrderDropDown
            // 
            this.bOrderDropDown.Location = new System.Drawing.Point(191, 86);
            this.bOrderDropDown.Margin = new System.Windows.Forms.Padding(0);
            this.bOrderDropDown.Name = "bOrderDropDown";
            this.bOrderDropDown.Size = new System.Drawing.Size(34, 19);
            this.bOrderDropDown.TabIndex = 15;
            this.bOrderDropDown.Text = "▼";
            this.bOrderDropDown.UseVisualStyleBackColor = true;
            this.bOrderDropDown.Click += new System.EventHandler(this.bMonthDropDown_Click);
            // 
            // tbOrder
            // 
            this.tbOrder.Location = new System.Drawing.Point(60, 85);
            this.tbOrder.Name = "tbOrder";
            this.tbOrder.Size = new System.Drawing.Size(166, 21);
            this.tbOrder.TabIndex = 14;
            this.tbOrder.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbMonth_KeyPress);
            // 
            // rbOn
            // 
            this.rbOn.AutoSize = true;
            this.rbOn.Location = new System.Drawing.Point(14, 86);
            this.rbOn.Name = "rbOn";
            this.rbOn.Size = new System.Drawing.Size(47, 16);
            this.rbOn.TabIndex = 13;
            this.rbOn.TabStop = true;
            this.rbOn.Text = "在：";
            this.rbOn.UseVisualStyleBackColor = true;
            this.rbOn.CheckedChanged += new System.EventHandler(this.rbOn_CheckedChanged);
            // 
            // bDaysDropDown
            // 
            this.bDaysDropDown.Location = new System.Drawing.Point(457, 52);
            this.bDaysDropDown.Margin = new System.Windows.Forms.Padding(0);
            this.bDaysDropDown.Name = "bDaysDropDown";
            this.bDaysDropDown.Size = new System.Drawing.Size(33, 19);
            this.bDaysDropDown.TabIndex = 11;
            this.bDaysDropDown.Text = "▼";
            this.bDaysDropDown.UseVisualStyleBackColor = true;
            this.bDaysDropDown.Click += new System.EventHandler(this.bMonthDropDown_Click);
            // 
            // tbDays
            // 
            this.tbDays.Location = new System.Drawing.Point(60, 51);
            this.tbDays.Name = "tbDays";
            this.tbDays.Size = new System.Drawing.Size(431, 21);
            this.tbDays.TabIndex = 10;
            this.tbDays.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbMonth_KeyPress);
            // 
            // rbDays
            // 
            this.rbDays.AutoSize = true;
            this.rbDays.Location = new System.Drawing.Point(14, 53);
            this.rbDays.Name = "rbDays";
            this.rbDays.Size = new System.Drawing.Size(47, 16);
            this.rbDays.TabIndex = 12;
            this.rbDays.Text = "天：";
            this.rbDays.UseVisualStyleBackColor = true;
            this.rbDays.CheckedChanged += new System.EventHandler(this.rbDays_CheckedChanged);
            // 
            // bMonthDropDown
            // 
            this.bMonthDropDown.Location = new System.Drawing.Point(457, 20);
            this.bMonthDropDown.Margin = new System.Windows.Forms.Padding(0);
            this.bMonthDropDown.Name = "bMonthDropDown";
            this.bMonthDropDown.Size = new System.Drawing.Size(33, 19);
            this.bMonthDropDown.TabIndex = 8;
            this.bMonthDropDown.Text = "▼";
            this.bMonthDropDown.UseVisualStyleBackColor = true;
            this.bMonthDropDown.Click += new System.EventHandler(this.bMonthDropDown_Click);
            // 
            // tbMonth
            // 
            this.tbMonth.Location = new System.Drawing.Point(60, 19);
            this.tbMonth.Name = "tbMonth";
            this.tbMonth.Size = new System.Drawing.Size(431, 21);
            this.tbMonth.TabIndex = 7;
            this.tbMonth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbMonth_KeyPress);
            // 
            // lMonth
            // 
            this.lMonth.AutoSize = true;
            this.lMonth.Location = new System.Drawing.Point(19, 22);
            this.lMonth.Name = "lMonth";
            this.lMonth.Size = new System.Drawing.Size(41, 12);
            this.lMonth.TabIndex = 6;
            this.lMonth.Text = "月份：";
            // 
            // rbPerMonth
            // 
            this.rbPerMonth.AutoSize = true;
            this.rbPerMonth.Location = new System.Drawing.Point(30, 133);
            this.rbPerMonth.Name = "rbPerMonth";
            this.rbPerMonth.Size = new System.Drawing.Size(47, 16);
            this.rbPerMonth.TabIndex = 8;
            this.rbPerMonth.TabStop = true;
            this.rbPerMonth.Text = "每月";
            this.rbPerMonth.UseVisualStyleBackColor = true;
            this.rbPerMonth.CheckedChanged += new System.EventHandler(this.rbPerMonth_CheckedChanged);
            // 
            // rbPerWeek
            // 
            this.rbPerWeek.AutoSize = true;
            this.rbPerWeek.Location = new System.Drawing.Point(30, 98);
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
            this.rbPerDay.Location = new System.Drawing.Point(30, 63);
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
            // gbMain
            // 
            this.gbMain.Controls.Add(this.lWeek);
            this.gbMain.Controls.Add(this.nudWeeks);
            this.gbMain.Controls.Add(this.clbWeekDays);
            this.gbMain.Controls.Add(this.lDay);
            this.gbMain.Controls.Add(this.nudDays);
            this.gbMain.Controls.Add(this.lPer);
            this.gbMain.Location = new System.Drawing.Point(119, 43);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(503, 115);
            this.gbMain.TabIndex = 5;
            this.gbMain.TabStop = false;
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
            // clbMonth
            // 
            this.clbMonth.CheckOnClick = true;
            this.clbMonth.FormattingEnabled = true;
            this.clbMonth.Items.AddRange(new object[] {
            "所有月份",
            "1月",
            "2月",
            "3月",
            "4月",
            "5月",
            "6月",
            "7月",
            "8月",
            "9月",
            "10月",
            "11月",
            "12月"});
            this.clbMonth.Location = new System.Drawing.Point(684, 12);
            this.clbMonth.Name = "clbMonth";
            this.clbMonth.Size = new System.Drawing.Size(136, 244);
            this.clbMonth.TabIndex = 4;
            this.clbMonth.Click += new System.EventHandler(this.clbMonth_Click);
            this.clbMonth.DoubleClick += new System.EventHandler(this.clbMonth_Click);
            // 
            // clbDays
            // 
            this.clbDays.CheckOnClick = true;
            this.clbDays.ColumnWidth = 50;
            this.clbDays.FormattingEnabled = true;
            this.clbDays.Items.AddRange(new object[] {
            "每天",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "-1"});
            this.clbDays.Location = new System.Drawing.Point(837, 12);
            this.clbDays.MultiColumn = true;
            this.clbDays.Name = "clbDays";
            this.clbDays.Size = new System.Drawing.Size(355, 100);
            this.clbDays.TabIndex = 5;
            this.clbDays.Click += new System.EventHandler(this.clbMonth_Click);
            // 
            // clbOrder
            // 
            this.clbOrder.CheckOnClick = true;
            this.clbOrder.FormattingEnabled = true;
            this.clbOrder.Items.AddRange(new object[] {
            "每周",
            "第1个",
            "第2个",
            "第3个",
            "第4个",
            "最后1个"});
            this.clbOrder.Location = new System.Drawing.Point(684, 262);
            this.clbOrder.Name = "clbOrder";
            this.clbOrder.Size = new System.Drawing.Size(136, 116);
            this.clbOrder.TabIndex = 6;
            this.clbOrder.Click += new System.EventHandler(this.clbMonth_Click);
            // 
            // clbWeeks
            // 
            this.clbWeeks.CheckOnClick = true;
            this.clbWeeks.FormattingEnabled = true;
            this.clbWeeks.Items.AddRange(new object[] {
            "所有星期",
            "星期日",
            "星期一",
            "星期二",
            "星期三",
            "星期四",
            "星期五",
            "星期六"});
            this.clbWeeks.Location = new System.Drawing.Point(868, 163);
            this.clbWeeks.Name = "clbWeeks";
            this.clbWeeks.Size = new System.Drawing.Size(136, 164);
            this.clbWeeks.TabIndex = 7;
            this.clbWeeks.Click += new System.EventHandler(this.clbMonth_Click);
            // 
            // ScheduleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1204, 399);
            this.Controls.Add(this.clbWeeks);
            this.Controls.Add(this.clbOrder);
            this.Controls.Add(this.clbDays);
            this.Controls.Add(this.clbMonth);
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
            this.gbPerMonth.ResumeLayout(false);
            this.gbPerMonth.PerformLayout();
            this.gbMain.ResumeLayout(false);
            this.gbMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeeks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDays)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbOneTime;
        private System.Windows.Forms.GroupBox gbMain;
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
        private System.Windows.Forms.RadioButton rbPerMonth;
        private System.Windows.Forms.Label lMonth;
        private System.Windows.Forms.Button bMonthDropDown;
        private System.Windows.Forms.TextBox tbMonth;
        private System.Windows.Forms.CheckedListBox clbMonth;
        private System.Windows.Forms.GroupBox gbPerMonth;
        private System.Windows.Forms.RadioButton rbDays;
        private System.Windows.Forms.Button bDaysDropDown;
        private System.Windows.Forms.TextBox tbDays;
        private System.Windows.Forms.Button bWeeksDropDown;
        private System.Windows.Forms.TextBox tbWeeks;
        private System.Windows.Forms.Button bOrderDropDown;
        private System.Windows.Forms.TextBox tbOrder;
        private System.Windows.Forms.RadioButton rbOn;
        private System.Windows.Forms.CheckedListBox clbDays;
        private System.Windows.Forms.CheckedListBox clbOrder;
        private System.Windows.Forms.CheckedListBox clbWeeks;
    }
}
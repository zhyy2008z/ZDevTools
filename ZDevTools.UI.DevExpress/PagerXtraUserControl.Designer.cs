namespace ZDevTools.UI.Devexpress
{
	partial class PagerXtraUserControl
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
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
			DevExpress.XtraEditors.PanelControl panelControl1;
			this.lcStatus = new DevExpress.XtraEditors.LabelControl();
			this.cbePerPageRecordsCount = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lcTotalPageCount = new DevExpress.XtraEditors.LabelControl();
			this.sbFirstPage = new DevExpress.XtraEditors.SimpleButton();
			this.sbPreviousPage = new DevExpress.XtraEditors.SimpleButton();
			this.teCurrentPage = new DevExpress.XtraEditors.TextEdit();
			this.sbNextPage = new DevExpress.XtraEditors.SimpleButton();
			this.sbLastPage = new DevExpress.XtraEditors.SimpleButton();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			panelControl1 = new DevExpress.XtraEditors.PanelControl();
			tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbePerPageRecordsCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teCurrentPage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(panelControl1)).BeginInit();
			panelControl1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.ColumnCount = 9;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.Controls.Add(this.lcStatus, 0, 0);
			tableLayoutPanel1.Controls.Add(this.cbePerPageRecordsCount, 1, 0);
			tableLayoutPanel1.Controls.Add(this.lcTotalPageCount, 2, 0);
			tableLayoutPanel1.Controls.Add(this.sbFirstPage, 4, 0);
			tableLayoutPanel1.Controls.Add(this.sbPreviousPage, 5, 0);
			tableLayoutPanel1.Controls.Add(this.teCurrentPage, 6, 0);
			tableLayoutPanel1.Controls.Add(this.sbNextPage, 7, 0);
			tableLayoutPanel1.Controls.Add(this.sbLastPage, 8, 0);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel1.Location = new System.Drawing.Point(2, 2);
			tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 1;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Size = new System.Drawing.Size(739, 28);
			tableLayoutPanel1.TabIndex = 0;
			// 
			// lcStatus
			// 
			this.lcStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lcStatus.Location = new System.Drawing.Point(20, 7);
			this.lcStatus.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
			this.lcStatus.Name = "lcStatus";
			this.lcStatus.Size = new System.Drawing.Size(114, 13);
			this.lcStatus.TabIndex = 0;
			this.lcStatus.Text = "共 1350 条记录，每页";
			// 
			// cbePerPageRecordsCount
			// 
			this.cbePerPageRecordsCount.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cbePerPageRecordsCount.EditValue = "20";
			this.cbePerPageRecordsCount.Location = new System.Drawing.Point(140, 4);
			this.cbePerPageRecordsCount.Name = "cbePerPageRecordsCount";
			this.cbePerPageRecordsCount.Properties.Appearance.Options.UseTextOptions = true;
			this.cbePerPageRecordsCount.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.cbePerPageRecordsCount.Properties.AutoComplete = false;
			this.cbePerPageRecordsCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbePerPageRecordsCount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.cbePerPageRecordsCount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.cbePerPageRecordsCount.Properties.Items.AddRange(new object[] {
            "20",
            "30",
            "50",
            "100"});
			this.cbePerPageRecordsCount.Size = new System.Drawing.Size(41, 20);
			this.cbePerPageRecordsCount.TabIndex = 1;
			this.cbePerPageRecordsCount.EditValueChanged += new System.EventHandler(this.cbePerPageRecordsCount_EditValueChanged);
			this.cbePerPageRecordsCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbePerPageRecordsCount_KeyPress);
			// 
			// lcTotalPageCount
			// 
			this.lcTotalPageCount.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lcTotalPageCount.Location = new System.Drawing.Point(187, 7);
			this.lcTotalPageCount.Name = "lcTotalPageCount";
			this.lcTotalPageCount.Size = new System.Drawing.Size(60, 13);
			this.lcTotalPageCount.TabIndex = 2;
			this.lcTotalPageCount.Text = "条，共21页";
			// 
			// sbFirstPage
			// 
			this.sbFirstPage.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.sbFirstPage.Location = new System.Drawing.Point(525, 4);
			this.sbFirstPage.Name = "sbFirstPage";
			this.sbFirstPage.Size = new System.Drawing.Size(41, 20);
			this.sbFirstPage.TabIndex = 3;
			this.sbFirstPage.Text = "首页";
			this.sbFirstPage.Click += new System.EventHandler(this.sbFirstPage_Click);
			// 
			// sbPreviousPage
			// 
			this.sbPreviousPage.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.sbPreviousPage.Location = new System.Drawing.Point(572, 4);
			this.sbPreviousPage.Name = "sbPreviousPage";
			this.sbPreviousPage.Size = new System.Drawing.Size(39, 20);
			this.sbPreviousPage.TabIndex = 4;
			this.sbPreviousPage.Text = "上页";
			this.sbPreviousPage.Click += new System.EventHandler(this.sbPreviousPage_Click);
			// 
			// teCurrentPage
			// 
			this.teCurrentPage.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.teCurrentPage.EditValue = "1";
			this.teCurrentPage.Location = new System.Drawing.Point(617, 4);
			this.teCurrentPage.Name = "teCurrentPage";
			this.teCurrentPage.Properties.Appearance.Options.UseTextOptions = true;
			this.teCurrentPage.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.teCurrentPage.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.teCurrentPage.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.teCurrentPage.Properties.Mask.EditMask = "f0";
			this.teCurrentPage.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
			this.teCurrentPage.Size = new System.Drawing.Size(29, 20);
			this.teCurrentPage.TabIndex = 5;
			this.teCurrentPage.EditValueChanged += new System.EventHandler(this.teCurrentPage_EditValueChanged);
			// 
			// sbNextPage
			// 
			this.sbNextPage.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.sbNextPage.Location = new System.Drawing.Point(652, 4);
			this.sbNextPage.Name = "sbNextPage";
			this.sbNextPage.Size = new System.Drawing.Size(39, 20);
			this.sbNextPage.TabIndex = 6;
			this.sbNextPage.Text = "下页";
			this.sbNextPage.Click += new System.EventHandler(this.sbNextPage_Click);
			// 
			// sbLastPage
			// 
			this.sbLastPage.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.sbLastPage.Location = new System.Drawing.Point(697, 4);
			this.sbLastPage.Name = "sbLastPage";
			this.sbLastPage.Size = new System.Drawing.Size(39, 20);
			this.sbLastPage.TabIndex = 7;
			this.sbLastPage.Text = "末页";
			this.sbLastPage.Click += new System.EventHandler(this.sbLastPage_Click);
			// 
			// panelControl1
			// 
			panelControl1.Controls.Add(tableLayoutPanel1);
			panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			panelControl1.Location = new System.Drawing.Point(0, 0);
			panelControl1.Margin = new System.Windows.Forms.Padding(0);
			panelControl1.Name = "panelControl1";
			panelControl1.Size = new System.Drawing.Size(743, 32);
			panelControl1.TabIndex = 1;
			// 
			// PagerXtraUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(panelControl1);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "PagerXtraUserControl";
			this.Size = new System.Drawing.Size(743, 32);
			this.Load += new System.EventHandler(this.PagerXtraUserControl_Load);
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbePerPageRecordsCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teCurrentPage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(panelControl1)).EndInit();
			panelControl1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private DevExpress.XtraEditors.LabelControl lcStatus;
		private DevExpress.XtraEditors.ComboBoxEdit cbePerPageRecordsCount;
		private DevExpress.XtraEditors.SimpleButton sbFirstPage;
		private DevExpress.XtraEditors.SimpleButton sbPreviousPage;
		private DevExpress.XtraEditors.TextEdit teCurrentPage;
		private DevExpress.XtraEditors.SimpleButton sbNextPage;
		private DevExpress.XtraEditors.SimpleButton sbLastPage;
		private DevExpress.XtraEditors.LabelControl lcTotalPageCount;
	}
}

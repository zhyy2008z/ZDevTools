using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
using System.ComponentModel;

namespace ZDevTools.UI.Devexpress
{
	/// <summary>
	/// 不支持高级搜索（同时不支持加入新值）的LookUpEdit控件，仅支持绑定一列数据
	/// </summary>
	[ToolboxItem(true)]
	class SingleColumnLookUpEdit : LookUpEdit
	{
		public void Init()
		{
			this.Init(false);
		}

		public void Init(bool allowUserInput)
		{
			var displayerMember = this.Properties.DisplayMember;
			var valueMember = this.Properties.ValueMember;

			this.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
			this.Properties.Columns.Clear();
			this.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(displayerMember, displayerMember)});
			this.Properties.NullText = null;
			this.Properties.PopupFormMinSize = new System.Drawing.Size(10, 0);
			this.Properties.ShowFooter = false;
			this.Properties.ShowHeader = false;
			this.Properties.ShowLines = false;
			this.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;

			this.Properties.ProcessNewValue += Properties_ProcessNewValue;

			if (allowUserInput)
				this.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		}

		void Properties_ProcessNewValue(object sender, DevExpress.XtraEditors.Controls.ProcessNewValueEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(e.DisplayValue.ToString()))
				this.Text = null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
using System.ComponentModel;
using DevExpress.XtraGrid.Columns;
using System.Data;
using System.Collections;

namespace ZDevTools.UI.Devexpress
{
	/// <summary>
	/// 用于显示单字段可绑定的下拉框，支持高级搜索。
	/// </summary>
	/// <remarks>
	/// 1.请在窗体的Load事件中调用Init方法以初始化GridLookUpEdit支持这一特殊用途。
	/// 2.仅需在属性窗口中设置ValueMember与DisplayMember属性，数据列及其他样式会在Init中自动设置，仅需设置与布局有关的属性，谢谢合作！
	/// 
	/// 对于数据源的支持：DataTable、实现了IList接口的类型。
	/// 
	/// 对于集合中单个元素的支持：匿名类型、具有默认构造函数的任何类型。
	/// 
	/// 对ValueMember与DisplayMember的要求：必须绑定正确的字段名称，并且在调用Init()之前设置完毕。
	/// </remarks>
	[ToolboxItem(true)]
	public class SingleColumnGridLookUpEdit : GridLookUpEdit
	{
		void SingleColumnGridLookUpEdit_ProcessNewValue(object sender, DevExpress.XtraEditors.Controls.ProcessNewValueEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(e.DisplayValue.ToString()))
			{
				this.Text = null;
				return;
			}

			if (!allowNewValue) return;

			var dataSource = this.Properties.DataSource;

			if (dataSource is DataTable)
			{
				var dataTable = dataSource as DataTable;
				var newRow = dataTable.NewRow();
				newRow[this.Properties.DisplayMember] = e.DisplayValue;
				newRow[this.Properties.ValueMember] = e.DisplayValue;
				dataTable.Rows.Add(newRow);
				newRow.AcceptChanges();

				e.Handled = true;
			}
			else if (dataSource is IList)
			{
				var list = dataSource as IList;

				if (list.Count > 0)
				{
					var firstElement = list[0];
					var objectType = firstElement.GetType();

					object instance = null;

					var valueProperty = objectType.GetProperty(this.Properties.ValueMember);
					var displayProperty = objectType.GetProperty(this.Properties.DisplayMember);
					var typeConverter = new StringConverter();
					var display = typeConverter.ConvertTo(e.DisplayValue, displayProperty.PropertyType);
					var value = typeConverter.ConvertTo(e.DisplayValue, valueProperty.PropertyType);

					if (objectType.Name.Contains("<>f__AnonymousType"))
					{
						var constructor = objectType.GetConstructors()[0];
						if (constructor.GetParameters()[0].ParameterType == displayProperty.PropertyType)
						{
							instance = constructor.Invoke(new object[] { display, value });
						}
						else
						{
							instance = constructor.Invoke(new object[] { value, display });
						}
					}
					else
					{
						instance = objectType.GetConstructor(Type.EmptyTypes).Invoke(null);
						displayProperty.SetValue(instance, display, null);
						valueProperty.SetValue(instance, value, null);
					}
					list.Add(instance);
				}
				else
				{
					System.Dynamic.ExpandoObject expandObject = new System.Dynamic.ExpandoObject();

					IDictionary<string, object> fields = expandObject;
					fields.Add(this.Properties.DisplayMember, e.DisplayValue);
					fields.Add(this.Properties.ValueMember, e.DisplayValue);

					list.Add(expandObject);
				}

				e.Handled = true;
			}
		}

		/// <summary>
		/// 初始化控件以支持该控件的特殊行为，该方法应在窗体的Load事件中调用。
		/// </summary>
		public void Init()
		{
			Init(false);
		}


		bool allowNewValue;
		public void Init(bool allowNewValue)
		{
			if (string.IsNullOrEmpty(this.Properties.DisplayMember) || string.IsNullOrEmpty(this.Properties.ValueMember))//绑定不完整，就不用继续了
				return;

			var glueThis = this;

			glueThis.EnterMoveNextControl = true;
			glueThis.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
			glueThis.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			glueThis.Properties.NullText = "";
			glueThis.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
			glueThis.Properties.PopupFormMinSize = new System.Drawing.Size(this.Width, 200);
			glueThis.Properties.PopupFormSize = new System.Drawing.Size(this.Width, 200);
			glueThis.Properties.ShowFooter = false;
			glueThis.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;

			GridColumn gcView = new GridColumn();
			var glueView = glueThis.Properties.View;

			// glueView
			glueView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			gcView});
			glueView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
			glueView.Name = "glueView";
			glueView.OptionsSelection.EnableAppearanceFocusedCell = false;
			glueView.OptionsView.ShowColumnHeaders = false;
			glueView.OptionsView.ShowGroupPanel = false;
			glueView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
			glueView.OptionsView.ShowIndicator = false;

			// gcView
			gcView.Caption = this.Properties.DisplayMember;
			gcView.FieldName = this.Properties.DisplayMember;
			gcView.Name = "displayMember";
			gcView.Visible = true;
			gcView.VisibleIndex = 0;

			this.ProcessNewValue += SingleColumnGridLookUpEdit_ProcessNewValue;

			this.allowNewValue = allowNewValue;
		}
	}
}

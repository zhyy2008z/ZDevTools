using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace ZDevTools.UI.Devexpress
{

	[DefaultEvent("CurrentPageNumChanged")]
	[DefaultProperty("TotalRecordsCount")]
	public partial class PagerXtraUserControl : XtraUserControl
	{
		public PagerXtraUserControl()
		{
			InitializeComponent();
		}

		private void cbePerPageRecordsCount_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!(char.IsDigit(e.KeyChar) || e.KeyChar == '\b'))
				e.Handled = true;
		}

		int totalRecordsCount = 0;
		/// <summary>
		/// 总记录条数
		/// </summary>
		[DefaultValue(0)]
		[Category("Pager")]
		[Description("总记录条数")]
		public int TotalRecordsCount
		{
			get { return totalRecordsCount; }
			set
			{
				if (TotalRecordsCount != value)
				{
					totalRecordsCount = value < 0 ? 0 : value;
					this.updateCurrentPage();
					this.updateDisplay();
				}
			}
		}

		int recordsCountPerPage = 20;
		/// <summary>
		/// 每页记录数
		/// </summary>
		[DefaultValue(20)]
		[Category("Pager")]
		[Description("每页记录数")]
		public int PageSize
		{
			get { return recordsCountPerPage; }
			set
			{
				if (PageSize != value)
				{
					recordsCountPerPage = value <= 0 ? recordsCountPerPage = 20 : value;
					this.updateCurrentPage();
					this.OnCurrentPageChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// 总页数
		/// </summary>
		[Category("Pager")]
		[Description("总页数")]
		public int TotalPage
		{
			get
			{
				if (TotalRecordsCount == 0)//如果总页数是零，其实就是记录为空时，直接显示为1页
					return 1;
				else
					return this.TotalRecordsCount % this.PageSize == 0 ? //是整页吗？
							this.TotalRecordsCount / this.PageSize : //是就不用加1
							this.TotalRecordsCount / this.PageSize + 1; //否则要多1页出来
			}
		}

		int currentPageNum = 1;
		/// <summary>
		/// 当前页位置
		/// </summary>
		[DefaultValue(1)]
		[Category("Pager")]
		[Description("当前页位置")]
		public int CurrentPageNum
		{
			get { return currentPageNum; }
			set
			{
				if (CurrentPageNum != value)
				{
					if (value <= 0)
						currentPageNum = 1;
					else if (value > TotalPage)
						currentPageNum = TotalPage;
					else
						currentPageNum = value;
					this.OnCurrentPageChanged(EventArgs.Empty);
				}
			}
		}

		void updateCurrentPage()
		{
			if (CurrentPageNum > TotalPage)
				CurrentPageNum = TotalPage;
		}

		void updateDisplay()
		{
			lcStatus.Text = string.Format("共 {0} 条记录，每页", TotalRecordsCount);
			cbePerPageRecordsCount.EditValue = this.PageSize;
			lcTotalPageCount.Text = string.Format("条，共 {0} 页", TotalPage);
			teCurrentPage.EditValue = this.CurrentPageNum;

			if (CurrentPageNum == 1)
			{
				this.sbFirstPage.Enabled = false;
				this.sbPreviousPage.Enabled = false;
			}
			else
			{
				this.sbFirstPage.Enabled = true;
				this.sbPreviousPage.Enabled = true;
			}
			if (CurrentPageNum == TotalPage)
			{
				this.sbLastPage.Enabled = false;
				this.sbNextPage.Enabled = false;
			}
			else
			{
				this.sbLastPage.Enabled = true;
				this.sbNextPage.Enabled = true;
			}
		}

		protected virtual void OnCurrentPageChanged(EventArgs e)
		{
			updateDisplay();
			if (CurrentPageChanged != null)
			{
				CurrentPageChanged(this, e);
			}
		}

		/// <summary>
		/// 当前页发生变化时发生
		/// </summary>
		[Category("Pager")]
		[Description("当前页发生变化时发生")]
		public event EventHandler CurrentPageChanged;

		private void cbePerPageRecordsCount_EditValueChanged(object sender, EventArgs e)
		{
			int perPageCount;
			if (int.TryParse(cbePerPageRecordsCount.Text, out perPageCount))
			{
				this.PageSize = perPageCount;
			}
		}

		private void teCurrentPage_EditValueChanged(object sender, EventArgs e)
		{
			this.CurrentPageNum = (int)teCurrentPage.EditValue;
		}

		private void sbFirstPage_Click(object sender, EventArgs e)
		{
			this.CurrentPageNum = 1;
		}

		private void sbPreviousPage_Click(object sender, EventArgs e)
		{
			this.CurrentPageNum -= 1;
		}

		private void sbNextPage_Click(object sender, EventArgs e)
		{
			this.CurrentPageNum += 1;
		}

		private void sbLastPage_Click(object sender, EventArgs e)
		{
			this.CurrentPageNum = TotalPage;
		}

		private void PagerXtraUserControl_Load(object sender, EventArgs e)
		{
			this.updateDisplay();
		}




	}
}

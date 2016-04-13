using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZDevTools.ServiceConsole.Schedules;
using System.Globalization;
using static ZDevTools.ServiceConsole.CommonFunctions;


namespace ZDevTools.ServiceConsole
{
    public partial class ScheduleForm : Form
    {

        public class PopupWindow : ToolStripDropDown
        {
            public PopupWindow(Control popupControl, TextBox relatedTextBox)
            {
                this.Margin = Padding.Empty;
                this.Padding = Padding.Empty;

                Target = popupControl;
                var toolStripControlHost = new ToolStripControlHost(popupControl);
                toolStripControlHost.Margin = Padding.Empty;
                toolStripControlHost.Padding = Padding.Empty;
                toolStripControlHost.AutoSize = false;
                this.Items.Add(toolStripControlHost);

                RelatedTextBox = relatedTextBox;
            }

            public Control Target { get; }

            public TextBox RelatedTextBox { get; }
        }

        PopupWindow pwMonth;
        PopupWindow pwDays;
        PopupWindow pwOrder;
        PopupWindow pwWeeks;
        public ScheduleForm()
        {
            InitializeComponent();

            mtbRepeatInterval.ValidatingType = typeof(DateTime);
            mtbRepeatUntil.ValidatingType = typeof(DateTime);

            //加载星期
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                clbWeekDays.Items.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(day));
            }

            pwMonth = new PopupWindow(clbMonth, tbMonth);
            pwMonth.Closed += pwMonth_Closed;

            pwDays = new PopupWindow(clbDays, tbDays);
            pwDays.Closed += pwMonth_Closed;

            pwOrder = new PopupWindow(clbOrder, tbOrder);
            pwOrder.Closed += pwMonth_Closed;

            pwWeeks = new PopupWindow(clbWeeks, tbWeeks);
            pwWeeks.Closed += pwMonth_Closed;


            this.Size = new Size(674, 421);
        }


        public void LoadModel(BasicSchedule schedule)
        {
            Type scheduleType = schedule.GetType();

            dtpStartDate.Value = schedule.BeginTime;
            dtpStartTime.Value = schedule.BeginTime;

            if (schedule.RepeatPeriod.HasValue)
            {
                cbRepeat.Checked = true;
                mtbRepeatInterval.Text = schedule.RepeatPeriod.Value.ToString("hhmmss");
                if (schedule.RepeatUntil.HasValue)
                {
                    cbRepeatUntil.Checked = true;
                    mtbRepeatUntil.Text = schedule.RepeatUntil.Value.ToString("hhmmss");
                }
            }

            if (schedule.EndTime.HasValue)
            {
                cbUntil.Checked = true;
                dtpUntilDate.Value = schedule.EndTime.Value;
                dtpUntilTime.Value = schedule.EndTime.Value;
            }

            cbEnabled.Checked = schedule.Enabled;

            if (scheduleType == typeof(BasicSchedule))
            {
                rbOneTime.Checked = true;
            }
            else if (scheduleType == typeof(DayRepeatSchedule))
            {
                var daySchedule = schedule as DayRepeatSchedule;
                rbPerDay.Checked = true;
                nudDays.Value = daySchedule.RepeatPerDays;
            }
            else if (scheduleType == typeof(WeekRepeatSchedule))
            {
                var weekSchedule = schedule as WeekRepeatSchedule;
                rbPerWeek.Checked = true;
                nudWeeks.Value = weekSchedule.RepeatPerWeeks;
                foreach (var day in weekSchedule.RepeatWeekDays)
                {
                    clbWeekDays.SetItemChecked((int)day, true);
                }
            }
            else if (scheduleType == typeof(MonthRepeatSchedule))
            {
                var monthSchedule = schedule as MonthRepeatSchedule;
                rbPerMonth.Checked = true;
                foreach (var index in monthSchedule.Months)
                    clbMonth.SetItemChecked(index, true);
                if (clbMonth.CheckedItems.Count == clbMonth.Items.Count - 1)
                    clbMonth.SetItemChecked(0, true);

                pwMonth_Closed(pwMonth, null);

                if (monthSchedule.Days != null)
                {
                    rbDays.Checked = true;
                    foreach (var index in monthSchedule.Days)
                        clbDays.SetItemChecked(index, true);
                    if (clbDays.CheckedItems.Count == clbDays.Items.Count - 1)
                        clbDays.SetItemChecked(0, true);
                    pwMonth_Closed(pwDays, null);
                }
                else
                {
                    rbOn.Checked = true;
                    foreach (var index in monthSchedule.WeekOrders)
                        clbOrder.SetItemChecked(index, true);
                    if (clbOrder.CheckedItems.Count == clbOrder.Items.Count - 1)
                        clbOrder.SetItemChecked(0, true);

                    foreach (var index in monthSchedule.WeekDays)
                        clbWeeks.SetItemChecked((int)index + 1, true);
                    if (clbWeeks.CheckedItems.Count == clbWeeks.Items.Count - 1)
                        clbWeeks.SetItemChecked(0, true);

                    pwMonth_Closed(pwOrder, null);
                    pwMonth_Closed(pwWeeks, null);
                }
            }
            else
                throw new ArgumentOutOfRangeException("未支持的计划类型");
        }

        public BasicSchedule SaveSchedule()
        {
            BasicSchedule schedule = null;
            if (rbOneTime.Checked)
            {
                schedule = new BasicSchedule();
            }
            else if (rbPerDay.Checked)
            {
                schedule = new DayRepeatSchedule();
                var daySchedule = schedule as DayRepeatSchedule;
                daySchedule.RepeatPerDays = getRepeatPerDays();
            }
            else if (rbPerWeek.Checked)
            {
                schedule = new WeekRepeatSchedule();
                var weekSchedule = schedule as WeekRepeatSchedule;
                weekSchedule.RepeatPerWeeks = getRepeatPerWeeks();
                weekSchedule.RepeatWeekDays = getRepeatWeekDays();
            }
            else if (rbPerMonth.Checked)
            {
                schedule = new MonthRepeatSchedule();
                var monthSchedule = schedule as MonthRepeatSchedule;
                monthSchedule.Months = (from i in clbMonth.CheckedIndices.Cast<int>()
                                        where i > 0
                                        select i).ToArray();

                if (rbDays.Checked)
                {
                    monthSchedule.Days = (from i in clbDays.CheckedIndices.Cast<int>()
                                          where i > 0
                                          select i).ToArray();
                }
                else if (rbOn.Checked)
                {
                    monthSchedule.WeekOrders = (from i in clbOrder.CheckedIndices.Cast<int>()
                                                where i > 0
                                                select i).ToArray();
                    monthSchedule.WeekDays = (from i in clbWeeks.CheckedIndices.Cast<int>()
                                              where i > 0
                                              select (DayOfWeek)(i - 1)).ToArray();
                }
            }


            schedule.BeginTime = getBeginTime();

            if (cbRepeat.Checked)
            {
                schedule.RepeatPeriod = getRepeatPeriod();

                if (cbRepeatUntil.Checked)
                {
                    schedule.RepeatUntil = getRepeatUntil();
                }
            }

            if (cbUntil.Checked)
            {
                schedule.EndTime = getEndTime();
            }

            schedule.Enabled = getEnabled();
            return schedule;
        }

        private bool getEnabled()
        {
            return cbEnabled.Checked;
        }

        private TimeSpan getRepeatPeriod()
        {
            return ((DateTime)(mtbRepeatInterval.ValidateText())).TimeOfDay;
        }

        private int getRepeatPerWeeks()
        {
            return (int)nudWeeks.Value;
        }

        private int getRepeatPerDays()
        {
            return (int)nudDays.Value;
        }

        private DayOfWeek[] getRepeatWeekDays()
        {
            return Array.ConvertAll<int, DayOfWeek>(clbWeekDays.CheckedIndices.Cast<int>().ToArray(), dayNum => (DayOfWeek)dayNum);
        }

        private DateTime getEndTime()
        {
            return dtpUntilDate.Value.Date + dtpUntilTime.Value.TimeOfDay;
        }

        private TimeSpan getRepeatUntil()
        {
            return ((DateTime)(mtbRepeatUntil.ValidateText())).TimeOfDay;
        }

        private DateTime getBeginTime()
        {
            return dtpStartDate.Value.Date + dtpStartTime.Value.TimeOfDay;
        }

        bool anyOneChecked(CheckedListBox clb)
        {
            var length = clb.Items.Count;
            for (int i = 1; i < length; i++)
            {
                if (clb.GetItemChecked(i))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 验证参数是否正常
        /// </summary>
        /// <returns></returns>
        bool checkOk()
        {
            if (!(rbOneTime.Checked || rbPerDay.Checked || rbPerWeek.Checked || rbPerMonth.Checked))
            {
                ShowMessage("请选择一种计划执行方式！");
                return false;
            }

            //检测通用参数是否正常
            if (cbRepeat.Checked)
            {
                if (getRepeatPeriod().TotalSeconds < 1)
                {
                    ShowMessage("重复周期不能小于1秒");
                    mtbRepeatInterval.Focus();
                    return false;
                }

                if (cbRepeatUntil.Checked)
                {
                    if (getRepeatUntil().TotalSeconds < 1)
                    {
                        ShowMessage("重复持续时间不能小于1秒");
                        mtbRepeatUntil.Focus();
                        return false;
                    }

                    if (getRepeatPeriod() >= getRepeatUntil())
                    {
                        ShowMessage("重复持续时间必须大于重复周期，请重设！");
                        return false;
                    }
                }
            }

            if (cbUntil.Checked)
            {
                if ((getEndTime() - getBeginTime()).TotalSeconds < 1)
                {
                    ShowMessage("截止时间必须大于开始时间！");
                    return false;
                }
            }

            //分情况检测参数是否正常
            if (rbOneTime.Checked) { }
            else if (rbPerDay.Checked) { }
            else if (rbPerWeek.Checked)
            {
                if (getRepeatWeekDays().Length == 0)
                {
                    ShowMessage("请选择一周中的至少一天！");
                    return false;
                }
            }
            else if (rbPerMonth.Checked)
            {
                //检测是否选择了月份
                if (!anyOneChecked(clbMonth))
                {
                    ShowMessage("请选择至少一个月份！");
                    return false;
                }

                //检测是否选择了一种计划方式
                if (!(rbDays.Checked || rbOn.Checked))
                {
                    ShowMessage("请选择按天还是按星期！");
                    return false;
                }

                //分别检测每种计划方式是否选择了正确的值
                if (rbDays.Checked)
                {
                    if (!anyOneChecked(clbDays))
                    {
                        ShowMessage("请选择至少一天！");
                        return false;
                    }
                }
                else if (rbOn.Checked)
                {
                    if (!anyOneChecked(clbOrder) || !anyOneChecked(clbWeeks))
                    {
                        ShowMessage("请选择第几天哪几个星期！");
                        return false;
                    }
                }

            }

            //检查通过

            return true;
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            if (!checkOk())
            {
                DialogResult = DialogResult.None;
            }
        }

        #region 界面自适应布局代码
        private void cbRepeat_CheckedChanged(object sender, EventArgs e)
        {
            var isRepeat = cbRepeat.Checked;
            mtbRepeatInterval.Enabled = isRepeat;
            cbRepeatUntil.Enabled = isRepeat;
            mtbRepeatUntil.Enabled = cbRepeatUntil.Enabled && cbRepeatUntil.Checked;
        }

        private void cbRepeatUntil_CheckedChanged(object sender, EventArgs e)
        {
            var isRepeatUntil = cbRepeatUntil.Checked;
            mtbRepeatUntil.Enabled = isRepeatUntil;
        }

        private void cbUntil_CheckedChanged(object sender, EventArgs e)
        {
            var isUntil = cbUntil.Checked;
            dtpUntilDate.Enabled = isUntil;
            dtpUntilTime.Enabled = isUntil;
        }

        private void rbOneTime_CheckedChanged(object sender, EventArgs e)
        {
            if (rbOneTime.Checked)
            {
                gbMain.Visible = true;
                gbPerMonth.Visible = false;

                lDay.Visible = false;
                lWeek.Visible = false;
                lPer.Visible = false;
                nudDays.Visible = false;
                nudWeeks.Visible = false;
                clbWeekDays.Visible = false;
            }
        }

        private void rbPerDay_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPerDay.Checked)
            {
                gbMain.Visible = true;
                gbPerMonth.Visible = false;

                lDay.Visible = true;
                lPer.Visible = true;
                nudDays.Visible = true;
                lWeek.Visible = false;
                nudWeeks.Visible = false;
                clbWeekDays.Visible = false;
            }

        }

        private void rbPerWeek_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPerWeek.Checked)
            {
                gbMain.Visible = true;
                gbPerMonth.Visible = false;

                lPer.Visible = true;
                lDay.Visible = false;
                nudDays.Visible = false;
                lWeek.Visible = true;
                nudWeeks.Visible = true;
                clbWeekDays.Visible = true;
            }
        }

        #region 处理按月份进行计划的所有有关事件

        private void rbPerMonth_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPerMonth.Checked)
            {
                gbMain.Visible = false;
                gbPerMonth.Visible = true;
            }
        }

        private void pwMonth_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            var popupWindow = (sender as PopupWindow);
            var clb = popupWindow.Target as CheckedListBox;
            var length = clb.Items.Count;

            List<string> list = new List<string>();
            for (int i = 1; i < length; i++)
            {
                if (clb.GetItemChecked(i))
                {
                    list.Add(clb.Items[i].ToString());
                }
            }

            popupWindow.RelatedTextBox.Text = string.Join("、", list);
        }

        private void clbMonth_Click(object sender, EventArgs e)
        {
            var clb = sender as CheckedListBox;
            var length = clb.Items.Count;

            if (clb.SelectedIndex == 0) //进入全选/不选模式
            {
                var check = clb.GetItemChecked(0);
                for (int i = 1; i < length; i++)
                    clb.SetItemChecked(i, !check);
            }
            else if (clb.SelectedIndex > 0)
            {
                bool restAllCheck = true;
                for (int i = 1; i < length; i++)
                {
                    if (!clb.GetItemChecked(i) && i != clb.SelectedIndex)
                    {
                        restAllCheck = false;
                        break;
                    }
                }

                bool currentCheck = clb.GetItemChecked(clb.SelectedIndex);

                clb.SetItemChecked(0, restAllCheck && !currentCheck);
            }
        }

        private void tbMonth_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void bMonthDropDown_Click(object sender, EventArgs e)
        {

            PopupWindow popupWindow = null;

            if (sender == bMonthDropDown)
                popupWindow = pwMonth;
            else if (sender == bDaysDropDown)
                popupWindow = pwDays;
            else if (sender == bOrderDropDown)
                popupWindow = pwOrder;
            else if (sender == bWeeksDropDown)
                popupWindow = pwWeeks;


            var clb = popupWindow.Target as CheckedListBox;

            clb.Width = popupWindow.RelatedTextBox.Width;

            var position = new Point(popupWindow.RelatedTextBox.Left, popupWindow.RelatedTextBox.Bottom - 1);
            popupWindow.Show(popupWindow.RelatedTextBox.Parent.PointToScreen(position));
        }

        private void rbDays_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDays.Checked)
            {
                tbDays.Enabled = true;
                bDaysDropDown.Enabled = true;

                tbOrder.Enabled = false;
                bOrderDropDown.Enabled = false;
                tbWeeks.Enabled = false;
                bWeeksDropDown.Enabled = false;
            }
        }

        private void rbOn_CheckedChanged(object sender, EventArgs e)
        {
            if (rbOn.Checked)
            {
                tbDays.Enabled = false;
                bDaysDropDown.Enabled = false;

                tbOrder.Enabled = true;
                bOrderDropDown.Enabled = true;
                tbWeeks.Enabled = true;
                bWeeksDropDown.Enabled = true;
            }
        }

        #endregion
        #endregion

        #region 修正MaskTextBox的Modified事件不可用问题
        string oldValue;

        private void mtbRepeatInterval_Enter(object sender, EventArgs e)
        {
            oldValue = mtbRepeatInterval.Text;
        }

        private void mtbRepeatInterval_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = mtbRepeatInterval.ValidateText() == null;
            if (!e.Cancel && mtbRepeatInterval.Text != oldValue)
                mtbRepeatInterval.Modified = true;
        }


        #endregion


    }
}

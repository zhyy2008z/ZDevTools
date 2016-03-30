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

        private void ScheduleForm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 验证参数是否正常
        /// </summary>
        /// <returns></returns>
        bool checkOk()
        {
            if (!(rbOneTime.Checked || rbPerDay.Checked || rbPerWeek.Checked))
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
                    MessageBox.Show("请选择一周中的至少一天！");
                    return false;
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
                lPer.Visible = true;
                lDay.Visible = false;
                nudDays.Visible = false;
                lWeek.Visible = true;
                nudWeeks.Visible = true;
                clbWeekDays.Visible = true;
            }
        }
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

using System;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using static ZDevTools.ServiceConsole.CommonFunctions;


namespace ZDevTools.ServiceConsole.ViewModels
{
    using Views;
    using Schedules;
    using Models;

    /// <summary>
    /// 不直接修改BasicSchedule对象成员
    /// </summary>
    public class ScheduleWindowViewModel : BindableBase
    {

        public ScheduleWindowViewModel()
        {
            OKCommand = new DelegateCommand(okOperate);
            StartAtDate = DateTime.Now;
            EndTime = DateTime.Now.AddDays(1);
            EveryDayIntervalDays = 1;
            EveryWeekIntervalWeeks = 1;
            RepeatPeriod = new TimeSpan(0, 30, 0);
            RepeatUntilTime = new TimeSpan(4, 0, 0);
            IsEnabled = true;
        }
        public DelegateCommand OKCommand { get; }
        private void okOperate()
        {
            if (checkOk())
                Window.DialogResult = true;
        }

        public Synchronizer Synchronizer { get; set; }

        public ScheduleWindow Window { get; set; }

        bool _isOnce;
        public bool IsOnce { get { return _isOnce; } set { SetProperty(ref _isOnce, value); } }


        private bool _isEveryDay;

        public bool IsEveryDay { get { return _isEveryDay; } set { SetProperty(ref _isEveryDay, value); } }


        bool _isEveryWeek;
        public bool IsEveryWeek { get { return _isEveryWeek; } set { SetProperty(ref _isEveryWeek, value); } }


        bool _isEveryMonth;
        public bool IsEveryMonth { get { return _isEveryMonth; } set { SetProperty(ref _isEveryMonth, value); } }

        DateTime _startAtDate;
        public DateTime StartAtDate { get { return _startAtDate; } set { SetProperty(ref _startAtDate, value); } }


        int _everyDayIntervalDays;
        public int EveryDayIntervalDays { get { return _everyDayIntervalDays; } set { SetProperty(ref _everyDayIntervalDays, value); } }


        int _everyWeekIntervalWeeks;
        public int EveryWeekIntervalWeeks { get { return _everyWeekIntervalWeeks; } set { SetProperty(ref _everyWeekIntervalWeeks, value); } }

        ObservableCollection<WeekDay> _everyWeekWeekDays = new ObservableCollection<WeekDay>();
        public ObservableCollection<WeekDay> EveryWeekWeekDays { get { return _everyWeekWeekDays; } set { SetProperty(ref _everyWeekWeekDays, value); } }


        bool _repeatSchedule;
        public bool RepeatSchedule { get { return _repeatSchedule; } set { SetProperty(ref _repeatSchedule, value); } }


        TimeSpan _repeatPeriod;
        public TimeSpan RepeatPeriod { get { return _repeatPeriod; } set { SetProperty(ref _repeatPeriod, value); } }


        bool _repeatUntil;
        public bool RepeatUntil { get { return _repeatUntil; } set { SetProperty(ref _repeatUntil, value); } }

        TimeSpan _repeatUntilTime;
        public TimeSpan RepeatUntilTime { get { return _repeatUntilTime; } set { SetProperty(ref _repeatUntilTime, value); } }

        DateTime _endTime;
        public DateTime EndTime { get { return _endTime; } set { SetProperty(ref _endTime, value); } }

        bool _hasEndTime;
        public bool HasEndTime { get { return _hasEndTime; } set { SetProperty(ref _hasEndTime, value); } }


        bool _isEnabled;
        public bool IsEnabled { get { return _isEnabled; } set { SetProperty(ref _isEnabled, value); } }


        ObservableCollection<WitchMonth> _everyMonthMonths = new ObservableCollection<WitchMonth>();
        public ObservableCollection<WitchMonth> EveryMonthMonths { get { return _everyMonthMonths; } set { SetProperty(ref _everyMonthMonths, value); } }


        ObservableCollection<MonthDay> _everyMonthDays = new ObservableCollection<MonthDay>();
        public ObservableCollection<MonthDay> EveryMonthDays { get { return _everyMonthDays; } set { SetProperty(ref _everyMonthDays, value); } }


        ObservableCollection<WeekDay> _everyMonthWeekDays = new ObservableCollection<WeekDay>();
        public ObservableCollection<WeekDay> EveryMonthWeekDays { get { return _everyMonthWeekDays; } set { SetProperty(ref _everyMonthWeekDays, value); } }


        ObservableCollection<WitchWeek> _everyMonthWeeks = new ObservableCollection<WitchWeek>();
        public ObservableCollection<WitchWeek> EveryMonthWeeks { get { return _everyMonthWeeks; } set { SetProperty(ref _everyMonthWeeks, value); } }

        bool _everyMonthDaySchedule;
        public bool EveryMonthDaySchedule { get { return _everyMonthDaySchedule; } set { SetProperty(ref _everyMonthDaySchedule, value); } }

        bool _everyMonthWeekSchedule;
        public bool EveryMonthWeekSchedule { get { return _everyMonthWeekSchedule; } set { SetProperty(ref _everyMonthWeekSchedule, value); } }

        public void LoadModel(BasicSchedule schedule)
        {
            Type scheduleType = schedule.GetType();

            StartAtDate = schedule.BeginTime;

            if (schedule.RepeatPeriod.HasValue)
            {
                RepeatSchedule = true;
                RepeatPeriod = schedule.RepeatPeriod.Value;
                if (schedule.RepeatUntil.HasValue)
                {
                    RepeatUntil = true;
                    RepeatUntilTime = schedule.RepeatUntil.Value;
                }
            }

            if (schedule.EndTime.HasValue)
            {
                HasEndTime = true;
                EndTime = schedule.EndTime.Value;
            }

            IsEnabled = schedule.Enabled;


            var weekdays = (WeekDay[])Window.AdvancedOptionsGroupBox.FindResource("WeekDays");
            var months = (WitchMonth[])Window.AdvancedOptionsGroupBox.FindResource("Months");
            var monthWeeks = (WitchWeek[])Window.AdvancedOptionsGroupBox.FindResource("MonthWeeks");
            var monthDays = (MonthDay[])Window.AdvancedOptionsGroupBox.FindResource("MonthDays");

            if (scheduleType == typeof(BasicSchedule))
            {
                IsOnce = true;
            }
            else if (scheduleType == typeof(DayRepeatSchedule))
            {
                var daySchedule = schedule as DayRepeatSchedule;
                IsEveryDay = true;
                EveryDayIntervalDays = daySchedule.RepeatPerDays;
            }
            else if (scheduleType == typeof(WeekRepeatSchedule))
            {
                var weekSchedule = schedule as WeekRepeatSchedule;
                IsEveryWeek = true;
                EveryWeekIntervalWeeks = weekSchedule.RepeatPerWeeks;

                var weekDays = new List<WeekDay>();
                foreach (var dayOfWeek in weekSchedule.RepeatWeekDays)
                    weekDays.Add(weekdays.First(wd => wd.DayOfWeek == dayOfWeek));

                EveryWeekWeekDays = new ObservableCollection<WeekDay>(weekDays);
            }
            else if (scheduleType == typeof(MonthRepeatSchedule))
            {
                var monthSchedule = schedule as MonthRepeatSchedule;
                IsEveryMonth = true;


                var selectedMonths = new List<WitchMonth>();

                foreach (var monthOrder in monthSchedule.Months)
                    selectedMonths.Add(months.First(m => m.Month == monthOrder));

                EveryMonthMonths = new ObservableCollection<WitchMonth>(selectedMonths);


                if (monthSchedule.Days != null)
                {
                    EveryMonthDaySchedule = true;

                    var selectedDays = new List<MonthDay>();

                    foreach (var day in monthSchedule.Days)
                        selectedDays.Add(monthDays.First(d => d.Day == day));

                    EveryMonthDays = new ObservableCollection<MonthDay>(selectedDays);
                }
                else
                {
                    EveryMonthWeekSchedule = true;
                    var selectedWeeks = new List<WitchWeek>();

                    foreach (var weekOrder in monthSchedule.WeekOrders)
                        selectedWeeks.Add(monthWeeks.First(mw => mw.WeekNumber == weekOrder));

                    EveryMonthWeeks = new ObservableCollection<WitchWeek>(selectedWeeks);

                    var selectedWeekDays = new List<WeekDay>();
                    foreach (var dayOfWeek in monthSchedule.WeekDays)
                        selectedWeekDays.Add(weekdays.First(wd => wd.DayOfWeek == dayOfWeek));

                    EveryMonthWeekDays = new ObservableCollection<WeekDay>(selectedWeekDays);
                }
            }
            else
                throw new ArgumentOutOfRangeException("未支持的计划类型");
        }

        public BasicSchedule SaveSchedule()
        {
            BasicSchedule schedule = null;
            if (IsOnce)
            {
                schedule = new BasicSchedule();
            }
            else if (IsEveryDay)
            {
                schedule = new DayRepeatSchedule();
                var daySchedule = schedule as DayRepeatSchedule;
                daySchedule.RepeatPerDays = EveryDayIntervalDays;
            }
            else if (IsEveryWeek)
            {
                schedule = new WeekRepeatSchedule();
                var weekSchedule = schedule as WeekRepeatSchedule;
                weekSchedule.RepeatPerWeeks = EveryWeekIntervalWeeks;
                weekSchedule.RepeatWeekDays = EveryWeekWeekDays.Select(wd => wd.DayOfWeek).ToArray();
            }
            else if (IsEveryMonth)
            {
                schedule = new MonthRepeatSchedule();
                var monthSchedule = schedule as MonthRepeatSchedule;
                monthSchedule.Months = EveryMonthMonths.Select(wm => wm.Month).ToArray();

                if (EveryMonthDaySchedule)
                {
                    monthSchedule.Days = EveryMonthDays.Select(md => md.Day).ToArray();
                }
                else if (EveryMonthWeekSchedule)
                {
                    monthSchedule.WeekOrders = EveryMonthWeeks.Select(ww => ww.WeekNumber).ToArray();
                    monthSchedule.WeekDays = EveryMonthWeekDays.Select(wd => wd.DayOfWeek).ToArray();
                }
            }


            schedule.BeginTime = StartAtDate;

            if (RepeatSchedule)
            {
                schedule.RepeatPeriod = RepeatPeriod;

                if (RepeatUntil)
                {
                    schedule.RepeatUntil = RepeatUntilTime;
                }
            }

            if (HasEndTime)
            {
                schedule.EndTime = EndTime;
            }

            schedule.Enabled = IsEnabled;
            return schedule;
        }

        /// <summary>
        /// 验证参数是否正常
        /// </summary>
        /// <returns></returns>
        bool checkOk()
        {
            if (!(IsOnce || IsEveryDay || IsEveryWeek || IsEveryMonth))
            {
                ShowMessage("请选择一种计划执行方式！");
                return false;
            }

            //检测通用参数是否正常
            if (RepeatSchedule)
            {
                if (RepeatPeriod.TotalSeconds < 1)
                {
                    ShowMessage("重复周期不能小于1秒");
                    return false;
                }

                if (RepeatUntil)
                {
                    if (RepeatUntilTime.TotalSeconds < 1)
                    {
                        ShowMessage("重复持续时间不能小于1秒");
                        return false;
                    }

                    if (RepeatPeriod >= RepeatUntilTime)
                    {
                        ShowMessage("重复持续时间必须大于重复周期，请重设！");
                        return false;
                    }
                }
            }

            if (HasEndTime)
            {
                if ((EndTime - StartAtDate).TotalSeconds < 1)
                {
                    ShowMessage("截止时间必须大于开始时间！");
                    return false;
                }
            }

            //分情况检测参数是否正常
            if (IsOnce) { }
            else if (IsEveryDay) { }
            else if (IsEveryWeek)
            {
                if (EveryWeekWeekDays.Count == 0)
                {
                    ShowMessage("请选择一周中的至少一天！");
                    return false;
                }
            }
            else if (IsEveryMonth)
            {
                //检测是否选择了月份
                if (EveryMonthMonths.Count == 0)
                {
                    ShowMessage("请选择至少一个月份！");
                    return false;
                }

                //检测是否选择了一种计划方式
                if (!(EveryMonthDaySchedule || EveryMonthWeekSchedule))
                {
                    ShowMessage("请选择按天还是按星期！");
                    return false;
                }

                //分别检测每种计划方式是否选择了正确的值
                if (EveryMonthDaySchedule)
                {
                    if (EveryMonthDays.Count == 0)
                    {
                        ShowMessage("请选择至少一天！");
                        return false;
                    }
                }
                else if (EveryMonthWeekSchedule)
                {
                    if (EveryMonthWeeks.Count == 0 || EveryMonthWeekDays.Count == 0)
                    {
                        ShowMessage("请选择第几天哪几个星期！");
                        return false;
                    }
                }

            }

            //检查通过

            return true;
        }
    }
}
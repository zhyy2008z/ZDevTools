using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using ZDevTools.ServiceConsole.Models;
using ZDevTools.ServiceConsole.Schedules;


namespace ZDevTools.ServiceConsole.ViewModels
{
    /// <summary>
    /// 不直接修改BasicSchedule对象成员
    /// </summary>
    public class ScheduleViewModel : ReactiveObject
    {
        readonly IDialogs Dialogs;

        public ScheduleViewModel(IDialogs dialogs)
        {
            this.Dialogs = dialogs;
            StartAtDate = DateTime.Now;
            EndTime = DateTime.Now.AddDays(1);
            EveryDayIntervalDays = 1;
            EveryWeekIntervalWeeks = 1;
            RepeatPeriod = new TimeSpan(0, 30, 0);
            RepeatUntilTime = new TimeSpan(4, 0, 0);
            IsEnabled = true;

            WeekDays = new WeekDay[]
            {
                new WeekDay(){ DayOfWeek= DayOfWeek.Monday, Text="星期一"},
                new WeekDay(){ DayOfWeek= DayOfWeek.Tuesday, Text="星期二"},
                new WeekDay(){ DayOfWeek= DayOfWeek.Wednesday, Text="星期三"},
                new WeekDay(){ DayOfWeek= DayOfWeek.Thursday, Text="星期四"},
                new WeekDay(){ DayOfWeek= DayOfWeek.Friday, Text="星期五"},
                new WeekDay(){ DayOfWeek= DayOfWeek.Saturday, Text="星期六"},
                new WeekDay(){ DayOfWeek= DayOfWeek.Sunday, Text="星期日"},
            };
            Months = Enumerable.Range(1, 12).Select(i => new WitchMonth() { Month = i, Text = i + "月" }).ToArray();
            MonthDays = Enumerable.Range(1, 31).Select(i => new MonthDay() { Day = i, Text = i.ToString() }).Append(new MonthDay() { Day = 32, Text = "最后一天" }).ToArray();
            MonthWeeks = new WitchWeek[]
            {
                new WitchWeek(){ WeekNumber=1, Text="第一个"},
                new WitchWeek(){WeekNumber=2, Text="第二个"},
                new WitchWeek(){WeekNumber=3, Text="第三个"},
                new WitchWeek(){WeekNumber=4,Text="第四个"},
                new WitchWeek(){WeekNumber=5,Text="最后一周"},
            };
        }

        public WitchWeek[] MonthWeeks { get; }
        public MonthDay[] MonthDays { get; }
        public WitchMonth[] Months { get; }
        public WeekDay[] WeekDays { get; }

        [Reactive]
        public bool IsOnce { get; set; }


        [Reactive]

        public bool IsEveryDay { get; set; }


        [Reactive]
        public bool IsEveryWeek { get; set; }


        [Reactive]
        public bool IsEveryMonth { get; set; }

        [Reactive]
        public DateTime StartAtDate { get; set; }


        [Reactive]
        public int EveryDayIntervalDays { get; set; }


        [Reactive]
        public int EveryWeekIntervalWeeks { get; set; }

        [Reactive]
        public bool RepeatSchedule { get; set; }


        [Reactive]
        public TimeSpan RepeatPeriod { get; set; }


        [Reactive]
        public bool RepeatUntil { get; set; }

        [Reactive]
        public TimeSpan RepeatUntilTime { get; set; }

        [Reactive]
        public DateTime EndTime { get; set; }

        [Reactive]
        public bool HasEndTime { get; set; }


        [Reactive]
        public bool IsEnabled { get; set; }

        [Reactive]
        public ObservableCollection<WeekDay> EveryWeekWeekDays { get; set; } = new ObservableCollection<WeekDay>();

        [Reactive]
        public ObservableCollection<WitchMonth> EveryMonthMonths { get; set; } = new ObservableCollection<WitchMonth>();


        [Reactive]
        public ObservableCollection<MonthDay> EveryMonthDays { get; set; } = new ObservableCollection<MonthDay>();


        [Reactive]
        public ObservableCollection<WeekDay> EveryMonthWeekDays { get; set; } = new ObservableCollection<WeekDay>();

        [Reactive]
        public ObservableCollection<WitchWeek> EveryMonthWeeks { get; set; } = new ObservableCollection<WitchWeek>();

        [Reactive]
        public bool EveryMonthDaySchedule { get; set; }

        [Reactive]
        public bool EveryMonthWeekSchedule { get; set; }

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
                    weekDays.Add(WeekDays.First(wd => wd.DayOfWeek == dayOfWeek));

                EveryWeekWeekDays = new ObservableCollection<WeekDay>(weekDays);
            }
            else if (scheduleType == typeof(MonthRepeatSchedule))
            {
                var monthSchedule = schedule as MonthRepeatSchedule;
                IsEveryMonth = true;


                var selectedMonths = new List<WitchMonth>();

                foreach (var monthOrder in monthSchedule.Months)
                    selectedMonths.Add(Months.First(m => m.Month == monthOrder));

                EveryMonthMonths = new ObservableCollection<WitchMonth>(selectedMonths);


                if (monthSchedule.Days != null)
                {
                    EveryMonthDaySchedule = true;

                    var selectedDays = new List<MonthDay>();

                    foreach (var day in monthSchedule.Days)
                        selectedDays.Add(MonthDays.First(d => d.Day == day));

                    EveryMonthDays = new ObservableCollection<MonthDay>(selectedDays);
                }
                else
                {
                    EveryMonthWeekSchedule = true;
                    var selectedWeeks = new List<WitchWeek>();

                    foreach (var weekOrder in monthSchedule.WeekOrders)
                        selectedWeeks.Add(MonthWeeks.First(mw => mw.WeekNumber == weekOrder));

                    EveryMonthWeeks = new ObservableCollection<WitchWeek>(selectedWeeks);

                    var selectedWeekDays = new List<WeekDay>();
                    foreach (var dayOfWeek in monthSchedule.WeekDays)
                        selectedWeekDays.Add(WeekDays.First(wd => wd.DayOfWeek == dayOfWeek));

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
        public bool CheckOk()
        {
            if (!(IsOnce || IsEveryDay || IsEveryWeek || IsEveryMonth))
            {
                Dialogs.ShowMessage("请选择一种计划执行方式！");
                return false;
            }

            //检测通用参数是否正常
            if (RepeatSchedule)
            {
                if (RepeatPeriod.TotalSeconds < 1)
                {
                    Dialogs.ShowMessage("重复周期不能小于1秒");
                    return false;
                }

                if (RepeatUntil)
                {
                    if (RepeatUntilTime.TotalSeconds < 1)
                    {
                        Dialogs.ShowMessage("重复持续时间不能小于1秒");
                        return false;
                    }

                    if (RepeatPeriod >= RepeatUntilTime)
                    {
                        Dialogs.ShowMessage("重复持续时间必须大于重复周期，请重设！");
                        return false;
                    }
                }
            }

            if (HasEndTime)
            {
                if ((EndTime - StartAtDate).TotalSeconds < 1)
                {
                    Dialogs.ShowMessage("截止时间必须大于开始时间！");
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
                    Dialogs.ShowMessage("请选择一周中的至少一天！");
                    return false;
                }
            }
            else if (IsEveryMonth)
            {
                //检测是否选择了月份
                if (EveryMonthMonths.Count == 0)
                {
                    Dialogs.ShowMessage("请选择至少一个月份！");
                    return false;
                }

                //检测是否选择了一种计划方式
                if (!(EveryMonthDaySchedule || EveryMonthWeekSchedule))
                {
                    Dialogs.ShowMessage("请选择按天还是按星期！");
                    return false;
                }

                //分别检测每种计划方式是否选择了正确的值
                if (EveryMonthDaySchedule)
                {
                    if (EveryMonthDays.Count == 0)
                    {
                        Dialogs.ShowMessage("请选择至少一天！");
                        return false;
                    }
                }
                else if (EveryMonthWeekSchedule)
                {
                    if (EveryMonthWeeks.Count == 0 || EveryMonthWeekDays.Count == 0)
                    {
                        Dialogs.ShowMessage("请选择第几天哪几个星期！");
                        return false;
                    }
                }

            }

            //检查通过

            return true;
        }
    }
}
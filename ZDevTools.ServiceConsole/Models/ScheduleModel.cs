using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;


namespace ZDevTools.ServiceConsole.Models
{
    using Schedules;


    public class ScheduleModel : BindableBase
    {
        public string StatusText { get { return Schedule.Enabled ? "已启用" : "已禁用"; } }


        public string Description { get { return Schedule.ToString(); } }

        public string Type { get { return Schedule.Title; } }


        BasicSchedule _schedule;
        public BasicSchedule Schedule
        {
            get { return _schedule; }
            set
            {
                SetProperty(ref _schedule, value);
                RaisePropertyChanged(nameof(StatusText));
                RaisePropertyChanged(nameof(Description));
                RaisePropertyChanged(nameof(Type));
            }
        }
    }
}

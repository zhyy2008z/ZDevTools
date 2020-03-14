using ZDevTools.ServiceConsole.Schedules;
using ReactiveUI;
using System.Reactive.Linq;
using ReactiveUI.Fody.Helpers;

namespace ZDevTools.ServiceConsole.Models
{
    public class ScheduleModel : ReactiveObject
    {
        public string StatusText { [ObservableAsProperty]get; }

        public string Description { [ObservableAsProperty]get; }

        public string Type { [ObservableAsProperty] get; }

        [Reactive]
        public BasicSchedule Schedule { get; set; } = new BasicSchedule();

        public ScheduleModel()
        {
            this.WhenAnyValue(vm => vm.Schedule).Select(s => s.Enabled ? "已启用" : "已禁用").ToPropertyEx(this, vm => vm.StatusText);
            this.WhenAnyValue(vm => vm.Schedule).Select(s => s.ToString()).ToPropertyEx(this, vm => vm.Description);
            this.WhenAnyValue(vm => vm.Schedule).Select(s => s.Title).ToPropertyEx(this, vm => vm.Type);

        }
    }
}

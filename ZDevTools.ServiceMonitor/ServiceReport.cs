using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Windows.Media;
using System.Reactive.Linq;

namespace ZDevTools.ServiceMonitor
{
    class ServiceReport : ReactiveObject
    {
        public ServiceReport()
        {
            var status = this.WhenAnyValue(vm => vm.UpdateTime, vm => vm.HasError)
                  .Select(m =>
                  {
                      if (m.Item2)
                          return -1;
                      else if ((DateTime.Now - m.Item1).Days > 1)
                      {
                          return 0;
                      }
                      else
                      {
                          return 1;
                      }
                  });

            status.Select(s => s switch { -1 => "异常", 0 => "长期未更新", 1 => "正常" }).ToPropertyEx(this, vm => vm.ServiceStatus);

            status.Select(s => s switch { -1 => Brushes.Red, 0 => Brushes.Orange, 1 => Brushes.Green });

            this.WhenAnyValue(vm => MessageArray).Select(ma => string.Join(Environment.NewLine, ma)).ToPropertyEx(this, vm => vm.Detail);
        }


        [Reactive]
        public string ServiceName { get; set; }

        [Reactive]
        public bool HasError { get; set; }

        [Reactive]
        public string Message { get; set; }

        [Reactive]
        public DateTime UpdateTime { get; set; }

        [Reactive]
        public string[] MessageArray { get; set; }


        public string ServiceStatus { [ObservableAsProperty] get; }


        public Brush StatusColorBrush { [ObservableAsProperty]get; }


        public string Detail { [ObservableAsProperty] get; }

    }
}

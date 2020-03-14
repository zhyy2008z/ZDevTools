using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using ServiceStack.Redis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ZDevTools.ServiceMonitor
{
    class MainViewModel : ReactiveObject
    {
        readonly RedisManagerPool RedisManagerPool;

        public MainViewModel(RedisManagerPool redisManagerPool, ILogger<MainViewModel> logger)
        {
            this.RedisManagerPool = redisManagerPool;

            Observable.Create<ReportItem>(observer =>
            {
                refreshAllReports(observer, out var serviceNames);

                var redisPubSubServer = redisManagerPool.CreatePubSubServer(RedisKeys.ServiceReports, (channel, message) =>
                  {
                      using (var client = redisManagerPool.GetClient())
                      {
                          var newReport = JsonSerializer.Deserialize<ServiceReport>(client.GetValueFromHash(RedisKeys.ServiceReports, message));

                          if (serviceNames.Contains(newReport.ServiceName)) //找到了这个服务，从reports中删除，然后再添加为第一条
                          {
                              observer.OnNext(new ReportItem() { IsSuccess = true, ServiceReport = newReport });
                          }
                          else //没找到这个服务，那么说明某些服务名称已更换，或者新增了服务，那么刷新所有的服务状态
                          {
                              refreshAllReports(observer, out serviceNames);
                          }
                      }
                  }).Start();

                return Disposable.Create(() => { redisPubSubServer.Dispose(); });
            }).ToPropertyEx(this, vm => vm.CurrentReport, scheduler: RxApp.MainThreadScheduler);

            this.WhenAnyValue(vm => vm.CurrentReport).Subscribe(ri =>
            {
                if (ri != null)
                {
                    if (ri.IsSuccess)
                    {
                        if (ri.IsAll)
                        {
                            Reports.Clear();
                            foreach (var report in ri.ServiceReports)
                            {
                                Reports.Add(report);
                            }
                        }
                        else
                        {
                            Reports.Remove(Reports.Single(sr => sr.ServiceName == ri.ServiceReport.ServiceName));
                            Reports.Insert(0, ri.ServiceReport);
                        }
                    }
                }
            });
        }


        public ReportItem CurrentReport { [ObservableAsProperty]get; }

        public ObservableCollection<ServiceReport> Reports { get; } = new ObservableCollection<ServiceReport>();

        void refreshAllReports(IObserver<ReportItem> observer, out HashSet<string> serviceNames)
        {
            try
            {
                using (var client = RedisManagerPool.GetClient())
                {
                    var dic = client.GetAllEntriesFromHash(RedisKeys.ServiceReports);
                    List<ServiceReport> reports = new List<ServiceReport>();

                    foreach (var json in dic.Values)
                    {
                        var report = JsonSerializer.Deserialize<ServiceReport>(json);

                        reports.Add(report);
                    }

                    serviceNames = new HashSet<string>(reports.Select(r => r.ServiceName));

                    observer.OnNext(new ReportItem() { IsAll = true, IsSuccess = true, ServiceReports = reports.OrderByDescending(sr => sr.UpdateTime).ToList() });
                }
            }
            catch (Exception ex)
            {
                serviceNames = new HashSet<string>();
                observer.OnNext(new ReportItem() { ErrorMessage = ex.Message });
            }
        }
    }
}

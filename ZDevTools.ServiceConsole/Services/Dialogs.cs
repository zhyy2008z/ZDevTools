using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using ZDevTools.ServiceConsole.Schedules;
using ZDevTools.ServiceConsole.Views;
using ZDevTools.ServiceConsole.Models;
using ZDevTools.ServiceConsole.ViewModels;
using ZDevTools.ServiceCore;
using ReactiveUI;
using System.Reactive.Disposables;

namespace ZDevTools.ServiceConsole.Services
{
    class Dialogs : IDialogs
    {
        readonly IOptions<ConsoleOptions> Options;
        readonly IServiceProvider ServiceProvider;


        public Dialogs(IOptions<ConsoleOptions> options, IServiceProvider serviceProvider)
        {
            this.Options = options;
            this.ServiceProvider = serviceProvider;

        }


        public void ShowOneKeyStartConfigDialog(Dictionary<string, ServiceItemConfig> configs)
        {
            OneKeyStartConfigWindow window = ServiceProvider.GetRequiredService<OneKeyStartConfigWindow>();
            window.Owner = getActiveWindow();
            window.ViewModel.Configs = new ObservableCollection<ServiceItemModel>(configs.Select(kv => new ServiceItemModel() { OneKeyStart = kv.Value.OneKeyStart, ServiceName = kv.Value.ServiceName, ServiceKey = kv.Key }));

            RxApp.MainThreadScheduler.Schedule<object>(null, (scheduler, state) =>
            {
                if (window.ShowDialog() == true)
                {
                    foreach (var config in window.ViewModel.Configs)
                        configs[config.ServiceKey].OneKeyStart = config.OneKeyStart;
                }

                return Disposable.Empty;
            });


        }

        public void ShowScheduleDialog(BasicSchedule basicSchedule, Action<BasicSchedule> onSuccess)
        {
            ScheduleWindow window = ServiceProvider.GetRequiredService<ScheduleWindow>();

            window.Owner = getActiveWindow();
            if (basicSchedule != null)
                window.ViewModel.LoadModel(basicSchedule);

            RxApp.MainThreadScheduler.Schedule<object>(null, (scheduler, state) =>
            {
                if (window.ShowDialog() == true)
                    onSuccess(window.ViewModel.SaveSchedule());
                return Disposable.Empty;
            });
        }

        public void ShowSchedulesDialog(bool canManage, List<BasicSchedule> schedules, Action<List<BasicSchedule>> onSuccess)
        {
            ScheduleManageWindow window = ServiceProvider.GetRequiredService<ScheduleManageWindow>();

            window.Owner = getActiveWindow();
            window.ViewModel.Schedules = new ObservableCollection<ScheduleModel>(schedules.Select(s => new ScheduleModel() { Schedule = s }));
            window.ViewModel.CanManage = canManage;

            RxApp.MainThreadScheduler.Schedule<object>(null, (scheduler, state) =>
            {
                if (window.ShowDialog() == true)
                    onSuccess(window.ViewModel.Schedules.Select(sm => sm.Schedule).ToList());
                return Disposable.Empty;
            });
        }

        public void ShowVersionsDialog()
        {
            VersionWindow window = ServiceProvider.GetRequiredService<VersionWindow>();

            window.Owner = getActiveWindow();
            window.ViewModel.Modules = new ObservableCollection<ModuleInfo>(
                Enumerable.Repeat(typeof(Dialogs).Assembly, 1).Concat(ServiceProvider.GetServices<IServiceBase>().Select(s => s.GetType().Assembly).Distinct()).Select(s => new ModuleInfo() { Name = s.ManifestModule.ScopeName, Version = FileVersionInfo.GetVersionInfo(s.Location).FileVersion }));
            window.ShowDialog();
        }


        public void ShowMessage(string message)
        {
            MessageBox.Show(getActiveWindow(), message, Options.Value.ServiceConsoleTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(getActiveWindow(), message, Options.Value.ServiceConsoleTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool ShowConfirm(string message)
        {
            return MessageBox.Show(getActiveWindow(), message, Options.Value.ServiceConsoleTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        Window getActiveWindow()
        {
            return App.Current.Windows.Cast<Window>().FirstOrDefault(window => window.IsActive) ?? App.Current.MainWindow;
        }
    }
}

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

namespace ZDevTools.ServiceConsole.DIServices
{
    using Schedules;
    using Views;
    using Models;
    using ViewModels;
    using static CommonFunctions;

    public class Dialogs : IDialogs
    {
        public void ShowOneKeyStartConfigDialog(Dictionary<string, ServiceItemConfig> configs)
        {
            OneKeyStartConfigWindow window = new OneKeyStartConfigWindow();
            window.Owner = GetActiveWindow();
            window.ViewModel.Configs = new ObservableCollection<ServiceItemModel>(configs.Select(kv => new ServiceItemModel() { OneKeyStart = kv.Value.OneKeyStart, ServiceName = kv.Value.ServiceName, ServiceKey = kv.Key }));

            if (window.ShowDialog() == true)
            {
                foreach (var config in window.ViewModel.Configs)
                    configs[config.ServiceKey].OneKeyStart = config.OneKeyStart;
            }
        }

        public BasicSchedule ShowScheduleDialog(BasicSchedule basicSchedule)
        {
            ScheduleWindow window = new ScheduleWindow();
            window.Owner = GetActiveWindow();
            if (basicSchedule != null)
                window.ViewModel.LoadModel(basicSchedule);
            if (window.ShowDialog() == true)
                return window.ViewModel.SaveSchedule();
            else
                return null;
        }

        public List<BasicSchedule> ShowSchedulesDialog(bool canManage, List<BasicSchedule> schedules)
        {
            ScheduleManageWindow window = new ScheduleManageWindow();
            window.Owner = GetActiveWindow();
            window.ViewModel.Schedules = new ObservableCollection<ScheduleModel>(schedules.Select(s => new ScheduleModel() { Schedule = s }));
            window.ViewModel.CanManage = canManage;

            if (window.ShowDialog() == true)
            {
                return window.ViewModel.Schedules.Select(sm => sm.Schedule).ToList();
            }
            else
                return null;
        }

        public void ShowVersionsDialog()
        {
            VersionWindow window = new VersionWindow();
            window.Owner = GetActiveWindow();
            window.ViewModel.Modules = new ObservableCollection<ModuleInfo>(
                Enumerable.Repeat(typeof(Dialogs).Assembly, 1).Concat(MainWindowViewModel.GetServicesFromMef().Select(s => s.GetType().Assembly).Distinct()).Select(s => new ModuleInfo() { Name = s.ManifestModule.ScopeName, Version = FileVersionInfo.GetVersionInfo(s.Location).FileVersion }));
            window.ShowDialog();
        }

    }
}

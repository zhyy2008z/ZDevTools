using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ZDevTools.ServiceConsole.Views;
using ZDevTools.ServiceConsole.Schedules;
using ZDevTools.ServiceConsole.Services;
using ZDevTools.ServiceConsole.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace ZDevTools.ServiceConsole.ViewModels
{
    public class ScheduleManageViewModel : ReactiveObject
    {

        public ScheduleManageViewModel(IDialogs dialogs)
        {

            EditScheduleCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedSchedule != null)
                {
                    dialogs.ShowScheduleDialog(SelectedSchedule.Schedule, schedule =>
                    {
                        SelectedSchedule.Schedule = schedule;
                        refreshItems();
                    });
                }
            });

            AddScheduleCommand = ReactiveCommand.Create(() =>
            {
                dialogs.ShowScheduleDialog(null, schedule =>
                {
                    Schedules.Add(new ScheduleModel() { Schedule = schedule });
                    refreshItems();

                });
            });

            DeleteScheduleCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedSchedule != null && dialogs.ShowConfirm("确定要删除选中的计划？"))
                {
                    Schedules.Remove(SelectedSchedule);
                    refreshItems();
                }
            });

            SelectionChangedCommand = ReactiveCommand.Create(refreshItems);

            refreshItems();
        }

        [Reactive]
        public bool CanManage { get; set; }


        [Reactive]
        public ObservableCollection<ScheduleModel> Schedules { get; set; }


        [Reactive]
        public ScheduleModel SelectedSchedule { get; set; }

        [Reactive]
        public bool DeleteEnabled { get; set; }


        [Reactive]
        public bool EditEnabled { get; set; }

        [Reactive]
        public bool AddEnabled { get; set; }


        [Reactive]
        public bool OKVisible { get; set; }

        public ReactiveCommand<Unit, Unit> SelectionChangedCommand { get; }

        public ReactiveCommand<Unit, Unit> EditScheduleCommand { get; }

        public ReactiveCommand<Unit, Unit> DeleteScheduleCommand { get; }

        public ReactiveCommand<Unit, Unit> AddScheduleCommand { get; }


        void refreshItems()
        {
            updateButtonStates();
        }

        private void updateButtonStates()
        {
            if (CanManage)
            {
                if (SelectedSchedule == null)
                {
                    DeleteEnabled = false;
                    EditEnabled = false;
                }
                else
                {
                    DeleteEnabled = true;
                    EditEnabled = true;
                }
            }
            else
            {
                DeleteEnabled = false;
                EditEnabled = false;
                AddEnabled = false;
                OKVisible = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using System.Collections.ObjectModel;
using static ZDevTools.ServiceConsole.CommonFunctions;


namespace ZDevTools.ServiceConsole.ViewModels
{
    using Views;
    using Schedules;
    using DIServices;
    using Models;

    public class ScheduleManageWindowViewModel : BindableBase
    {

        IDialogs _dialogs;

        public ScheduleManageWindowViewModel(IDialogs dialogs)
        {
            OKCommand = new DelegateCommand(okOperate);
            EditScheduleCommand = new DelegateCommand(editSchedule);
            AddScheduleCommand = new DelegateCommand(addSchedule);
            DeleteScheduleCommand = new DelegateCommand(deleteSchedule);
            SelectionChangedCommand = new DelegateCommand(refreshItems);
            _dialogs = dialogs;
            refreshItems();
        }

        private void okOperate()
        {
            Window.DialogResult = true;
        }
        public DelegateCommand OKCommand { get; }

        public Synchronizer Synchronizer { get; set; }

        public ScheduleManageWindow Window { get; set; }

        bool _canManage;
        public bool CanManage { get { return _canManage; } set { SetProperty(ref _canManage, value); } }


        ObservableCollection<ScheduleModel> _schedules;
        public ObservableCollection<ScheduleModel> Schedules { get { return _schedules; } set { SetProperty(ref _schedules, value); } }


        ScheduleModel _selectedSchedule;
        public ScheduleModel SelectedSchedule { get { return _selectedSchedule; } set { SetProperty(ref _selectedSchedule, value); } }


        void refreshItems()
        {
            updateButtonStates();
        }

        public DelegateCommand SelectionChangedCommand { get; }

        bool _deleteEnabled;
        public bool DeleteEnabled { get { return _deleteEnabled; } set { SetProperty(ref _deleteEnabled, value); } }


        bool _editEnabled;
        public bool EditEnabled { get { return _editEnabled; } set { SetProperty(ref _editEnabled, value); } }

        bool _addEnabled;
        public bool AddEnabled { get { return _addEnabled; } set { SetProperty(ref _addEnabled, value); } }


        bool _oKVisible;
        public bool OKVisible { get { return _oKVisible; } set { SetProperty(ref _oKVisible, value); } }

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


        public DelegateCommand EditScheduleCommand { get; }
        private void editSchedule()
        {
            if (SelectedSchedule != null)
            {
                var schedule = _dialogs.ShowScheduleDialog(SelectedSchedule.Schedule);
                if (schedule != null)
                {
                    SelectedSchedule.Schedule = schedule;
                    refreshItems();
                }
            }
        }

        public DelegateCommand DeleteScheduleCommand { get; }
        private void deleteSchedule()
        {
            if (SelectedSchedule != null && ShowConfirm("确定要删除选中的计划？"))
            {
                Schedules.Remove(SelectedSchedule);
                refreshItems();
            }
        }

        public DelegateCommand AddScheduleCommand { get; }
        private void addSchedule()
        {
            var schedule = _dialogs.ShowScheduleDialog(null);
            if (schedule != null)
            {
                Schedules.Add(new ScheduleModel() { Schedule = schedule });
                refreshItems();
            }
        }

    }
}

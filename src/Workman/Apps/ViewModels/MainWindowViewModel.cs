using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using Workman.Apps.Entities;
using Workman.Apps.Helpers;
using Workman.Core.Entities;
using Workman.Core.Repositories;
using Workman.Core.Services;

namespace Workman.Apps.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly IWorkmanService _workmanService;
        private bool _taskBarSettingExecuting;
        private bool _taskBarExitExecuting;

        public MainWindowViewModel(IDialogService dialogService, IWorkmanService workmanService)
        {
            _dialogService = dialogService;
            _workmanService = workmanService;
            Initialize();
        }

        [ObservableProperty]
        private DateTime _selectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        [ObservableProperty]
        private bool _isPin;

        [ObservableProperty]
        private bool _showLog;


        [ObservableProperty]
        private ObservableCollection<WorkLogVO> _workLogs;


        [RelayCommand]
        private void ShowWorkLog()
        {
            ShowLog = !ShowLog;
        }

        [RelayCommand]
        private void AppSetting()
        {
            _dialogService.ShowDialog("AppSettingView");
        }

        [RelayCommand]
        private void NextDay()
        {
            ShowLog = true;
            SelectedDate = SelectedDate.AddDays(1);
        }

        [RelayCommand]
        private void PreviousDay()
        {
            ShowLog = true;
            SelectedDate = SelectedDate.AddDays(-1);
        }

        [RelayCommand]
        private void Today()
        {
            ShowLog = true;
            DateTime day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            SelectedDate = day;
        }

        [RelayCommand]
        private void AddLog()
        {
            _dialogService.ShowDialog("CreateWorkLogView", new DialogParameters
            {
                { "date",SelectedDate }
            }, async dr =>
            {
                if (dr.Result == ButtonResult.OK)
                {
                    await RefreshDateWorkLog(SelectedDate);
                }
            });
        }

        [RelayCommand]
        private void ExportLog()
        {
            _dialogService.ShowDialog("ExportWorkLogView", new DialogParameters
            {
                { "date",SelectedDate }
            });
        }

        [RelayCommand]
        private void ProjectManage()
        {
            _dialogService.ShowDialog("ManageWorkProjectView");
        }

        [RelayCommand]
        private void TaskManage()
        {
            _dialogService.ShowDialog("ManageWorkTaskView");
        }

        [RelayCommand]
        private async Task DeleteLog(WorkLogVO? workLog)
        {
            if (workLog == null)
            {
                return;
            }
            bool success = await _workmanService.DeleteLog(workLog.Id);
            if (!success)
            {
                MessageHelper.ShowError(string.Format(LocalizationManager.Instance.FailedMessage, LocalizationManager.Instance.Delete));
                return;
            }
            WorkLogs.Remove(workLog);
        }

        [RelayCommand]
        private void EditLog(WorkLogVO? workLog)
        {
            if (workLog == null)
            {
                return;
            }
            _dialogService.ShowDialog("UpdateWorkLogView", new DialogParameters
            {
                { "workLogId", workLog.Id }
            },
            async () =>
            {
                await RefreshDateWorkLog(SelectedDate);
            });
        }

        private void TaskBarCommandExecuteProxy(ref bool isExecutingRef, Action action)
        {
            if (isExecutingRef)
            {
                return;
            }
            try
            {
                isExecutingRef = true;
                action?.Invoke();
            }
            finally
            {
                isExecutingRef = false;
            }
        }

        [RelayCommand]
        private void TaskBarActive()
        {
            Application.Current.MainWindow.Activate();
            IsPin = true;
        }

        [RelayCommand]
        private void TaskBarAppSetting()
        {
            TaskBarCommandExecuteProxy(ref _taskBarSettingExecuting, () =>
            {
                Application.Current.MainWindow.Activate();
                IsPin = true;
                _dialogService.ShowDialog("AppSettingView");
            });
        }

        [RelayCommand]
        private void TaskBarExit()
        {
            TaskBarCommandExecuteProxy(ref _taskBarExitExecuting, () =>
            {
                Application.Current.MainWindow.Activate();
                MessageBoxResult messageBoxResult = MessageHelper.ShowYesNo(LocalizationManager.Instance.ExitAppHint);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Environment.Exit(0);
                }
            });
        }


        partial void OnIsPinChanged(bool value)
        {
            if (!value)
            {
                ShowLog = false;
            }
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            RefreshDateWorkLog(SelectedDate);
        }

        private async void Initialize()
        {
            await RefreshDateWorkLog(SelectedDate);
        }

        private async Task RefreshDateWorkLog(DateTime dateTime)
        {
            DateTime day = new DateTime(dateTime.Year,dateTime.Month,dateTime.Day);
            List<WorkLog> logs = await _workmanService.GetLogs(day);

            List<WorkLogVO> logVOs = new List<WorkLogVO>();
            foreach (WorkLog log in logs)
            {
                WorkLogVO logVO = new WorkLogVO
                {
                    Id = log.Id,
                    Date = log.Date,
                    ElapsedTime = log.ElapsedTime,
                };
                WorkTask? task = await _workmanService.GetTask(log.TaskId);
                if (task != null)
                {
                    WorkTaskVO taskVO = new WorkTaskVO
                    {
                        Id = task.Id,
                        ArchivedTime = task.ArchivedTime,
                        CreatedTime = task.CreatedTime,
                        Name = task.Name,
                        IsArchived = task.IsArchived,
                    };
                    WorkProject? project = await _workmanService.GetProject(task.ProjectId);
                    if(project != null)
                    {
                        taskVO.Project = new WorkProjectVO
                        {
                            Id = project.Id,
                            ArchivedTime = project.ArchivedTime,
                            CreatedTime = project.CreatedTime,
                            Name = project.Name,
                            IsArchived = project.IsArchived,
                        };
                    }
                    logVO.Task = taskVO;
                }
                logVOs.Add(logVO);
            }

            for (int i = 0; i < logVOs.Count; i++)
            {
                logVOs[i].OrderId = i + 1;
            }
            WorkLogs = new ObservableCollection<WorkLogVO>(logVOs);
        }
    }
}

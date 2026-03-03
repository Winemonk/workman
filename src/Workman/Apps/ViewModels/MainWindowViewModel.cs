using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using Workman.Apps.Entities;
using Workman.Core.Entities;
using Workman.Core.Repositories;

namespace Workman.Apps.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<WorkLog> _workLogRepository;

        public MainWindowViewModel(IDialogService dialogService,
                                   IRepository<Project> projectRepository,
                                   IRepository<WorkLog> workLogRepository)
        {
            _dialogService = dialogService;
            _projectRepository = projectRepository;
            _workLogRepository = workLogRepository;
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
            SelectedDate = SelectedDate.AddDays(1);
        }

        [RelayCommand]
        private void PreviousDay()
        {
            SelectedDate = SelectedDate.AddDays(-1);
        }

        [RelayCommand]
        private void Today()
        {
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
                if (dr.Result != ButtonResult.OK)
                {
                    return;
                }
                List<WorkLogVO> workLogVOs = await QueryDateWorkLog(SelectedDate);
                WorkLogs = new(workLogVOs);
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
            _dialogService.ShowDialog("ProjectManageView");
        }

        [RelayCommand]
        private async Task DeleteLog(WorkLogVO? workLog)
        {
            if (workLog == null)
            {
                return;
            }
            bool success = await _workLogRepository.Delete(workLog.Id);
            if (!success)
            {
                MessageBox.Show("删除失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
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
                { "log", workLog }
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
            Task.Run(async () =>
            {
                List<WorkLogVO> workLogVOs = await QueryDateWorkLog(value);
                WorkLogs = new (workLogVOs);
            });
        }

        private async void Initialize()
        {
            List<WorkLogVO> workLogVOs = await QueryDateWorkLog(SelectedDate);
            WorkLogs = new(workLogVOs);
        }

        private async Task<List<WorkLogVO>> QueryDateWorkLog(DateTime dateTime)
        {
            IEnumerable<Project> projects = await _projectRepository.QueryRange();
            DateTime day = new DateTime(dateTime.Year,dateTime.Month,dateTime.Day);
            DateTime nextDay = day.AddDays(1);
            IEnumerable<WorkLog> workLogs = await _workLogRepository.QueryRange(q => q.Where(e => e.Date >= day && e.Date < nextDay));
            List<WorkLogVO> workLogVOs = workLogs.Select(log => new WorkLogVO
            {
                Content = log.Content,
                Date = log.Date,
                ElapsedTime = log.ElapsedTime,
                Id = log.Id,
                ProjectId = log.ProjectId,
                ProjectName = projects.First(p => p.Id == log.ProjectId).Name,
            }).ToList();
            for (int i = 0; i < workLogVOs.Count; i++)
            {
                workLogVOs[i].OrderId = i + 1;
            }
            return workLogVOs;
        }
    }
}

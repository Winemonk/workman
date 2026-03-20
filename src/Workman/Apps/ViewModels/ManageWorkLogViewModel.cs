using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using Workman.Apps.Entities;
using Workman.Apps.Helpers;
using Workman.Core.Entities;
using Workman.Core.Services;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("ManageWorkLogView")]
    internal partial class ManageWorkLogViewModel : ObservableObject, IDialogAware
    {
        private readonly IWorkmanService _workmanService;
        private readonly IDialogService _dialogService;

        public ManageWorkLogViewModel(IWorkmanService workmanService, IDialogService dialogService)
        {
            _workmanService = workmanService;
            _dialogService = dialogService;
        }

        [ObservableProperty]
        private List<WorkTaskVO> _tasks;

        [ObservableProperty]
        private WorkTaskVO _selectedTask;

        [ObservableProperty]
        private ObservableCollection<WorkLogVO> _logs;


        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private void CreateLog()
        {
            _dialogService.ShowDialog("CreateWorkLogView", new DialogParameters
            {
                { "workTaskId", SelectedTask.Id }
            }, async dr =>
            {
                if (dr.Result == ButtonResult.OK)
                {
                    await RefreshLogs(SelectedTask.Id);
                }
            });
        }

        [RelayCommand]
        private void EditLog(WorkLogVO? log)
        {
            if (log == null)
            {
                return;
            }
            _dialogService.ShowDialog("UpdateWorkLogView", 
                                      new DialogParameters
                                      {
                                          {"workLogId", log.Id }
                                      },
                                      async dr =>
                                      {
                                          if (dr.Result == ButtonResult.OK)
                                          {
                                              await RefreshLogs(SelectedTask.Id);
                                          }
                                      });
        }

        [RelayCommand]
        private async Task DeleteLog(WorkLogVO? log)
        {
            if (log == null)
            {
                return;
            }
            MessageBoxResult messageBoxResult = MessageHelper.ShowOKCancel(LocalizationManager.Instance.DeleteLogHint);
            if (messageBoxResult != MessageBoxResult.OK)
            {
                return;
            }
            bool success = await _workmanService.DeleteLog(log.Id);
            if (!success)
            {
                MessageHelper.ShowError(string.Format(LocalizationManager.Instance.FailedMessage, LocalizationManager.Instance.Delete));
                return;
            }
            Logs.Remove(log);
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose.Invoke(ButtonResult.Cancel);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        partial void OnSelectedTaskChanged(WorkTaskVO value)
        {
            RefreshLogs(value.Id);
        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            IEnumerable<WorkTask> tasks = await _workmanService.GetTasks();
            List<WorkTaskVO> taskVOs = new List<WorkTaskVO>();
            List<WorkProject> projects = await _workmanService.GetProjects();
            IEnumerable<WorkProjectVO> projectVOs = projects.Select(p => new WorkProjectVO
            {
                Id = p.Id,
                ArchivedTime = p.ArchivedTime,
                CreatedTime = p.CreatedTime,
                IsArchived = p.IsArchived,
                Name = p.Name,
            });
            foreach (var task in tasks)
            {
                WorkTaskVO taskVO = new WorkTaskVO
                {
                    Id = task.Id,
                    ArchivedTime = task.ArchivedTime,
                    CreatedTime = task.CreatedTime,
                    Name = task.Name,
                    IsArchived = task.IsArchived,
                    Project = projectVOs.FirstOrDefault(p => p.Id == task.ProjectId)!,
                };
                taskVOs.Add(taskVO);
            }
            Tasks = taskVOs;
            bool success = parameters.TryGetValue("workTaskId", out int taskId);
            if (!success)
            {
                SelectedTask = Tasks.LastOrDefault()!;
            }
            else
            {
                SelectedTask = Tasks.FirstOrDefault(t => t.Id == taskId)!;
            }
            if (SelectedTask != null)
            {
                await RefreshLogs(SelectedTask.Id);
            }
        }

        private async Task RefreshLogs(int taskId)
        {
            List<WorkLog> logs = await _workmanService.GetLogs(taskId);
            Logs = new ObservableCollection<WorkLogVO>();
            Logs.CollectionChanged += (s, e) =>
            {
                for (int i = 0; i < Logs.Count; i++)
                {
                    Logs[i].OrderId = i + 1;
                }
            };
            foreach (WorkLog log in logs)
            {
                WorkLogVO logVO = new WorkLogVO
                {
                    Content = log.Content,
                    Date = log.Date,
                    ElapsedTime = log.ElapsedTime,
                    Id = log.Id,
                };
                Logs.Add(logVO);
            }
        }
    }
}

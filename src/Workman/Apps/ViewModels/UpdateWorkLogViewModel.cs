using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Windows;
using Workman.Apps.Entities;
using Workman.Apps.Helpers;
using Workman.Core.Entities;
using Workman.Core.Repositories;
using Workman.Core.Services;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("UpdateWorkLogView")]
    internal partial class UpdateWorkLogViewModel : ObservableObject, IDialogAware
    {
        private readonly IWorkmanService _workmanService;

        public UpdateWorkLogViewModel(IWorkmanService workmanService)
        {
            _workmanService = workmanService;
        }

        private int _workLogId;

        [ObservableProperty]
        private WorkTaskVO? _selectedTask;

        [ObservableProperty]
        private List<WorkTaskVO> _tasks;

        [ObservableProperty]
        private float _elapsedTime = 1;

        [ObservableProperty]
        private DateTime _date;

        [ObservableProperty]
        private string _content = string.Empty;

        public DialogCloseListener RequestClose { get; }

        partial void OnSelectedTaskChanged(WorkTaskVO? value)
        {
            if (value == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(Content))
            {
                Content = value.Name;
            }
        }

        /// <summary>
        /// 耗时增加
        /// </summary>
        [RelayCommand]
        private void IncreaseElapsedTime()
        {
            ElapsedTime += 1f;
        }


        /// <summary>
        /// 耗时减少
        /// </summary>
        [RelayCommand]
        private void DecreaseElapsedTime()
        {
            if (ElapsedTime > 1f)
            {
                ElapsedTime -= 1f;
            }
        }


        [RelayCommand]
        private async Task Confirm()
        {
            if (SelectedTask == null)
            {
                MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.NotBeNullMessage, LocalizationManager.Instance.Task));
                return;
            }
            if (ElapsedTime <= 0)
            {
                MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.MustBeGraterThenZeroMessage, LocalizationManager.Instance.ElapsedTime));
                return;
            }
            if (string.IsNullOrEmpty(Content))
            {
                MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.NotBeNullMessage, LocalizationManager.Instance.Content));
                return;
            }

            WorkLog? workLog = await _workmanService.UpdateLog(_workLogId, SelectedTask.Id, Content, ElapsedTime, Date);
            if (workLog == null)
            {
                MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.FailedMessage, LocalizationManager.Instance.UpdateLog));
                return;
            }
            RequestClose.Invoke(ButtonResult.OK);
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

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            bool success = parameters.TryGetValue("workLogId", out _workLogId);
            if (!success)
            {
                RequestClose.Invoke(ButtonResult.Cancel);
                return;
            }
            WorkLog? workLog = await _workmanService.GetLog(_workLogId);
            if (workLog == null)
            {
                RequestClose.Invoke(ButtonResult.Cancel);
                return;
            }
            List<WorkTask> workTasks = await _workmanService.GetTasks();
            List<WorkTaskVO> taskVOs = new List<WorkTaskVO>();
            IEnumerable<WorkProject> projects = await _workmanService.GetProjects();
            List<WorkProjectVO> projectVOs = new List<WorkProjectVO>();
            foreach (WorkProject project in projects)
            {
                projectVOs.Add(new WorkProjectVO
                {
                    Id = project.Id,
                    ArchivedTime = project.ArchivedTime,
                    CreatedTime = project.CreatedTime,
                    Name = project.Name,
                    IsArchived = project.IsArchived,
                });
            }
            int orderId = 1;
            foreach (WorkTask wt in workTasks)
            {
                float taskElapsedTime = await _workmanService.GetTaskElapsedTime(wt.Id);
                WorkTaskVO taskVO = new WorkTaskVO
                {
                    OrderId = orderId++,
                    Id = wt.Id,
                    ArchivedTime = wt.ArchivedTime,
                    CreatedTime = wt.CreatedTime,
                    Name = wt.Name,
                    IsArchived = wt.IsArchived,
                    TotalElapsedTime = taskElapsedTime,
                    Project = projectVOs.FirstOrDefault(p => p.Id == wt.ProjectId)!,
                };
                taskVOs.Add(taskVO);
            }

            Tasks = taskVOs;
            SelectedTask = Tasks.FirstOrDefault(t => t.Id == workLog.TaskId);
            ElapsedTime = workLog.ElapsedTime;
            Date = workLog.Date;
        }
    }
}

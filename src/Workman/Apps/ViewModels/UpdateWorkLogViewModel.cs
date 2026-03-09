using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Windows;
using Workman.Apps.Entities;
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
                MessageBox.Show("任务不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ElapsedTime <= 0)
            {
                MessageBox.Show("耗时必须大于0！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(Content))
            {
                MessageBox.Show("内容不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            WorkLog? workLog = await _workmanService.UpdateLog(_workLogId, SelectedTask.Id, Content, ElapsedTime);
            if (workLog == null)
            {
                MessageBox.Show("更新日志失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
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
            List<WorkTaskVO> workTaskVOs = new List<WorkTaskVO>();
            foreach (WorkTask wt in workTasks)
            {
                float taskElapsedTime = await _workmanService.GetTaskElapsedTime(wt.Id);
                WorkTaskVO taskVO = new WorkTaskVO
                {
                    Id = wt.Id,
                    Name = wt.Name,
                    TotalElapsedTime = taskElapsedTime,
                };
                await _workmanService.GetProject(wt.ProjectId).ContinueWith(projectTask =>
                {
                    if (projectTask.Result != null)
                    {
                        taskVO.Project = new WorkProjectVO
                        {
                            Id = projectTask.Result.Id,
                            Name = projectTask.Result.Name
                        };
                    }
                });
                workTaskVOs.Add(taskVO);
            }

            Tasks = workTaskVOs;
            SelectedTask = Tasks.FirstOrDefault(t => t.Id == workLog.TaskId);
            ElapsedTime = workLog.ElapsedTime;
        }
    }
}

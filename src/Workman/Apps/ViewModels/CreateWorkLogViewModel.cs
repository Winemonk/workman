using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Collections.ObjectModel;
using System.Windows;
using Workman.Apps.Entities;
using Workman.Core.Entities;
using Workman.Core.Services;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("CreateWorkLogView")]
    internal partial class CreateWorkLogViewModel : ObservableObject, IDialogAware
    {
        private readonly IWorkmanService _workmanService;
        private DateTime _dateTime;

        public CreateWorkLogViewModel(IWorkmanService workmanService)
        {
            _workmanService = workmanService;
        }

        [ObservableProperty]
        private List<WorkTaskVO> _tasks;

        [ObservableProperty]
        private ObservableCollection<CreateWorkLogVO> _workLogs;

        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private async Task Confirm()
        {
            foreach (CreateWorkLogVO log in WorkLogs)
            {
                if (log.IsCreated)
                {
                    continue;
                }
                if (log.ElapsedTime <= 0)
                {
                    MessageBox.Show($"项：{log.OrderId} 耗时必须大于0！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (log.Task == null)
                {
                    MessageBox.Show($"项：{log.OrderId} 任务不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrEmpty(log.Content))
                {
                    MessageBox.Show($"项：{log.OrderId} 内容不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            foreach (CreateWorkLogVO log in WorkLogs.Where(t => !t.IsCreated))
            {
                WorkLog? newLog = await _workmanService.CreateLog(_dateTime, log.Task.Id, log.Content, log.ElapsedTime);
                if (newLog == null)
                {
                    MessageBox.Show($"添加日志：{log.OrderId}失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
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
            bool success = parameters.TryGetValue("date", out _dateTime);
            if (!success)
            {
                RequestClose.Invoke(ButtonResult.Cancel);
            }
            IEnumerable<WorkTask> workTasks = await _workmanService.GetTasks();
            List<WorkTaskVO> taskVOs = new List<WorkTaskVO>();
            int orderId = 1;
            foreach (WorkTask wt in workTasks)
            {
                float taskElapsedTime = await _workmanService.GetTaskElapsedTime(wt.Id);
                WorkTaskVO taskVO = new WorkTaskVO
                {
                    OrderId = orderId++,
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
                taskVOs.Add(taskVO);
            }
            Tasks = taskVOs.ToList();
            WorkLogs = new ObservableCollection<CreateWorkLogVO>();
            WorkLogs.CollectionChanged += WorkLogs_CollectionChanged;
        }

        private void WorkLogs_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                return;
            }
            foreach (CreateWorkLogVO item in e.NewItems ?? Array.Empty<CreateWorkLogVO>())
            {
                item.OrderId = WorkLogs.Count;
                item.ElapsedTime = 1;
            }
        }
    }
}

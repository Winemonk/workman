using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Collections.ObjectModel;
using System.Windows;
using Workman.Apps.Entities;
using Workman.Apps.Helpers;
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
                    MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.ItemMustBeGraterThenZeroMessage, log.OrderId, LocalizationManager.Instance.ElapsedTime));
                    return;
                }
                if (log.Task == null)
                {
                    MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.ItemNotBeNullMessage, log.OrderId, LocalizationManager.Instance.Task));
                    return;
                }
                if (string.IsNullOrEmpty(log.Content))
                {
                    MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.ItemNotBeNullMessage, log.OrderId, LocalizationManager.Instance.Content));
                    return;
                }
            }

            foreach (CreateWorkLogVO log in WorkLogs.Where(t => !t.IsCreated))
            {
                WorkLog? newLog = await _workmanService.CreateLog(_dateTime, log.Task.Id, log.Content, log.ElapsedTime);
                if (newLog == null)
                {
                    MessageHelper.ShowError(string.Format(LocalizationManager.Instance.ItemFailedMessage, log.OrderId, LocalizationManager.Instance.AddLog));
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

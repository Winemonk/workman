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

        public CreateWorkLogViewModel(IWorkmanService workmanService)
        {
            _workmanService = workmanService;
        }

        [ObservableProperty]
        private List<WorkTaskVO> _tasks;

        [ObservableProperty]
        private ObservableCollection<CreateWorkLogVO> _workLogs;

        private DateTime _setDateTime;
        private WorkTaskVO _setTaskVO;

        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private async Task Confirm()
        {
            foreach (CreateWorkLogVO log in WorkLogs.Where(t => !t.IsCreated))
            {
                if (log.Date == null)
                {
                    MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.ItemMustBeGraterThenZeroMessage, log.OrderId, LocalizationManager.Instance.Date));
                    return;
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
                    log.Content = log.Task.Name;
                    //MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.ItemNotBeNullMessage, log.OrderId, LocalizationManager.Instance.Content));
                    //return;
                }
                WorkLog? newLog = await _workmanService.CreateLog(log.Date.Value, log.Task.Id, log.Content, log.ElapsedTime);
                if (newLog == null)
                {
                    MessageHelper.ShowError(string.Format(LocalizationManager.Instance.ItemFailedMessage, log.OrderId, LocalizationManager.Instance.AddLog));
                    return;
                }
                log.IsCreated = true;
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
            bool success = parameters.TryGetValue("date", out _setDateTime);
            if (!success)
            {
                _setDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
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
            IEnumerable<WorkTask> workTasks = await _workmanService.GetNotArchivedTasks();
            List<WorkTaskVO> taskVOs = new List<WorkTaskVO>();
            int orderId = 1;
            foreach (WorkTask wt in workTasks)
            {
                WorkTaskVO taskVO = new WorkTaskVO
                {
                    OrderId = orderId++,
                    Id = wt.Id,
                    ArchivedTime = wt.ArchivedTime,
                    CreatedTime = wt.CreatedTime,
                    Name = wt.Name,
                    IsArchived = wt.IsArchived,
                    Project = projectVOs.FirstOrDefault(p => p.Id == wt.ProjectId)!,
                };
                taskVOs.Add(taskVO);
            }

            bool getTask = parameters.TryGetValue("workTaskId", out int setTaskId);
            if (getTask)
            {
                WorkTask? setTask = await _workmanService.GetTask(setTaskId);
                if (setTask != null && setTask.IsArchived)
                {
                    _setTaskVO = new WorkTaskVO
                    {
                        OrderId = orderId++,
                        Id = setTask.Id,
                        ArchivedTime = setTask.ArchivedTime,
                        CreatedTime = setTask.CreatedTime,
                        Name = setTask.Name,
                        IsArchived = setTask.IsArchived,
                        Project = projectVOs.FirstOrDefault(p => p.Id == setTask.ProjectId)!,
                    };
                    taskVOs.Add(_setTaskVO);
                }
            }

            Tasks = taskVOs.ToList();

            WorkLogs = new ObservableCollection<CreateWorkLogVO>();
            WorkLogs.CollectionChanged += (s, e) =>
            {
                for (int i = 0; i < WorkLogs.Count; i++)
                {
                    WorkLogs[i].OrderId = i + 1;
                }
                foreach (CreateWorkLogVO newItem in e.NewItems ?? Array.Empty<CreateWorkLogVO>())
                {
                    newItem.Date = _setDateTime;
                    newItem.Task = _setTaskVO;
                }
            };
        }
    }
}

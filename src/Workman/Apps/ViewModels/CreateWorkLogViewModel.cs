using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Collections.ObjectModel;
using System.Windows;
using Workman.Apps.Entities;
using Workman.Core.Entities;
using Workman.Core.Repositories;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("CreateWorkLogView")]
    internal partial class CreateWorkLogViewModel : ObservableObject, IDialogAware
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<WorkLog> _workLogRepository;
        private DateTime _dateTime;

        public CreateWorkLogViewModel(IRepository<WorkLog> workLogRepository,
                                      IRepository<Project> projectRepository)
        {
            _workLogRepository = workLogRepository;
            _projectRepository = projectRepository;
        }

        [ObservableProperty]
        private List<Project> _projects;

        [ObservableProperty]
        private ObservableCollection<CreateWorkLogVO> _workLogs;

        private Project _memoryProject;

        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private async Task Confirm()
        {
            foreach (CreateWorkLogVO log in WorkLogs)
            {
                if (string.IsNullOrWhiteSpace(log.Content))
                {
                    MessageBox.Show($"项：{log.OrderId} 内容不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (log.ElapsedTime <= 0)
                {
                    MessageBox.Show($"项：{log.OrderId} 耗时必须大于0！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            IEnumerable<WorkLog> workLogs = WorkLogs.Select(l => new WorkLog
            {
                Content = l.Content,
                Date = _dateTime,
                ElapsedTime = l.ElapsedTime,
                ProjectId = l.Project.Id,
            });

            IEnumerable<WorkLog> insertedWorkLogs = await _workLogRepository.InsertRange(workLogs);
            if (!insertedWorkLogs.Any())
            {
                MessageBox.Show("添加日志失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
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
            bool success = parameters.TryGetValue("date", out _dateTime);
            if (!success)
            {
                RequestClose.Invoke(ButtonResult.Cancel);
            }
            IEnumerable<Project> projects = await _projectRepository.QueryRange();
            Projects = projects.ToList();
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
                item.Project = _memoryProject;
                item.OrderId = WorkLogs.Count;
                item.ElapsedTime = 1;
                item.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(CreateWorkLogVO.Project)
                    && s is CreateWorkLogVO newItem)
                    {
                        _memoryProject = newItem.Project;
                    }
                };
            }
        }
    }
}

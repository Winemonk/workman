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
    [RegisterDialog("CreateWorkTaskView")]
    internal partial class CreateWorkTaskViewModel : ObservableObject, IDialogAware
    {
        private IWorkmanService _workmanService;

        public CreateWorkTaskViewModel(IWorkmanService workmanService)
        {
            _workmanService = workmanService;
        }

        [ObservableProperty]
        private List<WorkProject> _projects;

        [ObservableProperty]
        private WorkProject _selectedProject;

        [ObservableProperty]
        private ObservableCollection<CreateWorkTaskVO> _workTasks;

        private WorkProject _memoryProject;

        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private async Task Confirm()
        {
            foreach (CreateWorkTaskVO task in WorkTasks)
            {
                if (string.IsNullOrWhiteSpace(task.Content))
                {
                    MessageBox.Show($"项：{task.OrderId} 内容不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            foreach (var task in WorkTasks.Where(t => !t.IsCreated))
            {
                WorkTask? newTask = await _workmanService.CreateTask(task.Project.Id, task.Content);
                if (newTask == null)
                {
                    MessageBox.Show($"添加任务：{task.OrderId}失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
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
            IEnumerable<WorkProject> projects = await _workmanService.GetProjects();
            Projects = projects.ToList();
            WorkTasks = new ObservableCollection<CreateWorkTaskVO>();
            WorkTasks.CollectionChanged += WorkLogs_CollectionChanged;
        }

        private void WorkLogs_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                return;
            }
            foreach (CreateWorkTaskVO item in e.NewItems ?? Array.Empty<CreateWorkTaskVO>())
            {
                item.Project = _memoryProject;
                item.OrderId = WorkTasks.Count;
                item.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(CreateWorkTaskVO.Project)
                        && s is CreateWorkTaskVO newItem)
                    {
                        _memoryProject = newItem.Project;
                    }
                };
            }
        }
    }
}

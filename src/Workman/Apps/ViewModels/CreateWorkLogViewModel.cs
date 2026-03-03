using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Windows;
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
        private Project? _selectedProject;

        [ObservableProperty]
        private List<Project> _projects;

        [ObservableProperty]
        private string _content = string.Empty;

        [ObservableProperty]
        private float _elapsedTime = 1;

        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private async Task Confirm()
        {
            if (SelectedProject == null)
            {
                MessageBox.Show("项目不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(Content))
            {
                MessageBox.Show("内容不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ElapsedTime <= 0)
            {
                MessageBox.Show("耗时必须大于0！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            WorkLog workLog = new WorkLog
            {
                Content = Content,
                Date = _dateTime,
                ElapsedTime = ElapsedTime,
                ProjectId = SelectedProject.Id,
            };

            WorkLog? insertedWorkLog = await _workLogRepository.Insert(workLog);
            if (insertedWorkLog == null)
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
            SelectedProject = Projects.LastOrDefault();
        }
    }
}

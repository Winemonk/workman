using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Windows;
using Workman.Apps.Entities;
using Workman.Core.Entities;
using Workman.Core.Repositories;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("UpdateWorkLogView")]
    internal partial class UpdateWorkLogViewModel : ObservableObject, IDialogAware
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<WorkLog> _workLogRepository;
        private WorkLogVO _logVO;

        public UpdateWorkLogViewModel(IRepository<WorkLog> workLogRepository,
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

            WorkLog? workLog = await _workLogRepository.Query(_logVO.Id);
            if (workLog == null)
            {
                MessageBox.Show("未查询到日志！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            workLog.Content = Content;
            workLog.ElapsedTime = ElapsedTime;
            workLog.ProjectId = SelectedProject.Id;

            WorkLog updatedWorkLog = await _workLogRepository.Update(workLog);

            _logVO.Content = Content;
            _logVO.ElapsedTime = ElapsedTime;
            _logVO.ProjectId = SelectedProject.Id;

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
            bool success = parameters.TryGetValue("log", out _logVO!);
            if (!success)
            {
                RequestClose.Invoke(ButtonResult.Cancel);
            }
            IEnumerable<Project> projects = await _projectRepository.QueryRange();
            Projects = projects.ToList();
            SelectedProject = Projects.LastOrDefault();
            Content = _logVO!.Content;
            ElapsedTime = _logVO!.ElapsedTime;
            SelectedProject = Projects.First(p => p.Id == _logVO!.ProjectId);
        }
    }
}

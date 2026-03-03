using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Collections.ObjectModel;
using System.Windows;
using Workman.Core.Entities;
using Workman.Core.Repositories;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("ProjectManageView")]
    internal partial class ProjectManageViewModel : ObservableObject, IDialogAware
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IDialogService _dialogService;

        public ProjectManageViewModel(IRepository<Project> projectRepository,
                                      IDialogService dialogService)
        {
            _projectRepository = projectRepository;
            _dialogService = dialogService;
        }

        [ObservableProperty]
        private ObservableCollection<Project> _projects;

        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private void CreateProject()
        {
            _dialogService.ShowDialog("CreateProjectView", async dr =>
            {
                if(dr.Result == ButtonResult.OK)
                {
                    await RefreshProjects();
                }
            });
        }

        [RelayCommand]
        private async Task DeleteProject(Project? project)
        {
            if (project == null)
            {
                return;
            }
            MessageBoxResult messageBoxResult = MessageBox.Show("项目删除后相关日志也会删除，确认删除项目？",
                                                                "提示",
                                                                MessageBoxButton.OKCancel,
                                                                MessageBoxImage.Warning);
            if(messageBoxResult != MessageBoxResult.OK)
            {
                return;
            }
            bool success = await _projectRepository.Delete(project.Id);
            if (!success)
            {
                MessageBox.Show("删除项目失败！",
                                "提示",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }
            Projects.Remove(project);
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
            await RefreshProjects();
        }

        private async Task RefreshProjects()
        {
            IEnumerable<Project> projects = await _projectRepository.QueryRange();
            Projects = new ObservableCollection<Project>(projects);
        }
    }
}

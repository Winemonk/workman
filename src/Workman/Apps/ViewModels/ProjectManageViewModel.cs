using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Collections.ObjectModel;
using System.Windows;
using Workman.Core.Entities;
using Workman.Core.Services;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("ProjectManageView")]
    internal partial class ProjectManageViewModel : ObservableObject, IDialogAware
    {
        private readonly IWorkmanService _workmanService;
        private readonly IDialogService _dialogService;

        public ProjectManageViewModel(IDialogService dialogService, IWorkmanService workmanService)
        {
            _dialogService = dialogService;
            _workmanService = workmanService;
        }

        [ObservableProperty]
        private ObservableCollection<WorkProjectVO> _projects;

        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private void CreateProject()
        {
            _dialogService.ShowDialog("CreateWorkProjectView", async dr =>
            {
                if(dr.Result == ButtonResult.OK)
                {
                    await RefreshProjects();
                }
            });
        }

        [RelayCommand]
        private async Task DeleteProject(WorkProjectVO? project)
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
            bool success = await _workmanService.DeleteProject(project.Id);
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
            IEnumerable<WorkProject> projects = await _workmanService.GetProjects();
            int orderId = 1;
            List<WorkProjectVO> projectVOs = new List<WorkProjectVO>();
            foreach (WorkProject project in projects)
            {
                float time = await _workmanService.GetProjectElapsedTime(project.Id);
                int count = await _workmanService.GetProjectTaskCount(project.Id);
                projectVOs.Add(new WorkProjectVO
                {
                    Id = project.Id,
                    OrderId = orderId++,
                    Name = project.Name,
                    ElapsedTime = time,
                    TaskCount = count,
                });
            }
            Projects = new ObservableCollection<WorkProjectVO>(projectVOs);

            Projects.CollectionChanged += (s, e) =>
            {
                for (int i = 0; i < Projects.Count; i++)
                {
                    Projects[i].OrderId = i + 1;
                }
            };
        }
    }
}

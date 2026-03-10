using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Windows;
using Workman.Apps.Entities;
using Workman.Apps.Helpers;
using Workman.Core.Entities;
using Workman.Core.Repositories;
using Workman.Core.Services;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("UpdateWorkTaskView")]
    internal partial class UpdateWorkTaskViewModel : ObservableObject, IDialogAware
    {
        private readonly IWorkmanService _workmanService;
        private int _workTaskId;

        public UpdateWorkTaskViewModel(IWorkmanService workmanService)
        {
            _workmanService = workmanService;
        }

        [ObservableProperty]
        private WorkProject? _selectedProject;

        [ObservableProperty]
        private List<WorkProject> _projects;

        [ObservableProperty]
        private string _content = string.Empty;

        public DialogCloseListener RequestClose { get; }


        [RelayCommand]
        private async Task Confirm()
        {
            if (SelectedProject == null)
            {
                MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.NotBeNullMessage, LocalizationManager.Instance.Iteration));
                return;
            }
            if (string.IsNullOrWhiteSpace(Content))
            {
                MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.NotBeNullMessage, LocalizationManager.Instance.Content));
                return;
            }

            WorkTask? workTask = await _workmanService.UpdateTask(_workTaskId, SelectedProject.Id, Content);

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
            bool success = parameters.TryGetValue("taskId", out _workTaskId);
            if (!success)
            {
                RequestClose.Invoke(ButtonResult.Cancel);
                return;
            }
            WorkTask? workTask = await _workmanService.GetTask(_workTaskId);
            if (workTask == null)
            {
                RequestClose.Invoke(ButtonResult.Cancel);
                return;
            }
            IEnumerable<WorkProject> projects = await _workmanService.GetProjects();
            Projects = projects.ToList();
           
            SelectedProject = Projects.FirstOrDefault(p=>p.Id == workTask.ProjectId);
            Content = workTask.Name;
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using Microsoft.VisualBasic.Logging;
using System.Windows;
using Workman.Apps.Helpers;
using Workman.Core.Entities;
using Workman.Core.Services;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("CreateWorkProjectView")]
    internal partial class CreateWorkProjectViewModel : ObservableObject, IDialogAware
    {
        private readonly IWorkmanService _workmanService;

        public CreateWorkProjectViewModel(IWorkmanService workmanService)
        {
            _workmanService = workmanService;
        }

        [ObservableProperty]
        private string _name = string.Empty;

        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private async Task Confirm()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.NotBeNullMessage, LocalizationManager.Instance.Name));
                return;
            }
            WorkProject? newProject = await _workmanService.CreateProject(Name);
            if (newProject == null)
            {
                MessageHelper.ShowError(string.Format(LocalizationManager.Instance.FailedMessage, LocalizationManager.Instance.NewIteration));
                return;
            }
            RequestClose.Invoke(ButtonResult.OK);
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose.Invoke();
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
    }
}

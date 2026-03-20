using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using Workman.Apps.Helpers;
using Workman.Core.Entities;
using Workman.Core.Services;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("UpdateWorkProjectView")]
    internal partial class UpdateWorkProjectViewModel : ObservableObject, IDialogAware
    {
        private readonly IWorkmanService _workmanService;
        private int _workProjectId;

        public UpdateWorkProjectViewModel(IWorkmanService workmanService)
        {
            _workmanService = workmanService;
        }

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private bool _isArchived;

        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private async Task Confirm()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.NotBeNullMessage, LocalizationManager.Instance.Name));
                return;
            }
            WorkProject? newProject = await _workmanService.UpdateProject(_workProjectId, Name, IsArchived);
            if (newProject == null)
            {
                MessageHelper.ShowError(string.Format(LocalizationManager.Instance.FailedMessage, LocalizationManager.Instance.UpdateIteration));
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
            bool success = parameters.TryGetValue("workProjectId", out _workProjectId);
            if (!success)
            {
                RequestClose.Invoke(ButtonResult.Cancel);
                return;
            }
            _workmanService.GetProject(_workProjectId).ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    Name = t.Result?.Name ?? string.Empty;
                    IsArchived = t.Result?.IsArchived ?? false;
                }
            });
        }
    }
}

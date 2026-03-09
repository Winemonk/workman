using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Windows;
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
                MessageBox.Show("名称不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            WorkProject? newProject = await _workmanService.CreateProject(Name);
            if (newProject == null)
            {
                MessageBox.Show("创建失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Windows;
using Workman.Core.Entities;
using Workman.Core.Repositories;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("CreateProjectView")]
    internal partial class CreateProjectViewModel : ObservableObject, IDialogAware
    {
        private readonly IRepository<Project> _projectRepository;

        public CreateProjectViewModel(IRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
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
            Project? project = await _projectRepository.Insert(new Project
            {
                Name = Name
            });
            if(project == null)
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

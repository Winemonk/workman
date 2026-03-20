using CommunityToolkit.Mvvm.ComponentModel;

namespace Workman.Core.Entities
{
    internal partial class CheckWorkProjectVO : WorkProjectVO
    {
        [ObservableProperty]
        private bool _isChecked;
    }
}

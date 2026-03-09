using CommunityToolkit.Mvvm.ComponentModel;
using Workman.Core.Entities;

namespace Workman.Apps.Entities
{
    internal partial class CreateWorkTaskVO : ObservableObject
    {
        [ObservableProperty]
        private long _orderId;
        [ObservableProperty]
        private WorkProject _project;
        [ObservableProperty]
        private string _content = string.Empty;
        [ObservableProperty]
        private bool _isCreated;
    }
}

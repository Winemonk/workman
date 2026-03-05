using CommunityToolkit.Mvvm.ComponentModel;
using Workman.Core.Entities;

namespace Workman.Apps.Entities
{
    internal partial class CreateWorkLogVO : ObservableObject
    {
        [ObservableProperty]
        private long _orderId;
        [ObservableProperty]
        private Project _project;
        [ObservableProperty]
        private string _content = string.Empty;
        [ObservableProperty]
        private float _elapsedTime;
    }
}

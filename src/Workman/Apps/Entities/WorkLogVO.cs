using CommunityToolkit.Mvvm.ComponentModel;

namespace Workman.Apps.Entities
{
    internal partial class WorkLogVO : ObservableObject
    {
        [ObservableProperty]
        private long _orderId;
        [ObservableProperty]
        private long _id;
        [ObservableProperty]
        private long _projectId;
        [ObservableProperty]
        private string _projectName;
        [ObservableProperty]
        private DateTime _date;
        [ObservableProperty]
        private string _content = string.Empty;
        [ObservableProperty]
        private float _elapsedTime;
    }
}

using CommunityToolkit.Mvvm.ComponentModel;

namespace Workman.Core.Entities
{
    internal partial class WorkProjectVO : ObservableObject
    {
        [ObservableProperty]
        private long _orderId;

        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private int _taskCount;

        [ObservableProperty]
        private float _elapsedTime;
    }
}

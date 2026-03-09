using CommunityToolkit.Mvvm.ComponentModel;

namespace Workman.Core.Entities
{
    internal partial class WorkTaskVO : ObservableObject
    {
        [ObservableProperty]
        private long _orderId;

        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private WorkProjectVO _project;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private float _totalElapsedTime;

        public override string ToString()
        {
            return $"{_project?.Name}: {_name}";
        }
    }
}

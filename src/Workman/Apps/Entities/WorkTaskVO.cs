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

        [ObservableProperty]
        private bool _isArchived;

        [ObservableProperty]
        private DateTime _createdTime;

        [ObservableProperty]
        private DateTime? _archivedTime;

        public override string ToString()
        {
            return $"{Project}: {Name}" + (IsArchived ? "（已归档）" : "");
        }
    }
}

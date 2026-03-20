using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ObservableProperty]
        private bool _isArchived;

        [ObservableProperty]
        private DateTime _createdTime;

        [ObservableProperty]
        private DateTime? _archivedTime;

        public override string ToString()
        {
            return Name + (IsArchived ? $"（已归档）" : "");
        }
    }
}

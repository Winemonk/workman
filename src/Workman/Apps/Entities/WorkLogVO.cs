using CommunityToolkit.Mvvm.ComponentModel;
using Workman.Core.Entities;

namespace Workman.Apps.Entities
{
    internal partial class WorkLogVO : ObservableObject
    {
        [ObservableProperty]
        private int _orderId;

        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private WorkTaskVO _task;

        [ObservableProperty]
        private string _content;

        [ObservableProperty]
        private DateTime _date;

        [ObservableProperty]
        private float _elapsedTime;
    }
}

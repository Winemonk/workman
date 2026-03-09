using CommunityToolkit.Mvvm.ComponentModel;
using Workman.Core.Entities;

namespace Workman.Apps.Entities
{
    internal partial class CreateWorkLogVO : ObservableObject
    {
        [ObservableProperty]
        private long _orderId;

        [ObservableProperty]
        private WorkTaskVO _task;

        [ObservableProperty]
        private string _content;

        [ObservableProperty]
        private float _elapsedTime;

        [ObservableProperty]
        private bool _isCreated;

        partial void OnTaskChanged(WorkTaskVO value)
        {
            if(value == null)
            {
                Content = null;
                return;
            }
            if(string.IsNullOrEmpty(Content))
            {
                Content = value.Name;
            }
        }
    }
}

using System.Windows;

namespace Workman.Apps.Helpers
{
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore() => new BindingProxy();

        public object DataContext
        {
            get => GetValue(DataContextProperty);
            set => SetValue(DataContextProperty, value);
        }

        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register(nameof(DataContext), typeof(object), typeof(BindingProxy));
    }
}
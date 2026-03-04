using System.Globalization;
using System.Windows;

namespace Hearth.Prism.Toolkit
{
    internal static class ViewModelUtil
    {
        public static Type? GetViewTypeByViewModelType(Type viewModelType)
        {
            if (typeof(FrameworkElement).IsAssignableFrom(viewModelType))
            {
                return viewModelType;
            }
            string? viewModelTypeName = viewModelType.FullName;
            if (string.IsNullOrEmpty(viewModelTypeName))
            {
                return null;
            }
            if (viewModelTypeName.EndsWith("ViewModel", StringComparison.OrdinalIgnoreCase))
            {
                string? assemblyName = viewModelType.Assembly.FullName;
                if (string.IsNullOrWhiteSpace(assemblyName))
                {
                    return null;
                }
                string changeNamespace = viewModelTypeName.Replace(".ViewModels.", ".Views.");
                string viewTypeName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", changeNamespace[..^5], assemblyName);
                string nonViewTypeName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", changeNamespace[..^9], assemblyName);
                if (Type.GetType(nonViewTypeName) is Type nonViewType
                    && typeof(FrameworkElement).IsAssignableFrom(nonViewType))
                {
                    return nonViewType;
                }
                else if(Type.GetType(viewTypeName) is Type viewType
                    && typeof(FrameworkElement).IsAssignableFrom(viewType))
                {
                    return viewType;
                }
            }
            return null;
        }
    }
}

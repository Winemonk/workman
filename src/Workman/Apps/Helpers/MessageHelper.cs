using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Workman.Apps.Helpers
{
    internal static class MessageHelper
    {
        public static void ShowInfo(string message)
        {
            MessageBox.Show(message, LocalizationManager.Instance.Info, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarning(string message)
        {
            MessageBox.Show(message, LocalizationManager.Instance.Error, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, LocalizationManager.Instance.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowYesNo(string message)
        {
            return MessageBox.Show(message, LocalizationManager.Instance.Hint, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        public static MessageBoxResult ShowOKCancel(string message)
        {
            return MessageBox.Show(message, LocalizationManager.Instance.Hint, MessageBoxButton.OKCancel, MessageBoxImage.Question);
        }
    }
}

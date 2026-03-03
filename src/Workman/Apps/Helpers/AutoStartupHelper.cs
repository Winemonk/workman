using Microsoft.Win32;
using System.Reflection;

namespace Workman.Apps.Helpers
{
    internal static class AutoStartupHelper
    {
        private const string RunKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// 检查注册表启动项是否存在
        /// </summary>
        public static bool IsStartupEnabled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath))
            {
                return key?.GetValue(AppName) != null;
            }
        }

        /// <summary>
        /// 添加当前程序到注册表启动项
        /// </summary>
        public static void EnableStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
            {
                if (key != null)
                {
                    string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                    key.SetValue(AppName, $"\"{exePath}\""); // 加引号防止路径带空格出错
                }
            }
        }

        /// <summary>
        /// 从注册表删除启动项
        /// </summary>
        public static void DisableStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
            {
                key?.DeleteValue(AppName, false);
            }
        }
    }
}

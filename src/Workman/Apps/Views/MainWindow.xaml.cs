using Microsoft.Extensions.Options;
using System.Windows;
using System.Windows.Interop;
using Workman.Apps.Configs;
using Workman.Apps.Helpers;

namespace Workman.Apps.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IOptionsMonitor<AppSettings> _appSettings;
        public MainWindow(IOptionsMonitor<AppSettings> appSettings)
        {
            InitializeComponent();
            _appSettings = appSettings;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            AppSettings appSettings = _appSettings.CurrentValue;
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // 1. 定位到右上角
            WindowHelper.DockWindowToScreenRightTop(this, appSettings.DockOnlyMainScreen);
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, e) =>
            {
                // 重新执行定位逻辑
                WindowHelper.DockWindowToScreenRightTop(this,appSettings.DockOnlyMainScreen);
            };

            // 2. 设置为 ToolWindow (不在 Alt+Tab 显示)
            WindowHelper.SetWindowToolStyle(this);

            // 3. 关键：将窗口的所有者设为桌面 Progman
            // 这能防止 Win+D 最小化，同时保留输入焦点
            WindowHelper.SetWindowParentToProgman(this);

            // 4. 强制置底
            WindowHelper.SetWindowDownmost(this);

            // 5. 挂载钩子，防止窗口状态被系统改变（如 Win+D 触发的排序改变）
            WindowHelper.SetWindowHook(this);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 1. 定位到右上角
            AppSettings appSettings = _appSettings.CurrentValue;
            WindowHelper.DockWindowToScreenRightTop(this, appSettings.DockOnlyMainScreen);
        }
    }
}

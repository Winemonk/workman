using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using Workman.Apps.Configs;
using Workman.Apps.Helpers;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("AppSettingView")]
    internal partial class AppSettingViewModel : ObservableObject, IDialogAware
    {
        private static JsonStringEnumConverter _JSON_STRING_ENUM_CONVERTER = new JsonStringEnumConverter();
        private static JsonSerializerOptions _JSON_SERIALIZER_OPTIONS = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Converters = { _JSON_STRING_ENUM_CONVERTER },
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        private IDisposable? _settingListener;
        private readonly IOptionsMonitor<AppSettings> _appSettings;

        public AppSettingViewModel(IOptionsMonitor<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        [ObservableProperty]
        private bool _autostart;

        [ObservableProperty]
        private bool _dockOnlyMainScreen;

        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose.Invoke();
        }

        [RelayCommand]
        private void Exit()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("是否退出应用程序？",
                                                                "提示",
                                                                MessageBoxButton.YesNo,
                                                                MessageBoxImage.Question);
            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }
            RequestClose.Invoke();
            Environment.Exit(0);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            _settingListener?.Dispose();
            AppSettings appSettings = new AppSettings
            {
                Autostart = Autostart,
                DockOnlyMainScreen = DockOnlyMainScreen,
            };
            string json = JsonSerializer.Serialize(appSettings, _JSON_SERIALIZER_OPTIONS);
            File.WriteAllText("appsettings.json", json);
            if (Autostart && !AutoStartupHelper.IsStartupEnabled())
            {
                AutoStartupHelper.EnableStartup();
            }
            else if(!Autostart && AutoStartupHelper.IsStartupEnabled())
            {
                AutoStartupHelper.DisableStartup();
            }
        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            AppSettings appSettings = _appSettings.CurrentValue;
            Autostart = appSettings.Autostart;
            DockOnlyMainScreen = appSettings.DockOnlyMainScreen;
            _settingListener = _appSettings.OnChange(s =>
            {
                Autostart = appSettings.Autostart;
                DockOnlyMainScreen = appSettings.DockOnlyMainScreen;
            });
        }
    }
}

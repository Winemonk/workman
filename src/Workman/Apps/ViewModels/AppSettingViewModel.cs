using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using Microsoft.Extensions.Options;
using System.Windows;
using Workman.Apps.Configs;
using Workman.Apps.Helpers;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("AppSettingView")]
    internal partial class AppSettingViewModel : ObservableObject, IDialogAware
    {
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

        [ObservableProperty]
        private bool _turnOnReminder;

        [ObservableProperty]
        private int _reminderStartTimeHour;
        partial void OnReminderStartTimeHourChanged(int value)
        {
            if (value < 1 || value > 23)
            {
                ReminderStartTimeHour = 14;
            }
        }

        [ObservableProperty]
        private int _reminderStartTimeMinute;
        partial void OnReminderStartTimeMinuteChanged(int value)
        {
            if (value < 0 || value > 59)
            {
                ReminderStartTimeMinute = 0;
            }
        }

        [ObservableProperty]
        private int _reminderInterval;
        partial void OnReminderIntervalChanged(int value)
        {
            if (value < 1)
            {
                ReminderInterval = 30;
            }
        }

        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose.Invoke();
        }

        [RelayCommand]
        private void Exit()
        {
            MessageBoxResult result = MessageHelper.ShowYesNo(LocalizationManager.Instance.ExitAppHint);
            if (result != MessageBoxResult.Yes)
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
                ReminderInterval = ReminderInterval,
                ReminderOfStartTime = new TimeOnly(ReminderStartTimeHour, ReminderStartTimeMinute),
                TurnOnReminder = TurnOnReminder,
            };
            appSettings.Save();
            if (Autostart && !AutoStartupHelper.IsStartupEnabled())
            {
                AutoStartupHelper.EnableStartup();
            }
            else if (!Autostart && AutoStartupHelper.IsStartupEnabled())
            {
                AutoStartupHelper.DisableStartup();
            }
        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            AppSettings appSettings = _appSettings.CurrentValue;
            Autostart = appSettings.Autostart;
            DockOnlyMainScreen = appSettings.DockOnlyMainScreen;
            TurnOnReminder = appSettings.TurnOnReminder;
            ReminderInterval = appSettings.ReminderInterval;
            ReminderStartTimeHour = appSettings.ReminderOfStartTime.Hour;
            ReminderStartTimeMinute = appSettings.ReminderOfStartTime.Minute;

            _settingListener = _appSettings.OnChange(s =>
            {
                Autostart = appSettings.Autostart;
                DockOnlyMainScreen = appSettings.DockOnlyMainScreen;
                TurnOnReminder = appSettings.TurnOnReminder;
                ReminderInterval = appSettings.ReminderInterval;
                ReminderStartTimeHour = appSettings.ReminderOfStartTime.Hour;
                ReminderStartTimeMinute = appSettings.ReminderOfStartTime.Minute;
            });
        }
    }
}

using Hearth.Prism.Toolkit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Windows;
using Windows.UI.Notifications;
using Workman.Apps.Configs;
using Workman.Apps.Helpers;
using Workman.Core.Entities;
using Workman.Core.Services;

namespace Workman.Apps.Services
{
    [RegisterService(ServiceType = typeof(IWinToastService), Lifetime = ServiceLifetime.Singleton)]
    internal class WinToastService : IWinToastService
    {
        private readonly IDialogService _dialogService;
        private readonly IWorkmanService _workmanService;
        private readonly IOptionsMonitor<AppSettings> _appSettings;
        private static Timer? _timer;
        private static bool _isNotificationShown = false;


        public WinToastService(IWorkmanService workmanService,
                               IDialogService dialogService,
                               IOptionsMonitor<AppSettings> appSettings)
        {
            _workmanService = workmanService;
            _dialogService = dialogService;
            _appSettings = appSettings;
        }

        public void OpenTimerNotification()
        {
            if (_timer != null)
            {
                return;
            }
            AppSettings settings = _appSettings.CurrentValue;
            if (!settings.TurnOnReminder) 
            {
                settings.TurnOnReminder = true;
                settings.Save();
            }
            _timer = new Timer(async _ => await CheckTodayLogs(), null, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(settings.ReminderInterval));
            _appSettings.OnChange(s =>
            {
                if (s.ReminderInterval != settings.ReminderInterval)
                {
                    _timer?.Change(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(s.ReminderInterval));
                }
            });
        }

        public async Task CheckTodayLogs()
        {
            AppSettings settings = _appSettings.CurrentValue;
            if(!settings.TurnOnReminder || DateTime.Now.TimeOfDay < settings.ReminderOfStartTime.ToTimeSpan())
            {
                return;
            }
            DateTime today = DateTime.Today;
            List<WorkLog> workLogs = await _workmanService.GetLogs(today);
            if (workLogs.Count > 0)
            {
                return;
            }
            ToastNotifierCompat toastNotifierCompat = ToastNotificationManagerCompat.CreateToastNotifier();
            ToastNotification toastNotification = new ToastNotification(new ToastContentBuilder()
                .AddText("Workman")
                .AddText(LocalizationManager.Instance.ToastMessage)
                .GetXml());
            toastNotification.Activated += (s, args) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.MainWindow.Activate();
                    if (_isNotificationShown)
                    {
                        return;
                    }
                    _isNotificationShown = true;
                    _dialogService.ShowDialog("CreateWorkLogView", new DialogParameters
                    {
                        { "date", DateTime.Today }
                    }, p =>
                    {
                        _isNotificationShown = false;
                    });
                });
            };
            toastNotifierCompat.Show(toastNotification);
        }
    }
}

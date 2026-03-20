using DryIoc.Microsoft.DependencyInjection;
using GeoAppPackager.Infrastructure;
using Hearth.Prism.Toolkit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using Workman.Apps.Configs;
using Workman.Apps.Helpers;
using Workman.Apps.Views;
using Workman.Core.Services;

namespace Workman
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private static Mutex _MUTEX = null!;
        private const string _MUTEX_NAME = "HEARTH_WORKMAN";

        protected override void OnStartup(StartupEventArgs e)
        {
            if (CultureInfo.CurrentCulture.Name.Equals("zh-CN", StringComparison.OrdinalIgnoreCase))
            {
                CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                culture.DateTimeFormat.LongDatePattern = "yyyy年MM月dd日 dddd";
                Thread.CurrentThread.CurrentCulture = culture;
            }
            // LocalizationCultureHelper.SetCulture("en");
            if (!Debugger.IsAttached)
            {
                var exePath = Process.GetCurrentProcess().MainModule?.FileName;
                var exeDirectory = Path.GetDirectoryName(exePath);
                if (!string.IsNullOrEmpty(exeDirectory))
                {
                    Directory.SetCurrentDirectory(exeDirectory);
                }

                bool createdNew;
                try
                {
                    _MUTEX = new Mutex(true, _MUTEX_NAME, out createdNew);
                }
                catch (AbandonedMutexException)
                {
                    createdNew = false;
                }

                if (!createdNew)
                {
                    Shutdown();
                    return;
                }
            }
            
            base.OnStartup(e);
        }

        protected override Window CreateShell()
        {
            Container.RegisterViewsFromAssembly(Assembly.GetExecutingAssembly());
            IWinToastService winToastService = Container.Resolve<IWinToastService>();
            winToastService.OpenTimerNotification();
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterAllFromAssembly(Assembly.GetExecutingAssembly());
            containerRegistry.RegisterDialogWindow<Apps.Views.DialogWindow>();
            ServiceCollection services = new ServiceCollection();
            services.AddInfrastructure();
            IContainer container = containerRegistry.GetContainer();
            container.Populate(services);
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            containerRegistry.Config<AppSettings>(configuration.GetSection(nameof(AppSettings)));
        }
    }
}

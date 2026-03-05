using DryIoc.Microsoft.DependencyInjection;
using GeoAppPackager.Infrastructure;
using Hearth.Prism.Toolkit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Workman.Apps.Configs;
using Workman.Apps.Views;

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
            var exePath = Process.GetCurrentProcess().MainModule?.FileName;
            var exeDirectory = Path.GetDirectoryName(exePath);
            if (!string.IsNullOrEmpty(exeDirectory))
            {
                Directory.SetCurrentDirectory(exeDirectory);
            }

            // 尝试获取互斥体
            bool createdNew;
            try
            {
                _MUTEX = new Mutex(true, _MUTEX_NAME, out createdNew);
            }
            catch (AbandonedMutexException)
            {
                // 如果上一个实例异常终止，Mutex 可能被遗弃，但我们仍可视为已有实例
                createdNew = false;
            }

            if (!createdNew)
            {
                // 已有实例在运行，退出当前进程
                Shutdown();
                return;
            }
            // 禁用硬件加速
            //RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            // 正常启动应用程序
            base.OnStartup(e);
        }

        protected override Window CreateShell()
        {
            Container.RegisterViewsFromAssembly(Assembly.GetExecutingAssembly());
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
            containerRegistry.Config<AppSettings>(configuration);
        }
    }
}

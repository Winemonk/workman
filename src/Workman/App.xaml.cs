using DryIoc.Microsoft.DependencyInjection;
using GeoAppPackager.Infrastructure;
using Hearth.Prism.Toolkit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Windows;
using Workman.Apps.Configs;
using Workman.Apps.Views;

namespace Workman
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
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

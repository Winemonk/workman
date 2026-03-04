using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Hearth.Prism.Toolkit
{
    public static class PrismOptionsExtensions
    {
        public static IContainerRegistry Config<TOptions>(this IContainerRegistry containerRegistry, IConfiguration config, string name = null, Action<BinderOptions> configureBinder = null) where TOptions : class, new()
        {
            containerRegistry.RegisterSingleton(typeof(IOptions<>), typeof(PrismOptionsManager<>));
            containerRegistry.RegisterScoped(typeof(IOptionsSnapshot<>), typeof(OptionsManager<>));
            containerRegistry.RegisterSingleton(typeof(IOptionsMonitor<>), typeof(OptionsMonitor<>));
            containerRegistry.Register(typeof(IOptionsFactory<>), typeof(OptionsFactory<>));
            containerRegistry.RegisterSingleton(typeof(IOptionsMonitorCache<>), typeof(OptionsCache<>));
            containerRegistry.RegisterInstance<IOptionsChangeTokenSource<TOptions>>(new ConfigurationChangeTokenSource<TOptions>(name ?? Options.DefaultName, config));
            return containerRegistry.RegisterInstance<IConfigureOptions<TOptions>>(new NamedConfigureFromConfigurationOptions<TOptions>(name, config, configureBinder ?? new Action<BinderOptions>(_ => { })));
        }
    }
}

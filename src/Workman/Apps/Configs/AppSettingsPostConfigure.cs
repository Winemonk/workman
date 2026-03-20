using Hearth.Prism.Toolkit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Workman.Apps.Configs
{
    [RegisterService(ServiceType = typeof(IPostConfigureOptions<AppSettings>), Lifetime = ServiceLifetime.Singleton)]
    internal class AppSettingsPostConfigure : IPostConfigureOptions<AppSettings>
    {
        public void PostConfigure(string? name, AppSettings options)
        {
            if (options.ReminderInterval < 0)
            {
                options.ReminderInterval = 30;
            }
        }
    }
}

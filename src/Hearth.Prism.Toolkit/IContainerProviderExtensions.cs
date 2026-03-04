using System.Reflection;

namespace Hearth.Prism.Toolkit
{
    public static class IContainerProviderExtensions
    {
        public static IContainerProvider RegisterViewsFromAssembly(this IContainerProvider containerProvider, params Assembly[] assemblies)
        {
            IRegionViewRegistry regionViewRegistry = containerProvider.Resolve<IRegionViewRegistry>();
            regionViewRegistry.RegisterViewsFromAssembly(assemblies);
            return containerProvider;
        }
    }
}

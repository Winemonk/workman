using System.Reflection;

namespace Hearth.Prism.Toolkit
{
    public static class IRegionViewRegistryExtensions
    {
        public static IRegionViewRegistry RegisterViewsFromAssembly(this IRegionViewRegistry regionViewRegistry, params Assembly[] assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                IEnumerable<Type> nonAbstractTypes = types.Where(t => !t.IsAbstract);
                foreach (Type type in nonAbstractTypes)
                {
                    RegisterRegionAttribute? regionAttribute = type.GetCustomAttribute<RegisterRegionAttribute>();
                    if (regionAttribute == null)
                    {
                        continue;
                    }
                    string regionName = regionAttribute.Name;
                    Type? viewType = regionAttribute.ViewType ?? ViewModelUtil.GetViewTypeByViewModelType(type);
                    if (viewType == null)
                    {
                        continue;
                    }
                    regionViewRegistry.RegisterViewWithRegion(regionAttribute.Name, viewType);
                }
            }
            return regionViewRegistry;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Hearth.Prism.Toolkit
{
    public static class IContainerRegistryExtensions
    {
        public static IContainerRegistry RegisterAllFromAssembly(this IContainerRegistry containerRegistry, params Assembly[] assemblies)
        {
            return containerRegistry.RegisterDialogsFromAssembly(assemblies)
                                    .RegisterNavigationsFromAssembly(assemblies)
                                    .RegisterServicesFromAssembly(assemblies);
        }
        public static IContainerRegistry RegisterDialogsFromAssembly(this IContainerRegistry containerRegistry, params Assembly[] assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                IEnumerable<Type> nonAbstractTypes = types.Where(t => !t.IsAbstract);
                foreach (Type type in nonAbstractTypes)
                {
                    RegisterDialogAttribute? dialogAttribute = type.GetCustomAttribute<RegisterDialogAttribute>();
                    if (dialogAttribute == null)
                    {
                        continue;
                    }
                    string dialogName = dialogAttribute.Name;
                    Type? viewType = dialogAttribute.ViewType ?? ViewModelUtil.GetViewTypeByViewModelType(type);
                    if (viewType == null)
                    {
                        continue;
                    }
                    containerRegistry.Register(typeof(object), viewType, dialogName);
                }
            }
            return containerRegistry;
        }
        public static IContainerRegistry RegisterNavigationsFromAssembly(this IContainerRegistry containerRegistry, params Assembly[] assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                IEnumerable<Type> nonAbstractTypes = types.Where(t => !t.IsAbstract);
                foreach (Type type in nonAbstractTypes)
                {
                    RegisterNavigationAttribute? navigationAttribute = type.GetCustomAttribute<RegisterNavigationAttribute>();
                    if (navigationAttribute == null)
                    {
                        continue;
                    }
                    string navigationName = navigationAttribute.Name;
                    Type? viewType = navigationAttribute.ViewType ?? ViewModelUtil.GetViewTypeByViewModelType(type);
                    if (viewType == null)
                    {
                        continue;
                    }
                    containerRegistry.Register(typeof(object), viewType, navigationName);
                }
            }
            return containerRegistry;
        }
        public static IContainerRegistry RegisterServicesFromAssembly(this IContainerRegistry containerRegistry, params Assembly[] assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                IEnumerable<Type> nonAbstractTypes = types.Where(t => !t.IsAbstract);
                foreach (Type type in nonAbstractTypes)
                {
                    RegisterServiceAttribute? serviceAttribute = type.GetCustomAttribute<RegisterServiceAttribute>();
                    if (serviceAttribute == null)
                    {
                        continue;
                    }
                    switch (serviceAttribute.Lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            if (serviceAttribute.ServiceType == null)
                            {
                                if (serviceAttribute.Name == null)
                                {
                                    containerRegistry.RegisterSingleton(type);
                                }
                                else
                                {
                                    containerRegistry.RegisterSingleton(type, type, serviceAttribute.Name);
                                }
                            }
                            else
                            {
                                if (serviceAttribute.Name == null)
                                {
                                    containerRegistry.RegisterSingleton(serviceAttribute.ServiceType, type);
                                }
                                else
                                {
                                    containerRegistry.RegisterSingleton(serviceAttribute.ServiceType, type, serviceAttribute.Name);
                                }
                            }
                            break;
                        case ServiceLifetime.Scoped:
                            if (serviceAttribute.ServiceType == null)
                            {
                                containerRegistry.RegisterScoped(type);
                            }
                            else
                            {
                                containerRegistry.RegisterScoped(serviceAttribute.ServiceType, type);
                            }
                            break;
                        case ServiceLifetime.Transient:
                        default:
                            if (serviceAttribute.ServiceType == null)
                            {
                                if (serviceAttribute.Name == null)
                                {
                                    containerRegistry.Register(type);
                                }
                                else
                                {
                                    containerRegistry.Register(type, serviceAttribute.Name);
                                }
                            }
                            else
                            {
                                if (serviceAttribute.Name == null)
                                {
                                    containerRegistry.Register(serviceAttribute.ServiceType, type);
                                }
                                else
                                {
                                    containerRegistry.Register(serviceAttribute.ServiceType, type, serviceAttribute.Name);
                                }
                            }
                            break;
                    }
                }
            }
            return containerRegistry;
        }
    }
}

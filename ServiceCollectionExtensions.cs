namespace SunamoDependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

// Create an extension method for IServiceCollection
public static class ServiceCollectionExtensions
{
    public static void AddServicesEndingWithService(this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        AddServicesEndingWith(services, assembly, "Service", lifetime);
    }

    public static void AddServicesEndingWith(
        this IServiceCollection services,
        Assembly assembly,
        string suffix,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var serviceTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith(suffix));

        foreach (var type in serviceTypes)
        {
            // Attempt to find an interface that the class implements,
            // matching the class name without the suffix (e.g., IUserService for UserService)
            // This is a common convention, but you might need to adjust it.
            var implementedInterfaces = type.GetInterfaces();
            var interfaceToRegister = implementedInterfaces.FirstOrDefault(i => i.Name == $"I{type.Name.Substring(0, type.Name.Length - suffix.Length)}");

            if (interfaceToRegister != null)
            {
                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(interfaceToRegister, type);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(interfaceToRegister, type);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(interfaceToRegister, type);
                        break;
                }
            }
            else
            {
                // If no matching interface is found, register the type itself.
                // Or you could log a warning or throw an exception, depending on your requirements.
                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(type);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(type);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(type);
                        break;
                }
            }
        }
    }
}
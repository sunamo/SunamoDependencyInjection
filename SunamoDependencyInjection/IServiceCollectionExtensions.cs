// variables names: ok
namespace SunamoDependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to add services ending with a specific suffix.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds all services ending with "Service" from Sunamo assemblies to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="isAddingFromReferencedSunamoAssemblies">Whether to add services from referenced Sunamo assemblies.</param>
    /// <param name="lifetime">The service lifetime (Scoped, Singleton, or Transient).</param>
    /// <returns>A result containing the registered classes and interfaces.</returns>
    public static AddServicesEndingWithResult AddServicesEndingWithService(this IServiceCollection services,
        bool isAddingFromReferencedSunamoAssemblies = true,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        AddServicesEndingWithResult result = new AddServicesEndingWithResult();

        var directoryPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty);
        if (string.IsNullOrEmpty(directoryPath))
        {
            return result;
        }

        var dllFiles = Directory.GetFiles(directoryPath, "Sunamo*.dll", SearchOption.TopDirectoryOnly);

        foreach (var dllPath in dllFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(dllPath);

            if (fileName == "SunamoInterfaces")
            {
                continue;
            }

            Assembly.Load(fileName);
        }

        if (isAddingFromReferencedSunamoAssemblies)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var sunamoAssemblies = assemblies.Where(assembly => assembly.GetName().Name?.StartsWith("Sunamo") == true);

#if DEBUG
            var before = sunamoAssemblies.Count();
#endif

            var filteredAssemblies = sunamoAssemblies.Where(assembly => assembly.GetName().Name != "SunamoInterfaces");

#if DEBUG
            var after = filteredAssemblies.Count();
#endif

            foreach (var assembly in filteredAssemblies)
            {
                try
                {
                    AddServicesEndingWith(services, assembly, "Service", result, true, lifetime);
                }
                catch (Exception)
                {
                    // EN: Silently ignore failures when loading services from referenced assemblies
                    // CZ: Tiše ignorovat selhání při načítání služeb z odkazovaných assemblies
                }
            }
        }

        try
        {
            AddServicesEndingWith(services, Assembly.GetEntryAssembly(), "Service", result, false, lifetime);
        }
        catch (Exception)
        {
            throw;
        }

        return result;
    }

    /// <summary>
    /// Adds services ending with a specific suffix from an assembly to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="assembly">The assembly to scan for services.</param>
    /// <param name="suffix">The suffix to match (e.g., "Service").</param>
    /// <param name="addServicesEndingWithResult">The result object to populate with registered services.</param>
    /// <param name="isOnlyExported">Whether to only scan exported types.</param>
    /// <param name="lifetime">The service lifetime (Scoped, Singleton, or Transient).</param>
    public static void AddServicesEndingWith(
        this IServiceCollection services,
        Assembly assembly,
        string suffix,
        AddServicesEndingWithResult addServicesEndingWithResult,
        bool isOnlyExported,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        Type[] serviceTypes = [];

        try
        {
            serviceTypes = isOnlyExported ? assembly.GetExportedTypes() : assembly.GetTypes();
        }
        catch (Exception)
        {
            // EN: It can happen that e.g. I'm using a deprecated NuGet package that has dependencies that prevent it from working.
            // CZ: Může se stát že např. používám už deprecated NuGet balíček který má závislosti kvůli kterým to neprojde.
            // EN: Errors like this can occur:
            // CZ: Vznikají tak chyby jako:
            // Could not load type 'SunamoInterfaces.Interfaces.ITextBuilder' from assembly 'SunamoInterfaces, Version=25.3.29.1, Culture=neutral, PublicKeyToken=null'.
        }

        serviceTypes = serviceTypes.Where(type => type.IsClass && !type.IsAbstract && type.Name.EndsWith(suffix)).ToArray();

        foreach (var type in serviceTypes)
        {
            // EN: Attempt to find an interface that the class implements, matching the class name without the suffix (e.g., IUserService for UserService)
            // CZ: Pokus o nalezení rozhraní které třída implementuje, odpovídající názvu třídy bez suffixu (např. IUserService pro UserService)
            // EN: This is a common convention, but you might need to adjust it.
            // CZ: Toto je běžná konvence, ale možná ji budete muset upravit.
            var implementedInterfaces = type.GetInterfaces();
            var interfaceToRegister = implementedInterfaces.FirstOrDefault(interfaceType => interfaceType.Name == $"I{type.Name.Substring(0, type.Name.Length - suffix.Length)}");

            if (interfaceToRegister != null)
            {
                try
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

                    addServicesEndingWithResult.Interfaces.Add(interfaceToRegister.FullName);
                }
                catch (Exception)
                {
                    // EN: Silently ignore failures when registering services
                    // CZ: Tiše ignorovat selhání při registraci služeb
                }
            }
            else
            {
                try
                {
                    // EN: If no matching interface is found, register the type itself. Or you could log a warning or throw an exception, depending on your requirements.
                    // CZ: Pokud není nalezeno odpovídající rozhraní, zaregistruje se samotný typ. Nebo můžete zalogovat varování nebo vyhodit výjimku, podle vašich požadavků.
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

                    addServicesEndingWithResult.Classes.Add(type.FullName);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}

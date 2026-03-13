namespace SunamoDependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    /// <param name="logger">Logger for logging exceptions during assembly loading. REQUIRED.</param>
    /// <param name="additionalAssemblyPatterns">Additional assembly name patterns to scan (e.g., "SeznamkaCz").</param>
    /// <param name="isAddingFromReferencedSunamoAssemblies">Whether to add services from referenced Sunamo assemblies.</param>
    /// <param name="lifetime">The service lifetime (Scoped, Singleton, or Transient).</param>
    /// <param name="skipAlreadyRegistered">When true, skips types (or their matching interfaces) that are already registered in the collection.</param>
    /// <returns>A result containing the registered classes and interfaces.</returns>
    public static AddServicesEndingWithResult AddServicesEndingWithService(this IServiceCollection services,
        ILogger logger,
        string[] additionalAssemblyPatterns,
        bool isAddingFromReferencedSunamoAssemblies = true,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        bool skipAlreadyRegistered = false)
    {
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

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

            try
            {
                Assembly.Load(fileName);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to load Sunamo assembly: {AssemblyName}", fileName);
            }
        }

        if (additionalAssemblyPatterns != null && additionalAssemblyPatterns.Length > 0)
        {
            foreach (var pattern in additionalAssemblyPatterns)
            {
                var additionalDllFiles = Directory.GetFiles(directoryPath, $"{pattern}*.dll", SearchOption.TopDirectoryOnly);
                foreach (var dllPath in additionalDllFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(dllPath);
                    try
                    {
                        Assembly.Load(fileName);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to load additional assembly: {AssemblyName}", fileName);
                    }
                }
            }
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
                    AddServicesEndingWith(services, assembly, "Service", result, true, lifetime, logger, skipAlreadyRegistered);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to add services from Sunamo assembly: {AssemblyName}", assembly.GetName().Name);
                }
            }
        }

        if (additionalAssemblyPatterns != null && additionalAssemblyPatterns.Length > 0)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var pattern in additionalAssemblyPatterns)
            {
                var matchingAssemblies = assemblies.Where(assembly => assembly.GetName().Name?.StartsWith(pattern) == true);
                foreach (var assembly in matchingAssemblies)
                {
                    try
                    {
                        AddServicesEndingWith(services, assembly, "Service", result, false, lifetime, logger, skipAlreadyRegistered);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to add services from additional assembly: {AssemblyName}", assembly.GetName().Name);
                    }
                }
            }
        }

        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null)
        {
            try
            {
                AddServicesEndingWith(services, entryAssembly, "Service", result, false, lifetime, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to add services from entry assembly: {AssemblyName}", entryAssembly.GetName().Name);
                throw;
            }
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
    /// <param name="logger">Logger for logging exceptions. REQUIRED.</param>
    /// <param name="skipAlreadyRegistered">When true, skips types (or their matching interfaces) that are already registered in the collection.</param>
    public static void AddServicesEndingWith(
        this IServiceCollection services,
        Assembly assembly,
        string suffix,
        AddServicesEndingWithResult addServicesEndingWithResult,
        bool isOnlyExported,
        ServiceLifetime lifetime,
        ILogger logger,
        bool skipAlreadyRegistered = false)
    {
        Type[] serviceTypes = [];

        try
        {
            serviceTypes = isOnlyExported ? assembly.GetExportedTypes() : assembly.GetTypes();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get types from assembly: {AssemblyName}. This can happen with deprecated NuGet packages.", assembly.GetName().Name);
        }

        serviceTypes = serviceTypes
            .Where(type => type.IsClass && !type.IsAbstract &&
                (type.Name.EndsWith(suffix) || GetRelevantInterfaces(type).Any()))
            .ToArray();

        foreach (var type in serviceTypes)
        {
            var relevantInterfaces = GetRelevantInterfaces(type).ToArray();

            // Try exact naming convention first: I{NameWithoutSuffix} (e.g. UserService -> IUser)
            Type? interfaceToRegister = null;
            if (type.Name.EndsWith(suffix))
                interfaceToRegister = relevantInterfaces.FirstOrDefault(i => i.Name == $"I{type.Name[..^suffix.Length]}");

            // Fallback: any non-system interface the class implements
            interfaceToRegister ??= relevantInterfaces.FirstOrDefault();

            if (skipAlreadyRegistered && services.Any(d => d.ServiceType == (interfaceToRegister ?? type)))
                continue;

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

                    if (interfaceToRegister.FullName != null)
                        addServicesEndingWithResult.Interfaces.Add(interfaceToRegister.FullName);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to register service interface: {InterfaceName} -> {TypeName}", interfaceToRegister.FullName, type.FullName);
                }
            }
            else if (type.Name.EndsWith(suffix))
            {
                // No interface found — register concrete type (only for suffix-matching classes)
                try
                {
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

                    if (type.FullName != null)
                        addServicesEndingWithResult.Classes.Add(type.FullName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to register service type: {TypeName}", type.FullName);
                    throw;
                }
            }
        }
    }

    private static IEnumerable<Type> GetRelevantInterfaces(Type type) =>
        type.GetInterfaces().Where(i =>
            !i.IsGenericType &&
            i.Namespace != null &&
            !i.Namespace.StartsWith("System") &&
            !i.Namespace.StartsWith("Microsoft"));
}
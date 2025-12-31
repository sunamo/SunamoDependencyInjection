namespace SunamoDependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;

public static class IServiceCollectionExtensions
{
    public static AddServicesEndingWithResult AddServicesEndingWithService(this IServiceCollection services,
        bool isAddingFromReferencedSunamoAssemblies = true,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        AddServicesEndingWithResult result = new AddServicesEndingWithResult();

        /*
Assembly.GetEntryAssembly()?.Location
Environment.ProcessPath
Process.GetCurrentProcess().MainModule.FileName
*/

        var directoryPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        var dllFiles = Directory.GetFiles(directoryPath, "Sunamo*.dll", SearchOption.TopDirectoryOnly);

        foreach (var item in dllFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(item);

            if (fileName == "SunamoInterfaces")
            {
                continue;
            }

            Assembly.Load(fileName);
        }

        if (isAddingFromReferencedSunamoAssemblies)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var sunamoAssemblies = assemblies.Where(assembly => assembly.GetName().Name.StartsWith("Sunamo"));

#if DEBUG
            var before = sunamoAssemblies.Count();
#endif


            var filteredAssemblies = sunamoAssemblies.Where(assembly => assembly.GetName().Name != "SunamoInterfaces");

#if DEBUG
            var after = filteredAssemblies.Count();
#endif

            foreach (var item in filteredAssemblies)
            {
                try
                {
                    AddServicesEndingWith(services, item, "Service", result, true, lifetime);


                }
                catch (Exception ex)
                {


                }
            }
        }

        try
        {
            AddServicesEndingWith(services, Assembly.GetEntryAssembly(), "Service", result, false, lifetime);
        }
        catch (Exception ex)
        {
            throw;
        }

        return result;
    }

    public static void AddServicesEndingWith(
        this IServiceCollection services,
        Assembly assembly,
        string suffix,
        AddServicesEndingWithResult addServicesEndingWithResult,
        bool isOnlyExported,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
#if DEBUG
        if (assembly.GetName().Name == "SunamoTextBuilder")
        {

        }
#endif

        Type[] serviceTypes = [];

        try
        {
            serviceTypes = isOnlyExported ? assembly.GetExportedTypes() : assembly.GetTypes();
        }
        catch (Exception ex)
        {
            // M��e se st�t �e nap�. pou��v�m u� deprecated nuget kter� m� z�vislosti kv�li kter�m to neprojde. 
            // Vznikaj� tak chyby jako:
            // Could not load type 'SunamoInterfaces.Interfaces.ITextBuilder' from assembly 'SunamoInterfaces, Version=25.3.29.1, Culture=neutral, PublicKeyToken=null'.
        }

        serviceTypes = serviceTypes.Where(type => type.IsClass && !type.IsAbstract && type.Name.EndsWith(suffix)).ToArray();

        foreach (var type in serviceTypes)
        {
            // Attempt to find an interface that the class implements,
            // matching the class name without the suffix (e.g., IUserService for UserService)
            // This is argument common convention, but you might need to adjust it.
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
                catch (Exception ex)
                {

                }

            }
            else
            {
                try
                {


                    // If no matching interface is found, register the type itself.
                    // Or you could log argument warning or throw an exception, depending on your requirements.
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
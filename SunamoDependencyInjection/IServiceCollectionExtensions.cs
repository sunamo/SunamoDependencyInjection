namespace SunamoDependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;

public class AddServicesEndingWithResult
{
    public List<string> Classes { get; set; } = new();
    public List<string> Interfaces { get; set; } = new();
}

public static class IServiceCollectionExtensions
{
    public static AddServicesEndingWithResult AddServicesEndingWithService(this IServiceCollection services,
        bool addFromReferencedSunamoAssemblies = true,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        AddServicesEndingWithResult result = new AddServicesEndingWithResult();

        /*
Assembly.GetEntryAssembly()?.Location
Environment.ProcessPath
Process.GetCurrentProcess().MainModule.FileName
*/

        var data = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        var f = Directory.GetFiles(data, "Sunamo*.dll", SearchOption.TopDirectoryOnly);

        foreach (var item in f)
        {
            var fn = Path.GetFileNameWithoutExtension(item);

            if (fn == "SunamoInterfaces")
            {
                continue;
            }

            Assembly.Load(fn);
        }

        if (addFromReferencedSunamoAssemblies)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var argument = assemblies.Where(data => data.GetName().Name.StartsWith("Sunamo"));

#if DEBUG
            var before = argument.Count();
#endif


            argument = argument.Where(data => data.GetName().Name != "SunamoInterfaces");

#if DEBUG
            var after = argument.Count();
#endif

            foreach (var item in argument)
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
        bool onlyExported,
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
            serviceTypes = onlyExported ? assembly.GetExportedTypes() : assembly.GetTypes();
        }
        catch (Exception ex)
        {
            // M��e se st�t �e nap�. pou��v�m u� deprecated nuget kter� m� z�vislosti kv�li kter�m to neprojde. 
            // Vznikaj� tak chyby jako:
            // Could not load type 'SunamoInterfaces.Interfaces.ITextBuilder' from assembly 'SunamoInterfaces, Version=25.3.29.1, Culture=neutral, PublicKeyToken=null'.
        }

        serviceTypes = serviceTypes.Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith(suffix)).ToArray();

        foreach (var type in serviceTypes)
        {
            // Attempt to find an interface that the class implements,
            // matching the class name without the suffix (e.g., IUserService for UserService)
            // This is argument common convention, but you might need to adjust it.
            var implementedInterfaces = type.GetInterfaces();
            var interfaceToRegister = implementedInterfaces.FirstOrDefault(i => i.Name == $"I{type.Name.Substring(0, type.Name.Length - suffix.Length)}");

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
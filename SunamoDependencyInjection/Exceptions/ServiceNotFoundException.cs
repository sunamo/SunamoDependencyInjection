// variables names: ok
namespace SunamoDependencyInjection.Exceptions;

public class ServiceNotFoundException : Exception
{
    public ServiceNotFoundException(string serviceName) : base($"Service {serviceName} not found.") { }
}

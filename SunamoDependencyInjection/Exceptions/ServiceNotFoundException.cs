// variables names: ok
namespace SunamoDependencyInjection.Exceptions;

/// <summary>
/// Exception thrown when a requested service is not found in the dependency injection container.
/// </summary>
public class ServiceNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class.
    /// </summary>
    /// <param name="serviceName">The name of the service that was not found.</param>
    public ServiceNotFoundException(string serviceName) : base($"Service {serviceName} not found.") { }
}

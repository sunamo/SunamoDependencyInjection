namespace SunamoDependencyInjection.Exceptions;

/// <summary>
/// Represents the service not found exception type.
/// </summary>
public class ServiceNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the service not found exception class.
    /// </summary>
    /// <param name="serviceName">The service name.</param>
    public ServiceNotFoundException(string serviceName) : base($"Service {serviceName} not found.") { }
}

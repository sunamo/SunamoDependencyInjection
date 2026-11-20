namespace SunamoDependencyInjection.Exceptions;

/// <summary>
/// Represents the service not found exception t t type.
/// </summary>
public class ServiceNotFoundExceptionT<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the service not found exception t class.
    /// </summary>
    public ServiceNotFoundExceptionT() : base($"Service {typeof(T).Name} not found.") { }
}

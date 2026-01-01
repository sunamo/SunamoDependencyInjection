namespace SunamoDependencyInjection.Exceptions;

/// <summary>
/// Generic exception thrown when a requested service of type T is not found in the dependency injection container.
/// </summary>
/// <typeparam name="T">The type of the service that was not found.</typeparam>
public class ServiceNotFoundExceptionT<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNotFoundExceptionT{T}"/> class.
    /// </summary>
    public ServiceNotFoundExceptionT() : base($"Service {typeof(T).Name} not found.") { }
}

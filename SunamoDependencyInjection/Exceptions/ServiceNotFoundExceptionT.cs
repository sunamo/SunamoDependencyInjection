namespace SunamoDependencyInjection.Exceptions;
public class ServiceNotFoundExceptionT<T> : Exception
{
    public ServiceNotFoundExceptionT() : base($"Service {typeof(T).Name} not found.") { }
}

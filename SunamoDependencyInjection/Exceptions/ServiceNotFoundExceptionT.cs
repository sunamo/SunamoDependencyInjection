// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoDependencyInjection.Exceptions;

public class ServiceNotFoundExceptionT<T> : Exception
{
    public ServiceNotFoundExceptionT() : base($"Service {typeof(T).Name} not found.") { }
}

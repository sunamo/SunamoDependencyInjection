// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoDependencyInjection.Exceptions;

public class ServiceNotFoundException : Exception
{
    public ServiceNotFoundException(string serviceName) : base($"Service {serviceName} not found.") { }
}
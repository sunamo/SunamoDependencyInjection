public class ServiceNotFoundException : Exception
{
    public ServiceNotFoundException(string serviceName) : base($"Service {serviceName} not found.") { }
}
namespace SunamoDependencyInjection;

public class DummyServiceProvider : IServiceProvider
{
    public object GetService(Type serviceType)
    {
        throw new NotImplementedException();
    }
}

namespace SunamoDependencyInjection;

/// <summary>
/// Toto mi moc nepomůže. Sice bych neměl chybu že může být ServiceProvider potenciálně null ale zase nemůžu získat servisu přes GetService<T>(). Pouze přes GetService(Type). O dalších metodách ani nemluvě. 
/// </summary>
public class DummyServiceProvider : IServiceProvider
{
    public object GetService(Type serviceType)
    {
        throw new NotImplementedException();
    }
}

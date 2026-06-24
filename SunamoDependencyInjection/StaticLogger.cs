namespace SunamoDependencyInjection;

public class StaticLogger
{
    // It has to be a variable, not a property, to avoid looking weird.
    // Better one warning here that I can suppress than a million occurrences that look strangely.
    protected static ILogger Logger = NullLogger.Instance;
}

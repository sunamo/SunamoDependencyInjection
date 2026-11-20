namespace SunamoDependencyInjection;

/// <summary>
/// Represents the static logger type.
/// </summary>
public class StaticLogger
{
    // It has to be a variable, not a property, to avoid looking weird.
    // Better one warning here that I can suppress than a million occurrences that look strangely.
    /// <summary>
    /// The logger.
    /// </summary>
    protected static ILogger Logger = NullLogger.Instance;
}

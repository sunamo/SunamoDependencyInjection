// variables names: ok
namespace SunamoDependencyInjection;

/// <summary>
/// Base class providing static logger functionality for derived classes.
/// </summary>
public class StaticLogger
{
    /// <summary>
    /// Gets or sets the static logger instance. It has to be a variable, not a property, to avoid looking weird.
    /// Better one warning here that I can suppress than a million occurrences that look strangely.
    /// </summary>
    protected static ILogger Logger = NullLogger.Instance;
}

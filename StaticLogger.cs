namespace SunamoDependencyInjection;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

public class StaticLogger
{
    /// <summary>
    /// It has to be a variable, not a property. So that it doesn't look weird. Better one warning here that I can suppress than a million occurrences that look strangely.
    /// </summary>
    protected static ILogger logger = NullLogger.Instance;
}
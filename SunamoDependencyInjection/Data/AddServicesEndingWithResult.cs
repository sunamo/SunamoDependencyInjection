// variables names: ok
namespace SunamoDependencyInjection.Data;

/// <summary>
/// Represents the result of adding services ending with a specific suffix to the dependency injection container.
/// </summary>
public class AddServicesEndingWithResult
{
    /// <summary>
    /// Gets or sets the list of fully qualified class names that were registered.
    /// </summary>
    public List<string> Classes { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of fully qualified interface names that were registered.
    /// </summary>
    public List<string> Interfaces { get; set; } = new();
}

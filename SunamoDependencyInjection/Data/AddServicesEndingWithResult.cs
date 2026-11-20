namespace SunamoDependencyInjection.Data;

/// <summary>
/// Represents the add services ending with result type.
/// </summary>
public class AddServicesEndingWithResult
{
    /// <summary>
    /// Gets or sets the classes.
    /// </summary>
    public List<string> Classes { get; set; } = new();

    /// <summary>
    /// Gets or sets the interfaces.
    /// </summary>
    public List<string> Interfaces { get; set; } = new();
}

// variables names: ok
namespace SunamoDependencyInjection.Tests;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Unit tests for the SunamoDependencyInjection library.
/// </summary>
public class UnitTest1
{
    /// <summary>
    /// Tests that AddServicesEndingWithService returns a valid result without scanning referenced assemblies.
    /// </summary>
    [Fact]
    public void AddServicesEndingWithService_ReturnsValidResult()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddServicesEndingWithService(isAddingFromReferencedSunamoAssemblies: false);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Classes);
        Assert.NotNull(result.Interfaces);
    }

    /// <summary>
    /// Tests that AddServicesEndingWithService returns a valid result when scanning referenced Sunamo assemblies.
    /// </summary>
    [Fact]
    public void AddServicesEndingWithService_WithReferencedAssemblies_ReturnsValidResult()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddServicesEndingWithService(isAddingFromReferencedSunamoAssemblies: true);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Classes);
        Assert.NotNull(result.Interfaces);
    }
}

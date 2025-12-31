// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoDependencyInjection.Tests;

using Microsoft.Extensions.DependencyInjection;

public class UnitTest1
{
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
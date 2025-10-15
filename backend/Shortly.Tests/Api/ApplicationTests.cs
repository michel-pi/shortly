using Microsoft.Extensions.DependencyInjection;

using Shortly.Infrastructure.Data;
using Shortly.Tests.Setup;

namespace Shortly.Tests.Api;

public class ApplicationTests : IClassFixture<ShortlyApiFactory>
{
    private readonly ShortlyApiFactory _api;

    public ApplicationTests(ShortlyApiFactory api)
    {
        _api = api;
    }

    [Fact]
    public async Task Database_Is_Reachable()
    {
        using var scope = _api.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var canConnect = await context.Database.CanConnectAsync();
        Assert.True(canConnect);
    }

    [Fact]
    public async Task Api_Is_Reachable()
    {
        using var response = await _api.Client.GetAsync("");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
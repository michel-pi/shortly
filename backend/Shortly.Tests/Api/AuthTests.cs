using System;
using System.Net;
using System.Net.Http.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Shortly.Configuration.Options;
using Shortly.Features.Auth.Dto;
using Shortly.Tests.Setup;

namespace Shortly.Tests.Api;

public class AuthTests : IClassFixture<ShortlyApiFactory>
{
    private readonly ShortlyApiFactory _api;

    public AuthTests(ShortlyApiFactory api)
    {
        _api = api;
    }

    [Fact]
    public async Task Can_Login()
    {
        var loginCredentials = _api.Services.GetRequiredService<IOptions<DefaultAdminOptions>>().Value;

        using var response = await _api.Client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = loginCredentials.Email,
            Password = loginCredentials.Password
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(content);
        
        Assert.NotNull(content.AccessToken);
        Assert.NotEqual(string.Empty, content.AccessToken);

        Assert.NotEqual(Guid.Empty.ToString(), content.RefreshToken.ToString());
    }

    [Fact]
    public async Task Invalid_Login()
    {
        using var response = await _api.Client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "unit.tests@local.test",
            Password = "1234"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

using Testcontainers.PostgreSql;

namespace Shortly.Tests.Setup;

public class ShortlyApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder()
            .WithImage("postgres:18")
            .Build();

    private HttpClient _client = default!;

    public PostgreSqlContainer Db => _db;
    public HttpClient Client => _client;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseEnvironment("development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:Default", _db.GetConnectionString() }
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _db.StartAsync();

        _client = CreateClient();
        //_client = CreateClient(new WebApplicationFactoryClientOptions() { BaseAddress = new Uri("https://localhost:7090") });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        _client.Dispose();
        await _db.DisposeAsync();

        await base.DisposeAsync();
    }
}

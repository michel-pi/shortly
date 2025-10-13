using System;
using System.IO;
using System.Net;

using MaxMind.GeoIP2;

using Shortly.Domain.Services;

namespace Shortly.Infrastructure.Services;

public class MaxMindGeolocationService : IDisposable, IGeolocationService
{
    private readonly DatabaseReader _db;

    public MaxMindGeolocationService()
    {
        _db = LoadDatabase();
    }

    public string LookupCountry(IPAddress? address)
    {
        if (address == null || !_db.TryCountry(address, out var response))
        {
            response = null;
        }

        return response?.Country?.Name
            ?? response?.RegisteredCountry?.Name
            ?? response?.RepresentedCountry?.Name
            ?? "n/a";
    }

    private static DatabaseReader LoadDatabase()
    {
        var bytes = Properties.Resources.GeoLite2CountryDb;
        var stream = new MemoryStream(bytes);
        return new DatabaseReader(stream);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}

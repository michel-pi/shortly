using System.Net;

namespace Shortly.Domain.Services;

public interface IGeolocationService
{
    string LookupCountry(IPAddress? address);
}

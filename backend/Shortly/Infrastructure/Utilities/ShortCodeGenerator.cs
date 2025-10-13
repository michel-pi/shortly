using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using NanoidDotNet;

using Shortly.Configuration.Options;
using Shortly.Domain.Services;

namespace Shortly.Infrastructure.Utilities;

public class ShortCodeGenerator : IShortCodeGenerator
{
    private readonly ShortCodeOptions _options;

    public ShortCodeGenerator(IOptions<ShortCodeOptions> options)
    {
        _options = options.Value;
    }

    public string Generate()
    {
        return Nanoid.Generate(_options.Alphabet, _options.Length);
    }

    public Task<string> GenerateAsync()
    {
        return Nanoid.GenerateAsync(_options.Alphabet, _options.Length);
    }
}

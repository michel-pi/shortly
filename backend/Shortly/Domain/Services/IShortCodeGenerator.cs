using System.Threading.Tasks;

namespace Shortly.Domain.Services;

public interface IShortCodeGenerator
{
    string Generate();
    Task<string> GenerateAsync();
}

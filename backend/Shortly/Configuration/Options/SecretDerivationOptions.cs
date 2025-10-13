using System.ComponentModel.DataAnnotations;

namespace Shortly.Configuration.Options;

public class SecretDerivationOptions
{
    [StringLength(maximumLength: 1024, MinimumLength = 8)]
    public string SaltPlaintext { get; set; } = "secret-passphrase";

    [Range(10 * 1024, 1000 * 1024)]
    public int Iterations { get; set; } = 200 * 1024;

    [StringLength(maximumLength: 32, MinimumLength = 3)]
    public string HashAlgorithmName { get; set; } = "SHA256"; // https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithmname?view=net-9.0
}

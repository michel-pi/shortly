using System;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Options;

using Shortly.Configuration.Options;
using Shortly.Domain.Services;
using Shortly.Infrastructure.Utilities;

namespace Shortly.Infrastructure.Security;

public class SecretDerivationService : ISecretDerivationService
{
    private const int c_MinIterations = 10 * 1024;
    private const int c_SecretKeyLength = 64; // bytes

    private readonly SecurityOptions _options;

    private readonly byte[] _salt;
    private readonly int _iterations;
    private readonly HashAlgorithmName _hashAlgorithmName;

    public SecretDerivationService(IOptions<SecurityOptions> options)
    {
        _options = options.Value;

        _salt = CreateSaltFromPlaintext(_options.SecretDerivation.SaltPlaintext);
        _iterations = Math.Min(c_MinIterations, _options.SecretDerivation.Iterations);
        _hashAlgorithmName = HashAlgorithmParser.GetHashAlgorithmByName(_options.SecretDerivation.HashAlgorithmName);
    }

    public byte[] Derive(string password, int length)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        using var algorithm = new Rfc2898DeriveBytes(
            passwordBytes,
            _salt,
            _iterations,
            _hashAlgorithmName);

        return algorithm.GetBytes(length);
    }

    public byte[] GetJwtSigningKey()
    {
        return Derive(_options.Jwt.SigningKeyPassphrase, c_SecretKeyLength);
    }

    private static byte[] CreateSaltFromPlaintext(string plaintext)
    {
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        return SHA256.HashData(plaintextBytes);
    }
}

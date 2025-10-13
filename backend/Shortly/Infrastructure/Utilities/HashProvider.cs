using System;
using System.Security.Cryptography;
using System.Text;

namespace Shortly.Infrastructure.Utilities;

public static class HashProvider
{
    public static string Sha256HexString(string value)
    {
        var valueBytes = Encoding.UTF8.GetBytes(value);
        var hashBytes = SHA256.HashData(valueBytes);
        return Convert.ToHexString(hashBytes);
    }
}

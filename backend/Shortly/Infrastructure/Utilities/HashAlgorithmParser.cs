using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;

namespace Shortly.Infrastructure.Utilities;

public static class HashAlgorithmParser
{
    private static readonly Dictionary<string, HashAlgorithmName> s_HashAlgorithmMap;

    static HashAlgorithmParser()
    {
        s_HashAlgorithmMap = new Dictionary<string, HashAlgorithmName>(StringComparer.OrdinalIgnoreCase);

        foreach (var property in typeof(HashAlgorithmName).GetProperties(BindingFlags.Public | BindingFlags.Static))
        {
            var propertyValue = (HashAlgorithmName)property.GetValue(null)!;

            s_HashAlgorithmMap[property.Name] = propertyValue;

            if (!string.IsNullOrEmpty(propertyValue.Name))
            {
                s_HashAlgorithmMap[propertyValue.Name] = propertyValue;
            }
        }
    }

    public static HashAlgorithmName GetHashAlgorithmByName(string algorithmName)
    {
        return s_HashAlgorithmMap[algorithmName];
    }

    public static bool TryGetHashAlgorithmByName(string algorithmName, out HashAlgorithmName hashAlgorithm)
    {
        return s_HashAlgorithmMap.TryGetValue(algorithmName, out hashAlgorithm);
    }
}

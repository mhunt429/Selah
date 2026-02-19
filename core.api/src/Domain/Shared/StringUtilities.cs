using System.Security.Cryptography;

namespace Domain.Shared;

public class StringUtilities
{
    public static string GenerateSecret(int bytesize = 32)
    {
        byte[] key = RandomNumberGenerator.GetBytes(bytesize);
        return Convert.ToBase64String(key);
    }
    
}
using System.Security.Cryptography;
using System.Text;

namespace Domain.Shared;

public class StringUtilities
{
    public static string ConvertToBase64(string plainText)
    {
       var bytes = Encoding.UTF8.GetBytes(plainText);
       return Convert.ToBase64String(bytes);
    }

    public static string GenerateAesSecret()
    {
        byte[] key = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(key);
    }
    
    public static string GenerateJwtSecret()
    {
        byte[] key = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(key);
    }
}
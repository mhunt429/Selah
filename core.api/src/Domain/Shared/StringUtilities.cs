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

    public static string GenerateSecret(int bytesize = 32)
    {
        byte[] key = RandomNumberGenerator.GetBytes(bytesize);
        return Convert.ToBase64String(key);
    }
    
}
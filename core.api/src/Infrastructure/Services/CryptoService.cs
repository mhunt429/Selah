using HashidsNet;
using System.Security.Cryptography;
using System.Text;
using Domain.Configuration;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services;

public class CryptoService: ICryptoService
{
    
    private readonly SecurityConfig _securityConfig;
    private readonly IHashids _hashIds;
    private readonly IPasswordHasherService _passwordHasherService;

    public CryptoService(SecurityConfig securityConfig, IHashids hashIds, IPasswordHasherService passwordHasherService)
    {
        _securityConfig = securityConfig;
        _hashIds = hashIds;
        _passwordHasherService = passwordHasherService;
    }
    
    public string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Convert.FromBase64String(_securityConfig.CryptoSecret);
            aesAlg.GenerateIV(); // Generate a random 16-byte IV
            aesAlg.Padding = PaddingMode.PKCS7;

            using (var msEncrypt = new MemoryStream())
            {
                // Write the IV to the start of the output
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    // Write plaintext into the CryptoStream
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    csEncrypt.Write(plainBytes, 0, plainBytes.Length);

                    // Ensure all data is flushed to the stream
                    csEncrypt.FlushFinalBlock();
                }

                // Return the combined IV and ciphertext
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public string Decrypt(string encryptedData)
    {
        // Convert the Base64 string to a byte array
        byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

        // Validate input to ensure it contains at least the IV and ciphertext
        if (encryptedBytes.Length <= 16)
        {
            throw new ArgumentException("Encrypted data is too short to contain an IV and ciphertext.");
        }

        // Extract the IV (first 16 bytes)
        var iv = encryptedBytes.ToList().Take(16).ToArray();
        var cipherText = encryptedBytes.Skip(16).ToArray();

        using (var aesAlg = Aes.Create())
        {
            // Set the key and IV
            aesAlg.Key = Convert.FromBase64String(_securityConfig.CryptoSecret);
            aesAlg.IV = iv;

            // Initialize the decryptor
            using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
            {
                // Perform the decryption
                var decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

                // Convert decrypted bytes to a UTF-8 string
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }

    public string HashPassword(string password)
    {
        return _passwordHasherService.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
      return _passwordHasherService.VerifyPassword(password, passwordHash);
    }

    public string HashValue(string plainText)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            StringBuilder hashString = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                hashString.Append(b.ToString("x2")); // Converts byte to hex
            }

            return hashString.ToString();
        }
    }

    public long DecodeHashId(string hashId)
    {
       return _hashIds.DecodeSingle((hashId));
    }

    public string EncodeHashId(long id)
    {
        return _hashIds.EncodeLong(id);
    }
}
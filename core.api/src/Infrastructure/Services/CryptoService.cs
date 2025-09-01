using HashidsNet;
using System.Security.Cryptography;
using System.Text;
using Domain.Configuration;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services;

public class CryptoService : ICryptoService
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
            aesAlg.GenerateIV();
            aesAlg.Padding = PaddingMode.PKCS7;

            using (var msEncrypt = new MemoryStream())
            {
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    csEncrypt.Write(plainBytes, 0, plainBytes.Length);

                    csEncrypt.FlushFinalBlock();
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public string Decrypt(string encryptedData)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

        if (encryptedBytes.Length <= 16)
        {
            throw new ArgumentException("Encrypted data is too short to contain an IV and ciphertext.");
        }

        var iv = encryptedBytes.ToList().Take(16).ToArray();
        var cipherText = encryptedBytes.Skip(16).ToArray();

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Convert.FromBase64String(_securityConfig.CryptoSecret);
            aesAlg.IV = iv;

            using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
            {
                var decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

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
                hashString.Append(b.ToString("x2"));
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
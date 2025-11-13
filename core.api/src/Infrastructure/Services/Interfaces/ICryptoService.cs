namespace Infrastructure.Services.Interfaces;

public interface ICryptoService
{
    byte[] Encrypt(string plainText);

    string Decrypt(byte[] encryptedData);

    string HashPassword(string password);

    bool VerifyPassword(string password, string passwordHash);

    string HashValue(string plainText);

    long DecodeHashId(string hashId);

    string EncodeHashId(long id);
}
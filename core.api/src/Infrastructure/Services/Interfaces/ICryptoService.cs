namespace Infrastructure.Services.Interfaces;

public interface ICryptoService
{
    string Encrypt(string plainText);

    string Decrypt(string encryptedData);

    string HashPassword(string password);

    bool VerifyPassword(string password, string passwordHash);

    string HashValue(string plainText);

    long DecodeHashId(string hashId);

    string EncodeHashId(long id);
}
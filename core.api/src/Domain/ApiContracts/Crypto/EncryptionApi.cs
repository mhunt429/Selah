namespace Domain.ApiContracts.Crypto;

public class DecryptStringRequest
{
    public required string EncryptedString { get; set; }
}

public class DecryptStringResponse
{
    public required string DecryptedString { get; set; }
}
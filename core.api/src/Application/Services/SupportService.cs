using Infrastructure.Repository.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Application.Services;

public class SupportService(IAccountConnectorRepository accountConnectorRepository, ICryptoService cryptoService)
{
    public async Task<IEnumerable<string>> GetDecryptedConnectorKeys(int userId)
    {
        var keys = await accountConnectorRepository.GetAccountConnectorRecordsByUserId(userId);

        return keys.Select(x => cryptoService.Decrypt(x.EncryptedAccessToken));
    }
}
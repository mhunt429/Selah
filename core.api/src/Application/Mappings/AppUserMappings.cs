using Domain.Models.Entities.ApplicationUser;
using Infrastructure.Services.Interfaces;

namespace Application.Mappings;

public static class AppUserMappings
{
    /// <summary>
    /// Takes a record from the app_user table and maps it to the API response for a given user
    /// </summary>
    /// <param name="user"></param>
    public static Domain.ApiContracts.ApplicationUser MapAppUserDataAccessToApiContract(this ApplicationUserEntity user, ICryptoService cryptoService)
    {
        string[] parsedName = cryptoService.Decrypt(user.EncryptedName).Split("|");
        return new Domain.ApiContracts.ApplicationUser
        {
            Id = user.Id,
            AccountId = user.AccountId,
            Email = cryptoService.Decrypt(user.EncryptedEmail),
            FirstName = parsedName[0],
            LastName = parsedName[1],
            PhoneNumber = cryptoService.Decrypt(user.EncryptedPhone),
            CreatedDate = user.CreatedDate
        };
    }
}
using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.UserAccount;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Helpers;

public static class TestHelpers
{
    public static string TestConnectionString => "User ID=postgres;Password=postgres;Host=localhost;Port=65432;Database=postgres";

    public static AppDbContext BuildTestDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(TestConnectionString)
            .Options;

        return new AppDbContext(options);
    }
    

    /// <summary>
    /// It's simple, most records need a user, and each user needs a single account
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accountId"></param>
    /// <param name="repository"></param>
        
    public static async Task<(UserAccountEntity, ApplicationUserEntity)> SetUpBaseRecords(AppDbContext dbContext)
    {
        
        UserAccountEntity account = new UserAccountEntity
        {
            AccountName = "AccountName",
            CreatedOn = DateTimeOffset.UtcNow
        };

        ApplicationUserEntity user = new ApplicationUserEntity
        {
            EncryptedEmail = "email"u8.ToArray(),
            Password = "password",
            EncryptedName = "FirstName|LastName"u8.ToArray(),
            EncryptedPhone = "123-123-1234"u8.ToArray(),
            LastLoginIp = "127.0.0.1",
            EmailHash = "email",
            UserAccount =  account,
        };

        var registrationRepository = new RegistrationRepository(dbContext);

        (int, int) registrationResult = await registrationRepository.RegisterAccount(user);

        account.Id = registrationResult.Item1;
        user.Id = registrationResult.Item2;

        return (account, user);
    }
    
}
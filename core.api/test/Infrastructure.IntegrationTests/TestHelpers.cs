using Microsoft.EntityFrameworkCore;
using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.UserAccount;
using Infrastructure.Repository;
using System.Text;
namespace Infrastructure.IntegrationTests;

public static class TestHelpers
{
    public static string TestConnectionString { get; } =
        "User ID=postgres;Password=postgres;Host=localhost;Port=65432;Database=postgres";

    public static AppDbContext BuildTestDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(TestConnectionString)
            .Options;

        return new AppDbContext(options);
    }

    public static SelahDbConnectionFactory TestDbFactory { get; } = new SelahDbConnectionFactory(TestConnectionString);

    /// <summary>
    /// It's simple, most records need a user, and each user needs a single account
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accountId"></param>
    /// <param name="repository"></param>
    public static async Task<(UserAccountEntity, ApplicationUserEntity)> SetUpBaseRecords(
        IRegistrationRepository repository)
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
            EmailHash = "email"
        };


        (int, int) registrationResult = await repository.RegisterAccount(account, user);

        account.Id = registrationResult.Item1;
        user.Id = registrationResult.Item2;

        return (account, user);
    }

    public static async Task TearDownBaseRecords(int userId, int accountId, BaseRepository repository)
    {
        string deleteUserSql = "DELETE FROM app_user WHERE id = @id";
        string deleteAccountUser = "DELETE FROM user_account WHERE id = @id";

        await repository.DeleteAsync(deleteUserSql, new { id = userId });
        await repository.DeleteAsync(deleteAccountUser, new { id = accountId });
    }
}
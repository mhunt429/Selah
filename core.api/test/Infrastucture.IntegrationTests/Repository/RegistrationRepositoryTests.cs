using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.UserAccount;
using FluentAssertions;
using Infrastructure.Repository;

namespace Infrastructure.IntegrationTests.Repository;

public class RegistrationRepositoryTests : IAsyncLifetime
{
    private readonly BaseRepository _baseRepository = new BaseRepository(TestHelpers.TestDbFactory);

    private readonly RegistrationRepository _repository;

    private int _accountId;
    private int _userId;
    public RegistrationRepositoryTests()
    {
        _repository = new RegistrationRepository(TestHelpers.BuildTestDbContext());
    }

    [Fact]
    public async Task Register_ShouldSaveAccountAndUserRecord()
    {
        (UserAccountEntity, ApplicationUserEntity) result = await TestHelpers.SetUpBaseRecords(_repository);
        _accountId = result.Item1.Id;
        _userId = result.Item2.Id;;
        result.Should().NotBeNull();
        result.Item2.Id.Should().Be(_userId);
    }

    public async Task InitializeAsync()
    {
    }

    public async Task DisposeAsync()
    {
        string deleteUserSql = "DELETE FROM app_user WHERE id = @id";
        string deleteAccountUser = "DELETE FROM user_account WHERE id = @id";

        await _baseRepository.DeleteAsync(deleteUserSql, new { id = _userId });
        await _baseRepository.DeleteAsync(deleteAccountUser, new { id = _accountId });
    }
}
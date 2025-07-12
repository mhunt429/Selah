using FluentAssertions;
using Infrastructure.Repository;

namespace Infrastructure.IntegrationTests.Repository;

public class RegistrationRepositoryTests : IAsyncLifetime
{
    private readonly BaseRepository _baseRepository = new BaseRepository(TestHelpers.TestDbFactory);

    private readonly RegistrationRepository _repository;

    private readonly Guid _accountId = Guid.NewGuid();

    private readonly Guid _userId = Guid.NewGuid();

    public RegistrationRepositoryTests()
    {
        _repository = new RegistrationRepository(TestHelpers.BuildTestDbContext());
    }

    [Fact]
    public async Task Register_ShouldSaveAccountAndUserRecord()
    {
        var result = await TestHelpers.SetUpBaseRecords(_accountId, _userId, _repository);
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
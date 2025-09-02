using FluentAssertions;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.IntegrationTests.Repository;

[Collection("Database")]
public class AppUserRepositoryTests : IAsyncLifetime
{
    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();
    private readonly DatabaseFixture _fixture;
    private readonly IApplicationUserRepository _repository;
    private int _accountId;

    private int _userId;

    public AppUserRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new AppUserRepository(_dbContext);
    }


    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();

        var registrationRepository = new RegistrationRepository(_dbContext);
        var result = await TestHelpers.SetUpBaseRecords(registrationRepository);
        _accountId = result.Item1.Id;
        _userId = result.Item2.Id;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser()
    {
        var result = await _repository.GetUserByIdAsync(_userId);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserByEmailHashAsync_ShouldReturnUser()
    {
        var user = await _repository.GetUserByIdAsync(_userId);

        var result = await _repository.GetUserByEmail(user.EmailHash);
        result.Should().NotBeNull();
    }
}
using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.UserAccount;
using FluentAssertions;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;

namespace Infrastructure.IntegrationTests.Repository;

public class AppUserRepositoryTests : IAsyncLifetime
{
    private readonly AppDbContext _dbContext =  TestHelpers.BuildTestDbContext();
    
    private readonly BaseRepository _baseRepository = new BaseRepository(TestHelpers.TestDbFactory);
    
    private readonly IApplicationUserRepository _repository;

    private int _accountId;
    private int _userId;

    public AppUserRepositoryTests()
    {
        _repository = new AppUserRepository(_dbContext);
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


    public async Task InitializeAsync()
    {
        var registrationRepository = new RegistrationRepository(_dbContext);
        (UserAccountEntity, ApplicationUserEntity) result = await TestHelpers.SetUpBaseRecords(registrationRepository);
        _accountId = result.Item1.Id;
        _userId = result.Item2.Id;;
    }

    public async Task DisposeAsync()
    {
        await TestHelpers.TearDownBaseRecords(_userId, _accountId, _baseRepository);
    }
}
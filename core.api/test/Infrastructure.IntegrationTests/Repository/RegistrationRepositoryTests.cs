using FluentAssertions;
using Infrastructure.Repository;

namespace Infrastructure.IntegrationTests.Repository;

[Collection("Database")]
public class RegistrationRepositoryTests : IAsyncLifetime
{
    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();
    private readonly DatabaseFixture _fixture;
    private readonly RegistrationRepository _repository;
    private int _userId;


    public RegistrationRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new RegistrationRepository(TestHelpers.BuildTestDbContext());
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Register_ShouldSaveAccountAndUserRecord()
    {
        var result = await TestHelpers.SetUpBaseRecords(_repository);
        _userId = result.Item2.Id;
        result.Should().NotBeNull();
        result.Item2.Id.Should().Be(_userId);
    }
}
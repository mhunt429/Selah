using AwesomeAssertions;
using Domain.Models.Entities.Identity;
using Infrastructure;
using Infrastructure.Repository;
using IntegrationTests.Helpers;

namespace IntegrationTests.Repository;

[Collection("Database")]
public class TokenRepositoryTests : IAsyncLifetime
{
    private readonly IDbConnectionFactory _dbConnectionFactory = TestHelpers.TestDbFactory;
    private TokenRepository _tokenRepo;

    private readonly DatabaseFixture _fixture;
    private int _userId;

    public TokenRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _tokenRepo = new TokenRepository(_dbConnectionFactory);
    }

    [Fact]
    public async Task RepoShouldSaveToken()
    {
        var entityToSave = new TokenEntity
        {
            UserId = _userId,
            Token = "abc123",
            TokenType = TokenType.AccessToken,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(1),
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _tokenRepo.SaveTokenAsync(entityToSave);

        var token = await _tokenRepo.GetTokenByUserId(_userId, TokenType.AccessToken);
        token.Should().NotBeNull();
        token.Id.Should().BeGreaterThan(0);


        entityToSave.TokenType = TokenType.RefreshToken;
        await _tokenRepo.SaveTokenAsync(entityToSave);
    }

    [Fact]
    public async Task RepoShouldGetTokenByUser()
    {
        var entityToSave = new TokenEntity
        {
            UserId = _userId,
            Token = "abc123",
            TokenType = TokenType.RefreshToken,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(1),
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

        await _tokenRepo.SaveTokenAsync(entityToSave);

        var token = await _tokenRepo.GetTokenByUserId(_userId, TokenType.RefreshToken);
        token.Should().NotBeNull();
        token.Token.Should().NotBeNullOrEmpty();
        token.ExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
        token.CreatedAt.Should().BeBefore(DateTimeOffset.UtcNow);
        token.UserId.Should().Be(_userId);
        token.TokenType.Should().Be(TokenType.RefreshToken);
        token.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task RepoShouldDeleteTokenByUser()
    {
        await _tokenRepo.DeleteTokenAsync(_userId, TokenType.AccessToken);
        var token = await _tokenRepo.GetTokenByUserId(_userId, TokenType.AccessToken);
        token.Should().BeNull();
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
        var result = await TestHelpers.SetUpBaseRecords(TestHelpers.BuildTestDbContext());
        _userId = result.Item2.Id;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
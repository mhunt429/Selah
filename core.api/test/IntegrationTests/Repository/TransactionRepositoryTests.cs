using AwesomeAssertions;
using Domain.Models.DbUtils;
using Domain.Models.Entities.FinancialAccount;
using Domain.Models.Entities.Transactions;
using Infrastructure;
using Infrastructure.Repository;
using IntegrationTests.Helpers;

namespace IntegrationTests.Repository;

[Collection("Database")]
public class TransactionRepositoryTests : IAsyncLifetime
{
    private readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();
    private readonly DatabaseFixture _fixture;

    private readonly TransactionRepository _repo;
    private int _userId;
    private List<TransactionCategoryEntity> _categories;
    private FinancialAccountEntity _financialAccount;

    public TransactionRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repo = new TransactionRepository(_dbContext);
    }


    [Fact]
    public async Task Repo_ShouldSaveTransaction_WithLineItems()
    {
        var transaction = new TransactionEntity()
        {
            AccountId = _financialAccount.Id,
            UserId = _userId,
            Amount = 100.00m,
            TransactionDate = DateTimeOffset.UtcNow,
            ImportedDate = DateTimeOffset.UtcNow,
            Pending = false,
            MerchantName = "Publix",
            TransactionName = "Weekly Grocery Run",
            LineItems = new List<TransactionLineItemEntity>()
            {
                new()
                {
                    Category = _categories[0],
                    CategoryId =  _categories[0].Id,
                    Description = "Groceries",
                    Amount = 90
                },
                new()
                {
                    Category = _categories[1],
                    CategoryId =  _categories[1].Id,
                    Description = "Dog Food",
                    Amount = 10
                }
            }
        };

        DbOperationResult<TransactionEntity> savedTx =
            await _repo.CreateTransaction(transaction);

        savedTx.Status.Should().Be(ResultStatus.Success);
        savedTx.ErrorMessage.Should().BeNullOrEmpty();
        savedTx.Data.Should().NotBeNull();
        savedTx.Data.Id.Should().Be(transaction.Id);
        savedTx.Data.UserId.Should().Be(_userId);
        savedTx.Data.TransactionDate.Should().Be(transaction.TransactionDate);
        savedTx.Data.ImportedDate.Should().Be(transaction.ImportedDate);
        savedTx.Data.Pending.Should().Be(transaction.Pending);
        savedTx.Data.MerchantName.Should().Be(transaction.MerchantName);
        savedTx.Data.TransactionName.Should().Be(transaction.TransactionName);
        savedTx.Data.LineItems.Count.Should().Be(transaction.LineItems.Count);
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();

        RegistrationRepository registrationRepository = new(_dbContext);
        var result = await TestHelpers.SetUpBaseRecords(registrationRepository);
        _userId = result.Item2.Id;

        
        FinancialAccountRepository financialAccountRepository = new(_dbContext);
        _financialAccount =  await financialAccountRepository.AddAccountAsync(new FinancialAccountEntity
        {
            ConnectorId = null,
            UserId = _userId,
            ExternalId = null,
            CurrentBalance = 100,
            AccountMask = "1234",
            DisplayName = "Chase Sapphire Credit Card",
            Subtype = "CreditCard"
        });
        
        _categories = new List<TransactionCategoryEntity>()
        {
            new()
            {
                UserId = _userId,
                CategoryName = "Groceries"
            },
            new()
            {
                UserId = _userId,
                CategoryName = "Pets"
            }
        };

        await _dbContext.TransactionCategories.AddRangeAsync(_categories);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
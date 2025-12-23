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

    private List<TransactionEntity> _testTransactions;

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
                    CategoryId = _categories[0].Id,
                    Description = "Groceries",
                    Amount = 90
                },
                new()
                {
                    Category = _categories[1],
                    CategoryId = _categories[1].Id,
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

    [Theory]
    [InlineData("id", "DESC", "Nobu")]
    [InlineData("id", "ASC", "Rolex")]
    [InlineData("date", "DESC", "Rolex")]
    [InlineData("date", "ASC", "Nobu")]
    [InlineData("amount", "DESC", "Mercedes")]
    [InlineData("amount", "ASC", "Nobu")]
    [InlineData("inavlidColumn", "ASC", "Rolex")]
    public async Task Transactions_Can_Be_QueriedAndSorted(string sortParameter, string sortDirection, string merchant)
    {
        var sortParams = new SortParameters(sortParameter, sortDirection);
        var transactions = await _repo.GetTransactionsByUser(_userId, sortParameters: sortParams);
        transactions.Should().NotBeNull();
        transactions.Status.Should().Be(ResultStatus.Success);
        transactions.Data.Should().NotBeNullOrEmpty();
        transactions.Data.Count().Should().Be(3);

        transactions.Data.First().MerchantName.Should().Be(merchant);
    }

    [Fact]
    public async Task Transactions_Can_Be_Queried_With_Limit()
    {
        var transactions = await _repo.GetTransactionsByUser(
            _userId, limit: 1);
        transactions.Should().NotBeNull();
        transactions.Status.Should().Be(ResultStatus.Success);
        transactions.Data.Should().NotBeNullOrEmpty();
        transactions.Data.Count().Should().Be(1);

        var transaction = transactions.Data.First();

        transaction.MerchantName.Should().Be("Rolex");
        transaction.TransactionName.Should().Be("Annual Watch Haul");
        transaction.TransactionDate.Should().BeAfter(DateTimeOffset.MinValue);
        transaction.Amount.Should().Be(10000);

        var lineItems = await _repo.GetTransactionLineItems(transaction.Id);
        lineItems.Should().NotBeNull();
        lineItems.Data.Count().Should().Be(1);
        var lineItem = lineItems.Data.First();
        lineItem.Description.Should().Be("Watches");
    }

    [Fact]
    public async Task Transaction_and_LineItemsCanBeDeleted()
    {
        var transactionToBeDeleted = new TransactionEntity()
        {
            AccountId = _financialAccount.Id,
            UserId = _userId,
            LineItems =  new List<TransactionLineItemEntity>()
            {
                new TransactionLineItemEntity()
                {
                    CategoryId = _categories.First().Id, 
                },
                new TransactionLineItemEntity()
                {
                    CategoryId = _categories.Last().Id, 
                }
            }
        };
        
        await _repo.CreateTransaction(transactionToBeDeleted);
        
        await _repo.DeleteTransaction(transactionToBeDeleted.Id, _userId);
        
       var tx =  await _repo.GetTransaction(_userId, transactionToBeDeleted.Id);
       var lineItems = await _repo.GetTransactionLineItems(transactionToBeDeleted.Id);
       tx.Status.Should().Be(ResultStatus.Success);
       tx.Data.Should().BeNull();
       
       lineItems.Should().NotBeNull();
       lineItems.Data.Count().Should().Be(0);
    }

    [Fact]
    public async Task Transactions_Can_Be_Updated()
    {
        var transactionToBeUpdated = _testTransactions[1];
        transactionToBeUpdated.Pending = false;
        
        await _repo.UpdateTransaction(transactionToBeUpdated);
        
        var updated = await _dbContext.Transactions.FindAsync(transactionToBeUpdated.Id);
        
        updated.Should().NotBeNull();
    
        updated.Pending.Should().BeFalse();
    }

    [Fact]
    public async Task Transactions_Can_Be_Added_In_Bulk()
    {
        var transactions = new List<TransactionEntity>()
        {
            new()
            {
                AccountId = _financialAccount.Id,
                UserId = _userId,
                Amount =  1000,
            },
            new()
            {
                AccountId = _financialAccount.Id,
                UserId = _userId,
                Amount =  300,
            },
            new (){
                AccountId = _financialAccount.Id,
                UserId = _userId,
                Amount =  300,
            }
        };
        
        var result = await _repo.AddTransactionsInBulk(transactions);
        result.Status.Should().Be(ResultStatus.Success);
        result.Data.Should().Be(3);
    }
    
    [Fact]
    public async Task Transactions_Can_Be_Deleted_In_Bulk()
    {
        var transactions = new List<TransactionEntity>()
        {
            new()
            {
                AccountId = _financialAccount.Id,
                UserId = _userId,
                Amount =  1000,
                ExternalTransactionId = "123",
                TransactionName = "Import 1"
            },
            new()
            {
                AccountId = _financialAccount.Id,
                UserId = _userId,
                Amount =  300,
                ExternalTransactionId = "321",
                TransactionName = "Import 2"
            },
            new (){
                AccountId = _financialAccount.Id,
                UserId = _userId,
                Amount =  300,
                TransactionName = "Import 3"
            }
        };
        
        await _repo.AddTransactionsInBulk(transactions);
        
        var result = await _repo.DeleteTransactionsInBulk(transactions
            .Select(t => t.ExternalTransactionId)
            .ToList(), _userId);
        result.Status.Should().Be(ResultStatus.Success);
        result.Data.Should().Be(2);
    }
    
    [Fact]
    public async Task Transactions_Can_Be_Updated_In_Bulk()
    {
        var transactions = new List<TransactionEntity>()
        {
            new()
            {
                AccountId = _financialAccount.Id,
                UserId = _userId,
                Amount =  1000,
                ExternalTransactionId = "123",
                TransactionName = "Import 1",
                LineItems =  new List<TransactionLineItemEntity>()
                {
                    new TransactionLineItemEntity()
                    {
                        CategoryId = _categories[3].Id,
                    }
                }
            },
            new()
            {
                AccountId = _financialAccount.Id,
                UserId = _userId,
                Amount =  300,
                ExternalTransactionId = "321",
                TransactionName = "Import 2",
                LineItems =  new List<TransactionLineItemEntity>()
                {
                    new TransactionLineItemEntity()
                    {
                        CategoryId = _categories[3].Id,
                    }
                }
            },
            new (){
                AccountId = _financialAccount.Id,
                UserId = _userId,
                Amount =  300,
                TransactionName = "Import 3",
                LineItems =  new List<TransactionLineItemEntity>()
                {
                    new TransactionLineItemEntity()
                    {
                        CategoryId = _categories[3].Id,
                    }
                }
            }
        };
        
        await _repo.AddTransactionsInBulk(transactions);

  
        var result = await _repo.UpdateTransactionsInBulk(transactions, _userId);
        result.Status.Should().Be(ResultStatus.Success);
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
        var result = await TestHelpers.SetUpBaseRecords();
        _userId = result.Item2.Id;


        FinancialAccountRepository financialAccountRepository = new(_dbContext);
        _financialAccount = await financialAccountRepository.AddAccountAsync(new FinancialAccountEntity
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
            },
            new()
            {
                UserId = _userId,
                CategoryName = "Watches"
            },
            new()
            {
                UserId = _userId,
                CategoryName = "Vehicle"
            },
            new()
            {
                UserId = _userId,
                CategoryName = "Restaurants"
            }
        };

        await _dbContext.TransactionCategories.AddRangeAsync(_categories);

        _testTransactions = new List<TransactionEntity>()
        {
            new TransactionEntity()
            {
                AccountId = _financialAccount.Id,
                UserId = _userId,
                Amount = 10000m,
                TransactionDate = DateTimeOffset.UtcNow.AddDays(-1),
                ImportedDate = DateTimeOffset.UtcNow,
                Pending = false,
                MerchantName = "Rolex",
                TransactionName = "Annual Watch Haul",
                LineItems = new List<TransactionLineItemEntity>()
                {
                    new()
                    {
                        Category = _categories[3],
                        CategoryId = _categories[3].Id,
                        Description = "Watches",
                        Amount = 10000
                    },
                }
            },
            new()
            {
                AccountId = _financialAccount.Id,
                UserId = _userId,
                Amount = 150000,
                TransactionDate = DateTimeOffset.UtcNow.AddDays(-2),
                ImportedDate = DateTimeOffset.UtcNow,
                Pending = true,
                MerchantName = "Mercedes",
                TransactionName = "G Wagon",
                LineItems = new List<TransactionLineItemEntity>()
                {
                    new()
                    {
                        Category = _categories[4],
                        CategoryId = _categories[4].Id,
                        Description = "Vehicles",
                        Amount = 150000
                    },
                }
            },
            new()
            {
                AccountId = _financialAccount.Id,
                UserId = _userId,
                Amount = 200,
                TransactionDate = DateTimeOffset.UtcNow.AddDays(-3),
                ImportedDate = DateTimeOffset.UtcNow,
                Pending = false,
                MerchantName = "Nobu",
                TransactionName = "Monthly Date Night",
                LineItems = new List<TransactionLineItemEntity>()
                {
                    new()
                    {
                        Category = _categories[4],
                        CategoryId = _categories[4].Id,
                        Description = "Monthly Date Night",
                        Amount = 200
                    },
                }
            },
        };

        await _dbContext.Transactions.AddRangeAsync(_testTransactions);
        await _dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
using AwesomeAssertions;
using Domain.Models.Entities.Transactions;
using Infrastructure;
using Infrastructure.Repository;
using Infrastructure.Repository.Interfaces;
using IntegrationTests.Helpers;

namespace IntegrationTests.Repository;

[Collection("Database")]
public class TransactionCategoryRepositoryTests(DatabaseFixture fixture) : IAsyncLifetime
{
    private static readonly AppDbContext _dbContext = TestHelpers.BuildTestDbContext();

    private readonly ITransactionCategoryRepository _transactionCategoryRepository =
        new TransactionCategoryRepository(_dbContext);

    private int _userId;
    private int _categoryId;

    [Fact]
    public async Task GetCategoryByIdAndUserAsync_ShouldReturnCategory()
    {
        var category = await _transactionCategoryRepository.GetCategoryByIdAndUserAsync(_categoryId, _userId);
        category.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllCategoriesByUser_ShouldReturnCategories()
    {
        var categories = await _transactionCategoryRepository.GetAllCategoriesByUser(_userId);
        categories.Should().NotBeNullOrEmpty();
        categories.First().Id.Should().Be(_categoryId);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldDeleteCategory()
    {
        var deleted = await _transactionCategoryRepository.DeleteCategoryById(_categoryId, _userId);
        deleted.Should().Be(1);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ShouldUpdateCategory()
    {
        var updated =
            await _transactionCategoryRepository.UpdateCategoryAsync(_categoryId, _categoryId, "Updated Category");
        updated.Should().BeGreaterThan(-1);
    }


    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();
        var result = await TestHelpers.SetUpBaseRecords(_dbContext);

        _userId = result.Item2.Id;

        _categoryId = await _transactionCategoryRepository.AddCategoryAsync(new TransactionCategoryEntity
        {
            AppLastChangedBy = _userId,
            CategoryName = "Groceries",
            UserId = _userId
        });

        _categoryId.Should().BeGreaterThan(0);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
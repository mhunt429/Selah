using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Npgsql.NameTranslation;
using Domain.Models.Entities.AccountConnector;
using Domain.Models.Entities.ApplicationUser;
using Domain.Models.Entities.FinancialAccount;
using Domain.Models.Entities.Identity;
using Domain.Models.Entities.Transactions;
using Domain.Models.Entities.UserAccount;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUserEntity> ApplicationUsers { get; set; }
    
    public DbSet<UserAccountEntity> UserAccounts { get; set; }
    
    public DbSet<AccountConnectorEntity> AccountConnectors { get; set; }
    
    public DbSet<FinancialAccountEntity> FinancialAccounts { get; set; }

    public DbSet<UserSessionEntity> UserSessions { get; set; }

    public DbSet<AccountBalanceHistoryEntity> AccountBalanceHistory { get; set; }

    public DbSet<TokenEntity> Tokens { get; set; }

    public DbSet<TransactionCategoryEntity> TransactionCategories { get; set; }

    public DbSet<TransactionEntity> Transactions { get; set; }

    public DbSet<TransactionLineItemEntity> TransactionLineItems { get; set; }
}
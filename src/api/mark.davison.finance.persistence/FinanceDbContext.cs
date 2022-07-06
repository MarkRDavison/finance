using mark.davison.finance.models.EntityConfiguration;

namespace mark.davison.finance.persistence;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityConfiguration).Assembly);
    }

    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<AccountType> AccountTypes { get; set; } = null!;
    public DbSet<AutoBudget> AutoBudgets { get; set; } = null!;
    public DbSet<AvailableBudget> AvailableBudgets { get; set; } = null!;
    public DbSet<Bank> Banks { get; set; } = null!;
    public DbSet<Bill> Bills { get; set; } = null!;
    public DbSet<Budget> Budgets { get; set; } = null!;
    public DbSet<BudgetLimit> BudgetLimits { get; set; } = null!;
    public DbSet<BudgetLimitRepetition> BudgetLimitRepetitions { get; set; } = null!;
    public DbSet<BudgetTransaction> BudgetTransactions { get; set; } = null!;
    public DbSet<BudgetTransactionJournal> BudgetTransactionJournals { get; set; } = null!;
    public DbSet<Category> Categorys { get; set; } = null!;
    public DbSet<CategoryTransaction> CategoryTransactions { get; set; } = null!;
    public DbSet<CategoryTransactionJournal> CategoryTransactionJournals { get; set; } = null!;
    public DbSet<Currency> Currencys { get; set; } = null!;
    public DbSet<CurrencyExchangeRate> CurrencyExchangeRates { get; set; } = null!;
    public DbSet<Goal> Goals { get; set; } = null!;
    public DbSet<GoalEvent> GoalEvents { get; set; } = null!;
    public DbSet<GroupJournal> GroupJournals { get; set; } = null!;
    public DbSet<JournalLink> JournalLinks { get; set; } = null!;
    public DbSet<LinkType> LinkTypes { get; set; } = null!;
    public DbSet<Note> Notes { get; set; } = null!;
    public DbSet<Recurrence> Recurrences { get; set; } = null!;
    public DbSet<RecurrenceRepitition> RecurrenceRepititions { get; set; } = null!;
    public DbSet<RecurringTransaction> RecurringTransactions { get; set; } = null!;
    public DbSet<Rule> Rules { get; set; } = null!;
    public DbSet<RuleAction> RuleActions { get; set; } = null!;
    public DbSet<RuleGroup> RuleGroups { get; set; } = null!;
    public DbSet<RuleTrigger> RuleTriggers { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<TransactionGroup> TransactionGroups { get; set; } = null!;
    public DbSet<TransactionJournal> TransactionJournals { get; set; } = null!;
    public DbSet<TransactionType> TransactionTypes { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
}

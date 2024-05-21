namespace mark.davison.finance.migrations.sqlite;

[ExcludeFromCodeCoverage]
[DatabaseMigrationAssembly(DatabaseType.Sqlite)]
public class SqliteContextFactory : SqliteDbContextFactory<FinanceDbContext>
{
    protected override FinanceDbContext DbContextCreation(
            DbContextOptions<FinanceDbContext> options
        ) => new FinanceDbContext(options);
}

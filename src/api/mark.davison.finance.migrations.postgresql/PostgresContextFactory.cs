namespace mark.davison.finance.migrations.postgresql;

[ExcludeFromCodeCoverage]
[DatabaseMigrationAssembly(DatabaseType.Postgres)]
public class PostgresContextFactory : PostgresDbContextFactory<FinanceDbContext>
{
    protected override string ConfigName => "DATABASE";

    protected override FinanceDbContext DbContextCreation(
            DbContextOptions<FinanceDbContext> options
        ) => new FinanceDbContext(options);
}

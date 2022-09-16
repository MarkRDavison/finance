using mark.davison.finance.persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace mark.davison.finance.migrations.sqlite;

public class FinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
{
    public FinanceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
        optionsBuilder.UseSqlite(
            "Data Source=finance.db",
            b => b.MigrationsAssembly("mark.davison.finance.migrations.sqlite"));

        return new FinanceDbContext(optionsBuilder.Options);
    }
}

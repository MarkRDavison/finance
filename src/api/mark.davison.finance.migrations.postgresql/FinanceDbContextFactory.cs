using mark.davison.finance.persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace mark.davison.finance.migrations.postgresql;

public class FinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
{
    public FinanceDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var financeConfig = configuration.GetSection("FINANCE");

        var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();

        var conn = new NpgsqlConnectionStringBuilder();
        conn.Host = financeConfig["DB_HOST"];
        conn.Database = financeConfig["DB_DATABASE"];
        conn.Port = int.Parse(financeConfig["DB_PORT"]);
        conn.Username = financeConfig["DB_USERNAME"];
        conn.Password = financeConfig["DB_PASSWORD"];

        optionsBuilder.UseNpgsql(
            conn.ConnectionString,
            _ => _.MigrationsAssembly("mark.davison.finance.migrations.postgresql"));
        return new FinanceDbContext(optionsBuilder.Options);
    }
}

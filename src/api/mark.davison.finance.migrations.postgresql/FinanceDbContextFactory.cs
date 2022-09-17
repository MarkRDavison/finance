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
        Console.WriteLine("FinanceDbContextFactory.IDesignTimeDbContextFactory");
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var financeConfig = configuration.GetSection("FINANCE");

        var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();

        var conn = new NpgsqlConnectionStringBuilder();
        conn.Host = financeConfig["DB_HOST"];
        conn.Database = financeConfig["DB_DATABASE"];
        if (int.TryParse(financeConfig["DB_PORT"], out int port))
        {
            conn.Port = port;
        }
        else
        {
            conn.Port = 5432;
        }
        conn.Username = financeConfig["DB_USERNAME"];
        conn.Password = financeConfig["DB_PASSWORD"];

        Console.WriteLine("Host: {0}", conn.Host);

        optionsBuilder.UseNpgsql(
            conn.ConnectionString,
            _ => _.MigrationsAssembly("mark.davison.finance.migrations.postgresql"));
        return new FinanceDbContext(optionsBuilder.Options);
    }
}

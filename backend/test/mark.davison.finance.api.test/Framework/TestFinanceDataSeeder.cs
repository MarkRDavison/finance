using System;

namespace mark.davison.finance.api.test.Framework;

public class TestFinanceDataSeeder : FinanceDataSeeder
{
    public TestFinanceDataSeeder(IRepository repository) : base(repository)
    {
    }

    public override async Task EnsureDataSeeded(CancellationToken cancellationToken)
    {
        await base.EnsureDataSeeded(cancellationToken);
        await SeedData(_repository);
    }

    public Func<IRepository, Task> SeedData { get; set; } = _ => Task.CompletedTask;
}


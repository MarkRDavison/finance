namespace mark.davison.finance.persistence.Context;
public partial class FinanceDbContext
{
    public async Task DoCustomThing(Guid id, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}

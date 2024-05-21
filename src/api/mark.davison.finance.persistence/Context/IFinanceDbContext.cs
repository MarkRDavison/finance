using mark.davison.common.persistence;

namespace mark.davison.finance.persistence.Context;

public interface IFinanceDbContext : IDbContext<FinanceDbContext>
{
    Task DoCustomThing(Guid id, CancellationToken cancellationToken);
}

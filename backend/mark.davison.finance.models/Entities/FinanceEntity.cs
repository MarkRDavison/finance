using mark.davison.finance.common.server.abstractions;
using mark.davison.finance.common.server.abstractions.Identification;

namespace mark.davison.finance.models.Entities;

public class FinanceEntity : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}


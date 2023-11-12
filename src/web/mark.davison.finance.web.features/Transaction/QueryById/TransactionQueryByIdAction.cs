namespace mark.davison.finance.web.features.Transaction.QueryById;

public class TransactionQueryByIdAction : IAction<TransactionQueryByIdAction>
{
    public Guid TransactionGroupId { get; set; }
}

using mark.davison.finance.models.dtos.Commands.CreateTransaction;

namespace mark.davison.finance.web.features.Store.TransactionUseCase;

public sealed class FetchTransactionsByGroup : BaseAction
{
    public Guid TransactionGroupId { get; set; }
}

public sealed class FetchTransactionsByGroupResponse : BaseActionResponse<List<TransactionDto>>
{

}

public sealed class FetchTransactionsByAccount : BaseAction
{
    public Guid AccountId { get; set; }
}

public sealed class FetchTransactionsByAccountResponse : BaseActionResponse<List<TransactionDto>>
{

}

public sealed class CreateTransactionAction : BaseAction
{
    // TODO: Rename payload
    public CreateTransactionRequest Request { get; set; } = new();
}

public sealed class CreateTransactionActionResponse : BaseActionResponse
{
    // TODO: --> Create/EditTransactionData payload
    public TransactionGroupDto Group { get; set; } = new();
    public List<TransactionJournalDto> Journals { get; set; } = new();
    public List<TransactionDto> Transactions { get; set; } = new();
}
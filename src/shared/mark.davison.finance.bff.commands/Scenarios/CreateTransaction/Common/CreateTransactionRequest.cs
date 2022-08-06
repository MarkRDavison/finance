namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common;

// https://docs.firefly-iii.org/firefly-iii/about-firefly-iii/architecture/#transactions
/*
    Withdrawal
    -   from
        -   asset account
        -   liability account

    -   to
        -   expense account
        -   (on the fly created expense account)
    

    Deposit
    -   from
        -   revenue account
        -   (on the fly created revenue account)
    -   to
        -   asset account
        -   liability account

    Transfer
    -   from
        -   any
    -   to
        -   any

*/

public class CreateTransactionRequest : ICommand<CreateTransactionRequest, CreateTransactionResponse>
{
    public Guid TransactionTypeId { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<CreateTransactionDto> Transactions { get; set; } = new();
}

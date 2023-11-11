namespace mark.davison.finance.web.components.Pages.Transactions.ViewTransaction;

public class ViewTransactionItem
{
    public required string Description { get; init; }
    public required LinkInfo SourceAccount { get; init; }
    public required LinkInfo DestinationAccount { get; init; }
    public required string Amount { get; init; }
    public required long AmountValue { get; init; }
    public required string? ForeignAmount { get; init; }
    public required LinkInfo? Category { get; init; }
    public required string AmountStyle { get; init; }
}

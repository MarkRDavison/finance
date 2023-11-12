namespace mark.davison.finance.shared.utilities.Extensions;

public static class TransactionGroupExtensions
{
    public static TransactionGroupDto ToDto(this TransactionGroup transactionGroup)
    {
        // TODO: Populate the rest
        return new TransactionGroupDto
        {
            Id = transactionGroup.Id
        };
    }
}

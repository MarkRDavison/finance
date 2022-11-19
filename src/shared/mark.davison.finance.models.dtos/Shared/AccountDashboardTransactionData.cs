namespace mark.davison.finance.models.dtos.Shared;

public class AccountDashboardTransactionData
{
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly Date { get; set; }
    public long Amount { get; set; }
}

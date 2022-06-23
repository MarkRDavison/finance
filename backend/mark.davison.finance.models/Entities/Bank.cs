namespace mark.davison.finance.models.Entities;

[Lookup(PhaseName = "Startup", Phase = 0)]
public partial class Bank : FinanceEntity
{
    public static Guid KiwibankId = new Guid("6A9BF196-405F-4E3B-ABDE-28CC737A4B73");

    public static Guid BnzId = new Guid("268AD4E9-1A69-4CAA-B3E5-E02254B310C4");

    public string Name { get; set; } = string.Empty;
}

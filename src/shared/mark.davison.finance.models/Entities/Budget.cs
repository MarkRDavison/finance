namespace mark.davison.finance.models.Entities;

public partial class Budget : FinanceEntity
{
    public bool IsActive { get; set; } = true;
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}


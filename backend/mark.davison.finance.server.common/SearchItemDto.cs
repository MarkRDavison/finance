namespace mark.davison.finance.common.server;

public class SearchItemDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string PrimaryText { get; set; } = string.Empty;
    public string SecondaryText { get; set; } = string.Empty;
}


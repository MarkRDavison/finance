namespace mark.davison.finance.models.dtos.Commands.CreateCategory;

public class CreateCategoryResponse
{
    public bool Success { get; set; }
    public List<string> Error { get; set; } = new();
    public List<string> Warning { get; set; } = new();
}

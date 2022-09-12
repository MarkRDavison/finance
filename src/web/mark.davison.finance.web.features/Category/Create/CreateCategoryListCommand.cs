namespace mark.davison.finance.web.features.Category.Create;

public class CreateCategoryListCommand : ICommand<CreateCategoryListCommand, CreateCategoryListCommandResponse>
{
    public string Name { get; set; } = string.Empty;
}

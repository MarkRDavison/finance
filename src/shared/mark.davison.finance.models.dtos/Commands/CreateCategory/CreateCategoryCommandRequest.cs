namespace mark.davison.finance.models.dtos.Commands.CreateCategory;

[PostRequest(Path = "create-category")]
public class CreateCategoryCommandRequest : ICommand<CreateCategoryCommandRequest, CreateCategoryCommandResponse>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

using mark.davison.finance.models.dtos.Commands.CreateCategory;

namespace mark.davison.finance.web.features.Category.Create;

public class CreateCategoryListCommandHandler : ICommandHandler<CreateCategoryListCommand, CreateCategoryListCommandResponse>
{
    private readonly IClientHttpRepository _repository;

    public CreateCategoryListCommandHandler(
        IClientHttpRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateCategoryListCommandResponse> Handle(CreateCategoryListCommand command, CancellationToken cancellation)
    {
        var request = new CreateCategoryRequest
        {
            Id = Guid.NewGuid(),
            Name = command.Name
        };

        var response = await _repository.Post<CreateCategoryResponse, CreateCategoryRequest>(request, cancellation);
        if (!response.Success)
        {
            return new CreateCategoryListCommandResponse { Success = false };
        }

        return new CreateCategoryListCommandResponse
        {
            ItemId = request.Id,
            Success = true
        };
    }
}

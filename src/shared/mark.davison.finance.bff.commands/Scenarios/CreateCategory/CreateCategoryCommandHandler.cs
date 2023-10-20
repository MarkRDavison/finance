namespace mark.davison.finance.bff.commands.Scenarios.CreateCategory;

public class CreateCategoryCommandHandler : ValidateAndProcessCommandHandler<CreateCategoryCommandRequest, CreateCategoryCommandResponse>
{
    public CreateCategoryCommandHandler(
        ICommandProcessor<CreateCategoryCommandRequest, CreateCategoryCommandResponse> processor,
        ICommandValidator<CreateCategoryCommandRequest, CreateCategoryCommandResponse> validator
    ) : base(processor, validator)
    {
    }
}

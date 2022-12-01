namespace mark.davison.finance.web.features.Tag.Create;

public class CreateTagListCommandHandler : ICommandHandler<CreateTagListCommandRequest, CreateTagListCommandResponse>
{
    private readonly IClientHttpRepository _repository;

    public CreateTagListCommandHandler(
        IClientHttpRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateTagListCommandResponse> Handle(CreateTagListCommandRequest command, CancellationToken cancellationToken)
    {
        var request = new CreateTagCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = command.Name
        };

        var response = await _repository.Post<CreateTagCommandResponse, CreateTagCommandRequest>(request, cancellationToken);
        if (!response.Success)
        {
            return new CreateTagListCommandResponse { Success = false };
        }

        return new CreateTagListCommandResponse
        {
            ItemId = request.Id,
            Success = true
        };
    }
}

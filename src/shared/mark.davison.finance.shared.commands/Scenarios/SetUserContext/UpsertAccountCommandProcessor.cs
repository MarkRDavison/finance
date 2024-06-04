namespace mark.davison.finance.shared.commands.Scenarios.SetUserContext;

public sealed class UpsertAccountCommandProcessor : ICommandProcessor<SetUserContextCommandRequest, SetUserContextCommandResponse>
{
    private readonly IFinanceUserContext _financeUserContext;

    public UpsertAccountCommandProcessor(IFinanceUserContext financeUserContext)
    {
        _financeUserContext = financeUserContext;
    }

    public async Task<SetUserContextCommandResponse> ProcessAsync(SetUserContextCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await _financeUserContext.SetAsync(request.UserContext.StartRange, request.UserContext.EndRange, cancellationToken);

        return new SetUserContextCommandResponse
        {
            Value = request.UserContext
        };
    }
}

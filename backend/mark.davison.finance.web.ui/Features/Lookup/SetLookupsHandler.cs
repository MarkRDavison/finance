namespace mark.davison.finance.web.ui.Features.Lookup;

public partial class LookupState
{
    public class SetLookupsHandler : ActionHandler<SetLookupsAction>
    {

        public SetLookupsHandler(IStore store) : base(store)
        {
        }

        public override Task<Unit> Handle(SetLookupsAction action, CancellationToken cancellationToken)
        {
            Store.SetState(new LookupState(
                action.Banks,
                action.AccountTypes,
                action.Currencies,
                action.TransactionTypes
            ));
            return Unit.Task;
        }
    }
}
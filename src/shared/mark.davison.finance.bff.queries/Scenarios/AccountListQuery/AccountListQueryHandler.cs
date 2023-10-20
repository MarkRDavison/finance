using mark.davison.finance.accounting.constants;
using System.Linq.Expressions;

namespace mark.davison.finance.bff.queries.Scenarios.AccountListQuery;

public class AccountListQueryHandler : IQueryHandler<AccountListQueryRequest, AccountListQueryResponse>
{
    private readonly IRepository _repository;

    public AccountListQueryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<AccountListQueryResponse> Handle(AccountListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new AccountListQueryResponse();

        var accounts = await _repository.GetEntitiesAsync<Account>(
            _ =>
                _.Id != Account.Reconciliation && // TODO: Better single place that creates expression to filter these or add property to account
                _.Id != Account.OpeningBalance,
            new Expression<Func<Account, object>>[]
            {
                _ => _.AccountType!
            },
            cancellationToken);

        var accountIds = accounts.Select(_ => _.Id).ToList();

        var openingBalances = await _repository.GetEntitiesAsync<TransactionJournal>(
            _ =>
                _.Transactions.Any(__ => accountIds.Contains(__.AccountId)) &&
                _.TransactionTypeId == TransactionConstants.OpeningBalance,
            new Expression<Func<TransactionJournal, object>>[] {
                    _ => _.Transactions!
            },
            cancellationToken);

        foreach (var account in accounts)
        {
            var openingBalanceTransactionJournal = openingBalances
                .FirstOrDefault(_ =>
                    _.Transactions.Any(__ =>
                        __.AccountId == account.Id));
            var openingBalanceTransaction = openingBalanceTransactionJournal?
                .Transactions
                .FirstOrDefault(_ => _.AccountId == account.Id);

            response.Accounts.Add(new AccountListItemDto
            {
                Id = account.Id,
                Name = account.Name,
                AccountNumber = account.AccountNumber,
                AccountType = account.AccountType!.Type,
                AccountTypeId = account.AccountTypeId,
                Active = account.IsActive,
                CurrencyId = account.CurrencyId,
                LastModified = account.LastModified,
                VirtualBalance = account.VirtualBalance,
                OpeningBalance = openingBalanceTransaction?.Amount,
                OpeningBalanceDate = openingBalanceTransactionJournal?.Date,
                BalanceDifference = 0, // TODO
                CurrentBalance = 0 // TODO
            });
        }


        return response;
    }
}


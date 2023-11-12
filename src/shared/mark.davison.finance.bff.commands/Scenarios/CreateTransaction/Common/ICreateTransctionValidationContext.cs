namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common;

public interface ICreateTransctionValidationContext
{
    Task<Account?> GetAccountById(Guid accountId, CancellationToken cancellationToken);

    Task<Category?> GetCategoryById(Guid categoryId, CancellationToken cancellationToken);

}

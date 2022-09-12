namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common.Validators;

public interface ICreateTransctionValidationContext
{
    public Task<Account?> GetAccountById(Guid accountId, CancellationToken cancellationToken);
    public Task<Category?> GetCategoryById(Guid categoryId, CancellationToken cancellationToken);


}

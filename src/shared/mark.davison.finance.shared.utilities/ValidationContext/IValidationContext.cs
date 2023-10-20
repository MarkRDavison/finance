namespace mark.davison.finance.shared.utilities.ValidationContext;

public interface IValidationContext
{
    Task<T?> GetById<T>(Guid id, CancellationToken cancellationToken) where T : BaseEntity, new();
}

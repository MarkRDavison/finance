namespace mark.davison.finance.api.services.UserApplicationContext;

public interface IUserApplicationContext
{
    Task<TContext?> LoadContext<TContext>();
    Task<TContext> LoadRequiredContext<TContext>();
    void SetContext<TContext>(TContext context);
    Task WriteContext<TContext>();
}

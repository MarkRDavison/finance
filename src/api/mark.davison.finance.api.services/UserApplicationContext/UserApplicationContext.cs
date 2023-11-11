namespace mark.davison.finance.api.services.UserApplicationContext;

public class UserApplicationContext : IUserApplicationContext
{
    private readonly IDistributedCache _distributedCache;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IDateService _dateService;
    private readonly JsonSerializerOptions _options;

    private string Key => $"U_A_C_{_currentUserContext.CurrentUser.Id}";

    private object? Context { get; set; }

    public UserApplicationContext(
        IDistributedCache distributedCache,
        ICurrentUserContext currentUserContext,
        IDateService dateService
    )
    {
        _distributedCache = distributedCache;
        _currentUserContext = currentUserContext;
        _dateService = dateService;
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<TContext?> LoadContext<TContext>()
    {
        if (Context is TContext c)
        {
            return c;
        }

        await _distributedCache.RefreshAsync(Key);

        var serialized = await _distributedCache.GetStringAsync(Key);

        var deserialized = serialized == null
            ? default
            : JsonSerializer.Deserialize<TContext?>(serialized, _options);

        if (deserialized != null)
        {
            Context = deserialized;
        }

        return deserialized;
    }

    public async Task<TContext> LoadRequiredContext<TContext>()
    {
        var context = await LoadContext<TContext>();

        if (context == null)
        {
            if (typeof(TContext) == typeof(FinanceUserApplicationContext))
            {
                var (start, end) = _dateService.Today.GetMonthRange();

                var financeContext = new FinanceUserApplicationContext
                {
                    RangeStart = start,
                    RangeEnd = end
                };

                context = (TContext)Convert.ChangeType(financeContext, typeof(TContext));

                SetContext(context);
            }
            else
            {
                throw new InvalidOperationException($"Required context of type '{typeof(TContext)}' not found.");
            }
        }

        return context;
    }

    public void SetContext<TContext>(TContext context)
    {
        Context = context;
    }

    public async Task WriteContext<TContext>()
    {
        if (Context == null)
        {
            throw new InvalidOperationException("Cannot write null context");
        }

        await _distributedCache.SetStringAsync(Key, JsonSerializer.Serialize(Context, _options));
    }
}

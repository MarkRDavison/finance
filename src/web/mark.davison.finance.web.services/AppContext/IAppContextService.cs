namespace mark.davison.finance.web.services.AppContext;

public interface IAppContextService
{
    public DateOnly RangeStart { get; set; }
    public DateOnly RangeEnd { get; set; }
}

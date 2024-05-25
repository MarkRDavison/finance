namespace mark.davison.finance.shared.utilities.Services;

public interface IFinanceUserContext
{
    DateOnly RangeStart { get; }
    DateOnly RangeEnd { get; }

    Task LoadAsync(CancellationToken cancellationToken);
    Task SetAsync(DateOnly rangeStart, DateOnly rangeEnd, CancellationToken cancellationToken);
}

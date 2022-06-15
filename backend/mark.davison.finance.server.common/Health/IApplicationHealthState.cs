namespace mark.davison.finance.common.server.Health;

public interface IApplicationHealthState
{
    bool? Started { get; set; }
    bool? Ready { get; set; }
    bool? Healthy { get; set; }
}


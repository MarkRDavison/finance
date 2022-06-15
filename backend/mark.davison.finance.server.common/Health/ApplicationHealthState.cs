﻿namespace mark.davison.finance.common.server.Health;

public class ApplicationHealthState : IApplicationHealthState
{
    public bool? Started { get; set; } = null;
    public bool? Ready { get; set; } = null;
    public bool? Healthy { get; set; } = true;
}


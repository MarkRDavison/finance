﻿namespace mark.davison.finance.bff.Configuration;


public sealed class AppSettings : IAppSettings
{
    public string SECTION => "FINANCE";

    public AuthAppSettings AUTH { get; set; } = new();
    public DatabaseAppSettings DATABASE { get; set; } = new();
    public RedisAppSettings REDIS { get; set; } = new();
    public ClaimsAppSettings CLAIMS { get; set; } = new();
    public bool PRODUCTION_MODE { get; set; }
}

﻿namespace mark.davison.finance.web.ui.tests.Configuration;

public class EnvironmentSettings
{
    public string Section => "ENVIRONMENT";

    public string WEB_ORIGIN { get; set; } = string.Empty;
    public string BFF_ORIGIN { get; set; } = string.Empty;
    public string API_ORIGIN { get; set; } = string.Empty;


    public void EnsureValid()
    {
        if (string.IsNullOrEmpty(WEB_ORIGIN))
        {
            throw new InvalidOperationException("WEB_ORIGIN must have a value");
        }
        if (string.IsNullOrEmpty(BFF_ORIGIN))
        {
            throw new InvalidOperationException("BFF_ORIGIN must have a value");
        }
        if (string.IsNullOrEmpty(API_ORIGIN))
        {
            throw new InvalidOperationException("API_ORIGIN must have a value");
        }
    }
}

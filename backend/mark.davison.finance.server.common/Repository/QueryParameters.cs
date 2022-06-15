﻿namespace mark.davison.finance.common.server.Repository;

public class QueryParameters : Dictionary<string, string>
{
    public string CreateQueryString()
    {
        string uri = string.Empty;
        if (this.Any())
        {
            uri += "?";
            uri += string.Join("&", this.Select((kv) => $"{kv.Key}={kv.Value}"));
        }
        return uri;
    }
}


﻿namespace mark.davison.finance.models.dtos.Shared;

public sealed class DashboardSummaryData
{
    public Dictionary<Guid, string> AccountNames { get; set; } = [];
    public Dictionary<Guid, List<AccountDashboardTransactionData>> TransactionData { get; set; } = [];
}

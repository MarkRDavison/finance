﻿namespace mark.davison.finance.models.dtos.Queries.TransactionByAccountQuery;

[GetRequest(Path = "transaction-by-account-query")]
public class TransactionByAccountQueryRequest : IQuery<TransactionByAccountQueryRequest, TransactionByAccountQueryResponse>
{
    public Guid AccountId { get; set; }
    // TODO: Add range begin/end and update handler to use them
}

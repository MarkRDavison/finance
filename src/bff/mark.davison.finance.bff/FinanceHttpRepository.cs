namespace mark.davison.finance.bff;

public class FinanceHttpRepository : HttpRepository
{
    public FinanceHttpRepository(string baseUri, JsonSerializerOptions options) : base(baseUri, new HttpClient(), options)
    {

    }
    public FinanceHttpRepository(string baseUri, HttpClient client, JsonSerializerOptions options) : base(baseUri, client, options)
    {

    }
}

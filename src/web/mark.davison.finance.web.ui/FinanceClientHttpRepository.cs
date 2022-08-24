namespace mark.davison.finance.web.ui;

public class FinanceClientHttpRepository : ClientHttpRepository
{
    public FinanceClientHttpRepository(
        string remoteEndpoint,
        IHttpClientFactory httpClientFactory
    ) : base(
        remoteEndpoint,
        httpClientFactory.CreateClient(WebConstants.ApiClientName))
    {
    }
}

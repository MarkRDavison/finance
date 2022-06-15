namespace mark.davison.finance.common.server.Authentication;

public interface ICurrentUserContext
{
    public User CurrentUser { get; set; }
    public string Token { get; set; }
}


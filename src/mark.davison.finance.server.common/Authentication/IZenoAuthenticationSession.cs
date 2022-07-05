namespace mark.davison.finance.common.server.Authentication;

public interface IZenoAuthenticationSession
{
    public Task LoadSessionAsync(CancellationToken cancellationToken);
    public Task CommitSessionAsync(CancellationToken cancellationToken);

    void Clear();
    string GetString(string key);
    void SetString(string key, string value);
    void Remove(string key);

}
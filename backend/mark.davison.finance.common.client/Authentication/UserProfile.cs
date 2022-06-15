namespace mark.davison.finance.common.client.Authentication;
public class UserProfile
{
    public Guid sub { get; set; }
    public bool email_verified { get; set; }
    public string? name { get; set; }
    public string? preferred_username { get; set; }
    public string? given_name { get; set; }
    public string? family_name { get; set; }
    public string? email { get; set; }
}

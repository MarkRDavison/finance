namespace mark.davison.finance.persistence.EntityDefaulter;

public class UserDefaulter : IEntityDefaulter<User>
{
    public Task DefaultAsync(User entity, User user)
    {
        return Task.CompletedTask;
    }
}

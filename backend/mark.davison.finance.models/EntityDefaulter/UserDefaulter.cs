using mark.davison.finance.persistence.EntityDefaulter;

namespace mark.davison.finance.models.EntityDefaulter;

public class UserDefaulter : IEntityDefaulter<User>
{
    public async Task DefaultAsync(User entity, User user)
    {
        await Task.CompletedTask;
    }
}

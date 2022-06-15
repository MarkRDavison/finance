namespace mark.davison.finance.common.server;

public class BaseEntity
{
    public Guid Id { get; set; }

    public DateTime Created { get; set; }
    public DateTime LastModified { get; set; }
}

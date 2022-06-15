namespace mark.davison.finance.common.server.CQRS;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class GetRequestAttribute : Attribute
{
    public string Path { get; set; } = null!;
}


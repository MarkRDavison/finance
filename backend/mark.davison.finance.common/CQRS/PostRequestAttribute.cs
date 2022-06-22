namespace mark.davison.finance.common.CQRS;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PostRequestAttribute : Attribute
{
    public string Path { get; set; } = null!;
}


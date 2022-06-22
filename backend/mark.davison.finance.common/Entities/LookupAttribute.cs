namespace mark.davison.finance.common.Entities;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class LookupAttribute : Attribute
{
    public int Phase { get; set; } = -1;
}


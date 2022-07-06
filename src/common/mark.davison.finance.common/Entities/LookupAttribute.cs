namespace mark.davison.finance.common.Entities;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class LookupAttribute : Attribute
{
    public string PhaseName { get; set; } = string.Empty;

    public int Phase { get; set; } = -1;
}


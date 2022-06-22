namespace mark.davison.finance.common.client.components.Layout;

public class AppbarItem
{
    public AppbarItem(string name, string route)
    {
        Name = name;
        Route = route;
    }
    public string Name { get; set; }
    public string Route { get; set; }
}


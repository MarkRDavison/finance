namespace mark.davison.finance.web.ui.CommonCandidates.Layout;

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
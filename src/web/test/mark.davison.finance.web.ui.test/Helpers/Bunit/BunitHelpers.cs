namespace mark.davison.finance.web.ui.test.Helpers.Bunit;

public static class BunitHelpers
{
    public static IElement? FindInputByLabel<TComponent>(this IRenderedComponent<TComponent> cut, string label)
        where TComponent : IComponent
    {
        var labelElement = cut.FindAll("label").FirstOrDefault(_ => _.TextContent == label);
        if (labelElement == null)
        {
            return null;
        }
        var htmlForValue = labelElement.GetAttribute("htmlfor");
        return cut.Find($"#{htmlForValue}");
    }

    public static void SetTextInputValueByLabel<TComponent>(this IRenderedComponent<TComponent> cut, string label, string value)
        where TComponent : IComponent
    {
        var input = cut.FindInputByLabel(label);
        Assert.IsNotNull(input);
        Assert.AreEqual("text", input.GetAttribute("type"));
        input.Change(value);
        input.Blur();
    }

    public static void SetTextInputValueById<TComponent>(this IRenderedComponent<TComponent> cut, string id, string value)
        where TComponent : IComponent
    {
        var input = cut.Find($"#{id}");
        Assert.IsNotNull(input);
        Assert.AreEqual("text", input.GetAttribute("type"));
        input.Change(value);
        input.Blur();
    }

    public static void SetDropdownValueByLabel<TComponent, TKey>(this IRenderedComponent<TComponent> cut, string label, TKey value)
        where TComponent : IComponent
    {
        var input = cut.FindInputByLabel(label);
        Assert.IsNotNull(input);
        input.Focus();
        var option = cut.FindAll(".z-dropdownlist-item").FirstOrDefault(_ => _.HasAttribute("key") && _.GetAttribute("key") == value?.ToString());
        Assert.IsNotNull(option);
        option.Click();
    }

    public static void SetDropdownValueById<TComponent, TKey>(this IRenderedComponent<TComponent> cut, string id, TKey value)
        where TComponent : IComponent
    {
        var input = cut.Find($"#{id}");
        Assert.IsNotNull(input);
        input.Focus();
        var option = cut.FindAll(".z-dropdownlist-item").FirstOrDefault(_ => _.HasAttribute("key") && _.GetAttribute("key") == value?.ToString());
        Assert.IsNotNull(option);
        option.Click();
    }

    public static void SetDateValueByLabel<TComponent>(this IRenderedComponent<TComponent> cut, string label, DateOnly value)
        where TComponent : IComponent
    {
        var input = cut.FindInputByLabel(label);
        Assert.IsNotNull(input);
        Assert.AreEqual("date", input.GetAttribute("type"));
        input.Change(value.ToString("yyyy-MM-dd"));
        input.Blur();
    }

    public static void SetDateValueById<TComponent>(this IRenderedComponent<TComponent> cut, string id, DateOnly value)
        where TComponent : IComponent
    {
        var input = cut.Find($"#{id}");
        Assert.IsNotNull(input);
        Assert.AreEqual("date", input.GetAttribute("type"));
        input.Change(value.ToString("yyyy-MM-dd"));
        input.Blur();
    }

    public static void SetCurrencyValueByLabel<TComponent>(this IRenderedComponent<TComponent> cut, string label, decimal value)
        where TComponent : IComponent
    {
        var input = cut.FindInputByLabel(label);
        Assert.IsNotNull(input);
        Assert.AreEqual("text", input.GetAttribute("type"));
        input.Change(value.ToString());
        input.Blur();
    }

    public static void SetCurrencyValueById<TComponent>(this IRenderedComponent<TComponent> cut, string id, decimal value)
        where TComponent : IComponent
    {
        var input = cut.Find($"#{id}");
        Assert.IsNotNull(input);
        Assert.AreEqual("text", input.GetAttribute("type"));
        input.Change(value.ToString());
        input.Blur();
    }
}

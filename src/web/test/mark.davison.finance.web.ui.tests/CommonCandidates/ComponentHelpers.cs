namespace mark.davison.finance.web.ui.tests.CommonCandidates;
public static class ComponentHelpers
{
    public static async Task SetAutoComplete(IPage page, string label, string value)
    {
        var parentFandomAutocomplete = page.GetByLabel(label);

        await parentFandomAutocomplete.ClickAsync();
        await parentFandomAutocomplete.PressSequentiallyAsync(value);

        var popupOptions = await page.Locator(".mud-popover-open p").AllAsync();

        foreach (var option in popupOptions)
        {
            var text = await option.TextContentAsync();

            if (text == value)
            {
                await option.ClickAsync();
            }
        }
    }

    public static async Task SelectAsync(IPage page, string label, string value)
    {
        var control = page.GetByLabel(label);

        await control.ClickAsync();

        var popupOptions = await page.Locator(".mud-popover-open p").AllAsync();

        foreach (var option in popupOptions)
        {
            var text = await option.TextContentAsync();

            if (text == value)
            {
                await option.ClickAsync();
                return;
            }
        }

        Assert.Fail("Could not find '{0}' for '{1}' control.", value, label);
    }
}

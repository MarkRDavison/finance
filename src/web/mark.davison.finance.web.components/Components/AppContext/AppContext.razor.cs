﻿namespace mark.davison.finance.web.components.Components.AppContext;

public partial class AppContext
{
    [Inject]
    public required IAppContextService AppContextService { get; set; }
}

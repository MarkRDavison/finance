﻿namespace mark.davison.finance.web.components.CommonCandidates.Components.Dropdown;

public class DropdownItem : IDropdownItem
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }
}

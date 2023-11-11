﻿namespace mark.davison.finance.web.components.CommonCandidates.Utilities;

public static class DateUtilities
{
    public static string ToOrdinalShortDate(this DateOnly date)
    {
        return string.Format(date.ToString("MMMM d{0}, yyyy"), date.Day.ToOrdinal());
    }
    public static string ToOrdinalMonthDay(this DateOnly date)
    {
        return string.Format(date.ToString("MMMM d{0}"), date.Day.ToOrdinal());
    }
}

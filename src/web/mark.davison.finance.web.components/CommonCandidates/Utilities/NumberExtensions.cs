﻿namespace mark.davison.finance.web.components.CommonCandidates.Utilities;

public static class NumberExtensions
{
    public static string ToOrdinal(this int number)
    {
        var ones = number % 10;
        var tens = (int)(Math.Floor((decimal)number / 10)) % 10;

        if (tens == 1)
        {
            return "th";
        }
        else
        {
            switch (ones)
            {
                case 1: return "st";
                case 2: return "nd";
                case 3: return "rd";
                default: return "th";
            }
        }

    }
}

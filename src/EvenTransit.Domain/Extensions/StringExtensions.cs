using System;

namespace EvenTransit.Domain.Extensions;

public static class StringExtensions
{
    public static DateTime? ConvertToDateTime(this string dateTime)
    {
        var isSuccess = DateTime.TryParse(dateTime, out var convertedDateTime);
        if (isSuccess) return convertedDateTime;

        return null;
    }
}

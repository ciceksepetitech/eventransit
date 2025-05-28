using System.Globalization;

namespace EvenTransit.Domain.Extensions;

public static class StringExtensions
{
    public static string ConvertToLocalDateString(this DateTime dateTime)
    {
        return dateTime.ToString("dd-MM-yyyy HH:mm:ss");
    }

    public static string ConvertToLocalDateString(this DateTime? dateTime)
    {
        return !dateTime.HasValue ? string.Empty : ConvertToLocalDateString(dateTime.Value);
    }

    public static bool TryConvertToDate(this string data, out DateTime dateTime)
    {
        var value = DateTime.TryParseExact(data, "dd-MM-yyyy HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal, out dateTime);
        dateTime = dateTime.ToUniversalTime();
        return value;
    }

    public static DateTime ConvertToDate(this string data)
    {
        var value = DateTime.ParseExact(data, "dd-MM-yyyy HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
        return value;
    }
}

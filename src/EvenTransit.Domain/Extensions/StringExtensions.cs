using System.Globalization;

namespace EvenTransit.Domain.Extensions;

public static class StringExtensions
{
    public static string ConvertToLocalDateString(this DateTime dateTime)
    {
        return dateTime.ToString("dd-MM-yyyy HH:mm");
    }

    public static bool TryConvertToDate(this string data, out DateTime dateTime)
    {
        var value = DateTime.TryParseExact(data, "dd-MM-yyyy HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal, out dateTime);
        return value;
    }

    public static DateTime ConvertToDate(this string data)
    {
        var value = DateTime.ParseExact(data, "dd-MM-yyyy HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal);
        return value;
    }
}

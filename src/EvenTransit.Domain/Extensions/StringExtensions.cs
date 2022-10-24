
namespace EvenTransit.Domain.Extensions;

public static class StringExtensions
{
    public static DateTime ConvertToStartDate(this string dateTime)
    {
        var isSuccess = DateTime.TryParse(dateTime, out var convertedDateTime);
        return isSuccess ? convertedDateTime : DateTime.UtcNow.Date;
    }

    public static DateTime ConvertToEndDate(this string dateTime)
    {
        return ConvertToStartDate(dateTime).AddHours(23).AddMinutes(59);
    }

    public static string ConvertToLocalDateString(this DateTime dateTime)
    {
        return dateTime.AddHours(3).ToString("dd-MM-yyyy HH:mm");
    }
}

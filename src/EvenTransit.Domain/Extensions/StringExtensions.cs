
namespace EvenTransit.Domain.Extensions;

public static class StringExtensions
{
    public static string ConvertToLocalDateString(this DateTime dateTime)
    {
        return dateTime.AddHours(3).ToString("dd-MM-yyyy HH:mm");
    }
}

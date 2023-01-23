
namespace EvenTransit.Domain.Extensions;

public static class StringExtensions
{
    public static string ConvertToLocalDateString(this DateTime dateTime)
    {
        return dateTime.ToString("dd-MM-yyyy HH:mm");
    }
}

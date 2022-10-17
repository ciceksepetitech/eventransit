using System.Text.Json;
using System.Text.RegularExpressions;

namespace EvenTransit.Messaging.Core;

public static class Extensions
{
    private static readonly Regex FieldNameRegex = new("{{([a-zA-Z0-9-]+)}}", RegexOptions.Compiled);

    public static string ReplaceDynamicFieldValues(this string value, Dictionary<string, string> fields)
    {
        fields ??= new Dictionary<string, string>();

        var valueFields = FieldNameRegex.Matches(value);
        var newValue = value;

        foreach (Match field in valueFields)
        {
            var pattern = field.Groups[0].Value;
            var fieldName = field.Groups[1].Value;
            var newFieldValue = string.Empty;

            if (fields.ContainsKey(fieldName))
                newFieldValue = fields[fieldName];

            newValue = newValue.Replace(pattern, newFieldValue);
        }

        return newValue;
    }

    public static string Serialize(this object obj)
    {
        return obj == null ? string.Empty : JsonSerializer.Serialize(obj);
    }
}

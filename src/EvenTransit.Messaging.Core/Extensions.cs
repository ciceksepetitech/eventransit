using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EvenTransit.Messaging.Core;

public static class Extensions
{
    private const string FieldNameRegex = "{{([a-zA-Z0-9]+)}}";

    public static string ReplaceDynamicFieldValues(this string value, Dictionary<string, string> fields)
    {
        var fieldRegex = new Regex(FieldNameRegex);
        var valueFields = fieldRegex.Matches(value);
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
}

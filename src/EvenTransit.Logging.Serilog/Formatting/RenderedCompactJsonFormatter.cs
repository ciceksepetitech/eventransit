using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

namespace EvenTransit.Logging.Serilog.Formatting;

public class RenderedCompactJsonFormatter : ITextFormatter
{
    private readonly JsonValueFormatter _valueFormatter;

    public RenderedCompactJsonFormatter(JsonValueFormatter valueFormatter = null)
    {
        _valueFormatter = valueFormatter ?? new JsonValueFormatter("$type");
    }

    public void Format(LogEvent logEvent, TextWriter output)
    {
        FormatEvent(logEvent, output, _valueFormatter);
        output.WriteLine();
    }

    public static void FormatEvent(LogEvent logEvent, TextWriter output, JsonValueFormatter valueFormatter)
    {
        if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
        if (output == null) throw new ArgumentNullException(nameof(output));
        if (valueFormatter == null) throw new ArgumentNullException(nameof(valueFormatter));

        output.Write("{\"@t\":\"");
        output.Write(logEvent.Timestamp.UtcDateTime.ToString("O"));
        output.Write("\",\"@m\":");
        var message = logEvent.MessageTemplate.Render(logEvent.Properties);
        JsonValueFormatter.WriteQuotedJsonString(message, output);
        output.Write(",\"@i\":\"");
        var id = EventIdHash.Compute(logEvent.MessageTemplate.Text);
        output.Write(id.ToString("x8"));
        output.Write('"');

        output.Write(",\"@l\":\"");
        output.Write(logEvent.Level);
        output.Write('\"');

        if (logEvent.Exception != null)
        {
            output.Write(",\"@x\":");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), output);
        }

        foreach (var property in logEvent.Properties)
        {
            var name = property.Key;
            if (name.Length > 0 && name[0] == '@')
                // Escape first '@' by doubling
                name = '@' + name;

            output.Write(',');
            JsonValueFormatter.WriteQuotedJsonString(name, output);
            output.Write(':');
            valueFormatter.Format(property.Value, output);
        }

        output.Write('}');
    }
}

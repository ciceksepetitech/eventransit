using Serilog;
using Serilog.Events;
using Serilog.Formatting;

namespace EvenTransit.Logging.Serilog.Sanitazing;

internal class SanitizerFormatter : ITextFormatter
{
    private readonly ITextFormatter _mainFormatter;
    private readonly IEnumerable<ISanitizingFormatRule> _sanitizingFormatRules;

    public SanitizerFormatter(IEnumerable<ISanitizingFormatRule> sanitizingFormatRules,
        ITextFormatter mainFormatter)
    {
        _sanitizingFormatRules = sanitizingFormatRules;
        _mainFormatter = mainFormatter;
    }


    public void Format(LogEvent logEvent, TextWriter output)
    {
        var stringWriter = new StringWriter();

        _mainFormatter.Format(logEvent, stringWriter);

        var sanitizedContent = stringWriter.ToString();

        foreach (var sanitizingFormatRule in _sanitizingFormatRules)
            sanitizedContent = sanitizingFormatRule.Sanitize(sanitizedContent);

        try
        {
            output.Write(sanitizedContent);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Serilog Sanitizer Failed");
        }
    }
}

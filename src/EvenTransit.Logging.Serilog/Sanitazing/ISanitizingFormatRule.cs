namespace EvenTransit.Logging.Serilog.Sanitazing;

internal interface ISanitizingFormatRule
{
    string Sanitize(string input);
}

using System.Text.RegularExpressions;

namespace EvenTransit.Logging.Serilog.Sanitazing;

internal class EmailSanitizingFormatRule : RegexMaskingOperator
{
    private const string EmailPattern = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";

    public EmailSanitizingFormatRule() : base(EmailPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled)
    {
    }

    protected override string PreprocessInput(string input)
    {
        if (input.Contains("%40")) input = input.Replace("%40", "@");
        return input;
    }

    protected override bool ShouldMaskInput(string input)
    {
        return input.Contains('@');
    }
}

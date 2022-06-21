using System.Text.RegularExpressions;

namespace EvenTransit.Logging.Serilog.Sanitazing;

internal class PasswordSanitizingFormatRule : RegexMaskingOperator
{
    private const string PasswordRegex =
        @"(""|\\""|\\\\""|)(\b\w*(password|pass|pwd)\w*\b)(""|\\""|\\\\""|):(\s|)(""|\\""|\\\\"")(.*?)("",|,|"")";

    public PasswordSanitizingFormatRule() : base(PasswordRegex, "", RegexOptions.IgnoreCase | RegexOptions.Compiled)
    {
    }
}

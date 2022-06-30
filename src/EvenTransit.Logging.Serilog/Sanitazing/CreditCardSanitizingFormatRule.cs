using System.Text.RegularExpressions;

namespace EvenTransit.Logging.Serilog.Sanitazing;

internal class CreditCardSanitizingFormatRule : RegexMaskingOperator
{
    private const string CreditCardPartialReplacePattern =
        @"(?<leading4>\d{4}(?<sep>[ -]?))(?<toMask>\d{4}\k<sep>*\d{2})(?<trailing6>\d{2}\k<sep>*\d{4})";

    private const string CreditCardFullReplacePattern =
        @"(?<toMask>\d{4}(?<sep>[ -]?)\d{4}\k<sep>*\d{4}\k<sep>*\d{4})";

    private readonly string _replacementPattern;

    public CreditCardSanitizingFormatRule() : this(true)
    {
    }

    public CreditCardSanitizingFormatRule(bool fullMask)
        : base(fullMask ? CreditCardFullReplacePattern : CreditCardPartialReplacePattern,
            RegexOptions.IgnoreCase | RegexOptions.Compiled)
    {
        _replacementPattern = fullMask ? "{0}" : "${{leading4}}{0}${{trailing6}}";
    }

    protected override string PreprocessMask(string mask)
    {
        return string.Format(_replacementPattern, mask);
    }
}

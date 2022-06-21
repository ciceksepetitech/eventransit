using System.Text.RegularExpressions;

namespace EvenTransit.Logging.Serilog.Sanitazing;

internal class RegexMaskingOperator : ISanitizingFormatRule
{
    private readonly string _maskFormat = "***MASKED***";
    private readonly Regex _regex;

    protected RegexMaskingOperator(string regexString) : this(regexString, RegexOptions.Compiled)
    {
    }

    protected RegexMaskingOperator(string regexString, string maskFormat) : this(regexString, maskFormat,
        RegexOptions.Compiled)
    {
    }

    protected RegexMaskingOperator(string regexString, RegexOptions options)
    {
        _regex = new Regex(regexString ?? throw new ArgumentNullException(nameof(regexString)), options);

        if (string.IsNullOrWhiteSpace(regexString))
            throw new ArgumentOutOfRangeException(nameof(regexString),
                "Regex pattern cannot be empty or whitespace.");
    }

    protected RegexMaskingOperator(string regexString, string maskFormat, RegexOptions options)
    {
        _regex = new Regex(regexString ?? throw new ArgumentNullException(nameof(regexString)), options);

        if (string.IsNullOrWhiteSpace(regexString))
            throw new ArgumentOutOfRangeException(nameof(regexString),
                "Regex pattern cannot be empty or whitespace.");

        _maskFormat = maskFormat ??
                      throw new ArgumentNullException(nameof(maskFormat), "Mask format pattern cannot be null");
    }

    public string Sanitize(string input)
    {
        var preprocessedInput = PreprocessInput(input);

        if (!ShouldMaskInput(preprocessedInput)) return input;

        var maskedResult = _regex.Replace(preprocessedInput, PreprocessMask(_maskFormat));

        return maskedResult;
    }

    protected virtual bool ShouldMaskInput(string input)
    {
        return true;
    }

    protected virtual string PreprocessInput(string input)
    {
        return input;
    }

    protected virtual string PreprocessMask(string mask)
    {
        return mask;
    }
}

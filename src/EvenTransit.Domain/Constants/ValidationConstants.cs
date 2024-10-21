namespace EvenTransit.Domain.Constants;

public static class ValidationConstants
{
    public const string IsRequired = "{0} is required";
    public const string EventNameRegexError = "Event name should contain [a-z A-z -]";
    public const string ServiceNameRegexError = "Service name should contain [a-z A-z -]";
    public const string InvalidLogDateFrom = "Invalid LogDateFrom";
    public const string InvalidLogDateTo = "Invalid LogDateTo";
    public const string TimeoutMustBeGreaterThanZero = "Timeout must be greater than or equal to zero";
    public const string DelaySecondsMustBeGreaterThanZero = "Delay Seconds must be greater than or equal to zero";
    public const string MethodCannotBeNotEmpty = "Method cannot be empty";
    public const string InvalidMethod = "Invalid Method";
}

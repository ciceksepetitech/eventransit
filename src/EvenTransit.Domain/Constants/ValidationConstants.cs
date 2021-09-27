namespace EvenTransit.Domain.Constants
{
    public static class ValidationConstants
    {
        public const string IsRequired = "{0} is required";
        public const string EventNameRegexError = "Event name should contain [a-z A-z -]";
        public const string ServiceNameRegexError = "Service name should contain [a-z A-z -]";
    }
}
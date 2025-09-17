namespace OrderManagementAPI.Exceptions;

public class ValidationException : BaseException
{
    public List<string> Errors { get; } = new();

    public ValidationException(List<string> errors)
        : base("Validation failed")
    {
        Errors = errors;
    }

    public ValidationException(string message)
        : base(message)
    {
    }
}
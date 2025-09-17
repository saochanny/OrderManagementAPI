namespace OrderManagementAPI.Exceptions;

public class SimpleTryException : Exception
{
    // Message only
    public SimpleTryException(string message)
        : base(message)
    {
    }

    // Inner exception only
    public SimpleTryException(Exception cause)
        : base(cause.Message, cause) // sets InnerException
    {
    }

    // Message + inner exception
    public SimpleTryException(string message, Exception cause)
        : base(message, cause)
    {
    }
}
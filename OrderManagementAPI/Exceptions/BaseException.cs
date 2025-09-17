namespace OrderManagementAPI.Exceptions;

public class BaseException : Exception
{
    // Default constructor
    public BaseException() 
        : base() 
    { }

    // Message only
    public BaseException(string message) 
        : base(message) 
    { }
    

    // Message + inner exception
    public BaseException(string message, Exception cause) 
        : base(message, cause) 
    { }

    // Inner exception only
    public BaseException(Exception cause) 
        : base(cause.Message, cause) 
    { }

    // Advanced constructor (like enableSuppression & writableStackTrace in Java)
    // In C#, we can use serialization info for custom behaviors
    [Obsolete("Obsolete")]
    protected BaseException(
        System.Runtime.Serialization.SerializationInfo info, 
        System.Runtime.Serialization.StreamingContext context)
        : base(info, context) 
    { }
}
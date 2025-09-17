namespace OrderManagementAPI.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; private set; }
    public string OriginErrorCode { get; private set; } = string.Empty;

    public AppException()
    {
    }

    public AppException(string message) : base(message)
    {
    }

    public AppException(int errorCode, string message) : base(message)
    {
        StatusCode = errorCode;
    }

    public AppException(int errorCode, string originErrorCode, string message) : base(message)
    {
        StatusCode = errorCode;
        OriginErrorCode = originErrorCode;
    }

    public AppException(int errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = errorCode;
    }
}
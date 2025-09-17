namespace OrderManagementAPI.Exceptions;

public class ForbiddenException(string message) : UnauthorizedAccessException(message);
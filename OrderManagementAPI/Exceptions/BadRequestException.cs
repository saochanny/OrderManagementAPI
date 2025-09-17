namespace OrderManagementAPI.Exceptions;

public class BadRequestException(string message) : BaseException(message);
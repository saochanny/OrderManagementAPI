namespace OrderManagementAPI.Exceptions;

public class ResourceNotFoundException : ApiException
{
    // Resource name + ID
    public ResourceNotFoundException(string resourceName, long id)
        : base(StatusCodes.Status404NotFound, $"{resourceName} with id {id} is not found")
    {
    }

    // Resource name + UUID
    public ResourceNotFoundException(string resourceName, string uuid)
        : base(StatusCodes.Status404NotFound, $"{resourceName} with uuid {uuid} is not found")
    {
    }

    // Message + list of IDs
    public ResourceNotFoundException(string message, List<long> idList)
        : base(StatusCodes.Status404NotFound, $"{message} is not found with ids {string.Join(", ", idList)}")
    {
    }

    // Only message
    public ResourceNotFoundException(string message)
        : base(StatusCodes.Status404NotFound, message)
    {
    }

    // Message + inner exception
    public ResourceNotFoundException(string message, Exception cause)
        : base(StatusCodes.Status404NotFound, $"{message} is not found")
    {
    }
}
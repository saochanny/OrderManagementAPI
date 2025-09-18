namespace OrderManagementAPI.Security;

public interface IPasswordEncoder
{
    string Encode(string rawPassword);
    bool Verify(string rawPassword, string encodedPassword);
}
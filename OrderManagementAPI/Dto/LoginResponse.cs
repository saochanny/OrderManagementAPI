using System;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderManagementAPI.Dto
{
    public class LoginResponse
    {
        //[SwaggerSchema("JWT token used for authentication in subsequent requests")]
        public string Token { get; set; } = string.Empty;

        //[SwaggerSchema("Expiration date and time of the JWT token in UTC")]
        public DateTime Expiration { get; set; }

        //[SwaggerSchema("Type of the token, usually 'Bearer'")]
        public string TokenType { get; set; } = "Bearer";

        //[SwaggerSchema("Username of the logged-in user")]
        public string Username { get; set; } = string.Empty;

        //[SwaggerSchema("Comma-separated roles assigned to the user")]
        public List<string> Roles { get; set; } 

        //[SwaggerSchema("Email address of the logged-in user")]
        public string Email { get; set; } = string.Empty;

        //[SwaggerSchema("Full name of the logged-in user")]
        public string FullName { get; set; } = string.Empty;

        //[SwaggerSchema("Unique identifier of the logged-in user")]
        public int Id { get; set; }
    }
}
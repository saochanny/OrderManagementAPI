using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Utilizes;

public static class JwtTokenUtil
{
    public const string TokenType = "Bearer";
    public const string UserIdClaim = "userId";

    public static (string token, DateTime expiration) GenerateJwtToken(User user, IConfiguration config)
    {
        var jwtSettings = config.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(UserIdClaim, user.Id.ToString()),
        };


        // Get list of role names
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        // Add a claim for each role
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


        var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"]));
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, expiration);
    }
}
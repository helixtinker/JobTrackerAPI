using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JobTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// Password must match the SHA-256 hash stored in Auth:PasswordHash config.
    /// To generate a hash: SHA256(UTF8(password)) -> Base64
    /// PowerShell: [Convert]::ToBase64String([System.Security.Cryptography.SHA256]::Create().ComputeHash([System.Text.Encoding]::UTF8.GetBytes("yourpassword")))
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var expectedUsername = _configuration["Auth:Username"];
        var expectedPasswordHash = _configuration["Auth:PasswordHash"];

        var incomingHash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(request.Password)));

        if (request.Username != expectedUsername || incomingHash != expectedPasswordHash)
            return Unauthorized();

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(
            _configuration.GetValue<int>("Jwt:ExpirationHours", 8));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: [new Claim(ClaimTypes.Name, request.Username!)],
            expires: expiration,
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration
        });
    }
}

public record LoginRequest(string Username, string Password);

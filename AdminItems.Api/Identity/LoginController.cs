using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AdminItems.Api.Identity;

public record Request(string User, string Password);

[ApiController]
[Route("auth")]
public class LoginController : ControllerBase
{
    private readonly UsersStore _usersStore;
    private readonly PasswordHasher _hasher;
    private readonly IConfiguration _configuration;

    public LoginController(UsersStore usersStore, PasswordHasher hasher, IConfiguration configuration)
    {
        _usersStore = usersStore;
        _hasher = hasher;
        _configuration = configuration;
    }
    
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Request request)
    {
        var user = await _usersStore.Find(request.User);
        if (user is null)
        {
            return Unauthorized();
        }

        if (!_hasher.VerifyPassword(request.Password, user.Hash))
        {
            return Unauthorized();
        }

        var token = GenerateToken(user);
        
        return Ok(new
        {
            token = token,
        });
    }

    private string GenerateToken(UserEntity user)
    {
        var identity = _configuration.GetSection("identity").Get<IdentityConfig>();
        if (identity is null)
        {
            throw new ArgumentNullException(nameof(identity));
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(identity.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name,user.User),
            new(ClaimTypes.NameIdentifier,user.User),
            new(ClaimTypes.Email, user.User),
        };
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            identity.Issuer, 
            identity.Audience, 
            claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private class IdentityConfig
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
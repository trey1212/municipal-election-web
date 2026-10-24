using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using municipal_election_web.DTOs;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(
            loginDto.Email);

        if (user == null)
        {
            return Unauthorized();
        }

        var passwordIsCorrect =
            await _userManager.CheckPasswordAsync(
                user,
                loginDto.Password);

        if (!passwordIsCorrect)
        {
            return Unauthorized();
        }

        var userClaims = await _userManager.GetClaimsAsync(user);

        var name = userClaims
            .FirstOrDefault(claim => claim.Type == "fullName")
            ?.Value ?? "";

        var authClaims = new List<Claim>
        {
            new Claim(
                ClaimTypes.Name,
                user.UserName ?? ""),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()),

            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id)
        };

        var secretKey = _configuration["Jwt:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            return StatusCode(
                500,
                "Authentication is not configured.");
        }

        var authSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.UtcNow.AddDays(7),
            claims: authClaims,
            signingCredentials: new SigningCredentials(
                authSigningKey,
                SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler()
                .WriteToken(token),
            expiration = token.ValidTo,
            name = name,
            email = user.Email
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDto request)
    {
        var name = request.Name.Trim();
        var email = request.Email.Trim();

        if (name.Length < 2)
        {
            return BadRequest(
                "Name must contain at least 2 characters.");
        }

        var existingUser =
            await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            return Conflict(
                "An account with this email already exists.");
        }

        var user = new IdentityUser
        {
            Email = email,
            UserName = email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                " ",
                result.Errors.Select(error => error.Description));

            return BadRequest(errors);
        }

        var nameResult = await _userManager.AddClaimAsync(
            user,
            new Claim("fullName", name));

        if (!nameResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return StatusCode(
                500,
                "Could not save the account name.");
        }

        return Ok(new
        {
            message = "Account created successfully."
        });
    }
}

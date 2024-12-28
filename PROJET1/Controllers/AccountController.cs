using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using projet1.DTO;
using projet1.models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;

namespace projet1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, IConfiguration configuration, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewUser([FromBody] dtoNewUser user)
        {
            _logger.LogInformation("Received registration request for user: {Username}", user.UserName);

            if (ModelState.IsValid)
            {
                if (user.Password != user.ConfirmPassword)
                {
                    _logger.LogWarning("Passwords do not match for user: {Username}", user.UserName);
                    return BadRequest(new { Errors = new List<string> { "Passwords do not match" } });
                }

                var appUser = new AppUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                };

                var result = await _userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User registered successfully: {Username}", user.UserName);
                    return Ok(new { Message = "Registration successful" });
                }

                _logger.LogError("User registration failed for user: {Username}. Errors: {Errors}", user.UserName, result.Errors.Select(e => e.Description));
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }

            _logger.LogError("Invalid model state for user: {Username}. Errors: {Errors}", user.UserName, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return BadRequest(new { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] dtoLogin login)
        {
            _logger.LogInformation("Received login request for user: {Username}", login.UserName);

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(login.UserName);
                if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    var roles = await _userManager.GetRolesAsync(user);
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:Issuer"],
                        audience: _configuration["JWT:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddHours(1),
                        signingCredentials: credentials
                    );

                    _logger.LogInformation("User logged in successfully: {Username}", login.UserName);
                    return Ok(new
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Expiration = token.ValidTo
                    });
                }

                _logger.LogWarning("Invalid login attempt for user: {Username}", login.UserName);
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            _logger.LogError("Invalid model state for user: {Username}. Errors: {Errors}", login.UserName, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return BadRequest(new { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }
    }
}

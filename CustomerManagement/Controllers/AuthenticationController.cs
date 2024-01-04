using CustomerManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CustomerManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly CustomerContext _dbContext;
        private  IConfiguration _configuartion;

        public AuthenticationController(CustomerContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuartion = configuration;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(User user)
        {
            // Validate user input
            if (user == null || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest("Invalid user information.");
            }

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok("Registration successful");
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] UserCredentials userCredentials)
        {
            // Validate user input
            if (userCredentials == null || string.IsNullOrWhiteSpace(userCredentials.UserName) || string.IsNullOrWhiteSpace(userCredentials.Password))
            {
                return BadRequest("Invalid credentials.");
            }

            // Your authentication logic
            var authenticatedUser = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == userCredentials.UserName && u.Password == userCredentials.Password);

            if (authenticatedUser == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = GenerateJwtToken(authenticatedUser);

            // Return the token in the response
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuartion["JWT:Key"]));
            var credentials = new SigningCredentials (securitykey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_configuartion["Jwt:Issuer"], _configuartion["Jwt:Audience"], null, expires: DateTime.UtcNow.AddHours(8), signingCredentials: credentials);
            //{
            //    Subject = new ClaimsIdentity(new[]
            //    {
            //        new Claim(ClaimTypes.Name, user.Username),
            //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            //    }),
            //    Expires = DateTime.UtcNow.AddHours(8), // Token expiration time
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            //};

            //var token = tokenHandler.CreateToken(tokenDescriptor);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
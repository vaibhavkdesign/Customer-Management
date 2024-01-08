using cma_xds_soap.Models;
using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CustomerManagement.Services
{
    public class AuthenticationService: IAuthenticationService
    {
        private IConfiguration _configuartion;
        private readonly CustomerContext _dbContext;
        public AuthenticationService(IConfiguration configuration, CustomerContext dbContext)
        {
            _configuartion = configuration;
            _dbContext = dbContext;
        }
        public async Task RegisterUser(User user)
        {          
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> AuthenticateUser(UserCredentials userCredentials)
        {
            var authenticatedUser = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Username == userCredentials.UserName && u.Password == userCredentials.Password);

            if (authenticatedUser == null)
            {
                return ("Invalid username or password.");
            }

            var token = GenerateJwtToken(authenticatedUser);

            return token;
        }
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuartion["JWT:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuartion["Jwt:Issuer"],
                _configuartion["Jwt:Audience"],
                null,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

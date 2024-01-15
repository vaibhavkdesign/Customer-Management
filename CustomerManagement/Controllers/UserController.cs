using cma_xds_soap.Models;
using CustomerManagement.Models;
using CustomerManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace CustomerManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CustomerContext _dbContext;
        private readonly IAuthenticationService _authenticationService;
        private  IConfiguration _configuartion;

        public UserController(CustomerContext dbContext, IConfiguration configuration, IAuthenticationService authenticationService)
        {
            _dbContext = dbContext;
            _configuartion = configuration;
            _authenticationService = authenticationService;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(User user)
        {            
            if (user == null || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest("Invalid user information.");
            }
            await _authenticationService.RegisterUser(user);
            return Ok("Registration successful");
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] UserCredentials userCredentials)
        {           
            if (userCredentials == null || string.IsNullOrWhiteSpace(userCredentials.UserName) || string.IsNullOrWhiteSpace(userCredentials.Password))
            {
                return BadRequest("Invalid credentials.");
            }                         
            var token = await _authenticationService.AuthenticateUser(userCredentials);
            var userToken = new UserToken { Token = token };
            return Ok(userToken);

        }
    }       
}

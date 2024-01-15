using System.Linq.Expressions;
using CustomerManagement.Controllers;
using CustomerManagement.Models;
using CustomerManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using cma_xds_soap.Models;

namespace CustomerManagementTest
{
    public class UserControllerTest
    {
        [Fact]
        public async Task Register_ValidUser_SuccessfulRegistration()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CustomerContext>().UseInMemoryDatabase
                (databaseName: "InMemoryCustomerDatabase").Options;
            using var dbContext = new CustomerContext(options);
            var configuration = new Mock<IConfiguration>();
            var authenticationService = new Mock<IAuthenticationService>();
            var userController = new UserController(dbContext, configuration.Object, authenticationService.Object);

            var user = new User { Username = "testuser", Password = "password123" };


            // Act
            var result = await userController.Register(user);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Authenticate_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CustomerContext>().UseInMemoryDatabase
                    (databaseName: "InMemoryCustomerDatabase").Options;
            using var dbContext = new CustomerContext(options);
            var configuration = new Mock<IConfiguration>();
            var authenticationService = new Mock<IAuthenticationService>();
            var userController = new UserController(dbContext, configuration.Object, authenticationService.Object);

            var userCredentials = new UserCredentials { UserName = "testuser", Password = "password123" };
            var token = "fake-token";

            authenticationService.Setup(auth => auth.AuthenticateUser(userCredentials)).ReturnsAsync(token);

            // Act
            var result = await userController.Authenticate(userCredentials);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tokenResponse = Assert.IsType<UserToken>(okResult.Value);
            var actualToken = tokenResponse.Token;

            Assert.Equal(token, actualToken);
        }
        
    }
}

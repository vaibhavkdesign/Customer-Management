using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Resource;
using CustomerManagement.Controllers;
using CustomerManagement.Models;
using CustomerManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CustomerManagementTest
{
    public class CustomerControllerTest
    {
        [Fact]
        public async Task GetCustomers_ReturnsCustomers()
        {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();
            mockCustomerService.Setup(service => service.GetCustomersAsync())
                              .ReturnsAsync(new List<Customer> { new Customer(), new Customer() });

            var controller = new CustomerController(mockCustomerService.Object);

            // Act
            var result = await controller.GetCustomers();

            // Assert
            var actionResult = Xunit.Assert.IsType<ActionResult<IEnumerable<Customer>>>(result);
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult.Result);
            var customers = Xunit.Assert.IsType<List<Customer>>(okResult.Value);
            Xunit.Assert.Equal(2, customers.Count);
        }

        [Fact]
        public async Task GetCustomerById_ExistingId_ReturnsCustomer()
        {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();
            mockCustomerService.Setup(service => service.GetCustomerByIdAsync(It.IsAny<int>()))
                              .ReturnsAsync(new Customer { Id = 1, FirstName = "TestCustomer"});

            var controller = new CustomerController(mockCustomerService.Object);

            // Act
            var result = await controller.GetCustomerById(1);

            // Assert
            var actionResult = Xunit.Assert.IsType<ActionResult<Customer>>(result);
            var okResult = Xunit.Assert.IsType<OkObjectResult>(actionResult.Result);
            var customer = Xunit.Assert.IsType<Customer>(okResult.Value);
            Xunit.Assert.Equal(1, customer.Id);
            Xunit.Assert.Equal("TestCustomer", customer.FirstName);
        }

        [Fact]
        public async Task CreateCustomer_ValidCustomer_ReturnsCreatedResponse()
        {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();
            var controller = new CustomerController(mockCustomerService.Object);

            var newCustomer = new Customer { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Address = "123 Main St", BirthDate = DateTime.Now };
            mockCustomerService.Setup(service => service.CreateCustomerAsync(newCustomer))
                    .ReturnsAsync(newCustomer);

            // Act
            var result = await controller.CreateCustomer(newCustomer);

            // Assert
            var actionResult = Xunit.Assert.IsType<ActionResult<Customer>>(result);
            var createdAtActionResult = Xunit.Assert.IsType<CreatedAtActionResult>(actionResult.Result);         

            var createdCustomer = Xunit.Assert.IsType<Customer>(createdAtActionResult.Value);
            Xunit.Assert.Equal(newCustomer.FirstName, createdCustomer.FirstName);
            Xunit.Assert.Equal(newCustomer.LastName, createdCustomer.LastName);
            Xunit.Assert.Equal(newCustomer.Email, createdCustomer.Email);
            Xunit.Assert.Equal(newCustomer.Address, createdCustomer.Address);
            Xunit.Assert.Equal(newCustomer.BirthDate, createdCustomer.BirthDate);

        }

        [Fact]
        public async Task UpdateCustomer_ValidIdAndCustomer_ReturnsOkResponse()
        {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();
            var controller = new CustomerController(mockCustomerService.Object);

            var existingCustomerId = 1;
            var updatedCustomer = new Customer { Id = existingCustomerId, FirstName = "UpdatedJohn", LastName = "UpdatedDoe", Email = "updated.john.doe@example.com", Address = "456 Oak St", BirthDate = DateTime.Now };

            mockCustomerService.Setup(service => service.UpdateCustomerAsync(existingCustomerId, updatedCustomer))
                              .ReturnsAsync(true);

            // Act
            var result = await controller.UpdateCustomer(existingCustomerId, updatedCustomer);

            // Assert
            Xunit.Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_ValidId_ReturnsOkResponse()
        {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();
            var controller = new CustomerController(mockCustomerService.Object);

            var existingCustomerId = 1;

            mockCustomerService.Setup(service => service.DeleteCustomerAsync(existingCustomerId))
                              .ReturnsAsync(true);

            // Act
            var result = await controller.DeleteCustomer(existingCustomerId);

            // Assert
            Xunit.Assert.IsType<OkResult>(result);
        }        
    }
}

using CustomerManagement.Models;
using CustomerManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerManagement.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]   
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]        
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {

            var customers = await _customerService.GetCustomersAsync();

            if (customers == null)
            {
                return NotFound("No customers found.");
            }

            return Ok(customers);
        }

       
        [HttpGet("{id}")]       
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound($"Customer with ID {id} not found.");
            }

            return Ok(customer);
        }

        
        [HttpPost]        
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("Invalid customer data.");
            }

            var createdCustomer = await _customerService.CreateCustomerAsync(customer);

            return CreatedAtAction(nameof(GetCustomers), new { id = createdCustomer.Id }, createdCustomer);
        }



       
        [HttpPut("{id}")]        
        public async Task<IActionResult> UpdateCustomer(int id, Customer customer)
        {
            var updated = await _customerService.UpdateCustomerAsync(id, customer);

            if (!updated)
            {
                return BadRequest("Invalid update operation.");
            }

            return Ok();
        }    

                
        [HttpDelete("{id}")]        
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var deleted = await _customerService.DeleteCustomerAsync(id);

            if (!deleted)
            {
                return NotFound($"Customer with ID {id} not found.");
            }

            return Ok();
        }
    }
}


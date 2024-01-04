using CustomerManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]   
    public class CustomerController : ControllerBase
    {
        private readonly CustomerContext _dbContext;        

        public CustomerController(CustomerContext dbContext) 
        {
            _dbContext = dbContext;            
        }

       
        
        [HttpGet]        
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
           
            var customers = await _dbContext.Customers.ToListAsync();

            if (customers == null || customers.Count == 0)
            {
                return NotFound("No customers found.");
            }

            return Ok(customers);
        }

       
        [HttpGet("{id}")]       
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);

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

            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomers), new { id = customer.Id }, customer);
        }



       
        [HttpPut("{id}")]        
        public async Task<IActionResult> UpdateCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest("Customer ID mismatch.");
            }

            _dbContext.Entry(customer).State=EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!CustomerAvailable(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }
            return Ok();
        }
        private bool CustomerAvailable(int id)
        {
            return _dbContext.Customers.Any(e => e.Id == id);
        }


        
        [HttpDelete("{id}")]        
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var existingCustomer = await _dbContext.Customers.FindAsync(id);

            if (existingCustomer == null)
            {
                return NotFound($"Customer with ID {id} not found.");
            }

            _dbContext.Customers.Remove(existingCustomer);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}


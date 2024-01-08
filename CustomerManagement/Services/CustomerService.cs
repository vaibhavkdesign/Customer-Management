using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerContext _dbContext;

        public CustomerService(CustomerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _dbContext.Customers.FindAsync(id);
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> UpdateCustomerAsync(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return false;
            }

            _dbContext.Entry(customer).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerAvailable(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var existingCustomer = await _dbContext.Customers.FindAsync(id);

            if (existingCustomer == null)
            {
                return false;
            }

            _dbContext.Customers.Remove(existingCustomer);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private bool CustomerAvailable(int id)
        {
            return _dbContext.Customers.Any(e => e.Id == id);
        }
    }
}

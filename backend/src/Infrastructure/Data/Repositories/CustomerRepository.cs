using Microsoft.EntityFrameworkCore;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Customer entity
/// </summary>
public class CustomerRepository(NationalClothingStoreDbContext context) : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .Include(c => c.Loyalty)
            .Include(c => c.SalesTransactions)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .Include(c => c.Loyalty)
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<Customer?> GetByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .Include(c => c.Loyalty)
            .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<(IEnumerable<Customer> customers, int totalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Customers
            .Include(c => c.Loyalty)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => 
                c.FirstName.Contains(search) ||
                c.LastName.Contains(search) ||
                c.FullName.Contains(search) ||
                (c.Email != null && c.Email.Contains(search)) ||
                (c.PhoneNumber != null && c.PhoneNumber.Contains(search))
            );
        }

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var customers = await query
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (customers, totalCount);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .Include(c => c.Loyalty)
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .Include(c => c.Loyalty)
            .Where(c => c.IsActive)
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        // Validate unique constraints
        if (!string.IsNullOrEmpty(customer.Email))
        {
            var existingEmail = await GetByEmailAsync(customer.Email, cancellationToken);
            if (existingEmail != null)
            {
                throw new InvalidOperationException($"Customer with email '{customer.Email}' already exists.");
            }
        }

        if (!string.IsNullOrEmpty(customer.PhoneNumber))
        {
            var existingPhone = await GetByPhoneAsync(customer.PhoneNumber, cancellationToken);
            if (existingPhone != null)
            {
                throw new InvalidOperationException($"Customer with phone number '{customer.PhoneNumber}' already exists.");
            }
        }

        context.Customers.Add(customer);
        await context.SaveChangesAsync(cancellationToken);
        
        // Reload to include navigation properties
        return await GetByIdAsync(customer.Id, cancellationToken) ?? customer;
    }

    public async Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        var existingCustomer = await GetByIdAsync(customer.Id, cancellationToken);
        if (existingCustomer == null)
        {
            throw new InvalidOperationException($"Customer with ID '{customer.Id}' not found.");
        }

        // Validate unique constraints for changes
        if (!string.IsNullOrEmpty(customer.Email) && customer.Email != existingCustomer.Email)
        {
            var existingEmail = await GetByEmailAsync(customer.Email, cancellationToken);
            if (existingEmail != null)
            {
                throw new InvalidOperationException($"Customer with email '{customer.Email}' already exists.");
            }
        }

        if (!string.IsNullOrEmpty(customer.PhoneNumber) && customer.PhoneNumber != existingCustomer.PhoneNumber)
        {
            var existingPhone = await GetByPhoneAsync(customer.PhoneNumber, cancellationToken);
            if (existingPhone != null)
            {
                throw new InvalidOperationException($"Customer with phone number '{customer.PhoneNumber}' already exists.");
            }
        }

        // Update properties
        existingCustomer.FirstName = customer.FirstName;
        existingCustomer.LastName = customer.LastName;
        existingCustomer.Email = customer.Email;
        existingCustomer.PhoneNumber = customer.PhoneNumber;
        existingCustomer.DateOfBirth = customer.DateOfBirth;
        existingCustomer.Gender = customer.Gender;
        existingCustomer.AddressLine1 = customer.AddressLine1;
        existingCustomer.AddressLine2 = customer.AddressLine2;
        existingCustomer.City = customer.City;
        existingCustomer.State = customer.State;
        existingCustomer.PostalCode = customer.PostalCode;
        existingCustomer.Country = customer.Country;
        existingCustomer.EmailOptIn = customer.EmailOptIn;
        existingCustomer.SmsOptIn = customer.SmsOptIn;
        existingCustomer.IsActive = customer.IsActive;
        existingCustomer.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
        
        // Reload to include navigation properties
        return await GetByIdAsync(existingCustomer.Id, cancellationToken) ?? existingCustomer;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await GetByIdAsync(id, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID '{id}' not found.");
        }

        // Soft delete by setting IsActive to false
        customer.IsActive = false;
        customer.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Customers.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Customers.AnyAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<bool> PhoneExistsAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await context.Customers.AnyAsync(c => c.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.Customers.CountAsync(cancellationToken);
    }

    public async Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.Customers.CountAsync(c => c.IsActive, cancellationToken);
    }

    public async Task<(IEnumerable<Customer> customers, int totalCount)> SearchAsync(
        string searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetPagedAsync(pageNumber, pageSize, isActive: true, cancellationToken: cancellationToken);
        }

        var query = context.Customers
            .Include(c => c.Loyalty)
            .Where(c => c.IsActive && (
                c.FirstName.Contains(searchTerm) ||
                c.LastName.Contains(searchTerm) ||
                c.FullName.Contains(searchTerm) ||
                (c.Email != null && c.Email.Contains(searchTerm)) ||
                (c.PhoneNumber != null && c.PhoneNumber.Contains(searchTerm))
            ));

        var totalCount = await query.CountAsync(cancellationToken);

        var customers = await query
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (customers, totalCount);
    }

    public async Task<IEnumerable<Customer>> GetCustomersWithUpcomingBirthdaysAsync(
        int daysAhead = 7,
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var upcomingDate = today.AddDays(daysAhead);

        // Handle year-end wraparound
        if (upcomingDate.Year > today.Year)
        {
            // Query for birthdays in current year remainder + next year beginning
            return await context.Customers
                .Include(c => c.Loyalty)
                .Where(c => c.IsActive && c.DateOfBirth.HasValue && (
                    (c.DateOfBirth.Value.Month > today.Month || 
                     (c.DateOfBirth.Value.Month == today.Month && c.DateOfBirth.Value.Day >= today.Day)) ||
                    (c.DateOfBirth.Value.Month < upcomingDate.Month || 
                     (c.DateOfBirth.Value.Month == upcomingDate.Month && c.DateOfBirth.Value.Day <= upcomingDate.Day))
                ))
                .OrderBy(c => c.DateOfBirth!.Value.Month)
                .ThenBy(c => c.DateOfBirth!.Value.Day)
                .ToListAsync(cancellationToken);
        }

        // Simple case - same year
        return await context.Customers
            .Include(c => c.Loyalty)
            .Where(c => c.IsActive && c.DateOfBirth.HasValue &&
                        c.DateOfBirth.Value.Month >= today.Month &&
                        c.DateOfBirth.Value.Day >= today.Day &&
                        (c.DateOfBirth.Value.Month < upcomingDate.Month ||
                         (c.DateOfBirth.Value.Month == upcomingDate.Month && c.DateOfBirth.Value.Day <= upcomingDate.Day)))
            .OrderBy(c => c.DateOfBirth!.Value.Month)
            .ThenBy(c => c.DateOfBirth!.Value.Day)
            .ToListAsync(cancellationToken);
    }
}
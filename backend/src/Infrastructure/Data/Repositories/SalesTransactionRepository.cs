using Microsoft.EntityFrameworkCore;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for SalesTransaction entity
/// </summary>
public class SalesTransactionRepository(NationalClothingStoreDbContext context) : ISalesTransactionRepository
{
    public async Task<SalesTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.SalesTransactions
            .Include(st => st.Branch)
            .Include(st => st.Customer)
                .ThenInclude(c => c!.Loyalty)
            .Include(st => st.User)
            .Include(st => st.Items)
                .ThenInclude(sti => sti.Product)
            .Include(st => st.Items)
                .ThenInclude(sti => sti.ProductVariation)
            .Include(st => st.Items)
                .ThenInclude(sti => sti.Inventory)
            .Include(st => st.Payments)
            .FirstOrDefaultAsync(st => st.Id == id, cancellationToken);
    }

    public async Task<SalesTransaction?> GetByTransactionNumberAsync(string transactionNumber, CancellationToken cancellationToken = default)
    {
        return await context.SalesTransactions
            .Include(st => st.Branch)
            .Include(st => st.Customer)
                .ThenInclude(c => c!.Loyalty)
            .Include(st => st.User)
            .Include(st => st.Items)
                .ThenInclude(sti => sti.Product)
            .Include(st => st.Items)
                .ThenInclude(sti => sti.ProductVariation)
            .Include(st => st.Items)
                .ThenInclude(sti => sti.Inventory)
            .Include(st => st.Payments)
            .FirstOrDefaultAsync(st => st.TransactionNumber == transactionNumber, cancellationToken);
    }

    public async Task<(IEnumerable<SalesTransaction> transactions, int totalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? branchId = null,
        Guid? customerId = null,
        string? transactionType = null,
        string? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.SalesTransactions
            .Include(st => st.Branch)
            .Include(st => st.Customer)
                .ThenInclude(c => c!.Loyalty)
            .Include(st => st.User)
            .Include(st => st.Items)
            .AsQueryable();

        // Apply filters
        if (branchId.HasValue)
        {
            query = query.Where(st => st.BranchId == branchId.Value);
        }

        if (customerId.HasValue)
        {
            query = query.Where(st => st.CustomerId == customerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(transactionType))
        {
            query = query.Where(st => st.TransactionType == transactionType);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(st => st.Status == status);
        }

        if (startDate.HasValue)
        {
            query = query.Where(st => st.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(st => st.CreatedAt <= endDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var transactions = await query
            .OrderByDescending(st => st.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (transactions, totalCount);
    }

    public async Task<IEnumerable<SalesTransaction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.SalesTransactions
            .Include(st => st.Branch)
            .Include(st => st.Customer)
                .ThenInclude(c => c!.Loyalty)
            .Include(st => st.User)
            .OrderByDescending(st => st.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SalesTransaction>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await context.SalesTransactions
            .Include(st => st.Branch)
            .Include(st => st.Items)
                .ThenInclude(sti => sti.Product)
            .Include(st => st.Payments)
            .Where(st => st.CustomerId == customerId)
            .OrderByDescending(st => st.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SalesTransaction>> GetByBranchAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await context.SalesTransactions
            .Include(st => st.Customer)
                .ThenInclude(c => c!.Loyalty)
            .Include(st => st.User)
            .Include(st => st.Items)
                .ThenInclude(sti => sti.Product)
            .Include(st => st.Payments)
            .Where(st => st.BranchId == branchId)
            .OrderByDescending(st => st.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<SalesTransaction> CreateAsync(SalesTransaction transaction, CancellationToken cancellationToken = default)
    {
        // Ensure unique transaction number
        var existingTransaction = await GetByTransactionNumberAsync(transaction.TransactionNumber, cancellationToken);
        if (existingTransaction != null)
        {
            throw new InvalidOperationException($"Transaction with number '{transaction.TransactionNumber}' already exists.");
        }

        // Validate references exist
        var branch = await context.Branches.FindAsync(new object[] { transaction.BranchId }, cancellationToken);
        if (branch == null)
        {
            throw new InvalidOperationException($"Branch with ID '{transaction.BranchId}' not found.");
        }

        if (transaction.CustomerId.HasValue)
        {
            var customer = await context.Customers.FindAsync(new object[] { transaction.CustomerId.Value }, cancellationToken);
            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with ID '{transaction.CustomerId.Value}' not found.");
            }
        }

        var user = await context.Users.FindAsync(new object[] { transaction.UserId }, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID '{transaction.UserId}' not found.");
        }

        context.SalesTransactions.Add(transaction);
        await context.SaveChangesAsync(cancellationToken);
        
        // Reload to include navigation properties
        return await GetByIdAsync(transaction.Id, cancellationToken) ?? transaction;
    }

    public async Task<SalesTransaction> UpdateAsync(SalesTransaction transaction, CancellationToken cancellationToken = default)
    {
        var existingTransaction = await GetByIdAsync(transaction.Id, cancellationToken);
        if (existingTransaction == null)
        {
            throw new InvalidOperationException($"Sales transaction with ID '{transaction.Id}' not found.");
        }

        // Update properties
        existingTransaction.TransactionType = transaction.TransactionType;
        existingTransaction.Status = transaction.Status;
        existingTransaction.Subtotal = transaction.Subtotal;
        existingTransaction.TaxAmount = transaction.TaxAmount;
        existingTransaction.DiscountAmount = transaction.DiscountAmount;
        existingTransaction.TotalAmount = transaction.TotalAmount;
        existingTransaction.AmountPaid = transaction.AmountPaid;
        existingTransaction.ChangeGiven = transaction.ChangeGiven;
        existingTransaction.LoyaltyPointsEarned = transaction.LoyaltyPointsEarned;
        existingTransaction.LoyaltyPointsRedeemed = transaction.LoyaltyPointsRedeemed;
        existingTransaction.Notes = transaction.Notes;
        existingTransaction.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
        
        // Reload to include navigation properties
        return await GetByIdAsync(existingTransaction.Id, cancellationToken) ?? existingTransaction;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var transaction = await GetByIdAsync(id, cancellationToken);
        if (transaction == null)
        {
            throw new InvalidOperationException($"Sales transaction with ID '{id}' not found.");
        }

        context.SalesTransactions.Remove(transaction);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.SalesTransactions.AnyAsync(st => st.Id == id, cancellationToken);
    }

    public async Task<bool> TransactionNumberExistsAsync(string transactionNumber, CancellationToken cancellationToken = default)
    {
        return await context.SalesTransactions.AnyAsync(st => st.TransactionNumber == transactionNumber, cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.SalesTransactions.CountAsync(cancellationToken);
    }

    public async Task<SalesStatistics> GetSalesStatisticsAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.SalesTransactions
            .Where(st => st.CreatedAt >= startDate && st.CreatedAt <= endDate && st.Status == "COMPLETED");

        if (branchId.HasValue)
        {
            query = query.Where(st => st.BranchId == branchId.Value);
        }

        var transactions = await query.ToListAsync(cancellationToken);

        var stats = new SalesStatistics
        {
            TotalSales = transactions.Sum(st => st.TotalAmount),
            TotalTransactions = transactions.Count,
            TotalItemsSold = transactions.Sum(st => st.ItemCount),
            TotalDiscounts = transactions.Sum(st => st.DiscountAmount),
            TotalTax = transactions.Sum(st => st.TaxAmount),
            TotalCustomers = transactions.Where(st => st.CustomerId.HasValue).Select(st => st.CustomerId).Distinct().Count()
        };

        stats.AverageTransactionValue = stats.TotalTransactions > 0 
            ? stats.TotalSales / stats.TotalTransactions 
            : 0;

        return stats;
    }

    public async Task<IEnumerable<ProductSalesSummary>> GetTopSellingProductsAsync(
        DateTime startDate,
        DateTime endDate,
        int limit = 10,
        Guid? branchId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.SalesTransactionItems
            .Where(sti => sti.SalesTransaction.CreatedAt >= startDate && 
                         sti.SalesTransaction.CreatedAt <= endDate &&
                         sti.SalesTransaction.Status == "COMPLETED");

        if (branchId.HasValue)
        {
            query = query.Where(sti => sti.SalesTransaction.BranchId == branchId.Value);
        }

        return await query
            .GroupBy(sti => new { sti.ProductId, sti.Product.Name })
            .Select(g => new ProductSalesSummary
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                QuantitySold = g.Sum(sti => sti.Quantity),
                TotalRevenue = g.Sum(sti => sti.TotalPrice),
                AveragePrice = g.Sum(sti => sti.Quantity) > 0 ? g.Sum(sti => sti.TotalPrice) / g.Sum(sti => sti.Quantity) : 0
            })
            .OrderByDescending(pss => pss.QuantitySold)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DailySalesSummary>> GetDailySalesSummaryAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? branchId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.SalesTransactions
            .Where(st => st.CreatedAt >= startDate && st.CreatedAt <= endDate && st.Status == "COMPLETED");

        if (branchId.HasValue)
        {
            query = query.Where(st => st.BranchId == branchId.Value);
        }

        return await query
            .GroupBy(st => st.CreatedAt.Date)
            .Select(g => new DailySalesSummary
            {
                Date = g.Key,
                TotalSales = g.Sum(st => st.TotalAmount),
                TransactionCount = g.Count(),
                ItemCount = g.Sum(st => st.ItemCount),
                AverageTransactionValue = g.Count() > 0 ? g.Sum(st => st.TotalAmount) / g.Count() : 0
            })
            .OrderBy(dss => dss.Date)
            .ToListAsync(cancellationToken);
    }
}
using Microsoft.Extensions.DependencyInjection;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Application.Services;
using NationalClothingStore.Infrastructure.Data;
using NationalClothingStore.Infrastructure.Data.Repositories;
using NationalClothingStore.Infrastructure.External;
using NationalClothingStore.Infrastructure.Monitoring;
using NationalClothingStore.Infrastructure.Security;

namespace NationalClothingStore.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all repository implementations
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register generic repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Register specific repositories
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductVariationRepository, ProductVariationRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISalesTransactionRepository, SalesTransactionRepository>();
        
        return services;
    }

    /// <summary>
    /// Registers all application services
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductCatalogService, ProductCatalogService>();
        services.AddScoped<IInventoryManagementService, InventoryManagementService>();
        services.AddScoped<ISalesProcessingService, SalesProcessingService>();
        services.AddScoped<IPaymentService, PaymentService>();
                
        // Register monitoring services
        services.AddHostedService<SalesPerformanceMonitor>();
                
        // Register security services
        services.AddScoped<IPaymentSecurityService, PaymentSecurityService>();
        
        return services;
    }
}
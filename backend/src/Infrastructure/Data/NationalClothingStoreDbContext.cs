using Microsoft.EntityFrameworkCore;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for National Clothing Store
/// </summary>
public class NationalClothingStoreDbContext : DbContext
{
    public NationalClothingStoreDbContext(DbContextOptions<NationalClothingStoreDbContext> options) 
        : base(options)
    {
    }

    // User Management
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserBranch> UserBranches { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<AuditEvent> AuditEvents { get; set; }

    // Location Management
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }

    // Product Catalog
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariation> ProductVariations { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    // Inventory Management
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    // Customer Management
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerLoyalty> CustomerLoyalty { get; set; }
    public DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; }

    // Sales Processing
    public DbSet<SalesTransaction> SalesTransactions { get; set; }
    public DbSet<SalesTransactionItem> SalesTransactionItems { get; set; }
    public DbSet<SalesTransactionPayment> SalesTransactionPayments { get; set; }

    // Supplier Management
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure table names
        modelBuilder.Entity<Customer>().ToTable("Customers");
        modelBuilder.Entity<CustomerLoyalty>().ToTable("CustomerLoyalty");
        modelBuilder.Entity<LoyaltyTransaction>().ToTable("LoyaltyTransactions");
        modelBuilder.Entity<SalesTransaction>().ToTable("SalesTransactions");
        modelBuilder.Entity<SalesTransactionItem>().ToTable("SalesTransactionItems");
        modelBuilder.Entity<SalesTransactionPayment>().ToTable("SalesTransactionPayments");
        
        // Configure relationships
        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Loyalty)
            .WithOne(cl => cl.Customer)
            .HasForeignKey<CustomerLoyalty>(cl => cl.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<LoyaltyTransaction>()
            .HasOne(lt => lt.CustomerLoyalty)
            .WithMany(cl => cl.LoyaltyTransactions)
            .HasForeignKey(lt => lt.CustomerLoyaltyId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<LoyaltyTransaction>()
            .HasOne(lt => lt.SalesTransaction)
            .WithMany(st => st.LoyaltyTransaction)
            .HasForeignKey(lt => lt.SalesTransactionId)
            .OnDelete(DeleteBehavior.SetNull);
            
        modelBuilder.Entity<SalesTransaction>()
            .HasOne(st => st.Customer)
            .WithMany(c => c.SalesTransactions)
            .HasForeignKey(st => st.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);
            
        modelBuilder.Entity<SalesTransaction>()
            .HasOne(st => st.OriginalTransaction)
            .WithMany(st => st.ReturnTransactions)
            .HasForeignKey(st => st.OriginalTransactionId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<SalesTransactionItem>()
            .HasOne(sti => sti.SalesTransaction)
            .WithMany(st => st.Items)
            .HasForeignKey(sti => sti.SalesTransactionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<SalesTransactionPayment>()
            .HasOne(stp => stp.SalesTransaction)
            .WithMany(st => st.Payments)
            .HasForeignKey(stp => stp.SalesTransactionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Configure indexes
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email)
            .IsUnique(false);
            
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.PhoneNumber)
            .IsUnique(false);
            
        modelBuilder.Entity<CustomerLoyalty>()
            .HasIndex(cl => cl.LoyaltyCardNumber)
            .IsUnique();
            
        modelBuilder.Entity<SalesTransaction>()
            .HasIndex(st => st.TransactionNumber)
            .IsUnique();
            
        modelBuilder.Entity<SalesTransaction>()
            .HasIndex(st => st.CreatedAt);
            
        // Configure AuditEvent entity to handle the Metadata property
        modelBuilder.Entity<AuditEvent>()
            .Ignore(ae => ae.Metadata);
            
        // Configure composite key for RolePermission
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });
            
        // Configure composite key for UserRole
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });
            
        // Configure composite key for UserBranch
        modelBuilder.Entity<UserBranch>()
            .HasKey(ub => new { ub.UserId, ub.BranchId });
    }
}

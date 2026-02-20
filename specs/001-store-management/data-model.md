# Data Model: National Clothing Store Management System

**Date**: 2025-02-08  
**Purpose**: Define core entities, relationships, and validation rules for the system

## Core Entities

### User Management

#### User
```csharp
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<UserBranch> UserBranches { get; set; }
}
```

#### Role
```csharp
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<Permission> Permissions { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}
```

#### Permission
```csharp
public class Permission
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Resource { get; set; }
    public string Action { get; set; }
    public ICollection<Role> Roles { get; set; }
}
```

#### UserRole (Many-to-Many)
```csharp
public class UserRole
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; }
    public DateTime AssignedAt { get; set; }
    public Guid AssignedBy { get; set; }
}
```

### Branch & Location Management

#### Branch
```csharp
public class Branch
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<UserBranch> UserBranches { get; set; }
    public ICollection<Inventory> Inventories { get; set; }
    public ICollection<SalesTransaction> SalesTransactions { get; set; }
}
```

#### UserBranch (Many-to-Many)
```csharp
public class UserBranch
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; }
    public DateTime AssignedAt { get; set; }
    public bool IsPrimary { get; set; }
}
```

#### Warehouse
```csharp
public class Warehouse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Inventory> Inventories { get; set; }
}
```

### Product Catalog

#### Category
```csharp
public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Code { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Category ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Product> Products { get; set; }
}
```

#### Product
```csharp
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string SKU { get; set; }
    public string Barcode { get; set; }
    public decimal BasePrice { get; set; }
    public decimal CostPrice { get; set; }
    public string Brand { get; set; }
    public string Season { get; set; }
    public string Collection { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    public ICollection<ProductVariation> Variations { get; set; }
    public ICollection<ProductImage> Images { get; set; }
    public ICollection<Inventory> Inventories { get; set; }
    public ICollection<SalesTransactionItem> SalesTransactionItems { get; set; }
}
```

#### ProductVariation
```csharp
public class ProductVariation
{
    public Guid Id { get; set; }
    public string Size { get; set; }
    public string Color { get; set; }
    public string SKU { get; set; }
    public string Barcode { get; set; }
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public int Weight { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public ICollection<Inventory> Inventories { get; set; }
    public ICollection<SalesTransactionItem> SalesTransactionItems { get; set; }
}
```

#### ProductImage
```csharp
public class ProductImage
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string AltText { get; set; }
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
}
```

### Inventory Management

#### Inventory
```csharp
public class Inventory
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity => Quantity - ReservedQuantity;
    public decimal UnitCost { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public Guid? ProductVariationId { get; set; }
    public ProductVariation ProductVariation { get; set; }
    public Guid? BranchId { get; set; }
    public Branch Branch { get; set; }
    public Guid? WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
}
```

#### InventoryTransaction
```csharp
public class InventoryTransaction
{
    public Guid Id { get; set; }
    public TransactionType Type { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public Guid? ProductVariationId { get; set; }
    public ProductVariation ProductVariation { get; set; }
    public Guid? BranchId { get; set; }
    public Branch Branch { get; set; }
    public Guid? WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
    public Guid CreatedBy { get; set; }
    public User CreatedByUser { get; set; }
}

public enum TransactionType
{
    StockIn,
    StockOut,
    Transfer,
    Adjustment,
    Sale,
    Return
}
```

### Customer Management

#### Customer
```csharp
public class Customer
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<SalesTransaction> SalesTransactions { get; set; }
    public CustomerLoyalty LoyaltyInfo { get; set; }
}
```

#### CustomerLoyalty
```csharp
public class CustomerLoyalty
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public string LoyaltyNumber { get; set; }
    public int PointsBalance { get; set; }
    public decimal LifetimeSpent { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
}
```

### Sales Processing

#### SalesTransaction
```csharp
public class SalesTransaction
{
    public Guid Id { get; set; }
    public string TransactionNumber { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal ChangeAmount { get; set; }
    public TransactionStatus Status { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentReference { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public Guid? CustomerId { get; set; }
    public Customer Customer { get; set; }
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; }
    public Guid CreatedBy { get; set; }
    public User CreatedByUser { get; set; }
    public ICollection<SalesTransactionItem> Items { get; set; }
    public ICollection<SalesTransactionPayment> Payments { get; set; }
}

public enum TransactionStatus
{
    Pending,
    Completed,
    Cancelled,
    Refunded
}
```

#### SalesTransactionItem
```csharp
public class SalesTransactionItem
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Guid SalesTransactionId { get; set; }
    public SalesTransaction SalesTransaction { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public Guid? ProductVariationId { get; set; }
    public ProductVariation ProductVariation { get; set; }
}
```

#### SalesTransactionPayment
```csharp
public class SalesTransactionPayment
{
    public Guid Id { get; set; }
    public PaymentType PaymentType { get; set; }
    public decimal Amount { get; set; }
    public string PaymentReference { get; set; }
    public string CardLastFour { get; set; }
    public DateTime ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Guid SalesTransactionId { get; set; }
    public SalesTransaction SalesTransaction { get; set; }
}

public enum PaymentType
{
    Cash,
    CreditCard,
    DebitCard,
    GiftCard,
    StoreCredit,
    MobilePayment
}
```

### Supplier Management

#### Supplier
```csharp
public class Supplier
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string ContactPerson { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string TaxId { get; set; }
    public string PaymentTerms { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    public ICollection<Product> Products { get; set; }
}
```

#### PurchaseOrder
```csharp
public class PurchaseOrder
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    public Guid? WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
    public Guid CreatedBy { get; set; }
    public User CreatedByUser { get; set; }
    public ICollection<PurchaseOrderItem> Items { get; set; }
}

public enum PurchaseOrderStatus
{
    Draft,
    Sent,
    Confirmed,
    PartiallyReceived,
    FullyReceived,
    Cancelled
}
```

#### PurchaseOrderItem
```csharp
public class PurchaseOrderItem
{
    public Guid Id { get; set; }
    public int QuantityOrdered { get; set; }
    public int QuantityReceived { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public Guid? ProductVariationId { get; set; }
    public ProductVariation ProductVariation { get; set; }
}
```

## Validation Rules

### User Management
- Email must be unique and valid format
- Phone number must be valid format
- At least one role must be assigned
- At least one branch must be assigned for non-admin users

### Product Catalog
- SKU must be unique across all products
- Base price must be greater than cost price
- At least one product image is required
- Product variations must have unique SKU within product

### Inventory Management
- Quantity cannot be negative
- Reserved quantity cannot exceed total quantity
- Cost price must be positive
- All inventory transactions must be auditable

### Sales Processing
- Transaction numbers must be unique
- Total amount must equal subtotal + tax - discount
- Payment amounts must equal total amount
- Inventory must be available before sale

### Supplier Management
- Supplier codes must be unique
- Purchase order numbers must be unique
- Received quantity cannot exceed ordered quantity

## Indexing Strategy

### Primary Indexes
- All primary keys (Guid)
- Unique constraints on business keys (SKU, Email, etc.)

### Secondary Indexes
- Products: CategoryId, IsActive, CreatedAt
- Inventory: ProductId, BranchId, WarehouseId
- Sales Transactions: TransactionDate, BranchId, CustomerId
- Purchase Orders: OrderDate, SupplierId, Status

### Composite Indexes
- Inventory: (ProductId, BranchId, ProductVariationId)
- Sales Transactions: (BranchId, TransactionDate)
- Product Variations: (ProductId, Size, Color)

## Data Integrity

### Foreign Key Constraints
- All relationships properly constrained
- Cascade delete rules defined
- Referential integrity enforced

### Check Constraints
- Positive quantities and amounts
- Valid dates and status values
- Business rule validations

### Triggers
- Audit logging for critical operations
- Inventory quantity updates
- Transaction number generation

## Performance Considerations

### Partitioning
- Large tables partitioned by date (Sales Transactions)
- Archive old data periodically

### Caching
- Product catalog cached in Redis
- User sessions cached
- Frequently accessed reference data cached

### Optimization
- Query optimization for common patterns
- Proper indexing for search operations
- Efficient pagination for large datasets

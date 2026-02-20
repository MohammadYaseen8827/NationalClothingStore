-- Migration: Create Product Catalog Schema
-- Version: 002
-- Description: Creates tables for categories, products, product variations, and product images

-- Create Categories table
CREATE TABLE [Categories] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Code] NVARCHAR(50) NULL,
    [ParentCategoryId] UNIQUEIDENTIFIER NULL,
    [SortOrder] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Categories_Categories_ParentCategoryId] FOREIGN KEY ([ParentCategoryId]) REFERENCES [Categories] ([Id]) ON DELETE SET NULL
);

-- Create Products table
CREATE TABLE [Products] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [SKU] NVARCHAR(100) NOT NULL,
    [Barcode] NVARCHAR(50) NULL,
    [BasePrice] DECIMAL(18,2) NOT NULL,
    [CostPrice] DECIMAL(18,2) NOT NULL,
    [Brand] NVARCHAR(100) NULL,
    [Season] NVARCHAR(50) NULL,
    [Material] NVARCHAR(100) NULL,
    [Color] NVARCHAR(50) NULL,
    [CategoryId] UNIQUEIDENTIFIER NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE
);

-- Create ProductImages table
CREATE TABLE [ProductImages] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [ImageUrl] NVARCHAR(500) NOT NULL,
    [AltText] NVARCHAR(200) NULL,
    [Caption] NVARCHAR(500) NULL,
    [SortOrder] INT NOT NULL DEFAULT 0,
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ProductImages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductImages_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
);

-- Create ProductVariations table
CREATE TABLE [ProductVariations] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [Size] NVARCHAR(50) NOT NULL,
    [Color] NVARCHAR(50) NOT NULL,
    [SKU] NVARCHAR(100) NOT NULL,
    [AdditionalPrice] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [CostPrice] DECIMAL(18,2) NOT NULL,
    [StockQuantity] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ProductVariations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductVariations_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX [IX_Categories_ParentCategoryId] ON [Categories] ([ParentCategoryId]);
CREATE INDEX [IX_Categories_Code] ON [Categories] ([Code]);
CREATE INDEX [IX_Categories_IsActive] ON [Categories] ([IsActive]);
CREATE INDEX [IX_Categories_SortOrder] ON [Categories] ([SortOrder]);

CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);
CREATE INDEX [IX_Products_SKU] ON [Products] ([SKU]) WHERE [IsActive] = 1;
CREATE INDEX [IX_Products_Barcode] ON [Products] ([Barcode]) WHERE [Barcode] IS NOT NULL;
CREATE INDEX [IX_Products_Brand] ON [Products] ([Brand]) WHERE [Brand] IS NOT NULL;
CREATE INDEX [IX_Products_Season] ON [Products] ([Season]) WHERE [Season] IS NOT NULL;
CREATE INDEX [IX_Products_IsActive] ON [Products] ([IsActive]);
CREATE INDEX [IX_Products_Name] ON [Products] ([Name]);
CREATE INDEX [IX_Products_BasePrice] ON [Products] ([BasePrice]);

CREATE INDEX [IX_ProductImages_ProductId] ON [ProductImages] ([ProductId]);
CREATE INDEX [IX_ProductImages_SortOrder] ON [ProductImages] ([SortOrder]);
CREATE INDEX [IX_ProductImages_IsPrimary] ON [ProductImages] ([IsPrimary]);

CREATE INDEX [IX_ProductVariations_ProductId] ON [ProductVariations] ([ProductId]);
CREATE INDEX [IX_ProductVariations_SKU] ON [ProductVariations] ([SKU]) WHERE [IsActive] = 1;
CREATE INDEX [IX_ProductVariations_Size] ON [ProductVariations] ([Size]);
CREATE INDEX [IX_ProductVariations_Color] ON [ProductVariations] ([Color]);
CREATE INDEX [IX_ProductVariations_IsActive] ON [ProductVariations] ([IsActive]);
CREATE INDEX [IX_ProductVariations_StockQuantity] ON [ProductVariations] ([StockQuantity]);

-- Create full-text search index for products
CREATE FULLTEXT CATALOG [ft_ProductCatalog] AS DEFAULT;
CREATE FULLTEXT INDEX ON [Products] ([Name], [Description], [Brand], [Material], [Color]) KEY INDEX [PK_Products];

-- Create unique constraints
ALTER TABLE [Categories] ADD CONSTRAINT [UK_Categories_Code] UNIQUE ([Code]) WHERE [Code] IS NOT NULL;
ALTER TABLE [Products] ADD CONSTRAINT [UK_Products_SKU] UNIQUE ([SKU]);
ALTER TABLE [ProductVariations] ADD CONSTRAINT [UK_ProductVariations_SKU] UNIQUE ([SKU]);

-- Create composite unique constraint for product variations (same product cannot have duplicate size/color combinations)
CREATE UNIQUE INDEX [UK_ProductVariations_ProductId_Size_Color] ON [ProductVariations] ([ProductId], [Size], [Color]) WHERE [IsActive] = 1;

-- Insert default categories
INSERT INTO [Categories] ([Id], [Name], [Description], [Code], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt])
VALUES 
    (NEWID(), 'Men''s Clothing', 'Clothing for men', 'MENS', 1, 1, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'Women''s Clothing', 'Clothing for women', 'WOMENS', 2, 1, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'Kids'' Clothing', 'Clothing for children', 'KIDS', 3, 1, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'Accessories', 'Fashion accessories', 'ACCESSORIES', 4, 1, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'Footwear', 'Shoes and footwear', 'FOOTWEAR', 5, 1, GETUTCDATE(), GETUTCDATE());

-- Create trigger to update UpdatedAt timestamp
CREATE TRIGGER [TR_Categories_UpdateTimestamp]
ON [Categories]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [Categories]
    SET [UpdatedAt] = GETUTCDATE(),
        [UpdatedBy] = (SELECT [UpdatedBy] FROM inserted)
    WHERE [Id] IN (SELECT [Id] FROM inserted);
END;

CREATE TRIGGER [TR_Products_UpdateTimestamp]
ON [Products]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [Products]
    SET [UpdatedAt] = GETUTCDATE(),
        [UpdatedBy] = (SELECT [UpdatedBy] FROM inserted)
    WHERE [Id] IN (SELECT [Id] FROM inserted);
END;

CREATE TRIGGER [TR_ProductImages_UpdateTimestamp]
ON [ProductImages]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [ProductImages]
    SET [UpdatedAt] = GETUTCDATE(),
        [UpdatedBy] = (SELECT [UpdatedBy] FROM inserted)
    WHERE [Id] IN (SELECT [Id] FROM inserted);
END;

CREATE TRIGGER [TR_ProductVariations_UpdateTimestamp]
ON [ProductVariations]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [ProductVariations]
    SET [UpdatedAt] = GETUTCDATE(),
        [UpdatedBy] = (SELECT [UpdatedBy] FROM inserted)
    WHERE [Id] IN (SELECT [Id] FROM inserted);
END;

-- Create stored procedures for common operations
CREATE PROCEDURE [sp_GetCategoryHierarchy]
    @CategoryId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @CategoryId IS NULL
    BEGIN
        -- Get all root categories (no parent)
        SELECT * FROM [Categories] 
        WHERE [ParentCategoryId] IS NULL AND [IsActive] = 1
        ORDER BY [SortOrder], [Name];
    END
    ELSE
    BEGIN
        -- Get child categories of specified parent
        SELECT * FROM [Categories] 
        WHERE [ParentCategoryId] = @CategoryId AND [IsActive] = 1
        ORDER BY [SortOrder], [Name];
    END
END;

CREATE PROCEDURE [sp_SearchProducts]
    @SearchTerm NVARCHAR(MAX) = NULL,
    @CategoryId UNIQUEIDENTIFIER = NULL,
    @Brand NVARCHAR(100) = NULL,
    @Season NVARCHAR(50) = NULL,
    @MinPrice DECIMAL(18,2) = NULL,
    @MaxPrice DECIMAL(18,2) = NULL,
    @IsActive BIT = 1,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        p.*,
        c.Name as CategoryName,
        COUNT(*) OVER() as TotalCount
    FROM [Products] p
    INNER JOIN [Categories] c ON p.CategoryId = c.Id
    WHERE 
        (@SearchTerm IS NULL OR CONTAINS((p.Name, p.Description, p.Brand, p.Material, p.Color), @SearchTerm))
        AND (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
        AND (@Brand IS NULL OR p.Brand = @Brand)
        AND (@Season IS NULL OR p.Season = @Season)
        AND (@MinPrice IS NULL OR p.BasePrice >= @MinPrice)
        AND (@MaxPrice IS NULL OR p.BasePrice <= @MaxPrice)
        AND p.IsActive = @IsActive
        AND c.IsActive = 1
    ORDER BY p.Name
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;

CREATE PROCEDURE [sp_GetProductVariations]
    @ProductId UNIQUEIDENTIFIER,
    @Size NVARCHAR(50) = NULL,
    @Color NVARCHAR(50) = NULL,
    @IsActive BIT = 1,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        pv.*,
        COUNT(*) OVER() as TotalCount
    FROM [ProductVariations] pv
    WHERE 
        pv.ProductId = @ProductId
        AND (@Size IS NULL OR pv.Size = @Size)
        AND (@Color IS NULL OR pv.Color = @Color)
        AND pv.IsActive = @IsActive
    ORDER BY pv.Size, pv.Color
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;

CREATE PROCEDURE [sp_GetLowStockVariations]
    @Threshold INT = 10,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        pv.*,
        p.Name as ProductName,
        p.SKU as ProductSKU,
        c.Name as CategoryName,
        COUNT(*) OVER() as TotalCount
    FROM [ProductVariations] pv
    INNER JOIN [Products] p ON pv.ProductId = p.Id
    INNER JOIN [Categories] c ON p.CategoryId = c.Id
    WHERE 
        pv.StockQuantity <= @Threshold
        AND pv.IsActive = 1
        AND p.IsActive = 1
        AND c.IsActive = 1
    ORDER BY pv.StockQuantity, p.Name
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;

-- Create view for product catalog summary
CREATE VIEW [vw_ProductCatalogSummary]
AS
SELECT 
    p.Id as ProductId,
    p.Name as ProductName,
    p.SKU as ProductSKU,
    p.BasePrice,
    p.Brand,
    p.Season,
    c.Name as CategoryName,
    c.Code as CategoryCode,
    COUNT(pv.Id) as VariationCount,
    SUM(pv.StockQuantity) as TotalStock,
    COUNT(pi.Id) as ImageCount,
    CASE 
        WHEN COUNT(pv.Id) > 0 THEN MIN(pv.AdditionalPrice)
        ELSE 0 
    END as MinAdditionalPrice,
    CASE 
        WHEN COUNT(pv.Id) > 0 THEN MAX(pv.AdditionalPrice)
        ELSE 0 
    END as MaxAdditionalPrice
FROM [Products] p
INNER JOIN [Categories] c ON p.CategoryId = c.Id
LEFT JOIN [ProductVariations] pv ON p.Id = pv.ProductId AND pv.IsActive = 1
LEFT JOIN [ProductImages] pi ON p.Id = pi.ProductId
WHERE p.IsActive = 1 AND c.IsActive = 1
GROUP BY p.Id, p.Name, p.SKU, p.BasePrice, p.Brand, p.Season, c.Name, c.Code;

-- Grant permissions (adjust as needed for your environment)
-- GRANT SELECT, INSERT, UPDATE, DELETE ON [Categories] TO [AppRole];
-- GRANT SELECT, INSERT, UPDATE, DELETE ON [Products] TO [AppRole];
-- GRANT SELECT, INSERT, UPDATE, DELETE ON [ProductImages] TO [AppRole];
-- GRANT SELECT, INSERT, UPDATE, DELETE ON [ProductVariations] TO [AppRole];
-- GRANT EXECUTE ON [sp_GetCategoryHierarchy] TO [AppRole];
-- GRANT EXECUTE ON [sp_SearchProducts] TO [AppRole];
-- GRANT EXECUTE ON [sp_GetProductVariations] TO [AppRole];
-- GRANT EXECUTE ON [sp_GetLowStockVariations] TO [AppRole];
-- GRANT SELECT ON [vw_ProductCatalogSummary] TO [AppRole];

PRINT 'Product catalog schema migration completed successfully.';

-- Migration: 003_CreateInventoryManagementSchema
-- Created: 2026-02-09 03:04:00 UTC
-- Purpose: Create tables for inventory management and inventory transactions

-- Create Inventory table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Inventories' AND type = 'U')
BEGIN
    CREATE TABLE [Inventories] (
        [Id] uniqueidentifier NOT NULL PRIMARY KEY,
        [ProductId] uniqueidentifier NOT NULL,
        [ProductVariationId] uniqueidentifier NULL,
        [BranchId] uniqueidentifier NOT NULL,
        [WarehouseId] uniqueidentifier NULL,
        [Quantity] int NOT NULL DEFAULT 0,
        [ReservedQuantity] int NOT NULL DEFAULT 0,
        [AvailableQuantity] AS (Quantity - ReservedQuantity) PERSISTED COMPUTED,
        [UnitCost] decimal(18,2) NOT NULL DEFAULT 0.00,
        [LastUpdated] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE()
    );
END;

-- Create InventoryTransactions table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'InventoryTransactions' AND type = 'U')
BEGIN
    CREATE TABLE [InventoryTransactions] (
        [Id] uniqueidentifier NOT NULL PRIMARY KEY,
        [InventoryId] uniqueidentifier NOT NULL,
        [TransactionType] nvarchar(20) NOT NULL,
        [Quantity] int NOT NULL,
        [UnitCost] decimal(18,2) NOT NULL,
        [ReferenceNumber] nvarchar(100) NULL,
        [Reason] nvarchar(500) NULL,
        [FromBranchId] uniqueidentifier NULL,
        [ToBranchId] uniqueidentifier NULL,
        [FromWarehouseId] uniqueidentifier NULL,
        [ToWarehouseId] uniqueidentifier NULL,
        [CreatedByUserId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE()
    );
END;

-- Create indexes for better performance
CREATE INDEX [IX_Inventories_ProductId] ON [Inventories] ([ProductId]);
CREATE INDEX [IX_Inventories_ProductVariationId] ON [Inventories] ([ProductVariationId]);
CREATE INDEX [IX_Inventories_BranchId] ON [Inventories] ([BranchId]);
CREATE INDEX [IX_Inventories_WarehouseId] ON [Inventories] ([WarehouseId]);
CREATE INDEX [IX_InventoryTransactions_InventoryId] ON [InventoryTransactions] ([InventoryId]);
CREATE INDEX [IX_InventoryTransactions_TransactionDate] ON [InventoryTransactions] ([CreatedAt]);
CREATE INDEX [IX_InventoryTransactions_TransactionType] ON [InventoryTransactions] ([TransactionType]);
CREATE INDEX [IX_InventoryTransactions_CreatedByUserId] ON [InventoryTransactions] ([CreatedByUserId]);

-- Add foreign key constraints
ALTER TABLE [Inventories] 
ADD CONSTRAINT [FK_Inventories_Products] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE;
ALTER TABLE [Inventories] 
ADD CONSTRAINT [FK_Inventories_ProductVariations] FOREIGN KEY ([ProductVariationId]) REFERENCES [ProductVariations] ([Id]) ON DELETE CASCADE;
ALTER TABLE [Inventories] 
ADD CONSTRAINT [FK_Inventories_Branches] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE CASCADE;
ALTER TABLE [Inventories] 
ADD CONSTRAINT [FK_Inventories_Warehouses] FOREIGN KEY ([WarehouseId]) REFERENCES [Warehouses] ([Id]) ON DELETE CASCADE;

ALTER TABLE [InventoryTransactions] 
ADD CONSTRAINT [FK_InventoryTransactions_Inventories] FOREIGN KEY ([InventoryId]) REFERENCES [Inventories] ([Id]) ON DELETE CASCADE;
ALTER TABLE [InventoryTransactions] 
ADD CONSTRAINT [FK_InventoryTransactions_FromBranches] FOREIGN KEY ([FromBranchId]) REFERENCES [Branches] ([Id]) ON DELETE SET NULL;
ALTER TABLE [InventoryTransactions] 
ADD CONSTRAINT [FK_InventoryTransactions_ToBranches] FOREIGN KEY ([ToBranchId]) REFERENCES [Branches] ([Id]) ON DELETE SET NULL;
ALTER TABLE [InventoryTransactions] 
ADD CONSTRAINT [FK_InventoryTransactions_FromWarehouses] FOREIGN KEY ([FromWarehouseId]) REFERENCES [Warehouses] ([Id]) ON DELETE SET NULL;
ALTER TABLE [InventoryTransactions] 
ADD CONSTRAINT [FK_InventoryTransactions_ToWarehouses] FOREIGN KEY ([ToWarehouseId]) REFERENCES [Warehouses] ([Id]) ON DELETE SET NULL;
ALTER TABLE [InventoryTransactions] 
ADD CONSTRAINT [FK_InventoryTransactions_Users] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;

-- Insert initial data for low stock alerts (optional)
-- This would typically be populated by a background job or initial setup
-- INSERT INTO LowStockAlerts (Id, InventoryId, Threshold, IsResolved, AlertDate, ResolvedDate)
-- SELECT Id, Id, 10, 0, GETUTCDATE(), NULL FROM Inventories WHERE Quantity <= 10;

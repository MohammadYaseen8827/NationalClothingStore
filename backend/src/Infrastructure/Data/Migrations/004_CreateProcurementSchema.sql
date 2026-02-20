-- Migration: Create Procurement Management Schema
-- Version: 004
-- Description: Creates tables for Supplier, PurchaseOrder, and PurchaseOrderItem entities

-- Create Suppliers table
CREATE TABLE IF NOT EXISTS "Suppliers" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Name" VARCHAR(200) NOT NULL,
    "Code" VARCHAR(50) NOT NULL UNIQUE,
    "ContactPerson" VARCHAR(100),
    "Email" VARCHAR(255),
    "Phone" VARCHAR(20),
    "Address" VARCHAR(255),
    "City" VARCHAR(100),
    "Country" VARCHAR(100),
    "TaxNumber" VARCHAR(50),
    "PaymentTerms" VARCHAR(50),
    "Website" VARCHAR(500),
    "Rating" VARCHAR(20),
    "IsActive" BOOLEAN NOT NULL DEFAULT true,
    "Notes" VARCHAR(1000),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE
);

-- Create PurchaseOrders table
CREATE TABLE IF NOT EXISTS "PurchaseOrders" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "SupplierId" UUID NOT NULL,
    "OrderNumber" VARCHAR(50) NOT NULL UNIQUE,
    "OrderDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ExpectedDeliveryDate" TIMESTAMP WITH TIME ZONE,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Pending',
    "TotalAmount" DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    "Notes" VARCHAR(1000),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE
);

-- Create PurchaseOrderItems table
CREATE TABLE IF NOT EXISTS "PurchaseOrderItems" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "PurchaseOrderId" UUID NOT NULL,
    "ProductId" UUID NOT NULL,
    "ProductVariationId" UUID,
    "Quantity" INTEGER NOT NULL CHECK (Quantity > 0),
    "UnitPrice" DECIMAL(18,2) NOT NULL CHECK (UnitPrice >= 0),
    "DiscountAmount" DECIMAL(18,2) NOT NULL DEFAULT 0.00 CHECK (DiscountAmount >= 0),
    "TotalPrice" DECIMAL(18,2) NOT NULL CHECK (TotalPrice >= 0),
    "ReceivedQuantity" INTEGER NOT NULL DEFAULT 0 CHECK (ReceivedQuantity >= 0),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Create indexes for better performance

-- Suppliers indexes
CREATE INDEX IF NOT EXISTS "IX_Suppliers_Code" ON "Suppliers"("Code");
CREATE INDEX IF NOT EXISTS "IX_Suppliers_Name" ON "Suppliers"("Name");
CREATE INDEX IF NOT EXISTS "IX_Suppliers_IsActive" ON "Suppliers"("IsActive");
CREATE INDEX IF NOT EXISTS "IX_Suppliers_Email" ON "Suppliers"("Email") WHERE "Email" IS NOT NULL;

-- PurchaseOrders indexes
CREATE INDEX IF NOT EXISTS "IX_PurchaseOrders_SupplierId" ON "PurchaseOrders"("SupplierId");
CREATE INDEX IF NOT EXISTS "IX_PurchaseOrders_OrderNumber" ON "PurchaseOrders"("OrderNumber");
CREATE INDEX IF NOT EXISTS "IX_PurchaseOrders_Status" ON "PurchaseOrders"("Status");
CREATE INDEX IF NOT EXISTS "IX_PurchaseOrders_OrderDate" ON "PurchaseOrders"("OrderDate");
CREATE INDEX IF NOT EXISTS "IX_PurchaseOrders_ExpectedDeliveryDate" ON "PurchaseOrders"("ExpectedDeliveryDate") WHERE "ExpectedDeliveryDate" IS NOT NULL;

-- PurchaseOrderItems indexes
CREATE INDEX IF NOT EXISTS "IX_PurchaseOrderItems_PurchaseOrderId" ON "PurchaseOrderItems"("PurchaseOrderId");
CREATE INDEX IF NOT EXISTS "IX_PurchaseOrderItems_ProductId" ON "PurchaseOrderItems"("ProductId");
CREATE INDEX IF NOT EXISTS "IX_PurchaseOrderItems_ProductVariationId" ON "PurchaseOrderItems"("ProductVariationId") WHERE "ProductVariationId" IS NOT NULL;

-- Create foreign key constraints

-- PurchaseOrders -> Suppliers
ALTER TABLE "PurchaseOrders" 
ADD CONSTRAINT "FK_PurchaseOrders_Suppliers_SupplierId" 
FOREIGN KEY ("SupplierId") REFERENCES "Suppliers"("Id") 
ON DELETE RESTRICT ON UPDATE CASCADE;

-- PurchaseOrderItems -> PurchaseOrders
ALTER TABLE "PurchaseOrderItems" 
ADD CONSTRAINT "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId" 
FOREIGN KEY ("PurchaseOrderId") REFERENCES "PurchaseOrders"("Id") 
ON DELETE CASCADE ON UPDATE CASCADE;

-- PurchaseOrderItems -> Products (assuming Products table exists)
ALTER TABLE "PurchaseOrderItems" 
ADD CONSTRAINT "FK_PurchaseOrderItems_Products_ProductId" 
FOREIGN KEY ("ProductId") REFERENCES "Products"("Id") 
ON DELETE RESTRICT ON UPDATE CASCADE;

-- PurchaseOrderItems -> ProductVariations (assuming ProductVariations table exists)
ALTER TABLE "PurchaseOrderItems" 
ADD CONSTRAINT "FK_PurchaseOrderItems_ProductVariations_ProductVariationId" 
FOREIGN KEY ("ProductVariationId") REFERENCES "ProductVariations"("Id") 
ON DELETE RESTRICT ON UPDATE CASCADE;

-- Add check constraints for data integrity

-- PurchaseOrders status check
ALTER TABLE "PurchaseOrders" 
ADD CONSTRAINT "CK_PurchaseOrders_Status" 
CHECK ("Status" IN ('Pending', 'Confirmed', 'Shipped', 'PartiallyReceived', 'Received', 'Cancelled'));

-- Suppliers rating check
ALTER TABLE "Suppliers" 
ADD CONSTRAINT "CK_Suppliers_Rating" 
CHECK ("Rating" IN ('A', 'B', 'C', 'D', 'Gold', 'Silver', 'Bronze') OR "Rating" IS NULL);

-- PurchaseOrderItems quantity check
ALTER TABLE "PurchaseOrderItems" 
ADD CONSTRAINT "CK_PurchaseOrderItems_ReceivedQuantity" 
CHECK ("ReceivedQuantity" <= "Quantity");

-- Create triggers for automatic timestamp updates

-- Update Suppliers.UpdatedAt timestamp
CREATE OR REPLACE FUNCTION update_supplier_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW."UpdatedAt" = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER "TR_Suppliers_UpdatedAt"
    BEFORE UPDATE ON "Suppliers"
    FOR EACH ROW
    EXECUTE FUNCTION update_supplier_updated_at();

-- Update PurchaseOrders.UpdatedAt timestamp
CREATE OR REPLACE FUNCTION update_purchase_order_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW."UpdatedAt" = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER "TR_PurchaseOrders_UpdatedAt"
    BEFORE UPDATE ON "PurchaseOrders"
    FOR EACH ROW
    EXECUTE FUNCTION update_purchase_order_updated_at();

-- Insert sample data for testing (optional - can be removed in production)

-- Sample Suppliers
INSERT INTO "Suppliers" ("Name", "Code", "ContactPerson", "Email", "Phone", "Address", "City", "Country", "TaxNumber", "PaymentTerms", "Website", "Rating", "IsActive", "Notes")
VALUES 
    ('Fashion Forward Inc.', 'FF001', 'John Smith', 'john@fashionforward.com', '+1-555-0101', '123 Fashion Ave', 'New York', 'USA', '12-3456789', 'NET 30', 'https://fashionforward.com', 'A', true, 'Premium clothing supplier'),
    ('Style Dynamics Ltd.', 'SD002', 'Sarah Johnson', 'sarah@styledynamics.com', '+1-555-0102', '456 Style Blvd', 'Los Angeles', 'USA', '98-7654321', 'NET 60', 'https://styledynamics.com', 'Gold', true, 'Trend-focused supplier'),
    ('Quality Textiles Co.', 'QT003', 'Mike Wilson', 'mike@qualitytextiles.com', '+1-555-0103', '789 Textile Rd', 'Chicago', 'USA', '45-1234567', 'NET 45', 'https://qualitytextiles.com', 'B', true, 'Fabric specialist')
ON CONFLICT ("Code") DO NOTHING;

-- Sample Purchase Orders
INSERT INTO "PurchaseOrders" ("SupplierId", "OrderNumber", "OrderDate", "ExpectedDeliveryDate", "Status", "TotalAmount", "Notes")
SELECT 
    s."Id",
    'PO' || TO_CHAR(NOW(), 'YYYYMMDD') || LPAD((ROW_NUMBER() OVER (ORDER BY s."Id") + 100)::text, 4, '0'),
    NOW(),
    NOW() + INTERVAL '14 days',
    'Pending',
    1500.00,
    'Sample purchase order for testing'
FROM "Suppliers" s
WHERE s."Code" = 'FF001'
LIMIT 1
ON CONFLICT ("OrderNumber") DO NOTHING;

-- Comments for documentation
COMMENT ON TABLE "Suppliers" IS 'Stores information about suppliers and vendors';
COMMENT ON TABLE "PurchaseOrders" IS 'Stores purchase orders placed with suppliers';
COMMENT ON TABLE "PurchaseOrderItems" IS 'Stores individual items within purchase orders';

COMMENT ON COLUMN "Suppliers"."Code" IS 'Unique supplier code for identification';
COMMENT ON COLUMN "Suppliers"."TaxNumber" IS 'Tax identification number for billing purposes';
COMMENT ON COLUMN "Suppliers"."PaymentTerms" IS 'Payment terms agreed with supplier (e.g., NET 30, NET 60)';
COMMENT ON COLUMN "Suppliers"."Rating" IS 'Supplier performance rating (A, B, C, D, Gold, Silver, Bronze)';

COMMENT ON COLUMN "PurchaseOrders"."OrderNumber" IS 'Unique purchase order number with PO prefix';
COMMENT ON COLUMN "PurchaseOrders"."Status" IS 'Current status of the purchase order';
COMMENT ON COLUMN "PurchaseOrders"."TotalAmount" IS 'Total value of the purchase order including all items';

COMMENT ON COLUMN "PurchaseOrderItems"."Quantity" IS 'Ordered quantity for this item';
COMMENT ON COLUMN "PurchaseOrderItems"."ReceivedQuantity" IS 'Quantity actually received from supplier';
COMMENT ON COLUMN "PurchaseOrderItems"."UnitPrice" IS 'Price per unit before discount';
COMMENT ON COLUMN "PurchaseOrderItems"."DiscountAmount" IS 'Discount amount applied to this item';
COMMENT ON COLUMN "PurchaseOrderItems"."TotalPrice" IS 'Total price for this item (Quantity * UnitPrice - DiscountAmount)';

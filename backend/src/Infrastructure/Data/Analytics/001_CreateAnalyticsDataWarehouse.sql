-- Migration: Create Analytics Data Warehouse Schema
-- Version: 001
-- Description: Creates data warehouse schema for advanced analytics and reporting

-- Create analytics schema
CREATE SCHEMA IF NOT EXISTS "analytics";

-- Create date dimension table for time-based analytics
CREATE TABLE IF NOT EXISTS "analytics"."DateDimension" (
    "DateKey" INTEGER PRIMARY KEY,
    "FullDate" DATE NOT NULL,
    "DayOfMonth" INTEGER NOT NULL,
    "DayOfYear" INTEGER NOT NULL,
    "WeekOfYear" INTEGER NOT NULL,
    "Month" INTEGER NOT NULL,
    "Quarter" INTEGER NOT NULL,
    "Year" INTEGER NOT NULL,
    "DayName" VARCHAR(10) NOT NULL,
    "MonthName" VARCHAR(10) NOT NULL,
    "QuarterName" VARCHAR(10) NOT NULL,
    "IsWeekend" BOOLEAN NOT NULL,
    "IsHoliday" BOOLEAN NOT NULL DEFAULT FALSE,
    "IsWeekday" BOOLEAN NOT NULL,
    "IsMonthEnd" BOOLEAN NOT NULL,
    "IsQuarterEnd" BOOLEAN NOT NULL,
    "IsYearEnd" BOOLEAN NOT NULL,
    "FiscalYear" INTEGER,
    "FiscalQuarter" INTEGER,
    "FiscalMonth" INTEGER,
    "Season" VARCHAR(10),
    "CreatedDate" TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Create product dimension table
CREATE TABLE IF NOT EXISTS "analytics"."ProductDimension" (
    "ProductKey" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "ProductId" UUID NOT NULL UNIQUE,
    "ProductName" VARCHAR(255) NOT NULL,
    "ProductSku" VARCHAR(100) NOT NULL,
    "ProductCategory" VARCHAR(100),
    "ProductSubcategory" VARCHAR(100),
    "Brand" VARCHAR(100),
    "Size" VARCHAR(50),
    "Color" VARCHAR(50),
    "Material" VARCHAR(100),
    "Price" DECIMAL(18,2),
    "Cost" DECIMAL(18,2),
    "Margin" DECIMAL(18,2),
    "Weight" DECIMAL(10,3),
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ModifiedDate" TIMESTAMP WITH TIME ZONE,
    "StartDate" DATE NOT NULL DEFAULT CURRENT_DATE,
    "EndDate" DATE,
    "IsCurrent" BOOLEAN NOT NULL DEFAULT TRUE
);

-- Create customer dimension table (Type 2 SCD)
CREATE TABLE IF NOT EXISTS "analytics"."CustomerDimension" (
    "CustomerKey" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "CustomerId" UUID NOT NULL,
    "CustomerFirstName" VARCHAR(100),
    "CustomerLastName" VARCHAR(100),
    "CustomerEmail" VARCHAR(255),
    "CustomerPhone" VARCHAR(20),
    "CustomerCity" VARCHAR(100),
    "CustomerState" VARCHAR(100),
    "CustomerCountry" VARCHAR(100),
    "CustomerPostalCode" VARCHAR(20),
    "CustomerSegment" VARCHAR(50),
    "CustomerTier" VARCHAR(20),
    "RegistrationDate" DATE,
    "BirthDate" DATE,
    "Gender" VARCHAR(10),
    "AgeGroup" VARCHAR(20),
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ModifiedDate" TIMESTAMP WITH TIME ZONE,
    "StartDate" DATE NOT NULL DEFAULT CURRENT_DATE,
    "EndDate" DATE,
    "IsCurrent" BOOLEAN NOT NULL DEFAULT TRUE,
    UNIQUE ("CustomerId", "StartDate")
);

-- Create location dimension table
CREATE TABLE IF NOT EXISTS "analytics"."LocationDimension" (
    "LocationKey" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "LocationId" UUID NOT NULL,
    "LocationName" VARCHAR(255) NOT NULL,
    "LocationType" VARCHAR(50) NOT NULL, -- 'Branch' or 'Warehouse'
    "LocationCode" VARCHAR(50),
    "Address" VARCHAR(500),
    "City" VARCHAR(100),
    "State" VARCHAR(100),
    "Country" VARCHAR(100),
    "PostalCode" VARCHAR(20),
    "Region" VARCHAR(100),
    "District" VARCHAR(100),
    "Latitude" DECIMAL(10,8),
    "Longitude" DECIMAL(11,8),
    "SquareFootage" INTEGER,
    "EmployeeCount" INTEGER,
    "OpenDate" DATE,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ModifiedDate" TIMESTAMP WITH TIME ZONE,
    "StartDate" DATE NOT NULL DEFAULT CURRENT_DATE,
    "EndDate" DATE,
    "IsCurrent" BOOLEAN NOT NULL DEFAULT TRUE,
    UNIQUE ("LocationId", "StartDate")
);

-- Create supplier dimension table
CREATE TABLE IF NOT EXISTS "analytics"."SupplierDimension" (
    "SupplierKey" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "SupplierId" UUID NOT NULL,
    "SupplierName" VARCHAR(255) NOT NULL,
    "SupplierCode" VARCHAR(50),
    "ContactPerson" VARCHAR(100),
    "Email" VARCHAR(255),
    "Phone" VARCHAR(20),
    "Address" VARCHAR(500),
    "City" VARCHAR(100),
    "State" VARCHAR(100),
    "Country" VARCHAR(100),
    "PostalCode" VARCHAR(20),
    "PaymentTerms" VARCHAR(50),
    "Rating" VARCHAR(20),
    "Category" VARCHAR(100),
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ModifiedDate" TIMESTAMP WITH TIME ZONE,
    "StartDate" DATE NOT NULL DEFAULT CURRENT_DATE,
    "EndDate" DATE,
    "IsCurrent" BOOLEAN NOT NULL DEFAULT TRUE,
    UNIQUE ("SupplierId", "StartDate")
);

-- Create employee dimension table
CREATE TABLE IF NOT EXISTS "analytics"."EmployeeDimension" (
    "EmployeeKey" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "EmployeeId" UUID NOT NULL,
    "EmployeeFirstName" VARCHAR(100),
    "EmployeeLastName" VARCHAR(100),
    "EmployeeEmail" VARCHAR(255),
    "EmployeePhone" VARCHAR(20),
    "JobTitle" VARCHAR(100),
    "Department" VARCHAR(100),
    "LocationId" UUID,
    "HireDate" DATE,
    "BirthDate" DATE,
    "Gender" VARCHAR(10),
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ModifiedDate" TIMESTAMP WITH TIME ZONE,
    "StartDate" DATE NOT NULL DEFAULT CURRENT_DATE,
    "EndDate" DATE,
    "IsCurrent" BOOLEAN NOT NULL DEFAULT TRUE,
    UNIQUE ("EmployeeId", "StartDate")
);

-- Create sales fact table
CREATE TABLE IF NOT EXISTS "analytics"."SalesFact" (
    "SalesKey" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DateKey" INTEGER NOT NULL,
    "ProductKey" UUID NOT NULL,
    "CustomerKey" UUID NOT NULL,
    "LocationKey" UUID NOT NULL,
    "EmployeeKey" UUID,
    "TransactionId" UUID NOT NULL,
    "TransactionNumber" VARCHAR(50),
    "TransactionType" VARCHAR(50),
    "Quantity" INTEGER NOT NULL,
    "UnitPrice" DECIMAL(18,2) NOT NULL,
    "TotalAmount" DECIMAL(18,2) NOT NULL,
    "DiscountAmount" DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    "TaxAmount" DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    "NetAmount" DECIMAL(18,2) NOT NULL,
    "CostAmount" DECIMAL(18,2) NOT NULL,
    "GrossProfit" DECIMAL(18,2) NOT NULL,
    "MarginPercentage" DECIMAL(5,2),
    "PaymentMethod" VARCHAR(50),
    "PaymentStatus" VARCHAR(20),
    "RefundAmount" DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    "ReturnQuantity" INTEGER NOT NULL DEFAULT 0,
    "CreatedDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ProcessDate" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    FOREIGN KEY ("DateKey") REFERENCES "analytics"."DateDimension"("DateKey"),
    FOREIGN KEY ("ProductKey") REFERENCES "analytics"."ProductDimension"("ProductKey"),
    FOREIGN KEY ("CustomerKey") REFERENCES "analytics"."CustomerDimension"("CustomerKey"),
    FOREIGN KEY ("LocationKey") REFERENCES "analytics"."LocationDimension"("LocationKey"),
    FOREIGN KEY ("EmployeeKey") REFERENCES "analytics"."EmployeeDimension"("EmployeeKey")
);

-- Create inventory fact table
CREATE TABLE IF NOT EXISTS "analytics"."InventoryFact" (
    "InventoryKey" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DateKey" INTEGER NOT NULL,
    "ProductKey" UUID NOT NULL,
    "LocationKey" UUID NOT NULL,
    "TransactionId" UUID,
    "TransactionType" VARCHAR(50), -- 'Purchase', 'Sale', 'Transfer', 'Adjustment', 'Return'
    "QuantityChange" INTEGER NOT NULL,
    "QuantityBefore" INTEGER NOT NULL,
    "QuantityAfter" INTEGER NOT NULL,
    "UnitCost" DECIMAL(18,2),
    "TotalCost" DECIMAL(18,2),
    "ReasonCode" VARCHAR(50),
    "ReferenceNumber" VARCHAR(50),
    "FromLocationKey" UUID,
    "ToLocationKey" UUID,
    "CreatedDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ProcessDate" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    FOREIGN KEY ("DateKey") REFERENCES "analytics"."DateDimension"("DateKey"),
    FOREIGN KEY ("ProductKey") REFERENCES "analytics"."ProductDimension"("ProductKey"),
    FOREIGN KEY ("LocationKey") REFERENCES "analytics"."LocationDimension"("LocationKey"),
    FOREIGN KEY ("FromLocationKey") REFERENCES "analytics"."LocationDimension"("LocationKey"),
    FOREIGN KEY ("ToLocationKey") REFERENCES "analytics"."LocationDimension"("LocationKey")
);

-- Create procurement fact table
CREATE TABLE IF NOT EXISTS "analytics"."ProcurementFact" (
    "ProcurementKey" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DateKey" INTEGER NOT NULL,
    "ProductKey" UUID NOT NULL,
    "SupplierKey" UUID NOT NULL,
    "LocationKey" UUID NOT NULL,
    "PurchaseOrderId" UUID NOT NULL,
    "OrderNumber" VARCHAR(50),
    "OrderStatus" VARCHAR(50),
    "Quantity" INTEGER NOT NULL,
    "UnitPrice" DECIMAL(18,2) NOT NULL,
    "TotalAmount" DECIMAL(18,2) NOT NULL,
    "DiscountAmount" DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    "NetAmount" DECIMAL(18,2) NOT NULL,
    "ReceivedQuantity" INTEGER NOT NULL DEFAULT 0,
    "PendingQuantity" INTEGER NOT NULL,
    "QualityIssues" INTEGER NOT NULL DEFAULT 0,
    "ExpectedDeliveryDate" DATE,
    "ActualDeliveryDate" DATE,
    "DeliveryDaysLate" INTEGER,
    "PaymentTerms" VARCHAR(50),
    "PaymentStatus" VARCHAR(20),
    "CreatedDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ProcessDate" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    FOREIGN KEY ("DateKey") REFERENCES "analytics"."DateDimension"("DateKey"),
    FOREIGN KEY ("ProductKey") REFERENCES "analytics"."ProductDimension"("ProductKey"),
    FOREIGN KEY ("SupplierKey") REFERENCES "analytics"."SupplierDimension"("SupplierKey"),
    FOREIGN KEY ("LocationKey") REFERENCES "analytics"."LocationDimension"("LocationKey")
);

-- Create customer loyalty fact table
CREATE TABLE IF NOT EXISTS "analytics"."CustomerLoyaltyFact" (
    "LoyaltyKey" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DateKey" INTEGER NOT NULL,
    "CustomerKey" UUID NOT NULL,
    "LocationKey" UUID NOT NULL,
    "TransactionId" UUID,
    "PointsEarned" INTEGER NOT NULL DEFAULT 0,
    "PointsRedeemed" INTEGER NOT NULL DEFAULT 0,
    "PointsBalance" INTEGER NOT NULL,
    "TierLevel" VARCHAR(20),
    "PurchaseAmount" DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    "RewardType" VARCHAR(50),
    "RewardValue" DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    "CreatedDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ProcessDate" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    FOREIGN KEY ("DateKey") REFERENCES "analytics"."DateDimension"("DateKey"),
    FOREIGN KEY ("CustomerKey") REFERENCES "analytics"."CustomerDimension"("CustomerKey"),
    FOREIGN KEY ("LocationKey") REFERENCES "analytics"."LocationDimension"("LocationKey")
);

-- Create financial fact table
CREATE TABLE IF NOT EXISTS "analytics"."FinancialFact" (
    "FinancialKey" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DateKey" INTEGER NOT NULL,
    "LocationKey" UUID NOT NULL,
    "AccountType" VARCHAR(50) NOT NULL, -- 'Revenue', 'Expense', 'Asset', 'Liability', 'Equity'
    "AccountCategory" VARCHAR(100) NOT NULL,
    "AccountSubcategory" VARCHAR(100),
    "TransactionType" VARCHAR(50),
    "ReferenceNumber" VARCHAR(50),
    "Description" VARCHAR(500),
    "DebitAmount" DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    "CreditAmount" DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    "NetAmount" DECIMAL(18,2) NOT NULL,
    "Currency" VARCHAR(3) NOT NULL DEFAULT 'USD',
    "ExchangeRate" DECIMAL(10,6) NOT NULL DEFAULT 1.000000,
    "BudgetAmount" DECIMAL(18,2),
    "VarianceAmount" DECIMAL(18,2),
    "VariancePercentage" DECIMAL(5,2),
    "CreatedDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ProcessDate" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    FOREIGN KEY ("DateKey") REFERENCES "analytics"."DateDimension"("DateKey"),
    FOREIGN KEY ("LocationKey") REFERENCES "analytics"."LocationDimension"("LocationKey")
);

-- Create performance metrics table
CREATE TABLE IF NOT EXISTS "analytics"."PerformanceMetrics" (
    "MetricKey" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "DateKey" INTEGER NOT NULL,
    "LocationKey" UUID,
    "MetricType" VARCHAR(50) NOT NULL, -- 'Sales', 'Inventory', 'Customer', 'Financial', 'Operational'
    "MetricName" VARCHAR(100) NOT NULL,
    "MetricValue" DECIMAL(18,2) NOT NULL,
    "MetricUnit" VARCHAR(20),
    "TargetValue" DECIMAL(18,2),
    "Variance" DECIMAL(18,2),
    "VariancePercentage" DECIMAL(5,2),
    "TrendDirection" VARCHAR(10), -- 'Up', 'Down', 'Flat'
    "TrendPercentage" DECIMAL(5,2),
    "Ranking" INTEGER,
    "Percentile" DECIMAL(5,2),
    "CreatedDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ProcessDate" TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    FOREIGN KEY ("DateKey") REFERENCES "analytics"."DateDimension"("DateKey"),
    FOREIGN KEY ("LocationKey") REFERENCES "analytics"."LocationDimension"("LocationKey")
);

-- Create indexes for performance optimization

-- Date dimension indexes
CREATE INDEX IF NOT EXISTS "IX_DateDimension_FullDate" ON "analytics"."DateDimension"("FullDate");
CREATE INDEX IF NOT EXISTS "IX_DateDimension_Year" ON "analytics"."DateDimension"("Year");
CREATE INDEX IF NOT EXISTS "IX_DateDimension_Month" ON "analytics"."DateDimension"("Month");
CREATE INDEX IF NOT EXISTS "IX_DateDimension_Quarter" ON "analytics"."DateDimension"("Quarter");

-- Dimension table indexes
CREATE INDEX IF NOT EXISTS "IX_ProductDimension_ProductId" ON "analytics"."ProductDimension"("ProductId");
CREATE INDEX IF NOT EXISTS "IX_ProductDimension_Category" ON "analytics"."ProductDimension"("ProductCategory");
CREATE INDEX IF NOT EXISTS "IX_ProductDimension_Brand" ON "analytics"."ProductDimension"("Brand");

CREATE INDEX IF NOT EXISTS "IX_CustomerDimension_CustomerId" ON "analytics"."CustomerDimension"("CustomerId");
CREATE INDEX IF NOT EXISTS "IX_CustomerDimension_Segment" ON "analytics"."CustomerDimension"("CustomerSegment");
CREATE INDEX IF NOT EXISTS "IX_CustomerDimension_Tier" ON "analytics"."CustomerDimension"("CustomerTier");

CREATE INDEX IF NOT EXISTS "IX_LocationDimension_LocationId" ON "analytics"."LocationDimension"("LocationId");
CREATE INDEX IF NOT EXISTS "IX_LocationDimension_Type" ON "analytics"."LocationDimension"("LocationType");
CREATE INDEX IF NOT EXISTS "IX_LocationDimension_City" ON "analytics"."LocationDimension"("City");

CREATE INDEX IF NOT EXISTS "IX_SupplierDimension_SupplierId" ON "analytics"."SupplierDimension"("SupplierId");
CREATE INDEX IF NOT EXISTS "IX_SupplierDimension_Rating" ON "analytics"."SupplierDimension"("Rating");

CREATE INDEX IF NOT EXISTS "IX_EmployeeDimension_EmployeeId" ON "analytics"."EmployeeDimension"("EmployeeId");
CREATE INDEX IF NOT EXISTS "IX_EmployeeDimension_Department" ON "analytics"."EmployeeDimension"("Department");

-- Fact table indexes
CREATE INDEX IF NOT EXISTS "IX_SalesFact_DateKey" ON "analytics"."SalesFact"("DateKey");
CREATE INDEX IF NOT EXISTS "IX_SalesFact_ProductKey" ON "analytics"."SalesFact"("ProductKey");
CREATE INDEX IF NOT EXISTS "IX_SalesFact_CustomerKey" ON "analytics"."SalesFact"("CustomerKey");
CREATE INDEX IF NOT EXISTS "IX_SalesFact_LocationKey" ON "analytics"."SalesFact"("LocationKey");
CREATE INDEX IF NOT EXISTS "IX_SalesFact_TransactionDate" ON "analytics"."SalesFact"("CreatedDate");

CREATE INDEX IF NOT EXISTS "IX_InventoryFact_DateKey" ON "analytics"."InventoryFact"("DateKey");
CREATE INDEX IF NOT EXISTS "IX_InventoryFact_ProductKey" ON "analytics"."InventoryFact"("ProductKey");
CREATE INDEX IF NOT EXISTS "IX_InventoryFact_LocationKey" ON "analytics"."InventoryFact"("LocationKey");
CREATE INDEX IF NOT EXISTS "IX_InventoryFact_TransactionType" ON "analytics"."InventoryFact"("TransactionType");

CREATE INDEX IF NOT EXISTS "IX_ProcurementFact_DateKey" ON "analytics"."ProcurementFact"("DateKey");
CREATE INDEX IF NOT EXISTS "IX_ProcurementFact_SupplierKey" ON "analytics"."ProcurementFact"("SupplierKey");
CREATE INDEX IF NOT EXISTS "IX_ProcurementFact_LocationKey" ON "analytics"."ProcurementFact"("LocationKey");
CREATE INDEX IF NOT EXISTS "IX_ProcurementFact_OrderStatus" ON "analytics"."ProcurementFact"("OrderStatus");

CREATE INDEX IF NOT EXISTS "IX_CustomerLoyaltyFact_DateKey" ON "analytics"."CustomerLoyaltyFact"("DateKey");
CREATE INDEX IF NOT EXISTS "IX_CustomerLoyaltyFact_CustomerKey" ON "analytics"."CustomerLoyaltyFact"("CustomerKey");
CREATE INDEX IF NOT EXISTS "IX_CustomerLoyaltyFact_LocationKey" ON "analytics"."CustomerLoyaltyFact"("LocationKey");

CREATE INDEX IF NOT EXISTS "IX_FinancialFact_DateKey" ON "analytics"."FinancialFact"("DateKey");
CREATE INDEX IF NOT EXISTS "IX_FinancialFact_LocationKey" ON "analytics"."FinancialFact"("LocationKey");
CREATE INDEX IF NOT EXISTS "IX_FinancialFact_AccountType" ON "analytics"."FinancialFact"("AccountType");

CREATE INDEX IF NOT EXISTS "IX_PerformanceMetrics_DateKey" ON "analytics"."PerformanceMetrics"("DateKey");
CREATE INDEX IF NOT EXISTS "IX_PerformanceMetrics_MetricType" ON "analytics"."PerformanceMetrics"("MetricType");
CREATE INDEX IF NOT EXISTS "IX_PerformanceMetrics_MetricName" ON "analytics"."PerformanceMetrics"("MetricName");

-- Create composite indexes for common query patterns
CREATE INDEX IF NOT EXISTS "IX_SalesFact_Date_Product_Location" ON "analytics"."SalesFact"("DateKey", "ProductKey", "LocationKey");
CREATE INDEX IF NOT EXISTS "IX_InventoryFact_Date_Product_Location" ON "analytics"."InventoryFact"("DateKey", "ProductKey", "LocationKey");
CREATE INDEX IF NOT EXISTS "IX_PerformanceMetrics_Date_Type_Location" ON "analytics"."PerformanceMetrics"("DateKey", "MetricType", "LocationKey");

-- Create views for common analytics queries

-- Daily sales summary view
CREATE OR REPLACE VIEW "analytics"."DailySalesSummary" AS
SELECT 
    d.FullDate,
    d.Year,
    d.Month,
    d.Quarter,
    l.LocationName,
    l.LocationType,
    COUNT(*) AS TransactionCount,
    SUM(sf.Quantity) AS TotalQuantity,
    SUM(sf.TotalAmount) AS GrossSales,
    SUM(sf.DiscountAmount) AS TotalDiscount,
    SUM(sf.NetAmount) AS NetSales,
    SUM(sf.GrossProfit) AS GrossProfit,
    AVG(sf.MarginPercentage) AS AverageMargin,
    sf.PaymentMethod
FROM "analytics"."SalesFact" sf
JOIN "analytics"."DateDimension" d ON sf.DateKey = d.DateKey
JOIN "analytics"."LocationDimension" l ON sf.LocationKey = l.LocationKey
GROUP BY d.FullDate, d.Year, d.Month, d.Quarter, l.LocationName, l.LocationType, sf.PaymentMethod;

-- Product performance view
CREATE OR REPLACE VIEW "analytics"."ProductPerformance" AS
SELECT 
    p.ProductName,
    p.ProductSku,
    p.ProductCategory,
    p.Brand,
    COUNT(sf.SalesKey) AS TransactionCount,
    SUM(sf.Quantity) AS TotalQuantity,
    SUM(sf.TotalAmount) AS GrossSales,
    SUM(sf.NetAmount) AS NetSales,
    SUM(sf.GrossProfit) AS GrossProfit,
    AVG(sf.MarginPercentage) AS AverageMargin,
    RANK() OVER (ORDER BY SUM(sf.NetAmount) DESC) AS SalesRank,
    RANK() OVER (ORDER BY SUM(sf.Quantity) DESC) AS QuantityRank
FROM "analytics"."SalesFact" sf
JOIN "analytics"."ProductDimension" p ON sf.ProductKey = p.ProductKey
GROUP BY p.ProductName, p.ProductSku, p.ProductCategory, p.Brand;

-- Customer segment analysis view
CREATE OR REPLACE VIEW "analytics"."CustomerSegmentAnalysis" AS
SELECT 
    cd.CustomerSegment,
    cd.CustomerTier,
    COUNT(DISTINCT cd.CustomerId) AS CustomerCount,
    COUNT(sf.SalesKey) AS TransactionCount,
    SUM(sf.NetAmount) AS TotalSales,
    AVG(sf.NetAmount) AS AverageSaleValue,
    MAX(sf.CreatedDate) AS LastPurchaseDate,
    AVG(sf.Quantity) AS AverageQuantity
FROM "analytics"."CustomerDimension" cd
LEFT JOIN "analytics"."SalesFact" sf ON cd.CustomerKey = sf.CustomerKey
GROUP BY cd.CustomerSegment, cd.CustomerTier;

-- Inventory turnover view
CREATE OR REPLACE VIEW "analytics"."InventoryTurnover" AS
SELECT 
    p.ProductName,
    p.ProductSku,
    p.ProductCategory,
    l.LocationName,
    SUM(CASE WHEN inv.TransactionType = 'Sale' THEN inv.QuantityChange ELSE 0 END) AS SoldQuantity,
    SUM(CASE WHEN inv.TransactionType = 'Purchase' THEN inv.QuantityChange ELSE 0 END) AS PurchasedQuantity,
    AVG(inv.UnitCost) AS AverageCost,
    SUM(inv.TotalCost) AS TotalCost,
    (SUM(CASE WHEN inv.TransactionType = 'Sale' THEN inv.QuantityChange ELSE 0 END) * 365.0 / 
     NULLIF(SUM(CASE WHEN inv.TransactionType = 'Purchase' THEN inv.QuantityChange ELSE 0 END), 0)) AS TurnoverRatio
FROM "analytics"."InventoryFact" inv
JOIN "analytics"."ProductDimension" p ON inv.ProductKey = p.ProductKey
JOIN "analytics"."LocationDimension" l ON inv.LocationKey = l.LocationKey
GROUP BY p.ProductName, p.ProductSku, p.ProductCategory, l.LocationName;

-- Supplier performance view
CREATE OR REPLACE VIEW "analytics"."SupplierPerformance" AS
SELECT 
    s.SupplierName,
    s.SupplierCode,
    s.Rating,
    COUNT(pf.ProcurementKey) AS OrderCount,
    SUM(pf.Quantity) AS TotalQuantity,
    SUM(pf.NetAmount) AS TotalValue,
    AVG(pf.UnitPrice) AS AverageUnitPrice,
    AVG(pf.DeliveryDaysLate) AS AverageDeliveryDays,
    SUM(pf.QualityIssues) AS TotalQualityIssues,
    RANK() OVER (ORDER BY SUM(pf.NetAmount) DESC) AS ValueRank
FROM "analytics"."ProcurementFact" pf
JOIN "analytics"."SupplierDimension" s ON pf.SupplierKey = s.SupplierKey
GROUP BY s.SupplierName, s.SupplierCode, s.Rating;

-- Financial summary view
CREATE OR REPLACE VIEW "analytics"."FinancialSummary" AS
SELECT 
    d.FullDate,
    d.Year,
    d.Month,
    d.Quarter,
    l.LocationName,
    ff.AccountType,
    ff.AccountCategory,
    SUM(ff.DebitAmount) AS TotalDebits,
    SUM(ff.CreditAmount) AS TotalCredits,
    SUM(ff.NetAmount) AS NetAmount,
    SUM(ff.BudgetAmount) AS BudgetAmount,
    SUM(ff.VarianceAmount) AS VarianceAmount,
    AVG(ff.VariancePercentage) AS AverageVariancePercentage
FROM "analytics"."FinancialFact" ff
JOIN "analytics"."DateDimension" d ON ff.DateKey = d.DateKey
JOIN "analytics"."LocationDimension" l ON ff.LocationKey = l.LocationKey
GROUP BY d.FullDate, d.Year, d.Month, d.Quarter, l.LocationName, ff.AccountType, ff.AccountCategory;

-- Comments for documentation
COMMENT ON SCHEMA "analytics" IS 'Data warehouse schema for advanced analytics and business intelligence';

COMMENT ON TABLE "analytics"."DateDimension" IS 'Date dimension table for time-based analytics and reporting';
COMMENT ON TABLE "analytics"."ProductDimension" IS 'Product dimension table with slowly changing dimensions';
COMMENT ON TABLE "analytics"."CustomerDimension" IS 'Customer dimension table with Type 2 SCD for tracking changes';
COMMENT ON TABLE "analytics"."LocationDimension" IS 'Location dimension table for branches and warehouses';
COMMENT ON TABLE "analytics"."SupplierDimension" IS 'Supplier dimension table with slowly changing dimensions';
COMMENT ON TABLE "analytics"."EmployeeDimension" IS 'Employee dimension table for sales and operations tracking';

COMMENT ON TABLE "analytics"."SalesFact" IS 'Sales fact table for transactional data analysis';
COMMENT ON TABLE "analytics"."InventoryFact" IS 'Inventory fact table for stock movement analysis';
COMMENT ON TABLE "analytics"."ProcurementFact" IS 'Procurement fact table for purchase order analysis';
COMMENT ON TABLE "analytics"."CustomerLoyaltyFact" IS 'Customer loyalty fact table for loyalty program analysis';
COMMENT ON TABLE "analytics"."FinancialFact" IS 'Financial fact table for accounting and financial analysis';
COMMENT ON TABLE "analytics"."PerformanceMetrics" IS 'Performance metrics table for KPI tracking';

-- Create stored procedures for data loading

-- Procedure to populate date dimension
CREATE OR REPLACE FUNCTION "analytics"."PopulateDateDimension"(start_date DATE, end_date DATE)
RETURNS VOID AS $$
BEGIN
    -- This procedure would populate the date dimension table
    -- Implementation would generate all dates between start_date and end_date
    -- with appropriate attributes like day names, months, quarters, etc.
    
    -- Placeholder for actual implementation
    RAISE NOTICE 'Date dimension population procedure called for % to %', start_date, end_date;
END;
$$ LANGUAGE plpgsql;

-- Procedure to load sales data
CREATE OR REPLACE FUNCTION "analytics"."LoadSalesData"(target_date DATE)
RETURNS VOID AS $$
BEGIN
    -- This procedure would load sales data from operational tables
    -- into the data warehouse fact and dimension tables
    
    -- Placeholder for actual implementation
    RAISE NOTICE 'Sales data load procedure called for %', target_date;
END;
$$ LANGUAGE plpgsql;

-- Procedure to calculate performance metrics
CREATE OR REPLACE FUNCTION "analytics"."CalculatePerformanceMetrics"(target_date DATE)
RETURNS VOID AS $$
BEGIN
    -- This procedure would calculate and store performance metrics
    -- based on the fact table data
    
    -- Placeholder for actual implementation
    RAISE NOTICE 'Performance metrics calculation procedure called for %', target_date;
END;
$$ LANGUAGE plpgsql;

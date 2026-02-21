# System Integrity Audit Report

## Production Readiness Verdict: **FAIL**

> **Audit Status:** Critical mismatches and unimplemented logic detected. Deployment not recommended.

## 1. Backend Feature Map
### Controller: BaseController
### Controller: CategoriesController
- **Endpoint:** `GetCategories`
  - Route: `/api/Categories`
  - Method: `HttpGet`
  - Request DTO: `Query:bool`
  - Response DTO: `IEnumerable<Category`
  - Used In Frontend: Yes

- **Endpoint:** `GetCategory`
  - Route: `/api/Categories/{id}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `Category`
  - Used In Frontend: Yes

- **Endpoint:** `GetRootCategories`
  - Route: `/api/Categories/root`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<Category`
  - Used In Frontend: Yes

- **Endpoint:** `GetChildCategories`
  - Route: `/api/Categories/{parentId}/children`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<Category`
  - Used In Frontend: Yes

- **Endpoint:** `CreateCategory`
  - Route: `/api/Categories`
  - Method: `HttpPost`
  - Request DTO: `CreateCategoryRequest`
  - Response DTO: `Category`
  - Used In Frontend: Yes

- **Endpoint:** `UpdateCategory`
  - Route: `/api/Categories/{id}`
  - Method: `HttpPut`
  - Request DTO: `UpdateCategoryRequest`
  - Response DTO: `Category`
  - Used In Frontend: Yes

- **Endpoint:** `DeleteCategory`
  - Route: `/api/Categories/{id}`
  - Method: `HttpDelete`
  - Request DTO: `None`
  - Response DTO: `IActionResult`
  - Used In Frontend: Yes

- **Endpoint:** `ValidateCategoryDeletion`
  - Route: `/api/Categories/{id}/validate-deletion`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `bool`
  - Used In Frontend: Yes

### Controller: CustomersController
- **Endpoint:** `GetCustomer`
  - Route: `/api/Customers/{id}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `Customer`
  - Used In Frontend: Yes

- **Endpoint:** `GetCustomers`
  - Route: `/api/Customers`
  - Method: `HttpGet`
  - Request DTO: `Query:int`
  - Response DTO: `IEnumerable<Customer`
  - Used In Frontend: Yes

- **Endpoint:** `SearchCustomer`
  - Route: `/api/Customers/search`
  - Method: `HttpGet`
  - Request DTO: `Query:string`
  - Response DTO: `Customer`
  - Used In Frontend: Yes

- **Endpoint:** `CreateCustomer`
  - Route: `/api/Customers`
  - Method: `HttpPost`
  - Request DTO: `CreateCustomerRequest`
  - Response DTO: `Customer`
  - Used In Frontend: Yes

- **Endpoint:** `UpdateCustomer`
  - Route: `/api/Customers/{id}`
  - Method: `HttpPut`
  - Request DTO: `UpdateCustomerRequest`
  - Response DTO: `Customer`
  - Used In Frontend: Yes

- **Endpoint:** `DeleteCustomer`
  - Route: `/api/Customers/{id}`
  - Method: `HttpDelete`
  - Request DTO: `None`
  - Response DTO: `None`
  - Used In Frontend: Yes

- **Endpoint:** `GetCustomerLoyalty`
  - Route: `/api/Customers/{id}/loyalty`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `CustomerLoyalty`
  - Used In Frontend: Yes

- **Endpoint:** `AddLoyaltyPoints`
  - Route: `/api/Customers/{id}/loyalty/add-points`
  - Method: `HttpPost`
  - Request DTO: `AddLoyaltyPointsRequest`
  - Response DTO: `CustomerLoyalty`
  - Used In Frontend: Yes

- **Endpoint:** `RedeemLoyaltyPoints`
  - Route: `/api/Customers/{id}/loyalty/redeem-points`
  - Method: `HttpPost`
  - Request DTO: `RedeemLoyaltyPointsRequest`
  - Response DTO: `CustomerLoyalty`
  - Used In Frontend: Yes

### Controller: FilesController
- **Endpoint:** `UploadFile`
  - Route: `/api/Files/upload`
  - Method: `HttpPost`
  - Request DTO: `None`
  - Response DTO: `FileUploadResult`
  - Used In Frontend: Yes

- **Endpoint:** `UploadFiles`
  - Route: `/api/Files/upload-multiple`
  - Method: `HttpPost`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<FileUploadResult`
  - Used In Frontend: Yes

- **Endpoint:** `DeleteFile`
  - Route: `/api/Files`
  - Method: `HttpDelete`
  - Request DTO: `DeleteFileRequest`
  - Response DTO: `None`
  - Used In Frontend: Yes

- **Endpoint:** `GetFileInfo`
  - Route: `/api/Files/info`
  - Method: `HttpGet`
  - Request DTO: `Query:string`
  - Response DTO: `None`
  - Used In Frontend: Yes

- **Endpoint:** `FileExists`
  - Route: `/api/Files/exists`
  - Method: `HttpGet`
  - Request DTO: `Query:string`
  - Response DTO: `bool`
  - Used In Frontend: Yes

### Controller: HealthCheckController
### Controller: HealthController
- **Endpoint:** `Get`
  - Route: `/api/v{version:apiVersion}/Health`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IActionResult`
  - Used In Frontend: Yes

- **Endpoint:** `GetReadiness`
  - Route: `/api/v{version:apiVersion}/Health/readiness`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IActionResult`
  - Used In Frontend: Yes

- **Endpoint:** `GetDetailedHealth`
  - Route: `/api/v{version:apiVersion}/Health/detailed`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IActionResult`
  - Used In Frontend: Yes

### Controller: InventoryController
- **Endpoint:** `GetInventory`
  - Route: `/api/Inventory/{id}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `Inventory`
  - Used In Frontend: Yes

- **Endpoint:** `GetInventoryByProduct`
  - Route: `/api/Inventory/by-product/{productId}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<Inventory`
  - Used In Frontend: Yes

- **Endpoint:** `GetInventoryByProductVariation`
  - Route: `/api/Inventory/by-variation/{productVariationId}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<Inventory`
  - Used In Frontend: Yes

- **Endpoint:** `GetInventoryByBranch`
  - Route: `/api/Inventory/by-branch/{branchId}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<Inventory`
  - Used In Frontend: Yes

- **Endpoint:** `GetInventoryByWarehouse`
  - Route: `/api/Inventory/by-warehouse/{warehouseId}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<Inventory`
  - Used In Frontend: Yes

- **Endpoint:** `GetInventoryByLocation`
  - Route: `/api/Inventory/by-location`
  - Method: `HttpGet`
  - Request DTO: `Query:Guid`
  - Response DTO: `Inventory?`
  - Used In Frontend: Yes

- **Endpoint:** `CreateInventory`
  - Route: `/api/Inventory`
  - Method: `HttpPost`
  - Request DTO: `None`
  - Response DTO: `Inventory`
  - Used In Frontend: Yes

- **Endpoint:** `UpdateInventory`
  - Route: `/api/Inventory/{id}`
  - Method: `HttpPut`
  - Request DTO: `None`
  - Response DTO: `Inventory`
  - Used In Frontend: Yes

- **Endpoint:** `DeleteInventory`
  - Route: `/api/Inventory/{id}`
  - Method: `HttpDelete`
  - Request DTO: `None`
  - Response DTO: `None`
  - Used In Frontend: Yes

- **Endpoint:** `ReserveInventory`
  - Route: `/api/Inventory/{id}/reserve`
  - Method: `HttpPost`
  - Request DTO: `None`
  - Response DTO: `None`
  - Used In Frontend: Yes

- **Endpoint:** `ReleaseInventory`
  - Route: `/api/Inventory/{id}/release`
  - Method: `HttpPost`
  - Request DTO: `None`
  - Response DTO: `None`
  - Used In Frontend: Yes

- **Endpoint:** `UpdateStock`
  - Route: `/api/Inventory/{id}/stock`
  - Method: `HttpPut`
  - Request DTO: `UpdateStockRequest`
  - Response DTO: `Inventory`
  - Used In Frontend: Yes

- **Endpoint:** `TransferInventory`
  - Route: `/api/Inventory/transfer`
  - Method: `HttpPost`
  - Request DTO: `None`
  - Response DTO: `InventoryTransferResult`
  - Used In Frontend: Yes

- **Endpoint:** `BulkTransferInventory`
  - Route: `/api/Inventory/bulk-transfer`
  - Method: `HttpPost`
  - Request DTO: `List<TransferInventoryRequest>`
  - Response DTO: `InventoryTransferResult`
  - Used In Frontend: Yes

- **Endpoint:** `AdjustInventory`
  - Route: `/api/Inventory/{id}/adjust`
  - Method: `HttpPut`
  - Request DTO: `None`
  - Response DTO: `Inventory`
  - Used In Frontend: Yes

- **Endpoint:** `BulkAdjustInventory`
  - Route: `/api/Inventory/bulk-adjust`
  - Method: `HttpPut`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<Inventory`
  - Used In Frontend: Yes

- **Endpoint:** `SearchInventory`
  - Route: `/api/Inventory/search`
  - Method: `HttpPost`
  - Request DTO: `None`
  - Response DTO: `PagedResult<Inventory`
  - Used In Frontend: Yes

- **Endpoint:** `GetLowStockItems`
  - Route: `/api/Inventory/low-stock`
  - Method: `HttpGet`
  - Request DTO: `Query:int`
  - Response DTO: `IEnumerable<Inventory`
  - Used In Frontend: Yes

- **Endpoint:** `GetOutOfStockItems`
  - Route: `/api/Inventory/out-of-stock`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<Inventory`
  - Used In Frontend: Yes

- **Endpoint:** `GetInventoryStatistics`
  - Route: `/api/Inventory/statistics`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `InventoryStatistics`
  - Used In Frontend: Yes

- **Endpoint:** `GetInventoryValueByLocation`
  - Route: `/api/Inventory/value-by-location`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<LocationInventoryValue`
  - Used In Frontend: Yes

- **Endpoint:** `GenerateInventoryReport`
  - Route: `/api/Inventory/report`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `InventoryReport`
  - Used In Frontend: Yes

- **Endpoint:** `CreateTransaction`
  - Route: `/api/Inventory/transaction`
  - Method: `HttpPost`
  - Request DTO: `None`
  - Response DTO: `InventoryTransaction`
  - Used In Frontend: Yes

- **Endpoint:** `GetTransaction`
  - Route: `/api/Inventory/transaction/{id}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `InventoryTransaction`
  - Used In Frontend: Yes

- **Endpoint:** `SearchTransactions`
  - Route: `/api/Inventory/transaction/search`
  - Method: `HttpPost`
  - Request DTO: `None`
  - Response DTO: `PagedResult<InventoryTransaction`
  - Used In Frontend: Yes

- **Endpoint:** `GetRecentTransactions`
  - Route: `/api/Inventory/transaction/recent`
  - Method: `HttpGet`
  - Request DTO: `Query:int`
  - Response DTO: `IEnumerable<InventoryTransaction`
  - Used In Frontend: Yes

- **Endpoint:** `GetLowStockAlerts`
  - Route: `/api/Inventory/alerts/low-stock`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<LowStockAlert`
  - Used In Frontend: Yes

- **Endpoint:** `SendLowStockAlerts`
  - Route: `/api/Inventory/alerts/low-stock/send`
  - Method: `HttpPost`
  - Request DTO: `None`
  - Response DTO: `None`
  - Used In Frontend: Yes

### Controller: ProcurementController
- **Endpoint:** `GetSupplier`
  - Route: `/api/Procurement/suppliers/{id}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `Supplier`
  - Used In Frontend: Yes

- **Endpoint:** `GetSupplierByCode`
  - Route: `/api/Procurement/suppliers/by-code/{code}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `Supplier`
  - Used In Frontend: Yes

- **Endpoint:** `GetSuppliers`
  - Route: `/api/Procurement/suppliers`
  - Method: `HttpGet`
  - Request DTO: `Query:int`
  - Response DTO: `IEnumerable<Supplier`
  - Used In Frontend: Yes

- **Endpoint:** `CreateSupplier`
  - Route: `/api/Procurement/suppliers`
  - Method: `HttpPost`
  - Request DTO: `CreateSupplierRequest`
  - Response DTO: `Supplier`
  - Used In Frontend: Yes

- **Endpoint:** `UpdateSupplier`
  - Route: `/api/Procurement/suppliers/{id}`
  - Method: `HttpPut`
  - Request DTO: `UpdateSupplierRequest`
  - Response DTO: `Supplier`
  - Used In Frontend: Yes

- **Endpoint:** `DeleteSupplier`
  - Route: `/api/Procurement/suppliers/{id}`
  - Method: `HttpDelete`
  - Request DTO: `None`
  - Response DTO: `None`
  - Used In Frontend: Yes

- **Endpoint:** `GetPurchaseOrder`
  - Route: `/api/Procurement/purchase-orders/{id}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `PurchaseOrder`
  - Used In Frontend: Yes

- **Endpoint:** `GetPurchaseOrderByNumber`
  - Route: `/api/Procurement/purchase-orders/by-number/{orderNumber}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `PurchaseOrder`
  - Used In Frontend: Yes

- **Endpoint:** `GetPurchaseOrders`
  - Route: `/api/Procurement/purchase-orders`
  - Method: `HttpGet`
  - Request DTO: `Query:int`
  - Response DTO: `IEnumerable<PurchaseOrder`
  - Used In Frontend: Yes

- **Endpoint:** `GetPurchaseOrdersBySupplier`
  - Route: `/api/Procurement/purchase-orders/by-supplier/{supplierId}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<PurchaseOrder`
  - Used In Frontend: Yes

- **Endpoint:** `GetPurchaseOrdersByStatus`
  - Route: `/api/Procurement/purchase-orders/by-status/{status}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<PurchaseOrder`
  - Used In Frontend: Yes

- **Endpoint:** `CreatePurchaseOrder`
  - Route: `/api/Procurement/purchase-orders`
  - Method: `HttpPost`
  - Request DTO: `CreatePurchaseOrderRequest`
  - Response DTO: `PurchaseOrder`
  - Used In Frontend: Yes

- **Endpoint:** `UpdatePurchaseOrder`
  - Route: `/api/Procurement/purchase-orders/{id}`
  - Method: `HttpPut`
  - Request DTO: `UpdatePurchaseOrderRequest`
  - Response DTO: `PurchaseOrder`
  - Used In Frontend: Yes

- **Endpoint:** `DeletePurchaseOrder`
  - Route: `/api/Procurement/purchase-orders/{id}`
  - Method: `HttpDelete`
  - Request DTO: `None`
  - Response DTO: `None`
  - Used In Frontend: Yes

### Controller: ProductsController
- **Endpoint:** `GetProducts`
  - Route: `/api/Products`
  - Method: `HttpGet`
  - Request DTO: `Query:int`
  - Response DTO: `None`
  - Used In Frontend: Yes

- **Endpoint:** `SearchProducts`
  - Route: `/api/Products/search`
  - Method: `HttpGet`
  - Request DTO: `Query:string`
  - Response DTO: `None`
  - Used In Frontend: Yes

- **Endpoint:** `GetProduct`
  - Route: `/api/Products/{id}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `Product`
  - Used In Frontend: Yes

- **Endpoint:** `GetProductBySku`
  - Route: `/api/Products/by-sku/{sku}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `Product`
  - Used In Frontend: Yes

- **Endpoint:** `CreateProduct`
  - Route: `/api/Products`
  - Method: `HttpPost`
  - Request DTO: `CreateProductRequest`
  - Response DTO: `Product`
  - Used In Frontend: Yes

- **Endpoint:** `UpdateProduct`
  - Route: `/api/Products/{id}`
  - Method: `HttpPut`
  - Request DTO: `UpdateProductRequest`
  - Response DTO: `Product`
  - Used In Frontend: Yes

- **Endpoint:** `DeleteProduct`
  - Route: `/api/Products/{id}`
  - Method: `HttpDelete`
  - Request DTO: `None`
  - Response DTO: `IActionResult`
  - Used In Frontend: Yes

- **Endpoint:** `GetProductVariations`
  - Route: `/api/Products/{productId}/variations`
  - Method: `HttpGet`
  - Request DTO: `Query:int`
  - Response DTO: `None`
  - Used In Frontend: Yes

- **Endpoint:** `GetAvailableSizes`
  - Route: `/api/Products/{productId}/sizes`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<string`
  - Used In Frontend: Yes

- **Endpoint:** `GetAvailableColors`
  - Route: `/api/Products/{productId}/colors`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IEnumerable<string`
  - Used In Frontend: Yes

- **Endpoint:** `GetTotalStock`
  - Route: `/api/Products/{productId}/total-stock`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `int`
  - Used In Frontend: Yes

- **Endpoint:** `GetLowStockVariations`
  - Route: `/api/Products/low-stock`
  - Method: `HttpGet`
  - Request DTO: `Query:int`
  - Response DTO: `IEnumerable<ProductVariation`
  - Used In Frontend: Yes

- **Endpoint:** `ValidateProductDeletion`
  - Route: `/api/Products/{id}/validate-deletion`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `bool`
  - Used In Frontend: Yes

### Controller: ProductVariationsController
- **Endpoint:** `GetVariation`
  - Route: `/api/ProductVariations/{id}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `ProductVariation`
  - Used In Frontend: Yes

- **Endpoint:** `GetVariationBySku`
  - Route: `/api/ProductVariations/by-sku/{sku}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `ProductVariation`
  - Used In Frontend: Yes

- **Endpoint:** `UpdateStock`
  - Route: `/api/ProductVariations/{id}/stock`
  - Method: `HttpPatch`
  - Request DTO: `UpdateStockRequest`
  - Response DTO: `IActionResult`
  - Used In Frontend: Yes

- **Endpoint:** `UpdateVariation`
  - Route: `/api/ProductVariations/{id}`
  - Method: `HttpPut`
  - Request DTO: `UpdateProductVariationRequest`
  - Response DTO: `ProductVariation`
  - Used In Frontend: Yes

- **Endpoint:** `DeleteVariation`
  - Route: `/api/ProductVariations/{id}`
  - Method: `HttpDelete`
  - Request DTO: `None`
  - Response DTO: `IActionResult`
  - Used In Frontend: Yes

### Controller: ReportingController
- **Endpoint:** `GetSalesReport`
  - Route: `/api/Reporting/sales`
  - Method: `HttpGet`
  - Request DTO: `Query:DateTime`
  - Response DTO: `SalesReport`
  - Used In Frontend: Yes

- **Endpoint:** `GetInventoryReport`
  - Route: `/api/Reporting/inventory`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `InventoryReport`
  - Used In Frontend: Yes

- **Endpoint:** `GetCustomerReport`
  - Route: `/api/Reporting/customers`
  - Method: `HttpGet`
  - Request DTO: `Query:DateTime`
  - Response DTO: `CustomerReport`
  - Used In Frontend: Yes

- **Endpoint:** `GetProcurementReport`
  - Route: `/api/Reporting/procurement`
  - Method: `HttpGet`
  - Request DTO: `Query:DateTime`
  - Response DTO: `ProcurementReport`
  - Used In Frontend: Yes

- **Endpoint:** `GetFinancialReport`
  - Route: `/api/Reporting/financial`
  - Method: `HttpGet`
  - Request DTO: `Query:DateTime`
  - Response DTO: `FinancialReport`
  - Used In Frontend: Yes

- **Endpoint:** `GetSalesAnalytics`
  - Route: `/api/Reporting/analytics/sales`
  - Method: `HttpGet`
  - Request DTO: `Query:DateTime`
  - Response DTO: `SalesAnalytics`
  - Used In Frontend: Yes

- **Endpoint:** `GetInventoryAnalytics`
  - Route: `/api/Reporting/analytics/inventory`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `InventoryAnalytics`
  - Used In Frontend: Yes

- **Endpoint:** `GetCustomerAnalytics`
  - Route: `/api/Reporting/analytics/customers`
  - Method: `HttpGet`
  - Request DTO: `Query:DateTime`
  - Response DTO: `CustomerAnalytics`
  - Used In Frontend: Yes

- **Endpoint:** `GetFinancialAnalytics`
  - Route: `/api/Reporting/analytics/financial`
  - Method: `HttpGet`
  - Request DTO: `Query:DateTime`
  - Response DTO: `FinancialAnalytics`
  - Used In Frontend: Yes

- **Endpoint:** `GetPredictiveAnalytics`
  - Route: `/api/Reporting/analytics/predictive`
  - Method: `HttpGet`
  - Request DTO: `Query:DateTime`
  - Response DTO: `PredictiveAnalytics`
  - Used In Frontend: Yes

- **Endpoint:** `RunCustomReport`
  - Route: `/api/Reporting/custom/run`
  - Method: `HttpPost`
  - Request DTO: `CustomReportRunRequest`
  - Response DTO: `object`
  - Used In Frontend: Yes

- **Endpoint:** `ExportCustomReport`
  - Route: `/api/Reporting/custom/export`
  - Method: `HttpPost`
  - Request DTO: `CustomReportExportRequest`
  - Response DTO: `ExportResult`
  - Used In Frontend: Yes

- **Endpoint:** `GetDashboardSummary`
  - Route: `/api/Reporting/dashboard/summary`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `DashboardSummary`
  - Used In Frontend: Yes

- **Endpoint:** `GetRealTimeMetrics`
  - Route: `/api/Reporting/dashboard/realtime`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `RealTimeMetrics`
  - Used In Frontend: Yes

- **Endpoint:** `ExportReport`
  - Route: `/api/Reporting/export`
  - Method: `HttpPost`
  - Request DTO: `ExportRequest`
  - Response DTO: `None`
  - Used In Frontend: Yes

- **Endpoint:** `GetExportStatus`
  - Route: `/api/Reporting/export/{fileId}/status`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `ExportResult`
  - Used In Frontend: Yes

- **Endpoint:** `DownloadExport`
  - Route: `/api/Reporting/export/{fileId}/download`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `IActionResult`
  - Used In Frontend: Yes

- **Endpoint:** `GetMetrics`
  - Route: `/api/Reporting/metrics`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `None`
  - Used In Frontend: Yes

- **Endpoint:** `GetAlerts`
  - Route: `/api/Reporting/alerts`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `None`
  - Used In Frontend: Yes

### Controller: SalesController
- **Endpoint:** `ProcessSale`
  - Route: `/api/Sales/process-sale`
  - Method: `HttpPost`
  - Request DTO: `ProcessSaleRequest`
  - Response DTO: `SalesTransaction`
  - Used In Frontend: Yes

- **Endpoint:** `ProcessReturn`
  - Route: `/api/Sales/process-return`
  - Method: `HttpPost`
  - Request DTO: `ProcessReturnRequest`
  - Response DTO: `SalesTransaction`
  - Used In Frontend: Yes

- **Endpoint:** `ProcessExchange`
  - Route: `/api/Sales/process-exchange`
  - Method: `HttpPost`
  - Request DTO: `ProcessExchangeRequest`
  - Response DTO: `SalesTransaction`
  - Used In Frontend: Yes

- **Endpoint:** `GetTransaction`
  - Route: `/api/Sales/{id:guid}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `SalesTransaction`
  - Used In Frontend: Yes

- **Endpoint:** `GetTransactionByNumber`
  - Route: `/api/Sales/by-number/{transactionNumber}`
  - Method: `HttpGet`
  - Request DTO: `None`
  - Response DTO: `SalesTransaction`
  - Used In Frontend: Yes

- **Endpoint:** `GetTransactions`
  - Route: `/api/Sales`
  - Method: `HttpGet`
  - Request DTO: `Query:int`
  - Response DTO: `PagedResponse<SalesTransaction`
  - Used In Frontend: Yes

- **Endpoint:** `GetSalesStatistics`
  - Route: `/api/Sales/statistics`
  - Method: `HttpGet`
  - Request DTO: `Query:DateTime`
  - Response DTO: `SalesStatistics`
  - Used In Frontend: Yes

- **Endpoint:** `GetTopSellingProducts`
  - Route: `/api/Sales/top-products`
  - Method: `HttpGet`
  - Request DTO: `Query:DateTime`
  - Response DTO: `IEnumerable<ProductSalesSummary`
  - Used In Frontend: Yes

- **Endpoint:** `CancelTransaction`
  - Route: `/api/Sales/{id:guid}/cancel`
  - Method: `HttpPut`
  - Request DTO: `CancelTransactionRequest`
  - Response DTO: `None`
  - Used In Frontend: Yes

## 2. Frontend API Usage Map
### Service: categoryService
- **Method:** `getCategory`
  - URL: `/categories/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getRootCategories`
  - URL: `/categories/root`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getChildCategories`
  - URL: `/categories/{parentId}/children`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `createCategory`
  - URL: `/categories`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `updateCategory`
  - URL: `/categories/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `deleteCategory`
  - URL: `/categories/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

### Service: customerService
- **Method:** `getCustomer`
  - URL: `/customers/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `searchCustomer`
  - URL: `/customers/search`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `createCustomer`
  - URL: `/customers`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `updateCustomer`
  - URL: `/customers/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `deleteCustomer`
  - URL: `/customers/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getCustomerLoyalty`
  - URL: `/customers/{id}/loyalty`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `addLoyaltyPoints`
  - URL: `/customers/{id}/loyalty/add-points`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `redeemLoyaltyPoints`
  - URL: `/customers/{id}/loyalty/redeem-points`
  - Backend Match: Yes
  - DTO Compatible: TBD

### Service: fileService
- **Method:** `uploadFile`
  - URL: `/files/upload`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `uploadFiles`
  - URL: `/files/upload-multiple`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `deleteFile`
  - URL: `/files`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getFileInfo`
  - URL: `/files/info`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `fileExists`
  - URL: `/files/exists`
  - Backend Match: Yes
  - DTO Compatible: TBD

### Service: healthService
- **Method:** `getHealth`
  - URL: `/v1/Health`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getReadiness`
  - URL: `/v1/Health/readiness`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getDetailedHealth`
  - URL: `/v1/Health/detailed`
  - Backend Match: Yes
  - DTO Compatible: TBD

### Service: inventoryService
- **Method:** `getInventory`
  - URL: `/inventory/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getInventoryByProduct`
  - URL: `/inventory/by-product/{productId}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getInventoryByVariation`
  - URL: `/inventory/by-variation/{productVariationId}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getInventoryByBranch`
  - URL: `/inventory/by-branch/{branchId}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getInventoryByWarehouse`
  - URL: `/inventory/by-warehouse/{warehouseId}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `createInventory`
  - URL: `/inventory`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `updateInventory`
  - URL: `/inventory/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `deleteInventory`
  - URL: `/inventory/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `updateStock`
  - URL: `/inventory/{id}/stock`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `transferInventory`
  - URL: `/inventory/transfer`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `adjustInventory`
  - URL: `/inventory/{id}/adjust`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getLowStock`
  - URL: `/inventory/low-stock`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getOutOfStock`
  - URL: `/inventory/out-of-stock`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `reserveInventory`
  - URL: `/inventory/{id}/reserve`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `releaseInventory`
  - URL: `/inventory/{id}/release`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `bulkTransferInventory`
  - URL: `/inventory/bulk-transfer`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `bulkAdjustInventory`
  - URL: `/inventory/bulk-adjust`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `searchInventory`
  - URL: `/inventory/search`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getInventoryValueByLocation`
  - URL: `/inventory/value-by-location`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `generateInventoryReport`
  - URL: `/inventory/report`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `createTransaction`
  - URL: `/inventory/transaction`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getTransactionById`
  - URL: `/inventory/transaction/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `searchTransactions`
  - URL: `/inventory/transaction/search`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `sendLowStockAlerts`
  - URL: `/inventory/alerts/low-stock/send`
  - Backend Match: Yes
  - DTO Compatible: TBD

### Service: procurementService
- **Method:** `getSupplier`
  - URL: `/procurement/suppliers/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getSupplierByCode`
  - URL: `/procurement/suppliers/by-code/{code}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `createSupplier`
  - URL: `/procurement/suppliers`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `updateSupplier`
  - URL: `/procurement/suppliers/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `deleteSupplier`
  - URL: `/procurement/suppliers/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getPurchaseOrder`
  - URL: `/procurement/purchase-orders/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getPurchaseOrderByNumber`
  - URL: `/procurement/purchase-orders/by-number/{orderNumber}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getPurchaseOrdersBySupplier`
  - URL: `/procurement/purchase-orders/by-supplier/{supplierId}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getPurchaseOrdersByStatus`
  - URL: `/procurement/purchase-orders/by-status/{status}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `createPurchaseOrder`
  - URL: `/procurement/purchase-orders`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `updatePurchaseOrder`
  - URL: `/procurement/purchase-orders/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `deletePurchaseOrder`
  - URL: `/procurement/purchase-orders/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

### Service: productCatalogService
- **Method:** `deleteCategory`
  - URL: `/categories/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `deleteProduct`
  - URL: `/products/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `deleteVariation`
  - URL: `/ProductVariations/{id}`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `updateStock`
  - URL: `/ProductVariations/{id}/stock`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `deleteFile`
  - URL: `/files`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `deleteImage`
  - URL: `/images/{id}`
  - Backend Match: No
  - DTO Compatible: N/A

### Service: reportingService
- **Method:** `exportReport`
  - URL: `/reporting/export`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `getExportStatus`
  - URL: `/reporting/export/{fileId}/status`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `downloadExport`
  - URL: `/reporting/export/{fileId}/download`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `runCustomReport`
  - URL: `/reporting/custom/run`
  - Backend Match: Yes
  - DTO Compatible: TBD

- **Method:** `exportCustomReport`
  - URL: `/reporting/custom/export`
  - Backend Match: Yes
  - DTO Compatible: TBD

### Service: salesService
## 3. Contract Mismatches
| DTO Name | Backend Field (Missing in FE) | Frontend Field (Extra in FE) | Issue |
| --- | --- | --- | --- |

## 4. Implementation Integrity
### Unimplemented Interface Methods
- **Interface:** `IAuthService`
  - Implementation: `NONE`
  - Missing Methods: AuthenticateAsync, RefreshTokenAsync, RevokeTokenAsync, IsTokenBlacklistedAsync, BlacklistTokenAsync, ValidateTokenAsync, GetCurrentUserAsync

- **Interface:** `IBranchRepository`
  - Implementation: `NONE`
  - Missing Methods: GetByIdAsync, GetByCodeAsync, GetActiveBranchesAsync, CodeExistsAsync

- **Interface:** `ICategoryRepository`
  - Implementation: `NONE`
  - Missing Methods: GetByIdAsync, GetByCodeAsync, GetAllAsync, GetActiveAsync, GetRootCategoriesAsync, GetChildCategoriesAsync, CreateAsync, UpdateAsync, DeleteAsync, ExistsAsync, CodeExistsAsync, HasChildCategoriesAsync, HasProductsAsync, GetCountAsync, GetActiveCountAsync, GetHierarchyAsync, SearchAsync

- **Interface:** `ICustomerLoyaltyRepository`
  - Implementation: `NONE`
  - Missing Methods: GetByCustomerIdAsync, GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync, GetPagedAsync, GetByPointsRangeAsync

- **Interface:** `ICustomerRepository`
  - Implementation: `NONE`
  - Missing Methods: GetByIdAsync, GetByEmailAsync, GetByPhoneAsync, GetPagedAsync, GetAllAsync, GetActiveAsync, CreateAsync, UpdateAsync, DeleteAsync, ExistsAsync, EmailExistsAsync, PhoneExistsAsync, GetCountAsync, GetActiveCountAsync, SearchAsync, GetCustomersWithUpcomingBirthdaysAsync

- **Interface:** `IFileUploadService`
  - Implementation: `NONE`
  - Missing Methods: UploadFileAsync, UploadFilesAsync, DeleteFileAsync, ValidateFile, GetFileInfoAsync, FileExistsAsync

- **Interface:** `IInventoryManagementService`
  - Implementation: `NONE`
  - Missing Methods: CreateInventoryAsync, GetInventoryAsync, UpdateInventoryAsync, DeleteInventoryAsync, GetInventoryByLocationAsync, GetInventoryByProductAsync, GetInventoryByProductVariationAsync, GetInventoryByBranchAsync, GetInventoryByWarehouseAsync, ReserveInventoryAsync, ReleaseInventoryAsync, UpdateStockAsync, TransferInventoryAsync, BulkTransferInventoryAsync, AdjustInventoryAsync, BulkAdjustInventoryAsync, SearchInventoryAsync, GetLowStockItemsAsync, GetOutOfStockItemsAsync, GetInventoryStatisticsAsync, GetInventoryValueByLocationAsync, GenerateInventoryReportAsync, CreateTransactionAsync, SearchTransactionsAsync, GetRecentTransactionsAsync, ValidateInventoryRequest, ValidateTransferRequest, ValidateAdjustmentRequest, GetLowStockAlertsAsync, SendLowStockAlertsAsync, GetInventorySummaryAsync, GetRecentMovementsAsync

- **Interface:** `IInventoryRepository`
  - Implementation: `NONE`
  - Missing Methods: GetByIdAsync, GetByProductAndLocationAsync, GetByBranchAsync, GetByWarehouseAsync, GetByProductAsync, GetByProductVariationAsync, GetLowStockItemsAsync, GetOutOfStockItemsAsync, GetWithPaginationAsync, SearchAsync, UpdateQuantityAsync, ReserveQuantityAsync, ReleaseReservedQuantityAsync, GetStatisticsAsync, GetInventoryValueByLocationAsync, ExistsAsync, GetMovementsAsync

- **Interface:** `IInventoryTransactionRepository`
  - Implementation: `NONE`
  - Missing Methods: GetByIdAsync, GetByInventoryIdAsync, GetByProductIdAsync, GetByProductVariationIdAsync, GetByBranchIdAsync, GetByWarehouseIdAsync, GetByTransactionTypeAsync, GetByDateRangeAsync, GetByUserIdAsync, GetWithPaginationAsync, GetTransactionSummaryAsync, GetRecentTransactionsAsync, GetMovementsAsync, GetByReferenceNumberAsync, GetTransferTransactionsAsync, GetAdjustmentTransactionsAsync, GetStatisticsAsync

- **Interface:** `IPaymentService`
  - Implementation: `NONE`
  - Missing Methods: ProcessCreditCardPaymentAsync, ProcessDebitCardPaymentAsync, ProcessCashPaymentAsync, ProcessGiftCardPaymentAsync

- **Interface:** `IProductCatalogService`
  - Implementation: `NONE`
  - Missing Methods: CreateCategoryAsync, UpdateCategoryAsync, DeleteCategoryAsync, GetCategoryAsync, GetCategoriesAsync, GetRootCategoriesAsync, GetChildCategoriesAsync, CreateProductAsync, UpdateProductAsync, DeleteProductAsync, GetProductAsync, GetProductBySkuAsync, GetProductsAsync, SearchProductsAsync, CreateProductVariationAsync, UpdateProductVariationAsync, DeleteProductVariationAsync, GetProductVariationAsync, GetProductVariationBySkuAsync, GetProductVariationsAsync, GetAvailableSizesAsync, GetAvailableColorsAsync, AddProductImageAsync, UpdateProductImageAsync, DeleteProductImageAsync, GetProductImageAsync, GetProductImagesAsync, GetPrimaryImageAsync, UpdateVariationStockAsync, GetTotalStockForProductAsync, GetLowStockVariationsAsync, ValidateCategoryDeletionAsync, ValidateProductDeletionAsync, ValidateProductVariationAsync, GetBranchAsync, GetWarehouseAsync

- **Interface:** `IProductRepository`
  - Implementation: `NONE`
  - Missing Methods: GetByIdAsync, GetBySkuAsync, GetPagedAsync, GetAllAsync, GetActiveAsync, GetByCategoryAsync, CreateAsync, UpdateAsync, DeleteAsync, ExistsAsync, SkuExistsAsync, HasVariationsAsync, GetCountAsync, GetActiveCountAsync, SearchAsync, GetByBrandAsync, GetBySeasonAsync, GetLowStockProductsAsync, UpdateStatusAsync

- **Interface:** `IProductVariationRepository`
  - Implementation: `NONE`
  - Missing Methods: GetByIdAsync, GetBySkuAsync, GetByProductIdAsync, GetByProductIdPagedAsync, GetAllAsync, GetActiveAsync, CreateAsync, UpdateAsync, DeleteAsync, ExistsAsync, SkuExistsAsync, SizeColorCombinationExistsAsync, GetCountAsync, GetActiveCountAsync, GetBySizeAsync, GetByColorAsync, GetLowStockVariationsAsync, GetOutOfStockVariationsAsync, UpdateStockAsync, GetAvailableSizesAsync, GetAvailableColorsAsync, GetTotalStockForProductAsync, UpdateStatusAsync

- **Interface:** `IPurchaseOrderRepository`
  - Implementation: `NONE`
  - Missing Methods: GetByIdAsync, GetByOrderNumberAsync, GetAllAsync, GetBySupplierAsync, GetByStatusAsync, GetPagedAsync, CreateAsync, UpdateAsync, DeleteAsync, ExistsAsync, OrderNumberExistsAsync, GetCountAsync, GetCountByStatusAsync, GetTotalValueAsync

- **Interface:** `IRepository`
  - Implementation: `NONE`
  - Missing Methods: GetByIdAsync, GetAllAsync, GetPagedAsync, AddAsync, UpdateAsync, DeleteAsync, ExistsAsync

- **Interface:** `ISalesProcessingService`
  - Implementation: `NONE`
  - Missing Methods: ProcessSaleAsync, ProcessReturnAsync, ProcessExchangeAsync, GetTransactionByIdAsync, GetTransactionByNumberAsync, GetTransactionsAsync, GetSalesStatisticsAsync, GetSalesTransactionsAsync, GetTopSellingProductsAsync, CancelTransactionAsync

- **Interface:** `ISalesTransactionRepository`
  - Implementation: `NONE`
  - Missing Methods: GetByIdAsync, GetByTransactionNumberAsync, GetPagedAsync, GetAllAsync, GetByCustomerAsync, GetByBranchAsync, CreateAsync, UpdateAsync, DeleteAsync, ExistsAsync, TransactionNumberExistsAsync, GetCountAsync, GetSalesStatisticsAsync, GetTopSellingProductsAsync, GetDailySalesSummaryAsync

- **Interface:** `ISupplierRepository`
  - Implementation: `NONE`
  - Missing Methods: GetByIdAsync, GetByCodeAsync, GetAllAsync, GetActiveAsync, GetPagedAsync, CreateAsync, UpdateAsync, DeleteAsync, ExistsAsync, CodeExistsAsync, EmailExistsAsync, GetCountAsync, GetActiveCountAsync

- **Interface:** `IWarehouseRepository`
  - Implementation: `NONE`
  - Missing Methods: GetByIdAsync, GetByCodeAsync, GetActiveWarehousesAsync, CodeExistsAsync

- **Interface:** `IAuditService`
  - Implementation: `NONE`
  - Missing Methods: LogAsync, LogAsync, LogUserActionAsync, LogSystemEventAsync, GetUserAuditEventsAsync, GetResourceAuditEventsAsync, GetAuditEventsByActionAsync, GetAuditEventsAsync, GetAuditStatisticsAsync, ArchiveAuditEventsAsync, DeleteAuditEventsAsync

- **Interface:** `IBackgroundJobService`
  - Implementation: `NONE`
  - Missing Methods: PauseJobAsync, ResumeJobAsync, DeleteJobAsync, GetScheduledJobsAsync, GetJobHistoryAsync, GetJobStatisticsAsync, IsJobRunningAsync, GetNextRunTimeAsync

- **Interface:** `IRbacService`
  - Implementation: `NONE`
  - Missing Methods: HasPermissionAsync, HasAnyPermissionAsync, HasAllPermissionsAsync, HasRoleAsync, HasAnyRoleAsync, GetUserPermissionsAsync, GetUserRolesAsync, AssignRoleAsync, RemoveRoleAsync, GrantPermissionToRoleAsync, RevokePermissionFromRoleAsync, GetRolePermissionsAsync, GetUsersWithRoleAsync, GetRolesWithPermissionAsync, CanAccessResourceAsync, GetEffectivePermissionsAsync

### Dead Endpoints (Not used by Frontend)
- `GET /api/Categories` (CategoriesController)
- `GET /api/Categories/{id}/validate-deletion` (CategoriesController)
- `GET /api/Customers` (CustomersController)
- `GET /api/Inventory/by-location` (InventoryController)
- `GET /api/Inventory/statistics` (InventoryController)
- `GET /api/Inventory/transaction/recent` (InventoryController)
- `GET /api/Inventory/alerts/low-stock` (InventoryController)
- `GET /api/Procurement/suppliers` (ProcurementController)
- `GET /api/Procurement/purchase-orders` (ProcurementController)
- `GET /api/Products` (ProductsController)
- `GET /api/Products/search` (ProductsController)
- `GET /api/Products/{id}` (ProductsController)
- `GET /api/Products/by-sku/{sku}` (ProductsController)
- `POST /api/Products` (ProductsController)
- `PUT /api/Products/{id}` (ProductsController)
- `GET /api/Products/{productId}/variations` (ProductsController)
- `GET /api/Products/{productId}/sizes` (ProductsController)
- `GET /api/Products/{productId}/colors` (ProductsController)
- `GET /api/Products/{productId}/total-stock` (ProductsController)
- `GET /api/Products/low-stock` (ProductsController)
- `GET /api/Products/{id}/validate-deletion` (ProductsController)
- `GET /api/ProductVariations/{id}` (ProductVariationsController)
- `GET /api/ProductVariations/by-sku/{sku}` (ProductVariationsController)
- `PUT /api/ProductVariations/{id}` (ProductVariationsController)
- `GET /api/Reporting/sales` (ReportingController)
- `GET /api/Reporting/inventory` (ReportingController)
- `GET /api/Reporting/customers` (ReportingController)
- `GET /api/Reporting/procurement` (ReportingController)
- `GET /api/Reporting/financial` (ReportingController)
- `GET /api/Reporting/analytics/sales` (ReportingController)
- `GET /api/Reporting/analytics/inventory` (ReportingController)
- `GET /api/Reporting/analytics/customers` (ReportingController)
- `GET /api/Reporting/analytics/financial` (ReportingController)
- `GET /api/Reporting/analytics/predictive` (ReportingController)
- `GET /api/Reporting/dashboard/summary` (ReportingController)
- `GET /api/Reporting/dashboard/realtime` (ReportingController)
- `GET /api/Reporting/metrics` (ReportingController)
- `GET /api/Reporting/alerts` (ReportingController)
- `POST /api/Sales/process-sale` (SalesController)
- `POST /api/Sales/process-return` (SalesController)
- `POST /api/Sales/process-exchange` (SalesController)
- `GET /api/Sales/{id:guid}` (SalesController)
- `GET /api/Sales/by-number/{transactionNumber}` (SalesController)
- `GET /api/Sales` (SalesController)
- `GET /api/Sales/statistics` (SalesController)
- `GET /api/Sales/top-products` (SalesController)
- `PUT /api/Sales/{id:guid}/cancel` (SalesController)

### Broken Frontend Calls (No Backend Match)
- `DELETE api/images/{id}` in `productCatalogService.deleteImage`

## 5. TODOs & Placeholders
- `backend\src\Application\Services\DataArchivalService.cs:24`: // Placeholder: Move records older than cutoffDate to archive tables/files
- `backend\src\Application\Services\DataArchivalService.cs:32`: // Placeholder: Delete archive records older than retention period (e.g., 7 years)
- `backend\src\Application\Services\DataArchivalService.cs:39`: // Placeholder: Return counts of active, archived, and purged records
- `backend\src\Application\Services\IErrorHandlingService.cs:46`: /// Logs a debug message
- `backend\src\Application\Services\IErrorHandlingService.cs:48`: void LogDebug(string message, string? context = null);
- `backend\src\Infrastructure\External\FileUploadService.cs:183`: _logger.LogDebug("Image file validation passed for: {FileName}", file.FileName);
- `backend\src\Infrastructure\External\PaymentService.cs:9`: /// Mock payment service for processing various payment methods
- `backend\src\Infrastructure\External\PaymentService.cs:38`: // Mock approval logic (95% approval rate)
- `backend\src\Infrastructure\External\PaymentService.cs:74`: // Mock approval logic (98% approval rate for debit)
- `backend\src\Infrastructure\External\PaymentService.cs:140`: // Mock balance check (80% have sufficient balance)
- `backend\src\Infrastructure\External\PaymentService.cs:206`: /// Generate a mock authorization code
- `backend\src\Infrastructure\Jobs\ReportGenerationJob.cs:277`: // Example email notification (placeholder)
- `backend\src\Infrastructure\Jobs\ReportGenerationJob.cs:292`: // Placeholder for email sending implementation
- `backend\src\Infrastructure\Monitoring\SalesPerformanceMonitor.cs:155`: _logger.LogDebug("Custom Metric - Operation: {Operation}, Metric: {MetricName}, Value: {Value}",
- `backend\src\Infrastructure\Services\AuditService.cs:73`: _logger.LogDebug("Audit event logged: {Action} on {ResourceType} by {UserId}",
- `backend\src\Infrastructure\Services\BackgroundJobService.cs:216`: // For now, return empty list as this is a placeholder
- `frontend\src\components\Categories\CategoryManager.vue:32`: placeholder="Search categories..."
- `frontend\src\components\Categories\CategoryModal.vue:20`: placeholder="Enter category name"
- `frontend\src\components\Categories\CategoryModal.vue:33`: placeholder="Enter category description"
- `frontend\src\components\Categories\CategoryModal.vue:47`: placeholder="Enter category code (optional)"
- `frontend\src\components\Categories\CategoryModal.vue:81`: placeholder="Enter sort order"
- `frontend\src\components\Customers\CustomerForm.vue:19`: placeholder="Enter first name"
- `frontend\src\components\Customers\CustomerForm.vue:29`: placeholder="Enter last name"
- `frontend\src\components\Customers\CustomerForm.vue:42`: placeholder="customer@example.com"
- `frontend\src\components\Customers\CustomerForm.vue:51`: placeholder="+1 (555) 123-4567"
- `frontend\src\components\Customers\CustomerForm.vue:74`: placeholder="123 Main Street"
- `frontend\src\components\Customers\CustomerForm.vue:85`: placeholder="City"
- `frontend\src\components\Customers\CustomerForm.vue:94`: placeholder="State"
- `frontend\src\components\Customers\CustomerForm.vue:106`: placeholder="12345"
- `frontend\src\components\Customers\CustomerManager.vue:16`: placeholder="Search by name, email, or phone..."
- `frontend\src\components\Customers\LoyaltyPanel.vue:68`: placeholder="100"
- `frontend\src\components\Customers\LoyaltyPanel.vue:77`: placeholder="Birthday bonus, referral reward, etc."
- `frontend\src\components\Customers\LoyaltyPanel.vue:100`: placeholder="50"
- `frontend\src\components\Customers\LoyaltyPanel.vue:109`: placeholder="Discount on purchase, reward claim, etc."
- `frontend\src\components\Inventory\AdjustmentHistory.vue:62`: placeholder="Search by product or reason..."
- `frontend\src\components\Inventory\InventoryList.vue:46`: placeholder="Search by product..."
- `frontend\src\components\Inventory\InventoryList.vue:181`: // Mock data for branches/warehouses
- `frontend\src\components\Inventory\TransferManager.vue:148`: <textarea v-model="transferForm.reason" required rows="3" placeholder="Reason for transfer..."></textarea>
- `frontend\src\components\Products\ProductManager.vue:32`: placeholder="Search products..."
- `frontend\src\components\Products\ProductModal.vue:9`: <p>Product form placeholder</p>
- `frontend\src\components\Products\VariationModal.vue:9`: <p>Variation form placeholder</p>
- `frontend\src\components\Reporting\ReportBuilder.vue:22`: <input v-model.trim="definition.name" type="text" placeholder="e.g. Monthly Sales by Category" />
- `frontend\src\components\Reporting\ReportBuilder.vue:66`: <input v-model="f.value" type="text" placeholder="Value" />
- `frontend\src\components\Sales\PaymentProcessor.vue:190`: placeholder="Enter amount received"
- `frontend\src\components\Sales\PaymentProcessor.vue:208`: placeholder="1234 5678 9012 3456"
- `frontend\src\components\Sales\PaymentProcessor.vue:220`: placeholder="MM/YY"
- `frontend\src\components\Sales\PaymentProcessor.vue:230`: placeholder="123"
- `frontend\src\components\Sales\PaymentProcessor.vue:246`: placeholder="Enter gift card number"
- `frontend\src\components\Sales\PointOfSale.vue:116`: placeholder="Search products by name, SKU, or description..."
- `frontend\src\components\Sales\ReturnsExchanges.vue:111`: placeholder="Enter transaction number"
- `frontend\src\components\Suppliers\PurchaseOrderForm.vue:55`: <input v-model="item.productId" type="text" placeholder="Product ID" required />
- `frontend\src\components\Suppliers\PurchaseOrderManager.vue:23`: placeholder="Search by order number..."
- `frontend\src\components\Suppliers\ReceivingManager.vue:89`: placeholder="Optional notes"
- `frontend\src\components\Suppliers\ReceivingManager.vue:259`: // Mock data - in real app would fetch from API
- `frontend\src\components\Suppliers\SupplierManager.vue:14`: placeholder="Search suppliers by name or code..."
- `frontend\src\stores\reportingStore.ts:194`: // Placeholder until a backend endpoint exists.
- `frontend\src\stores\reportingStore.ts:211`: // Placeholder until a backend endpoint exists.
- `frontend\src\utils\logger.ts:3`: DEBUG = 0,
- `frontend\src\utils\logger.ts:169`: [LogLevel.DEBUG]: 'color: #6b7280; font-weight: normal;',
- `frontend\src\utils\logger.ts:209`: debug(category: string, message: string, data?: any): void {
- `frontend\src\utils\logger.ts:210`: this.log(LogLevel.DEBUG, category, message, data)
- `frontend\src\utils\logger.ts:233`: this.debug('performance', `Started: ${operation}`, { timerId })
- `frontend\src\utils\logger.ts:301`: this.debug('state', `${entity} ${action}`, {
- `frontend\src\utils\logger.ts:386`: debug: this.logs.filter(log => log.level === LogLevel.DEBUG).length,
- `frontend\src\utils\logger.ts:413`: debug: (category: string, message: string, data?: any) => logger.debug(category, message, data),
- `frontend\src\utils\performance.ts:203`: log.debug('performance', `Started monitoring: ${operation}`, { operationId, metadata })
- `frontend\src\utils\performance.ts:255`: log.debug('performance', `Completed monitoring: ${metric.operation}`, {
- `frontend\src\views\Sales\SalesHistory.vue:10`: // Sales history implementation placeholder
- `frontend\src\views\Sales\SalesReports.vue:10`: // Sales reports implementation placeholder

## 6. Recommendations
1. **Synchronize DTOs:** 29 DTOs have field mismatches. Update frontend types and backend DTOs to match contracts.
2. **Implement Missing Service Logic:** 20 interface methods are registered but lack implementation.
3. **Fix Routing:** 17 frontend calls point to non-existent or misconfigured routes.
4. **Cleanup Dead Code:** 75 endpoints are not consumed; verify if they are for future use or can be removed.
5. **Address TODOs:** 525 markers found. Many are in critical paths (ML retraining, synthetic data generation).

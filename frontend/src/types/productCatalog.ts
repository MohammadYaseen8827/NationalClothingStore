// Base entity interface
export interface BaseEntity {
  id: string
  createdAt: string
  updatedAt: string
  createdBy?: string
  updatedBy?: string
  isActive: boolean
}

// Category interfaces
export interface Category extends BaseEntity {
  name: string
  description?: string
  code?: string
  parentCategoryId?: string
  sortOrder: number
  children?: Category[]
  productCount?: number
}

export interface CreateCategoryRequest {
  name: string
  description?: string
  code?: string
  parentId?: string
  sortOrder?: number
  isActive?: boolean
}

export interface UpdateCategoryRequest {
  name: string
  description?: string
  code?: string
  parentId?: string
  sortOrder: number
  isActive: boolean
}

// Product interfaces
export interface Product extends BaseEntity {
  name: string
  description?: string
  sku: string
  barcode?: string
  basePrice: number
  costPrice: number
  brand?: string
  season?: string
  material?: string
  color?: string
  categoryId: string
  variations?: ProductVariation[]
  images?: ProductImage[]
  totalStock?: number
  variationCount?: number
}

export interface CreateProductRequest {
  name: string
  description?: string
  sku: string
  barcode?: string
  basePrice: number
  costPrice: number
  brand?: string
  season?: string
  material?: string
  color?: string
  categoryId: string
  isActive?: boolean
}

export interface UpdateProductRequest {
  name: string
  description?: string
  barcode?: string
  basePrice: number
  costPrice: number
  brand?: string
  season?: string
  material?: string
  color?: string
  categoryId: string
  isActive: boolean
}

// Product Variation interfaces
export interface ProductVariation extends BaseEntity {
  productId: string
  size: string
  color: string
  sku: string
  additionalPrice: number
  costPrice: number
  stockQuantity: number
  product?: Product
  inventories?: any[]
}

export interface CreateProductVariationRequest {
  productId: string
  size: string
  color: string
  sku: string
  additionalPrice?: number
  costPrice: number
  stockQuantity?: number
  isActive?: boolean
}

export interface UpdateProductVariationRequest {
  size: string
  color: string
  sku: string
  additionalPrice: number
  costPrice: number
  stockQuantity: number
  isActive: boolean
}

// Product Image interfaces
export interface ProductImage extends BaseEntity {
  productId: string
  imageUrl: string
  altText?: string
  caption?: string
  sortOrder: number
  isPrimary: boolean
}

export interface AddProductImageRequest {
  productId: string
  imageUrl: string
  altText?: string
  caption?: string
  sortOrder?: number
  isPrimary?: boolean
}

export interface UpdateProductImageRequest {
  imageUrl: string
  altText?: string
  caption?: string
  sortOrder: number
  isPrimary: boolean
}

// Pagination interfaces
export interface PaginationMetadata {
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export interface PaginatedResponse<T> {
  data: T[]
  pagination: PaginationMetadata
}

// File Upload interfaces
export interface FileUploadResult {
  success: boolean
  fileName: string
  originalFileName: string
  filePath: string
  fileUrl: string
  fileSize: number
  contentType: string
  errorMessage?: string
  uploadedAt: string
}

export interface FileValidationResult {
  isValid: boolean
  errors: string[]
}

// API Response interfaces
export interface ApiResponse<T> {
  success: boolean
  data: T
  message?: string
  errors?: string[]
  timestamp?: string
  requestId?: string
}

export interface ErrorResponse {
  success: boolean
  errorCode: string
  message: string
  validationErrors?: ValidationError[]
  timestamp: string
  requestId?: string
}

export interface ValidationError {
  field: string
  message: string
}

// Search and Filter interfaces
export interface ProductSearchRequest {
  searchTerm: string
  pageNumber?: number
  pageSize?: number
  includeInactive?: boolean
}

export interface ProductFilterRequest {
  categoryId?: string
  brand?: string
  season?: string
  minPrice?: number
  maxPrice?: number
  isActive?: boolean
  pageNumber?: number
  pageSize?: number
}

export interface VariationFilterRequest {
  size?: string
  color?: string
  isActive?: boolean
  pageNumber?: number
  pageSize?: number
}

// Stock Management interfaces
export interface StockUpdateRequest {
  quantity: number
  reason?: string
}

export interface StockAdjustment {
  variationId: string
  previousQuantity: number
  newQuantity: number
  adjustment: number
  reason: string
  adjustedAt: string
  adjustedBy: string
}

// Utility interfaces
export interface StockStatus {
  quantity: number
  status: 'in-stock' | 'low-stock' | 'out-of-stock'
  threshold: number
}

export interface PriceCalculation {
  basePrice: number
  additionalPrice: number
  totalPrice: number
  currency: string
}

// Category Tree interfaces
export interface CategoryTreeNode {
  category: Category
  children: CategoryTreeNode[]
  level: number
  expanded: boolean
}

export interface CategoryPath {
  id: string
  name: string
  level: number
}

// Product Summary interfaces
export interface ProductSummary {
  id: string
  name: string
  sku: string
  basePrice: number
  categoryName: string
  totalStock: number
  variationCount: number
  imageCount: number
  isActive: boolean
  minPrice: number
  maxPrice: number
}

// Inventory interfaces
export interface InventoryItem {
  id: string
  variationId: string
  warehouseId: string
  branchId: string
  quantity: number
  reservedQuantity: number
  availableQuantity: number
  lastUpdated: string
}

export interface InventoryTransaction {
  id: string
  variationId: string
  transactionType: 'in' | 'out' | 'adjustment' | 'transfer'
  quantity: number
  reason: string
  referenceId?: string
  createdAt: string
  createdBy: string
}

// Dashboard interfaces
export interface CatalogDashboard {
  totalProducts: number
  totalCategories: number
  totalVariations: number
  lowStockItems: number
  outOfStockItems: number
  recentProducts: ProductSummary[]
  topCategories: CategorySummary[]
}

export interface CategorySummary {
  id: string
  name: string
  productCount: number
  totalStock: number
}

// Export interfaces
export interface ProductExportRequest {
  format: 'csv' | 'excel' | 'json'
  categoryId?: string
  includeInactive?: boolean
  includeVariations?: boolean
  includeImages?: boolean
}

export interface CategoryExportRequest {
  format: 'csv' | 'excel' | 'json'
  includeHierarchy?: boolean
  includeProductCount?: boolean
}

// Import interfaces
export interface ProductImportRequest {
  file: File
  format: 'csv' | 'excel'
  updateExisting?: boolean
  validateOnly?: boolean
}

export interface CategoryImportRequest {
  file: File
  format: 'csv' | 'excel'
  updateExisting?: boolean
  validateOnly?: boolean
}

export interface ImportResult {
  success: boolean
  totalRows: number
  importedRows: number
  failedRows: number
  errors: ImportError[]
  warnings: ImportWarning[]
}

export interface ImportError {
  row: number
  field: string
  message: string
  value?: any
}

export interface ImportWarning {
  row: number
  field: string
  message: string
  value?: any
}

// Configuration interfaces
export interface CatalogSettings {
  defaultPageSize: number
  lowStockThreshold: number
  allowedImageFormats: string[]
  maxImageSize: number
  defaultCurrency: string
  enableVersioning: boolean
  autoGenerateSkus: boolean
}

// Validation interfaces
export interface ValidationRule {
  field: string
  required: boolean
  minLength?: number
  maxLength?: number
  pattern?: string
  custom?: (value: any) => boolean | string
}

export interface FormValidation {
  [key: string]: ValidationRule[]
}

// Search interfaces
export interface SearchSuggestion {
  type: 'product' | 'category' | 'variation'
  id: string
  text: string
  description?: string
  imageUrl?: string
}

export interface SearchFilters {
  query: string
  categories: string[]
  brands: string[]
  seasons: string[]
  priceRange: [number, number]
  inStockOnly: boolean
}

// Chart interfaces
export interface SalesData {
  period: string
  sales: number
  revenue: number
}

export interface InventoryData {
  category: string
  inStock: number
  lowStock: number
  outOfStock: number
}

// Notification interfaces
export interface CatalogNotification {
  id: string
  type: 'info' | 'warning' | 'error' | 'success'
  title: string
  message: string
  timestamp: string
  read: boolean
  actionUrl?: string
}

// Audit interfaces
export interface CatalogAuditLog {
  id: string
  entityType: 'product' | 'category' | 'variation'
  entityId: string
  action: 'create' | 'update' | 'delete'
  changes: AuditChange[]
  userId: string
  timestamp: string
  ipAddress?: string
}

export interface AuditChange {
  field: string
  oldValue: any
  newValue: any
}

// Export all types for easy importing - types are already exported above

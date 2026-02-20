import type { 
  CreateCategoryRequest,
  UpdateCategoryRequest,
  CreateProductRequest,
  UpdateProductRequest,
  CreateProductVariationRequest,
  UpdateProductVariationRequest,
  Category,
  Product,
  ProductVariation
} from '@/types/productCatalog'

// Validation result interface
export interface ValidationResult {
  isValid: boolean
  errors: string[]
  warnings: string[]
}

// Validation rules
export const VALIDATION_RULES = {
  // Category validation
  CATEGORY_NAME: {
    required: true,
    minLength: 2,
    maxLength: 100,
    pattern: /^[a-zA-Z0-9\s\-_]+$/,
    message: 'Category name must be 2-100 characters and contain only letters, numbers, spaces, hyphens, and underscores'
  },
  CATEGORY_CODE: {
    required: false,
    minLength: 2,
    maxLength: 20,
    pattern: /^[A-Z0-9\-_]+$/,
    message: 'Category code must be 2-20 characters and contain only uppercase letters, numbers, hyphens, and underscores'
  },
  CATEGORY_DESCRIPTION: {
    required: false,
    minLength: 0,
    maxLength: 500,
    pattern: /.*/,
    message: 'Category description must not exceed 500 characters'
  },
  
  // Product validation
  PRODUCT_NAME: {
    required: true,
    minLength: 3,
    maxLength: 200,
    pattern: /^[a-zA-Z0-9\s\-_.,()]+$/,
    message: 'Product name must be 3-200 characters and contain only letters, numbers, spaces, and basic punctuation'
  },
  PRODUCT_SKU: {
    required: true,
    minLength: 3,
    maxLength: 50,
    pattern: /^[A-Z0-9\-_]+$/,
    message: 'Product SKU must be 3-50 characters and contain only uppercase letters, numbers, hyphens, and underscores'
  },
  PRODUCT_BARCODE: {
    required: false,
    minLength: 8,
    maxLength: 20,
    pattern: /^[0-9]+$/,
    message: 'Product barcode must be 8-20 digits'
  },
  PRODUCT_DESCRIPTION: {
    required: false,
    minLength: 0,
    maxLength: 2000,
    pattern: /^.*$/,
    message: 'Product description must not exceed 2000 characters'
  },
  PRODUCT_BRAND: {
    required: false,
    minLength: 2,
    maxLength: 100,
    pattern: /^[a-zA-Z0-9\s\-_]+$/,
    message: 'Product brand must be 2-100 characters and contain only letters, numbers, spaces, hyphens, and underscores'
  },
  PRODUCT_PRICE: {
    required: true,
    min: 0,
    max: 999999.99,
    message: 'Product price must be between 0 and 999,999.99'
  },
  PRODUCT_COST: {
    required: true,
    min: 0,
    max: 999999.99,
    message: 'Product cost must be between 0 and 999,999.99'
  },
  
  // Variation validation
  VARIATION_SIZE: {
    required: true,
    minLength: 1,
    maxLength: 20,
    pattern: /^[a-zA-Z0-9\-_\/]+$/,
    message: 'Variation size must be 1-20 characters and contain only letters, numbers, hyphens, underscores, and forward slashes'
  },
  VARIATION_COLOR: {
    required: true,
    minLength: 2,
    maxLength: 50,
    pattern: /^[a-zA-Z0-9\s\-_]+$/,
    message: 'Variation color must be 2-50 characters and contain only letters, numbers, spaces, hyphens, and underscores'
  },
  VARIATION_STOCK: {
    required: true,
    min: 0,
    max: 999999,
    message: 'Stock quantity must be between 0 and 999,999'
  }
}

// Validation helper functions
export const ValidationHelpers = {
  // String validation
  validateString: (value: string, rules: typeof VALIDATION_RULES.CATEGORY_NAME): ValidationResult => {
    const errors: string[] = []
    const warnings: string[] = []

    // Required check
    if (rules.required && (!value || value.trim() === '')) {
      errors.push('This field is required')
      return { isValid: false, errors, warnings }
    }

    // Skip other validations if field is empty and not required
    if (!value || value.trim() === '') {
      return { isValid: true, errors, warnings }
    }

    // Length validation
    if (rules.minLength && value.length < rules.minLength) {
      errors.push(`Must be at least ${rules.minLength} characters`)
    }
    if (rules.maxLength && value.length > rules.maxLength) {
      errors.push(`Must not exceed ${rules.maxLength} characters`)
    }

    // Pattern validation
    if (rules.pattern && !rules.pattern.test(value)) {
      errors.push(rules.message)
    }

    return {
      isValid: errors.length === 0,
      errors,
      warnings
    }
  },

  // Number validation
  validateNumber: (value: number, rules: typeof VALIDATION_RULES.PRODUCT_PRICE): ValidationResult => {
    const errors: string[] = []
    const warnings: string[] = []

    // Required check
    if (rules.required && (value === null || value === undefined || isNaN(value))) {
      errors.push('This field is required')
      return { isValid: false, errors, warnings }
    }

    // Skip other validations if value is null/undefined and not required
    if (value === null || value === undefined || isNaN(value)) {
      return { isValid: true, errors, warnings }
    }

    // Range validation
    if (rules.min !== undefined && value < rules.min) {
      errors.push(`Must be at least ${rules.min}`)
    }
    if (rules.max !== undefined && value > rules.max) {
      errors.push(`Must not exceed ${rules.max}`)
    }

    return {
      isValid: errors.length === 0,
      errors,
      warnings
    }
  },

  // SKU uniqueness validation (async)
  validateSkuUniqueness: async (sku: string, excludeId?: string): Promise<ValidationResult> => {
    const errors: string[] = []
    const warnings: string[] = []

    try {
      // This would typically make an API call to check SKU uniqueness
      // For now, we'll simulate the check
      const existingSkus = ['EXISTING-SKU-1', 'EXISTING-SKU-2'] // This would come from API
      
      if (existingSkus.includes(sku.toUpperCase())) {
        errors.push('SKU already exists')
      }
    } catch (error) {
      warnings.push('Could not verify SKU uniqueness')
    }

    return {
      isValid: errors.length === 0,
      errors,
      warnings
    }
  },

  // Category hierarchy validation
  validateCategoryHierarchy: (categoryId: string, parentCategoryId: string | null, allCategories: Category[]): ValidationResult => {
    const errors: string[] = []
    const warnings: string[] = []

    // Check for circular reference
    if (parentCategoryId) {
      const checkCircularReference = (catId: string, parentId: string, categories: Category[], visited: Set<string> = new Set()): boolean => {
        if (visited.has(catId)) return true
        visited.add(catId)

        const parent = categories.find(c => c.id === parentId)
        if (!parent) return false

        if (parent.id === catId) return true
        if (!parent.parentCategoryId) return false

        return checkCircularReference(catId, parent.parentCategoryId, categories, visited)
      }

      if (checkCircularReference(categoryId, parentCategoryId, allCategories)) {
        errors.push('Cannot create circular reference in category hierarchy')
      }
    }

    return {
      isValid: errors.length === 0,
      errors,
      warnings
    }
  },

  // Product-Category relationship validation
  validateProductCategory: (categoryId: string, allCategories: Category[]): ValidationResult => {
    const errors: string[] = []
    const warnings: string[] = []

    const category = allCategories.find(c => c.id === categoryId)
    if (!category) {
      errors.push('Selected category does not exist')
    } else if (!category.isActive) {
      warnings.push('Selected category is inactive')
    }

    return {
      isValid: errors.length === 0,
      errors,
      warnings
    }
  },

  // Price validation (cost vs selling price)
  validatePriceRelationship: (basePrice: number, costPrice: number): ValidationResult => {
    const errors: string[] = []
    const warnings: string[] = []

    if (costPrice > basePrice) {
      errors.push('Cost price cannot be higher than selling price')
    } else if (basePrice - costPrice < 0) {
      warnings.push('Profit margin is very low')
    } else if ((basePrice - costPrice) / basePrice < 0.1) {
      warnings.push('Profit margin is less than 10%')
    }

    return {
      isValid: errors.length === 0,
      errors,
      warnings
    }
  }
}

// Main validation functions
export const ProductCatalogValidation = {
  // Category validation
  validateCategory: (categoryData: CreateCategoryRequest | UpdateCategoryRequest, allCategories: Category[] = []): ValidationResult => {
    const errors: string[] = []
    const warnings: string[] = []

    // Name validation
    const nameResult = ValidationHelpers.validateString(categoryData.name || '', VALIDATION_RULES.CATEGORY_NAME)
    errors.push(...nameResult.errors)
    warnings.push(...nameResult.warnings)

    // Code validation
    if (categoryData.code) {
      const codeResult = ValidationHelpers.validateString(categoryData.code, VALIDATION_RULES.CATEGORY_CODE)
      errors.push(...codeResult.errors)
      warnings.push(...codeResult.warnings)
    }

    // Description validation
    if (categoryData.description) {
      const descResult = ValidationHelpers.validateString(categoryData.description, VALIDATION_RULES.CATEGORY_DESCRIPTION)
      errors.push(...descResult.errors)
      warnings.push(...descResult.warnings)
    }

    // Hierarchy validation (only for update operations with id)
    if (categoryData.parentId && 'id' in categoryData) {
      const hierarchyResult = ValidationHelpers.validateCategoryHierarchy(
        (categoryData as any).id, 
        categoryData.parentId, 
        allCategories
      )
      errors.push(...hierarchyResult.errors)
      warnings.push(...hierarchyResult.warnings)
    }

    return {
      isValid: errors.length === 0,
      errors,
      warnings
    }
  },

  // Product validation
  validateProduct: async (productData: CreateProductRequest | UpdateProductRequest, allCategories: Category[] = []): Promise<ValidationResult> => {
    const errors: string[] = []
    const warnings: string[] = []

    // Name validation
    const nameResult = ValidationHelpers.validateString(productData.name || '', VALIDATION_RULES.PRODUCT_NAME)
    errors.push(...nameResult.errors)
    warnings.push(...nameResult.warnings)

    // SKU validation (only for create operations)
    if ('sku' in productData && productData.sku) {
      const skuResult = ValidationHelpers.validateString(productData.sku, VALIDATION_RULES.PRODUCT_SKU)
      errors.push(...skuResult.errors)
      warnings.push(...skuResult.warnings)

      // SKU uniqueness check
      if (skuResult.isValid) {
        const uniquenessResult = await ValidationHelpers.validateSkuUniqueness(
          productData.sku, 
          'id' in productData ? (productData as any).id : undefined
        )
        errors.push(...uniquenessResult.errors)
        warnings.push(...uniquenessResult.warnings)
      }
    }

    // Barcode validation
    if (productData.barcode) {
      const barcodeResult = ValidationHelpers.validateString(productData.barcode, VALIDATION_RULES.PRODUCT_BARCODE)
      errors.push(...barcodeResult.errors)
      warnings.push(...barcodeResult.warnings)
    }

    // Description validation
    if (productData.description) {
      const descResult = ValidationHelpers.validateString(productData.description, VALIDATION_RULES.PRODUCT_DESCRIPTION)
      errors.push(...descResult.errors)
      warnings.push(...descResult.warnings)
    }

    // Brand validation
    if (productData.brand) {
      const brandResult = ValidationHelpers.validateString(productData.brand, VALIDATION_RULES.PRODUCT_BRAND)
      errors.push(...brandResult.errors)
      warnings.push(...brandResult.warnings)
    }

    // Price validation
    const priceResult = ValidationHelpers.validateNumber(productData.basePrice || 0, VALIDATION_RULES.PRODUCT_PRICE)
    errors.push(...priceResult.errors)
    warnings.push(...priceResult.warnings)

    // Cost validation
    const costResult = ValidationHelpers.validateNumber(productData.costPrice || 0, VALIDATION_RULES.PRODUCT_COST)
    errors.push(...costResult.errors)
    warnings.push(...costResult.warnings)

    // Price relationship validation
    if (productData.basePrice && productData.costPrice) {
      const priceRelResult = ValidationHelpers.validatePriceRelationship(
        productData.basePrice, 
        productData.costPrice
      )
      errors.push(...priceRelResult.errors)
      warnings.push(...priceRelResult.warnings)
    }

    // Category validation
    if (productData.categoryId) {
      const categoryResult = ValidationHelpers.validateProductCategory(productData.categoryId, allCategories)
      errors.push(...categoryResult.errors)
      warnings.push(...categoryResult.warnings)
    }

    return {
      isValid: errors.length === 0,
      errors,
      warnings
    }
  },

  // Product variation validation
  validateProductVariation: (variationData: CreateProductVariationRequest | UpdateProductVariationRequest): ValidationResult => {
    const errors: string[] = []
    const warnings: string[] = []

    // Size validation
    const sizeResult = ValidationHelpers.validateString(variationData.size || '', VALIDATION_RULES.VARIATION_SIZE)
    errors.push(...sizeResult.errors)
    warnings.push(...sizeResult.warnings)

    // Color validation
    const colorResult = ValidationHelpers.validateString(variationData.color || '', VALIDATION_RULES.VARIATION_COLOR)
    errors.push(...colorResult.errors)
    warnings.push(...colorResult.warnings)

    // Stock validation
    const stockResult = ValidationHelpers.validateNumber(variationData.stockQuantity || 0, VALIDATION_RULES.VARIATION_STOCK)
    errors.push(...stockResult.errors)
    warnings.push(...stockResult.warnings)

    return {
      isValid: errors.length === 0,
      errors,
      warnings
    }
  },

  // Bulk validation
  validateBulkOperations: (items: any[], validator: (item: any) => ValidationResult): ValidationResult => {
    const allErrors: string[] = []
    const allWarnings: string[] = []

    items.forEach((item, index) => {
      const result = validator(item)
      if (!result.isValid) {
        allErrors.push(...result.errors.map(error => `Item ${index + 1}: ${error}`))
      }
      allWarnings.push(...result.warnings.map(warning => `Item ${index + 1}: ${warning}`))
    })

    return {
      isValid: allErrors.length === 0,
      errors: allErrors,
      warnings: allWarnings
    }
  }
}

// Error handling utilities
export const ErrorHandler = {
  // Format validation errors for display
  formatValidationErrors: (result: ValidationResult): string => {
    if (result.isValid) return ''
    
    const errorMessages = result.errors.map(error => `• ${error}`)
    const warningMessages = result.warnings.map(warning => `⚠ ${warning}`)
    
    return [
      ...(result.errors.length > 0 ? ['Validation Errors:'] : []),
      ...errorMessages,
      ...(result.warnings.length > 0 ? ['Warnings:'] : []),
      ...warningMessages
    ].join('\n')
  },

  // Create user-friendly error messages
  createUserFriendlyMessage: (error: any): string => {
    if (error?.response?.data?.message) {
      return error.response.data.message
    }
    
    if (error?.message) {
      return error.message
    }
    
    if (typeof error === 'string') {
      return error
    }
    
    return 'An unexpected error occurred. Please try again.'
  },

  // Determine error severity
  getErrorSeverity: (error: any): 'low' | 'medium' | 'high' => {
    if (error?.response?.status >= 500) return 'high'
    if (error?.response?.status >= 400) return 'medium'
    return 'low'
  }
}

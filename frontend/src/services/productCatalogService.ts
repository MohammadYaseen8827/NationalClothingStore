import axios from 'axios'
import type { 
  Category, 
  Product, 
  ProductVariation, 
  ProductImage,
  CreateCategoryRequest,
  UpdateCategoryRequest,
  CreateProductRequest,
  UpdateProductRequest,
  CreateProductVariationRequest,
  UpdateProductVariationRequest,
  PaginationMetadata,
  FileUploadResult
} from '@/types/productCatalog'

// API base URL
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '/api'

// Create axios instance with default configuration
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor for adding auth token
apiClient.interceptors.request.use(
  (config: any) => {
    const token = localStorage.getItem('authToken')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error: any) => Promise.reject(error)
)

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response: any) => response,
  (error: any) => {
    console.error('API Error:', error)
    
    if (error.response?.status === 401) {
      // Handle unauthorized - redirect to login
      localStorage.removeItem('authToken')
      window.location.href = '/login'
    }
    
    return Promise.reject(error)
  }
)

// API Response wrapper
interface ApiResponse<T> {
  data: T
  success: boolean
  message?: string
  errors?: string[]
}

// Category API
export const categoryApi = {
  // Get all categories
  async getCategories(includeHierarchy = false): Promise<Category[]> {
    const response = await apiClient.get<ApiResponse<Category[]>>('/categories', {
      params: { includeHierarchy }
    })
    return response.data.data
  },

  // Get category by ID
  async getCategory(id: string): Promise<Category> {
    const response = await apiClient.get<ApiResponse<Category>>(`/categories/${id}`)
    return response.data.data
  },

  // Get root categories
  async getRootCategories(): Promise<Category[]> {
    const response = await apiClient.get<ApiResponse<Category[]>>('/categories/root')
    return response.data.data
  },

  // Get child categories
  async getChildCategories(parentId: string): Promise<Category[]> {
    const response = await apiClient.get<ApiResponse<Category[]>>(`/categories/${parentId}/children`)
    return response.data.data
  },

  // Create category
  async createCategory(category: CreateCategoryRequest): Promise<Category> {
    const response = await apiClient.post<ApiResponse<Category>>('/categories', category)
    return response.data.data
  },

  // Update category
  async updateCategory(id: string, category: UpdateCategoryRequest): Promise<Category> {
    const response = await apiClient.put<ApiResponse<Category>>(`/categories/${id}`, category)
    return response.data.data
  },

  // Delete category
  async deleteCategory(id: string): Promise<void> {
    await apiClient.delete(`/categories/${id}`)
  },

  // Validate category deletion
  async validateCategoryDeletion(id: string): Promise<boolean> {
    const response = await apiClient.get<ApiResponse<boolean>>(`/categories/${id}/validate-deletion`)
    return response.data.data
  }
}

// Product API
export const productApi = {
  // Get products with pagination and filtering
  async getProducts(params: {
    pageNumber?: number
    pageSize?: number
    categoryId?: string
    search?: string
    isActive?: boolean
    brand?: string
    season?: string
  } = {}): Promise<{ products: Product[], pagination: PaginationMetadata }> {
    const response = await apiClient.get<ApiResponse<{ products: Product[], pagination: PaginationMetadata }>>('/products', {
      params
    })
    return response.data.data
  },

  // Search products
  async searchProducts(params: {
    searchTerm: string
    pageNumber?: number
    pageSize?: number
    includeInactive?: boolean
  }): Promise<{ products: Product[], pagination: PaginationMetadata }> {
    const response = await apiClient.get<ApiResponse<{ products: Product[], pagination: PaginationMetadata }>>('/products/search', {
      params
    })
    return response.data.data
  },

  // Get product by ID
  async getProduct(id: string): Promise<Product> {
    const response = await apiClient.get<ApiResponse<Product>>(`/products/${id}`)
    return response.data.data
  },

  // Get product by SKU
  async getProductBySku(sku: string): Promise<Product> {
    const response = await apiClient.get<ApiResponse<Product>>(`/products/by-sku/${sku}`)
    return response.data.data
  },

  // Create product
  async createProduct(product: CreateProductRequest): Promise<Product> {
    const response = await apiClient.post<ApiResponse<Product>>('/products', product)
    return response.data.data
  },

  // Update product
  async updateProduct(id: string, product: UpdateProductRequest): Promise<Product> {
    const response = await apiClient.put<ApiResponse<Product>>(`/products/${id}`, product)
    return response.data.data
  },

  // Delete product
  async deleteProduct(id: string): Promise<void> {
    await apiClient.delete(`/products/${id}`)
  },

  // Get product variations
  async getProductVariations(productId: string, params: {
    pageNumber?: number
    pageSize?: number
    size?: string
    color?: string
    isActive?: boolean
  } = {}): Promise<{ variations: ProductVariation[], pagination: PaginationMetadata }> {
    const response = await apiClient.get<ApiResponse<{ variations: ProductVariation[], pagination: PaginationMetadata }>>(
      `/products/${productId}/variations`,
      { params }
    )
    return response.data.data
  },

  // Get available sizes for product
  async getAvailableSizes(productId: string): Promise<string[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(`/products/${productId}/sizes`)
    return response.data.data
  },

  // Get available colors for product
  async getAvailableColors(productId: string): Promise<string[]> {
    const response = await apiClient.get<ApiResponse<string[]>>(`/products/${productId}/colors`)
    return response.data.data
  },

  // Get total stock for product
  async getTotalStock(productId: string): Promise<number> {
    const response = await apiClient.get<ApiResponse<number>>(`/products/${productId}/total-stock`)
    return response.data.data
  },

  // Get low stock variations
  async getLowStockVariations(threshold = 10): Promise<ProductVariation[]> {
    const response = await apiClient.get<ApiResponse<ProductVariation[]>>('/products/low-stock', {
      params: { threshold }
    })
    return response.data.data
  },

  // Validate product deletion
  async validateProductDeletion(id: string): Promise<boolean> {
    const response = await apiClient.get<ApiResponse<boolean>>(`/products/${id}/validate-deletion`)
    return response.data.data
  }
}

// Product Variation API
export const productVariationApi = {
  // Get variation by ID
  async getVariation(id: string): Promise<ProductVariation> {
    const response = await apiClient.get<ApiResponse<ProductVariation>>(`/ProductVariations/${id}`)
    return response.data.data
  },

  // Create product variation
  async createVariation(productId: string, variation: CreateProductVariationRequest): Promise<ProductVariation> {
    const response = await apiClient.post<ApiResponse<ProductVariation>>(`/products/${productId}/variations`, variation)
    return response.data.data
  },

  // Update product variation
  async updateVariation(id: string, variation: UpdateProductVariationRequest): Promise<ProductVariation> {
    const response = await apiClient.put<ApiResponse<ProductVariation>>(`/ProductVariations/${id}`, variation)
    return response.data.data
  },

  // Delete product variation
  async deleteVariation(id: string): Promise<void> {
    await apiClient.delete(`/ProductVariations/${id}`)
  },

  // Update variation stock
  async updateStock(id: string, quantity: number): Promise<void> {
    await apiClient.patch(`/ProductVariations/${id}/stock`, { quantity })
  },

  // Get variation by SKU
  async getVariationBySku(sku: string): Promise<ProductVariation> {
    const response = await apiClient.get<ApiResponse<ProductVariation>>(`/ProductVariations/by-sku/${sku}`)
    return response.data.data
  }
}

// File Upload API
export const fileUploadApi = {
  // Upload single file
  async uploadFile(file: File, folder: string): Promise<FileUploadResult> {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('folder', folder)

    const response = await apiClient.post<ApiResponse<FileUploadResult>>('/files/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    return response.data.data
  },

  // Upload multiple files
  async uploadFiles(files: File[], folder: string): Promise<FileUploadResult[]> {
    const formData = new FormData()
    files.forEach(file => formData.append('files', file))
    formData.append('folder', folder)

    const response = await apiClient.post<ApiResponse<FileUploadResult[]>>('/files/upload-multiple', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    return response.data.data
  },

  // Delete file
  async deleteFile(filePath: string): Promise<void> {
    await apiClient.delete('/files', { data: { filePath } } as any)
  },

  // Get file info
  async getFileInfo(filePath: string): Promise<any> {
    const response = await apiClient.get<ApiResponse<any>>('/files/info', {
      params: { filePath }
    })
    return response.data.data
  },

  // Check if file exists
  async fileExists(filePath: string): Promise<boolean> {
    const response = await apiClient.get<ApiResponse<boolean>>('/files/exists', {
      params: { filePath }
    })
    return response.data.data
  }
}

// Product Image API
export const productImageApi = {
  // Add product image
  async addImage(productId: string, imageData: {
    imageUrl: string
    altText?: string
    caption?: string
    sortOrder?: number
    isPrimary?: boolean
  }): Promise<ProductImage> {
    const response = await apiClient.post<ApiResponse<ProductImage>>(`/products/${productId}/images`, imageData)
    return response.data.data
  },

  // Update product image
  async updateImage(id: string, imageData: {
    imageUrl: string
    altText?: string
    caption?: string
    sortOrder?: number
    isPrimary?: boolean
  }): Promise<ProductImage> {
    const response = await apiClient.put<ApiResponse<ProductImage>>(`/images/${id}`, imageData)
    return response.data.data
  },

  // Delete product image
  async deleteImage(id: string): Promise<void> {
    await apiClient.delete(`/images/${id}`)
  },

  // Get product images
  async getProductImages(productId: string): Promise<ProductImage[]> {
    const response = await apiClient.get<ApiResponse<ProductImage[]>>(`/products/${productId}/images`)
    return response.data.data
  },

  // Get primary image
  async getPrimaryImage(productId: string): Promise<ProductImage | null> {
    const response = await apiClient.get<ApiResponse<ProductImage | null>>(`/products/${productId}/images/primary`)
    return response.data.data
  }
}

// Utility functions
export const productCatalogUtils = {
  // Format currency
  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount)
  },

  // Format date
  formatDate(date: string | Date): string {
    return new Intl.DateTimeFormat('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    }).format(new Date(date))
  },

  // Format date with time
  formatDateTime(date: string | Date): string {
    return new Intl.DateTimeFormat('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(new Date(date))
  },

  // Get stock status class
  getStockClass(quantity: number): string {
    if (quantity === 0) return 'stock-out'
    if (quantity <= 10) return 'stock-low'
    return 'stock-good'
  },

  // Get stock status text
  getStockStatus(quantity: number): string {
    if (quantity === 0) return 'Out of Stock'
    if (quantity <= 10) return 'Low Stock'
    return 'In Stock'
  },

  // Generate unique SKU
  generateSku(productName: string, size?: string, color?: string): string {
    const namePart = productName
      .toUpperCase()
      .replace(/[^A-Z0-9]/g, '')
      .substring(0, 6)
    
    const sizePart = size ? `-${size.toUpperCase()}` : ''
    const colorPart = color ? `-${color.toUpperCase().substring(0, 3)}` : ''
    const randomPart = Math.random().toString(36).substring(2, 5).toUpperCase()
    
    return `${namePart}${sizePart}${colorPart}-${randomPart}`
  },

  // Validate SKU format
  isValidSku(sku: string): boolean {
    return /^[A-Z0-9-]{8,20}$/.test(sku)
  },

  // Calculate total price
  calculateTotalPrice(basePrice: number, additionalPrice: number): number {
    return basePrice + additionalPrice
  },

  // Sort products
  sortProducts(products: Product[], field: string, direction: 'asc' | 'desc'): Product[] {
    return [...products].sort((a, b) => {
      let aValue = a[field as keyof Product]
      let bValue = b[field as keyof Product]
      
      if (aValue === undefined || bValue === undefined) {
        return 0
      }
      
      if (typeof aValue === 'string') {
        aValue = aValue.toLowerCase()
        bValue = (bValue as string).toLowerCase()
      }
      
      if (direction === 'asc') {
        return aValue > bValue ? 1 : -1
      } else {
        return aValue < bValue ? 1 : -1
      }
    })
  },

  // Filter products
  filterProducts(products: Product[], filters: {
    search?: string
    categoryId?: string
    brand?: string
    season?: string
    isActive?: boolean
    minPrice?: number
    maxPrice?: number
  }): Product[] {
    return products.filter(product => {
      // Search filter
      if (filters.search) {
        const search = filters.search.toLowerCase()
        const matchesSearch = 
          product.name.toLowerCase().includes(search) ||
          product.description?.toLowerCase().includes(search) ||
          product.sku.toLowerCase().includes(search) ||
          product.brand?.toLowerCase().includes(search)
        
        if (!matchesSearch) return false
      }

      // Category filter
      if (filters.categoryId && product.categoryId !== filters.categoryId) {
        return false
      }

      // Brand filter
      if (filters.brand && product.brand !== filters.brand) {
        return false
      }

      // Season filter
      if (filters.season && product.season !== filters.season) {
        return false
      }

      // Active filter
      if (filters.isActive !== undefined && product.isActive !== filters.isActive) {
        return false
      }

      // Price range filter
      if (filters.minPrice !== undefined && product.basePrice < filters.minPrice) {
        return false
      }
      if (filters.maxPrice !== undefined && product.basePrice > filters.maxPrice) {
        return false
      }

      return true
    })
  }
}

// Export all APIs as default
export default {
  category: categoryApi,
  product: productApi,
  variation: productVariationApi,
  file: fileUploadApi,
  image: productImageApi,
  utils: productCatalogUtils
}

// Export types for external use
export type {
  Category,
  Product,
  ProductVariation,
  ProductImage,
  CreateCategoryRequest,
  UpdateCategoryRequest,
  CreateProductRequest,
  UpdateProductRequest,
  CreateProductVariationRequest,
  UpdateProductVariationRequest,
  PaginationMetadata,
  FileUploadResult
}

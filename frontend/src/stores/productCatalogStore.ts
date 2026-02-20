import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import productCatalogService from '@/services/productCatalogService'
import type { 
  Category, 
  Product, 
  ProductVariation, 
  ProductImage,
  CreateCategoryRequest,
  UpdateCategoryRequest,
  CreateProductRequest,
  UpdateProductRequest,
  ProductSearchRequest,
  ProductFilterRequest,
  CreateProductVariationRequest,
  UpdateProductVariationRequest,
  VariationFilterRequest,
  FileUploadResult,
  PaginationMetadata,
  StockStatus,
  PriceCalculation
} from '@/types/productCatalog'

export const useProductCatalogStore = defineStore('productCatalog', () => {
  // State
  const categories = ref<Category[]>([])
  const categoriesLoading = ref(false)
  const categoriesError = ref<string | null>(null)
  
  const products = ref<Product[]>([])
  const productsLoading = ref(false)
  const productsError = ref<string | null>(null)
  const productsPagination = ref<PaginationMetadata | null>(null)
  
  const productVariations = ref<Record<string, ProductVariation[]>>({})
  const variationsLoading = ref<Record<string, boolean>>({})
  const variationsError = ref<Record<string, string | null>>({})
  
  const productImages = ref<Record<string, ProductImage[]>>({})
  const imagesLoading = ref<Record<string, boolean>>({})
  const imagesError = ref<Record<string, string | null>>({})
  
  const searchQuery = ref('')
  const searchResults = ref<Product[]>([])
  const searchLoading = ref(false)
  const searchError = ref<string | null>(null)
  
  const selectedCategory = ref<string | null>(null)
  const selectedProduct = ref<string | null>(null)
  const selectedVariation = ref<string | null>(null)
  const activeFilters = ref<ProductFilterRequest>({})
  
  const totalProducts = ref(0)
  const totalCategories = ref(0)
  const totalVariations = ref(0)
  const lowStockCount = ref(0)
  const outOfStockCount = ref(0)
  const totalStockValue = ref(0)

  // Getters
  const activeCategories = computed(() => categories.value.filter(cat => cat.isActive))
  const categoryById = computed(() => {
    return (id: string) => categories.value.find(cat => cat.id === id)
  })
  const rootCategories = computed(() => categories.value.filter(cat => !cat.parentCategoryId))
  const categoriesByParent = computed(() => {
    const grouped: Record<string, Category[]> = {}
    categories.value.forEach(category => {
      const parentId = category.parentCategoryId || 'root'
      if (!grouped[parentId]) {
        grouped[parentId] = []
      }
      grouped[parentId].push(category)
    })
    return grouped
  })
  const categoryTree = computed(() => {
    const buildTree = (categories: Category[]): Category[] => {
      const categoryMap = new Map<string, Category>()
      const rootCategories: Category[] = []
      
      // Create category map
      categories.forEach(cat => {
        categoryMap.set(cat.id, { ...cat, children: [] })
      })
      
      // Build tree structure
      categories.forEach(cat => {
        if (cat.parentCategoryId) {
          const parent = categoryMap.get(cat.parentCategoryId)
          if (parent && parent.children) {
            parent.children.push(categoryMap.get(cat.id)!)
          }
        } else {
          rootCategories.push(categoryMap.get(cat.id)!)
        }
      })
      
      // Sort children
      rootCategories.forEach(category => {
        if (category.children) {
          category.children.sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0))
        }
      })
      
      return rootCategories
    }
    
    return buildTree(categories.value)
  })
  
  // Product getters
  const activeProducts = computed(() => products.value.filter(product => product.isActive))
  const productById = computed(() => {
    return (id: string) => products.value.find(product => product.id === id)
  })
  const productBySku = computed(() => {
    return (sku: string) => products.value.find(product => product.sku === sku)
  })
  const productsByCategory = computed(() => {
    return (categoryId: string) => products.value.filter(product => product.categoryId === categoryId)
  })
  const productsByBrand = computed(() => {
    const grouped: Record<string, Product[]> = {}
    products.value.forEach(product => {
      const brand = product.brand || 'Uncategorized'
      if (!grouped[brand]) {
        grouped[brand] = []
      }
      grouped[brand].push(product)
    })
    return grouped
  })
  const productsBySeason = computed(() => {
    const grouped: Record<string, Product[]> = {}
    products.value.forEach(product => {
      const season = product.season || 'All-Season'
      if (!grouped[season]) {
        grouped[season] = []
      }
      grouped[season].push(product)
    })
    return grouped
  })
  const filteredProducts = computed(() => {
    let filtered = products.value
    
    // Apply active filter
    if (activeFilters.value.isActive !== undefined) {
      filtered = filtered.filter(product => product.isActive === activeFilters.value.isActive)
    }
    
    // Apply category filter
    if (activeFilters.value.categoryId) {
      filtered = filtered.filter(product => product.categoryId === activeFilters.value.categoryId)
    }
    
    // Apply brand filter
    if (activeFilters.value.brand) {
      filtered = filtered.filter(product => product.brand === activeFilters.value.brand)
    }
    
    // Apply season filter
    if (activeFilters.value.season) {
      filtered = filtered.filter(product => product.season === activeFilters.value.season)
    }
    
    // Apply search filter
    if (searchQuery.value) {
      const query = searchQuery.value.toLowerCase()
      filtered = filtered.filter(product => 
        product.name.toLowerCase().includes(query) ||
        product.description?.toLowerCase().includes(query) ||
        product.sku.toLowerCase().includes(query) ||
        product.brand?.toLowerCase().includes(query)
      )
    }
    
    // Apply price range filter
    if (activeFilters.value.minPrice !== undefined) {
      filtered = filtered.filter(product => product.basePrice >= activeFilters.value.minPrice!)
    }
    if (activeFilters.value.maxPrice !== undefined) {
      filtered = filtered.filter(product => product.basePrice <= activeFilters.value.maxPrice!)
    }
    
    return filtered
  })
  
  // Variation getters
  const variationsByProduct = computed(() => {
    return (productId: string) => productVariations.value[productId] || []
  })
  const variationById = computed(() => {
    return (id: string) => {
      for (const productId in productVariations.value) {
        const variations = productVariations.value[productId]
        if (variations) {
          const variation = variations.find(v => v.id === id)
          if (variation) return variation
        }
      }
      return null
    }
  })
  const activeVariations = computed(() => {
    const allVariations: ProductVariation[] = []
    Object.values(productVariations.value).forEach(variations => {
      allVariations.push(...variations)
    })
    return allVariations.filter(v => v.isActive)
  })
  const variationsBySize = computed(() => {
    const grouped: Record<string, ProductVariation[]> = {}
    activeVariations.value.forEach(variation => {
      const { size } = variation
      if (!grouped[size]) {
        grouped[size] = []
      }
      grouped[size].push(variation)
    })
    return grouped
  })
  const variationsByColor = computed(() => {
    const grouped: Record<string, ProductVariation[]> = {}
    activeVariations.value.forEach(variation => {
      const { color } = variation
      if (!grouped[color]) {
        grouped[color] = []
      }
      grouped[color].push(variation)
    })
    return grouped
  })
  
  // Image getters
  const imagesByProduct = computed(() => {
    return (productId: string) => productImages.value[productId] || []
  })
  const primaryImage = computed(() => {
    return (productId: string) => {
      const images = productImages.value[productId] || []
      return images.find(img => img.isPrimary) || images[0] || null
    }
  })
  
  // Search getters
  const searchResultsWithPagination = computed(() => ({
    results: searchResults.value,
    pagination: productsPagination.value
  }))
  
  // Statistics getters
  const totalActiveProducts = computed(() => products.value.filter(p => p.isActive).length)
  const totalActiveCategories = computed(() => categories.value.filter(c => c.isActive).length)
  const totalActiveVariations = computed(() => activeVariations.value.length)
  const totalActiveStock = computed(() => activeVariations.value.reduce((total, v) => total + v.stockQuantity, 0))
  const averageProductPrice = computed(() => {
    const activeProducts = products.value.filter(p => p.isActive)
    if (activeProducts.length === 0) return 0
    const totalPrice = activeProducts.reduce((sum, p) => sum + p.basePrice, 0)
    return totalPrice / activeProducts.length
  })
  const mostExpensiveProduct = computed(() => {
    if (activeProducts.value.length === 0) return null
    return activeProducts.value.reduce((max, product) => 
      product.basePrice > (max?.basePrice || 0) ? product : max
    )
  })
  const leastExpensiveProduct = computed(() => {
    if (activeProducts.value.length === 0) return null
    return activeProducts.value.reduce((min, product) => 
      product.basePrice < (min?.basePrice || Infinity) ? product : min
    )
  })
  
  // Stock status getters
  const lowStockVariations = computed(() => activeVariations.value.filter(v => v.stockQuantity > 0 && v.stockQuantity <= 10))
  const outOfStockVariations = computed(() => activeVariations.value.filter(v => v.stockQuantity === 0))
  const inStockVariations = computed(() => activeVariations.value.filter(v => v.stockQuantity > 10))
  
  // Utility getters
  const uniqueBrands = computed(() => {
    const brands = [...new Set(products.value.map(p => p.brand).filter(Boolean))]
    return brands.sort()
  })
  const uniqueSeasons = computed(() => {
    const seasons = [...new Set(products.value.map(p => p.season).filter(Boolean))]
    return seasons.sort()
  })
  const uniqueSizes = computed(() => {
    const sizes = [...new Set(activeVariations.value.map(v => v.size))]
    return sizes.sort()
  })
  const uniqueColors = computed(() => {
    const colors = [...new Set(activeVariations.value.map(v => v.color))]
    return colors.sort()
  })

  // Actions
  const fetchCategories = async (includeHierarchy = false) => {
    categoriesLoading.value = true
    categoriesError.value = null
    
    try {
      categories.value = await productCatalogService.category.getCategories(includeHierarchy)
      totalCategories.value = categories.value.length
    } catch (error: any) {
      categoriesError.value = error.message || 'Failed to fetch categories'
      console.error('Error fetching categories:', error)
    } finally {
      categoriesLoading.value = false
    }
  }

  const createCategory = async (categoryData: CreateCategoryRequest) => {
    try {
      const category = await productCatalogService.category.createCategory(categoryData)
      categories.value.push(category)
      totalCategories.value = categories.value.length
      return category
    } catch (error: any) {
      categoriesError.value = error.message || 'Failed to create category'
      console.error('Error creating category:', error)
      throw error
    }
  }

  const updateCategory = async (id: string, categoryData: UpdateCategoryRequest) => {
    try {
      const updatedCategory = await productCatalogService.category.updateCategory(id, categoryData)
      const index = categories.value.findIndex(cat => cat.id === id)
      if (index !== -1) {
        categories.value[index] = updatedCategory
      }
      return updatedCategory
    } catch (error: any) {
      categoriesError.value = error.message || 'Failed to update category'
      console.error('Error updating category:', error)
      throw error
    }
  }

  const deleteCategory = async (id: string) => {
    try {
      await productCatalogService.category.deleteCategory(id)
      const index = categories.value.findIndex(cat => cat.id === id)
      if (index !== -1) {
        categories.value.splice(index, 1)
      }
      totalCategories.value = categories.value.length
    } catch (error: any) {
      categoriesError.value = error.message || 'Failed to delete category'
      console.error('Error deleting category:', error)
      throw error
    }
  }

  const fetchProducts = async (params: ProductFilterRequest = {}) => {
    productsLoading.value = true
    productsError.value = null
    
    try {
      const response = await productCatalogService.product.getProducts(params)
      products.value = response.products
      productsPagination.value = response.pagination
      totalProducts.value = response.pagination.totalCount
    } catch (error: any) {
      productsError.value = error.message || 'Failed to fetch products'
      console.error('Error fetching products:', error)
    } finally {
      productsLoading.value = false
    }
  }

  const searchProducts = async (searchRequest: ProductSearchRequest) => {
    searchLoading.value = true
    searchError.value = null
    
    try {
      const response = await productCatalogService.product.searchProducts(searchRequest)
      searchResults.value = response.products
      productsPagination.value = response.pagination
    } catch (error: any) {
      searchError.value = error.message || 'Failed to search products'
      console.error('Error searching products:', error)
    } finally {
      searchLoading.value = false
    }
  }

  const createProduct = async (productData: CreateProductRequest) => {
    try {
      const product = await productCatalogService.product.createProduct(productData)
      products.value.push(product)
      totalProducts.value = products.value.length
      return product
    } catch (error: any) {
      productsError.value = error.message || 'Failed to create product'
      console.error('Error creating product:', error)
      throw error
    }
  }

  const updateProduct = async (id: string, productData: UpdateProductRequest) => {
    try {
      const updatedProduct = await productCatalogService.product.updateProduct(id, productData)
      const index = products.value.findIndex(product => product.id === id)
      if (index !== -1) {
        products.value[index] = updatedProduct
      }
      return updatedProduct
    } catch (error: any) {
      productsError.value = error.message || 'Failed to update product'
      console.error('Error updating product:', error)
      throw error
    }
  }

  const deleteProduct = async (id: string) => {
    try {
      await productCatalogService.product.deleteProduct(id)
      const index = products.value.findIndex(product => product.id === id)
      if (index !== -1) {
        products.value.splice(index, 1)
      }
      totalProducts.value = products.value.length
      
      // Clean up related data
      delete productVariations.value[id]
      delete productImages.value[id]
      delete variationsLoading.value[id]
      delete variationsError.value[id]
      delete imagesLoading.value[id]
      delete imagesError.value[id]
    } catch (error: any) {
      productsError.value = error.message || 'Failed to delete product'
      console.error('Error deleting product:', error)
      throw error
    }
  }

  const fetchProductVariations = async (productId: string) => {
    variationsLoading.value[productId] = true
    variationsError.value[productId] = null
    
    try {
      const response = await productCatalogService.product.getProductVariations(productId)
      productVariations.value[productId] = response.variations
      totalVariations.value = Object.values(productVariations.value).reduce((total, variations) => total + variations.length, 0)
    } catch (error: any) {
      variationsError.value[productId] = error.message || 'Failed to fetch product variations'
      console.error('Error fetching product variations:', error)
    } finally {
      variationsLoading.value[productId] = false
    }
  }

  const createProductVariation = async (productId: string, variationData: CreateProductVariationRequest) => {
    try {
      const variation = await productCatalogService.variation.createVariation(productId, variationData)
      
      if (!productVariations.value[productId]) {
        productVariations.value[productId] = []
      }
      productVariations.value[productId].push(variation)
      totalVariations.value = Object.values(productVariations.value).reduce((total, variations) => total + variations.length, 0)
      
      return variation
    } catch (error: any) {
      variationsError.value[productId] = error.message || 'Failed to create product variation'
      console.error('Error creating product variation:', error)
      throw error
    }
  }

  const updateProductVariation = async (id: string, variationData: UpdateProductVariationRequest) => {
    try {
      const updatedVariation = await productCatalogService.variation.updateVariation(id, variationData)
      
      // Find and update the variation in the correct product
      for (const productId in productVariations.value) {
        const variations = productVariations.value[productId]
        if (variations) {
          const index = variations.findIndex(v => v.id === id)
          if (index !== -1) {
            variations[index] = updatedVariation
            break
          }
        }
      }
      
      return updatedVariation
    } catch (error: any) {
      console.error('Error updating product variation:', error)
      throw error
    }
  }

  const deleteProductVariation = async (id: string) => {
    try {
      await productCatalogService.variation.deleteVariation(id)
      
      // Find and remove the variation
      for (const productId in productVariations.value) {
        const variations = productVariations.value[productId]
        if (variations) {
          const index = variations.findIndex(v => v.id === id)
          if (index !== -1) {
            variations.splice(index, 1)
            break
          }
        }
      }
      
      totalVariations.value = Object.values(productVariations.value).reduce((total, variations) => total + variations.length, 0)
    } catch (error: any) {
      console.error('Error deleting product variation:', error)
      throw error
    }
  }

  const updateVariationStock = async (id: string, quantity: number) => {
    try {
      await productCatalogService.variation.updateStock(id, quantity)
      
      // Find and update the variation stock
      for (const productId in productVariations.value) {
        const variations = productVariations.value[productId]
        if (variations) {
          const variation = variations.find(v => v.id === id)
          if (variation) {
            variation.stockQuantity = quantity
            variation.updatedAt = new Date().toISOString()
            break
          }
        }
      }
    } catch (error: any) {
      console.error('Error updating variation stock:', error)
      throw error
    }
  }

  const fetchProductImages = async (productId: string) => {
    imagesLoading.value[productId] = true
    imagesError.value[productId] = null
    
    try {
      const images = await productCatalogService.image.getProductImages(productId)
      productImages.value[productId] = images
    } catch (error: any) {
      imagesError.value[productId] = error.message || 'Failed to fetch product images'
      console.error('Error fetching product images:', error)
    } finally {
      imagesLoading.value[productId] = false
    }
  }

  const addProductImage = async (productId: string, imageData: {
    imageUrl: string
    altText?: string
    caption?: string
    sortOrder?: number
    isPrimary?: boolean
  }) => {
    try {
      const image = await productCatalogService.image.addImage(productId, imageData)
      
      if (!productImages.value[productId]) {
        productImages.value[productId] = []
      }
      productImages.value[productId].push(image)
      
      // If this is set as primary, unset other primary images
      if (imageData.isPrimary) {
        productImages.value[productId].forEach(img => {
          if (img.id !== image.id) {
            img.isPrimary = false
          }
        })
      }
      
      return image
    } catch (error: any) {
      imagesError.value[productId] = error.message || 'Failed to add product image'
      console.error('Error adding product image:', error)
      throw error
    }
  }

  const updateProductImage = async (id: string, imageData: {
    imageUrl: string
    altText?: string
    caption?: string
    sortOrder?: number
    isPrimary?: boolean
  }) => {
    try {
      const updatedImage = await productCatalogService.image.updateImage(id, imageData)
      
      // Find and update the image
      for (const productId in productImages.value) {
        const images = productImages.value[productId]
        if (images) {
          const index = images.findIndex(img => img.id === id)
          if (index !== -1) {
            images[index] = updatedImage
            
            // If this is set as primary, unset other primary images
            if (imageData.isPrimary) {
              images.forEach(img => {
                if (img.id !== id) {
                  img.isPrimary = false
                }
              })
            }
            break
          }
        }
      }
      
      return updatedImage
    } catch (error: any) {
      console.error('Error updating product image:', error)
      throw error
    }
  }

  const deleteProductImage = async (id: string) => {
    try {
      await productCatalogService.image.deleteImage(id)
      
      // Find and remove the image
      for (const productId in productImages.value) {
        const images = productImages.value[productId]
        if (images) {
          const index = images.findIndex(img => img.id === id)
          if (index !== -1) {
            images.splice(index, 1)
            break
          }
        }
      }
    } catch (error: any) {
      console.error('Error deleting product image:', error)
      throw error
    }
  }

  const clearSearch = () => {
    searchQuery.value = ''
    searchResults.value = []
    searchError.value = null
  }

  const setActiveFilters = (filters: ProductFilterRequest) => {
    activeFilters.value = { ...activeFilters.value, ...filters }
  }

  const clearFilters = () => {
    activeFilters.value = {}
  }

  const selectCategory = (categoryId: string | null) => {
    selectedCategory.value = categoryId
    selectedProduct.value = null
    selectedVariation.value = null
  }

  const selectProduct = (productId: string | null) => {
    selectedProduct.value = productId
    selectedVariation.value = null
  }

  const selectVariation = (variationId: string | null) => {
    selectedVariation.value = variationId
  }

  const clearSelection = () => {
    selectedCategory.value = null
    selectedProduct.value = null
    selectedVariation.value = null
  }

  const refreshStatistics = async () => {
    try {
      await Promise.all([
        fetchCategories(),
        fetchProducts(),
        updateStockStatistics()
      ])
    } catch (error: any) {
      console.error('Error refreshing statistics:', error)
    }
  }

  const updateStockStatistics = () => {
    lowStockCount.value = lowStockVariations.value.length
    outOfStockCount.value = outOfStockVariations.value.length
    totalStockValue.value = activeVariations.value.reduce((total, v) => 
      total + (v.product?.basePrice || 0) * v.stockQuantity, 0
    )
  }

  const reset = () => {
    // Reset all state to initial values
    categories.value = []
    categoriesLoading.value = false
    categoriesError.value = null
    
    products.value = []
    productsLoading.value = false
    productsError.value = null
    productsPagination.value = null
    
    productVariations.value = {}
    variationsLoading.value = {}
    variationsError.value = {}
    
    productImages.value = {}
    imagesLoading.value = {}
    imagesError.value = {}
    
    searchQuery.value = ''
    searchResults.value = []
    searchLoading.value = false
    searchError.value = null
    
    selectedCategory.value = null
    selectedProduct.value = null
    selectedVariation.value = null
    activeFilters.value = {}
    
    totalProducts.value = 0
    totalCategories.value = 0
    totalVariations.value = 0
    lowStockCount.value = 0
    outOfStockCount.value = 0
    totalStockValue.value = 0
  }

  const bulkDeleteProducts = async (productIds: string[]) => {
    const errors: string[] = []
    
    for (const id of productIds) {
      try {
        await deleteProduct(id)
      } catch (error: any) {
        errors.push(`Failed to delete product ${id}: ${error.message}`)
      }
    }
    
    if (errors.length > 0) {
      throw new Error(`Bulk delete failed: ${errors.join(', ')}`)
    }
  }

  const bulkUpdateStock = async (stockUpdates: Array<{id: string, quantity: number}>) => {
    const errors: string[] = []
    
    for (const { id, quantity } of stockUpdates) {
      try {
        await updateVariationStock(id, quantity)
      } catch (error: any) {
        errors.push(`Failed to update stock for variation ${id}: ${error.message}`)
      }
    }
    
    if (errors.length > 0) {
      throw new Error(`Bulk stock update failed: ${errors.join(', ')}`)
    }
  }

  return {
    // State
    categories,
    categoriesLoading,
    categoriesError,
    products,
    productsLoading,
    productsError,
    productsPagination,
    productVariations,
    variationsLoading,
    variationsError,
    productImages,
    imagesLoading,
    imagesError,
    searchQuery,
    searchResults,
    searchLoading,
    searchError,
    selectedCategory,
    selectedProduct,
    selectedVariation,
    activeFilters,
    totalProducts,
    totalCategories,
    totalVariations,
    lowStockCount,
    outOfStockCount,
    totalStockValue,

    // Getters
    activeCategories,
    categoryById,
    rootCategories,
    categoriesByParent,
    categoryTree,
    activeProducts,
    productById,
    productBySku,
    productsByCategory,
    productsByBrand,
    productsBySeason,
    filteredProducts,
    variationsByProduct,
    variationById,
    activeVariations,
    variationsBySize,
    variationsByColor,
    imagesByProduct,
    primaryImage,
    searchResultsWithPagination,
    totalActiveProducts,
    totalActiveCategories,
    totalActiveVariations,
    totalActiveStock,
    averageProductPrice,
    mostExpensiveProduct,
    leastExpensiveProduct,
    lowStockVariations,
    outOfStockVariations,
    inStockVariations,
    uniqueBrands,
    uniqueSeasons,
    uniqueSizes,
    uniqueColors,

    // Actions
    fetchCategories,
    createCategory,
    updateCategory,
    deleteCategory,
    fetchProducts,
    searchProducts,
    createProduct,
    updateProduct,
    deleteProduct,
    fetchProductVariations,
    createProductVariation,
    updateProductVariation,
    deleteProductVariation,
    updateVariationStock,
    fetchProductImages,
    addProductImage,
    updateProductImage,
    deleteProductImage,
    clearSearch,
    setActiveFilters,
    clearFilters,
    selectCategory,
    selectProduct,
    selectVariation,
    clearSelection,
    refreshStatistics,
    updateStockStatistics,
    reset,
    bulkDeleteProducts,
    bulkUpdateStock
  }
})

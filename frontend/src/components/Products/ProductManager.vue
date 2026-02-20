<template>
  <div class="product-manager">
    <!-- Header -->
    <div class="product-header">
      <h2>Product Management</h2>
      <div class="header-actions">
        <button 
          @click="showCreateModal = true" 
          class="btn btn-primary"
          :disabled="loading"
        >
          <i class="fas fa-plus"></i>
          Add Product
        </button>
        <button 
          @click="refreshProducts" 
          class="btn btn-secondary"
          :disabled="loading"
        >
          <i class="fas fa-sync-alt" :class="{ 'fa-spin': loading }"></i>
          Refresh
        </button>
      </div>
    </div>

    <!-- Search and Filters -->
    <div class="product-filters">
      <div class="search-box">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search products..."
          class="form-control"
          @input="handleSearch"
        />
        <i class="fas fa-search"></i>
      </div>
      
      <div class="filter-controls">
        <select
          v-model="selectedCategory"
          class="form-control"
          @change="handleFilterChange"
        >
          <option value="">All Categories</option>
          <option
            v-for="category in categories"
            :key="category.id"
            :value="category.id"
          >
            {{ category.name }}
          </option>
        </select>
        
        <select
          v-model="selectedBrand"
          class="form-control"
          @change="handleFilterChange"
        >
          <option value="">All Brands</option>
          <option
            v-for="brand in uniqueBrands"
            :key="brand"
            :value="brand"
          >
            {{ brand }}
          </option>
        </select>
        
        <select
          v-model="selectedSeason"
          class="form-control"
          @change="handleFilterChange"
        >
          <option value="">All Seasons</option>
          <option value="Spring">Spring</option>
          <option value="Summer">Summer</option>
          <option value="Fall">Fall</option>
          <option value="Winter">Winter</option>
          <option value="All-Season">All-Season</option>
        </select>
        
        <label class="checkbox-label">
          <input
            v-model="showInactive"
            type="checkbox"
            @change="handleFilterChange"
          />
          Show Inactive
        </label>
      </div>
    </div>

    <!-- Products Table -->
    <div class="products-container">
      <div v-if="loading && products.length === 0" class="loading-spinner">
        <i class="fas fa-spinner fa-spin"></i>
        Loading products...
      </div>
      
      <div v-else-if="filteredProducts.length === 0" class="empty-state">
        <i class="fas fa-box-open"></i>
        <h3>No products found</h3>
        <p>Get started by adding your first product.</p>
        <button @click="showCreateModal = true" class="btn btn-primary">
          Add First Product
        </button>
      </div>

      <div v-else class="products-table-container">
        <table class="products-table">
          <thead>
            <tr>
              <th @click="sortBy('name')" class="sortable">
                Product Name
                <i class="fas fa-sort" :class="getSortIcon('name')"></i>
              </th>
              <th @click="sortBy('sku')" class="sortable">
                SKU
                <i class="fas fa-sort" :class="getSortIcon('sku')"></i>
              </th>
              <th @click="sortBy('category')" class="sortable">
                Category
                <i class="fas fa-sort" :class="getSortIcon('category')"></i>
              </th>
              <th @click="sortBy('brand')" class="sortable">
                Brand
                <i class="fas fa-sort" :class="getSortIcon('brand')"></i>
              </th>
              <th @click="sortBy('basePrice')" class="sortable">
                Price
                <i class="fas fa-sort" :class="getSortIcon('basePrice')"></i>
              </th>
              <th @click="sortBy('stock')" class="sortable">
                Stock
                <i class="fas fa-sort" :class="getSortIcon('stock')"></i>
              </th>
              <th @click="sortBy('status')" class="sortable">
                Status
                <i class="fas fa-sort" :class="getSortIcon('status')"></i>
              </th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="product in paginatedProducts"
              :key="product.id"
              class="product-row"
              :class="{ inactive: !product.isActive }"
            >
              <td class="product-name">
                <div class="product-info">
                  <div class="name">{{ product.name }}</div>
                  <div v-if="product.description" class="description">
                    {{ truncateText(product.description, 50) }}
                  </div>
                </div>
              </td>
              <td class="sku">{{ product.sku }}</td>
              <td class="category">
                <span class="badge badge-secondary">
                  {{ getCategoryName(product.categoryId) }}
                </span>
              </td>
              <td class="brand">{{ product.brand || '-' }}</td>
              <td class="price">${{ formatCurrency(product.basePrice) }}</td>
              <td class="stock">
                <div class="stock-info">
                  <span :class="getStockClass(product.totalStock)">
                    {{ product.totalStock || 0 }}
                  </span>
                  <small v-if="product.variationCount > 0" class="text-muted">
                    ({{ product.variationCount }} variants)
                  </small>
                </div>
              </td>
              <td class="status">
                <span class="badge" :class="product.isActive ? 'badge-success' : 'badge-danger'">
                  {{ product.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td class="actions">
                <div class="action-buttons">
                  <button
                    @click="viewProduct(product)"
                    class="btn btn-sm btn-outline-info"
                    title="View Details"
                  >
                    <i class="fas fa-eye"></i>
                  </button>
                  <button
                    @click="editProduct(product)"
                    class="btn btn-sm btn-outline-primary"
                    title="Edit"
                  >
                    <i class="fas fa-edit"></i>
                  </button>
                  <button
                    @click="manageVariations(product)"
                    class="btn btn-sm btn-outline-secondary"
                    title="Manage Variations"
                  >
                    <i class="fas fa-layer-group"></i>
                  </button>
                  <button
                    @click="deleteProduct(product)"
                    class="btn btn-sm btn-outline-danger"
                    title="Delete"
                    :disabled="hasVariations(product)"
                  >
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Pagination -->
        <div v-if="totalPages > 1" class="pagination">
          <button
            @click="currentPage = 1"
            :disabled="currentPage === 1"
            class="btn btn-sm btn-outline-secondary"
          >
            <i class="fas fa-angle-double-left"></i>
          </button>
          <button
            @click="currentPage--"
            :disabled="currentPage === 1"
            class="btn btn-sm btn-outline-secondary"
          >
            <i class="fas fa-angle-left"></i>
          </button>
          
          <span class="page-info">
            Page {{ currentPage }} of {{ totalPages }}
          </span>
          
          <button
            @click="currentPage++"
            :disabled="currentPage === totalPages"
            class="btn btn-sm btn-outline-secondary"
          >
            <i class="fas fa-angle-right"></i>
          </button>
          <button
            @click="currentPage = totalPages"
            :disabled="currentPage === totalPages"
            class="btn btn-sm btn-outline-secondary"
          >
            <i class="fas fa-angle-double-right"></i>
          </button>
        </div>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <ProductModal
      v-if="showCreateModal || showEditModal"
      :show="showCreateModal || showEditModal"
      :product="selectedProduct"
      :categories="categories"
      :is-edit="showEditModal"
      @save="handleSaveProduct"
      @cancel="closeModal"
    />

    <!-- Product Details Modal -->
    <ProductDetailsModal
      v-if="showDetailsModal"
      :show="showDetailsModal"
      :product="selectedProduct"
      @close="showDetailsModal = false"
    />

    <!-- Delete Confirmation Modal -->
    <DeleteConfirmationModal
      v-if="showDeleteModal"
      :show="showDeleteModal"
      :item="selectedProduct"
      item-type="product"
      @confirm="handleDeleteProduct"
      @cancel="showDeleteModal = false"
    />

    <!-- Toast Notifications -->
    <ToastNotification
      v-for="toast in toasts"
      :key="toast.id"
      :message="toast.message"
      :type="toast.type"
      @close="removeToast(toast.id)"
    />
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { useProductCatalogStore } from '@/stores/productCatalogStore'
import ProductModal from './ProductModal.vue'
import ProductDetailsModal from './ProductDetailsModal.vue'
import DeleteConfirmationModal from '@/components/common/DeleteConfirmationModal.vue'
import ToastNotification from '@/components/common/ToastNotification.vue'

export default {
  name: 'ProductManager',
  components: {
    ProductModal,
    ProductDetailsModal,
    DeleteConfirmationModal,
    ToastNotification
  },
  setup() {
    const store = useProductCatalogStore()
    
    // Reactive state
    const loading = ref(false)
    const searchQuery = ref('')
    const selectedCategory = ref('')
    const selectedBrand = ref('')
    const selectedSeason = ref('')
    const showInactive = ref(false)
    const currentPage = ref(1)
    const pageSize = ref(20)
    const sortField = ref('name')
    const sortDirection = ref('asc')
    
    // Modal states
    const showCreateModal = ref(false)
    const showEditModal = ref(false)
    const showDetailsModal = ref(false)
    const showDeleteModal = ref(false)
    const selectedProduct = ref(null)
    const toasts = ref([])

    // Computed properties
    const products = computed(() => store.products)
    const categories = computed(() => store.categories)
    
    const uniqueBrands = computed(() => {
      const brands = [...new Set(products.value.map(p => p.brand).filter(Boolean))]
      return brands.sort()
    })
    
    const filteredProducts = computed(() => {
      let filtered = products.value

      // Filter by search query
      if (searchQuery.value) {
        const query = searchQuery.value.toLowerCase()
        filtered = filtered.filter(product => 
          product.name.toLowerCase().includes(query) ||
          product.description?.toLowerCase().includes(query) ||
          product.sku.toLowerCase().includes(query) ||
          product.brand?.toLowerCase().includes(query)
        )
      }

      // Filter by category
      if (selectedCategory.value) {
        filtered = filtered.filter(product => product.categoryId === selectedCategory.value)
      }

      // Filter by brand
      if (selectedBrand.value) {
        filtered = filtered.filter(product => product.brand === selectedBrand.value)
      }

      // Filter by season
      if (selectedSeason.value) {
        filtered = filtered.filter(product => product.season === selectedSeason.value)
      }

      // Filter by active status
      if (!showInactive.value) {
        filtered = filtered.filter(product => product.isActive)
      }

      // Sort
      filtered.sort((a, b) => {
        let aValue = a[sortField.value]
        let bValue = b[sortField.value]
        
        if (sortField.value === 'category') {
          aValue = getCategoryName(a.categoryId)
          bValue = getCategoryName(b.categoryId)
        } else if (sortField.value === 'stock') {
          aValue = a.totalStock || 0
          bValue = b.totalStock || 0
        } else if (sortField.value === 'status') {
          aValue = a.isActive ? 1 : 0
          bValue = b.isActive ? 1 : 0
        }
        
        if (typeof aValue === 'string') {
          aValue = aValue.toLowerCase()
          bValue = bValue.toLowerCase()
        }
        
        if (sortDirection.value === 'asc') {
          return aValue > bValue ? 1 : -1
        } else {
          return aValue < bValue ? 1 : -1
        }
      })

      return filtered
    })
    
    const totalPages = computed(() => {
      return Math.ceil(filteredProducts.value.length / pageSize.value)
    })
    
    const paginatedProducts = computed(() => {
      const start = (currentPage.value - 1) * pageSize.value
      const end = start + pageSize.value
      return filteredProducts.value.slice(start, end)
    })

    // Methods
    const refreshProducts = async () => {
      loading.value = true
      try {
        await Promise.all([
          store.fetchProducts(),
          store.fetchCategories()
        ])
      } catch (error) {
        addToast('Failed to load products', 'error')
        console.error('Error loading products:', error)
      } finally {
        loading.value = false
      }
    }

    const handleSearch = debounce(() => {
      currentPage.value = 1
    }, 300)

    const handleFilterChange = () => {
      currentPage.value = 1
    }

    const sortBy = (field) => {
      if (sortField.value === field) {
        sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
      } else {
        sortField.value = field
        sortDirection.value = 'asc'
      }
    }

    const getSortIcon = (field) => {
      if (sortField.value !== field) return 'fa-sort'
      return sortDirection.value === 'asc' ? 'fa-sort-up' : 'fa-sort-down'
    }

    const getCategoryName = (categoryId) => {
      const category = categories.value.find(c => c.id === categoryId)
      return category ? category.name : 'Unknown'
    }

    const getStockClass = (stock) => {
      if (stock === 0) return 'stock-out'
      if (stock < 10) return 'stock-low'
      return 'stock-good'
    }

    const formatCurrency = (amount) => {
      return amount.toFixed(2)
    }

    const truncateText = (text, maxLength) => {
      if (!text) return ''
      return text.length > maxLength ? text.substring(0, maxLength) + '...' : text
    }

    const hasVariations = (product) => {
      return product.variationCount > 0
    }

    const viewProduct = (product) => {
      selectedProduct.value = product
      showDetailsModal.value = true
    }

    const editProduct = (product) => {
      selectedProduct.value = { ...product }
      showEditModal.value = true
    }

    const manageVariations = (product) => {
      // Navigate to variations page or open variations modal
      addToast(`Opening variations for ${product.name}`, 'info')
    }

    const deleteProduct = (product) => {
      selectedProduct.value = product
      showDeleteModal.value = true
    }

    const handleSaveProduct = async (productData) => {
      try {
        if (showEditModal.value) {
          await store.updateProduct(selectedProduct.value.id, productData)
          addToast('Product updated successfully', 'success')
        } else {
          await store.createProduct(productData)
          addToast('Product created successfully', 'success')
        }
        closeModal()
        await refreshProducts()
      } catch (error) {
        addToast('Failed to save product', 'error')
        console.error('Error saving product:', error)
      }
    }

    const handleDeleteProduct = async () => {
      try {
        await store.deleteProduct(selectedProduct.value.id)
        addToast('Product deleted successfully', 'success')
        showDeleteModal.value = false
        selectedProduct.value = null
        await refreshProducts()
      } catch (error) {
        addToast('Failed to delete product', 'error')
        console.error('Error deleting product:', error)
      }
    }

    const closeModal = () => {
      showCreateModal.value = false
      showEditModal.value = false
      selectedProduct.value = null
    }

    const addToast = (message, type = 'info') => {
      const toast = {
        id: Date.now(),
        message,
        type
      }
      toasts.value.push(toast)
      
      setTimeout(() => {
        removeToast(toast.id)
      }, 5000)
    }

    const removeToast = (id) => {
      const index = toasts.value.findIndex(toast => toast.id === id)
      if (index > -1) {
        toasts.value.splice(index, 1)
      }
    }

    const debounce = (func, wait) => {
      let timeout
      return function executedFunction(...args) {
        const later = () => {
          clearTimeout(timeout)
          func(...args)
        }
        clearTimeout(timeout)
        timeout = setTimeout(later, wait)
      }
    }

    // Lifecycle
    onMounted(() => {
      refreshProducts()
    })

    return {
      // State
      loading,
      searchQuery,
      selectedCategory,
      selectedBrand,
      selectedSeason,
      showInactive,
      currentPage,
      pageSize,
      sortField,
      sortDirection,
      showCreateModal,
      showEditModal,
      showDetailsModal,
      showDeleteModal,
      selectedProduct,
      toasts,
      
      // Computed
      products,
      categories,
      uniqueBrands,
      filteredProducts,
      totalPages,
      paginatedProducts,
      
      // Methods
      refreshProducts,
      handleSearch,
      handleFilterChange,
      sortBy,
      getSortIcon,
      getCategoryName,
      getStockClass,
      formatCurrency,
      truncateText,
      hasVariations,
      viewProduct,
      editProduct,
      manageVariations,
      deleteProduct,
      handleSaveProduct,
      handleDeleteProduct,
      closeModal,
      addToast,
      removeToast
    }
  }
}
</script>

<style scoped>
.product-manager {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
}

.product-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  padding-bottom: 15px;
  border-bottom: 1px solid #e0e0e0;
}

.product-header h2 {
  margin: 0;
  color: #333;
  font-size: 24px;
  font-weight: 600;
}

.header-actions {
  display: flex;
  gap: 10px;
}

.product-filters {
  display: flex;
  gap: 20px;
  align-items: center;
  margin-bottom: 25px;
  padding: 15px;
  background: #f8f9fa;
  border-radius: 8px;
  flex-wrap: wrap;
}

.search-box {
  position: relative;
  flex: 1;
  min-width: 250px;
}

.search-box input {
  padding-left: 40px;
  height: 40px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 14px;
}

.search-box i {
  position: absolute;
  left: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: #666;
}

.filter-controls {
  display: flex;
  gap: 15px;
  align-items: center;
  flex-wrap: wrap;
}

.filter-controls .form-control {
  min-width: 150px;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  cursor: pointer;
  white-space: nowrap;
}

.products-container {
  min-height: 400px;
}

.loading-spinner {
  text-align: center;
  padding: 60px 20px;
  color: #666;
}

.loading-spinner i {
  font-size: 24px;
  margin-bottom: 10px;
  display: block;
}

.empty-state {
  text-align: center;
  padding: 60px 20px;
  color: #666;
}

.empty-state i {
  font-size: 48px;
  margin-bottom: 20px;
  color: #ccc;
}

.empty-state h3 {
  margin: 0 0 10px 0;
  color: #333;
}

.empty-state p {
  margin: 0 0 20px 0;
}

.products-table-container {
  background: white;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.products-table {
  width: 100%;
  border-collapse: collapse;
}

.products-table th {
  background: #f8f9fa;
  padding: 12px;
  text-align: left;
  font-weight: 600;
  color: #333;
  border-bottom: 1px solid #e0e0e0;
  font-size: 14px;
}

.products-table th.sortable {
  cursor: pointer;
  user-select: none;
  transition: background-color 0.2s ease;
}

.products-table th.sortable:hover {
  background: #e9ecef;
}

.products-table th i {
  margin-left: 5px;
  font-size: 12px;
  color: #666;
}

.products-table td {
  padding: 12px;
  border-bottom: 1px solid #f0f0f0;
  font-size: 14px;
}

.product-row:hover {
  background: #f8f9fa;
}

.product-row.inactive {
  opacity: 0.6;
  background: #fafafa;
}

.product-name .name {
  font-weight: 600;
  color: #333;
}

.product-name .description {
  color: #666;
  font-size: 12px;
  margin-top: 2px;
}

.badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
  text-transform: uppercase;
}

.badge-secondary {
  background: #6c757d;
  color: white;
}

.badge-success {
  background: #28a745;
  color: white;
}

.badge-danger {
  background: #dc3545;
  color: white;
}

.stock-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.stock-good {
  color: #28a745;
  font-weight: 600;
}

.stock-low {
  color: #ffc107;
  font-weight: 600;
}

.stock-out {
  color: #dc3545;
  font-weight: 600;
}

.action-buttons {
  display: flex;
  gap: 5px;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 10px;
  padding: 20px;
  border-top: 1px solid #e0e0e0;
}

.page-info {
  font-size: 14px;
  color: #666;
  margin: 0 10px;
}

/* Button Styles */
.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  display: inline-flex;
  align-items: center;
  gap: 8px;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-primary {
  background: #007bff;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: #0056b3;
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background: #545b62;
}

.btn-outline-primary {
  background: transparent;
  color: #007bff;
  border: 1px solid #007bff;
}

.btn-outline-primary:hover:not(:disabled) {
  background: #007bff;
  color: white;
}

.btn-outline-secondary {
  background: transparent;
  color: #6c757d;
  border: 1px solid #6c757d;
}

.btn-outline-secondary:hover:not(:disabled) {
  background: #6c757d;
  color: white;
}

.btn-outline-info {
  background: transparent;
  color: #17a2b8;
  border: 1px solid #17a2b8;
}

.btn-outline-info:hover:not(:disabled) {
  background: #17a2b8;
  color: white;
}

.btn-outline-danger {
  background: transparent;
  color: #dc3545;
  border: 1px solid #dc3545;
}

.btn-outline-danger:hover:not(:disabled) {
  background: #dc3545;
  color: white;
}

.btn-sm {
  padding: 4px 8px;
  font-size: 12px;
}

/* Form Control Styles */
.form-control {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.form-control:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.25);
}

/* Responsive Design */
@media (max-width: 1200px) {
  .products-table {
    font-size: 12px;
  }
  
  .products-table th,
  .products-table td {
    padding: 8px;
  }
  
  .action-buttons {
    flex-direction: column;
    gap: 2px;
  }
}

@media (max-width: 768px) {
  .product-header {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }
  
  .product-filters {
    flex-direction: column;
    gap: 15px;
  }
  
  .search-box {
    min-width: none;
  }
  
  .filter-controls {
    width: 100%;
    justify-content: space-between;
  }
  
  .filter-controls .form-control {
    flex: 1;
    min-width: 120px;
  }
  
  .header-actions {
    justify-content: stretch;
  }
  
  .header-actions .btn {
    flex: 1;
  }
  
  .products-table-container {
    overflow-x: auto;
  }
  
  .pagination {
    flex-wrap: wrap;
  }
}
</style>

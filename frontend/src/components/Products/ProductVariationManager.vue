<template>
  <div class="variation-manager">
    <!-- Header -->
    <div class="variation-header">
      <div class="product-info">
        <h3>{{ product.name }}</h3>
        <p class="product-sku">SKU: {{ product.sku }}</p>
      </div>
      <div class="header-actions">
        <button 
          @click="showCreateModal = true" 
          class="btn btn-primary"
          :disabled="loading"
        >
          <i class="fas fa-plus"></i>
          Add Variation
        </button>
        <button 
          @click="refreshVariations" 
          class="btn btn-secondary"
          :disabled="loading"
        >
          <i class="fas fa-sync-alt" :class="{ 'fa-spin': loading }"></i>
          Refresh
        </button>
      </div>
    </div>

    <!-- Filters and Stats -->
    <div class="variation-filters">
      <div class="filter-controls">
        <div class="filter-group">
          <label>Size:</label>
          <select
            v-model="selectedSize"
            class="form-control"
            @change="handleFilterChange"
          >
            <option value="">All Sizes</option>
            <option
              v-for="size in availableSizes"
              :key="size"
              :value="size"
            >
              {{ size }}
            </option>
          </select>
        </div>
        
        <div class="filter-group">
          <label>Color:</label>
          <select
            v-model="selectedColor"
            class="form-control"
            @change="handleFilterChange"
          >
            <option value="">All Colors</option>
            <option
              v-for="color in availableColors"
              :key="color"
              :value="color"
            >
              {{ color }}
            </option>
          </select>
        </div>
        
        <div class="filter-group">
          <label>Stock Status:</label>
          <select
            v-model="stockFilter"
            class="form-control"
            @change="handleFilterChange"
          >
            <option value="">All</option>
            <option value="in-stock">In Stock</option>
            <option value="low-stock">Low Stock</option>
            <option value="out-of-stock">Out of Stock</option>
          </select>
        </div>
        
        <label class="checkbox-label">
          <input
            v-model="showInactive"
            type="checkbox"
            @change="handleFilterChange"
          />
          Show Inactive
        </label>
      </div>
      
      <!-- Stock Summary -->
      <div class="stock-summary">
        <div class="summary-item">
          <span class="label">Total Stock:</span>
          <span class="value">{{ totalStock }}</span>
        </div>
        <div class="summary-item">
          <span class="label">Variations:</span>
          <span class="value">{{ filteredVariations.length }}</span>
        </div>
        <div class="summary-item low-stock">
          <span class="label">Low Stock:</span>
          <span class="value">{{ lowStockCount }}</span>
        </div>
      </div>
    </div>

    <!-- Variations Grid -->
    <div class="variations-container">
      <div v-if="loading && variations.length === 0" class="loading-spinner">
        <i class="fas fa-spinner fa-spin"></i>
        Loading variations...
      </div>
      
      <div v-else-if="filteredVariations.length === 0" class="empty-state">
        <i class="fas fa-layer-group"></i>
        <h3>No variations found</h3>
        <p>Add your first product variation to get started.</p>
        <button @click="showCreateModal = true" class="btn btn-primary">
          Add First Variation
        </button>
      </div>

      <div v-else class="variations-grid">
        <div
          v-for="variation in paginatedVariations"
          :key="variation.id"
          class="variation-card"
          :class="{ 
            inactive: !variation.isActive,
            'low-stock': variation.stockQuantity <= 10 && variation.stockQuantity > 0,
            'out-of-stock': variation.stockQuantity === 0
          }"
        >
          <div class="variation-header">
            <div class="variation-info">
              <h4>{{ variation.size }} - {{ variation.color }}</h4>
              <span class="sku">{{ variation.sku }}</span>
            </div>
            <div class="variation-actions">
              <button
                @click="editVariation(variation)"
                class="btn btn-sm btn-outline-primary"
                title="Edit"
              >
                <i class="fas fa-edit"></i>
              </button>
              <button
                @click="deleteVariation(variation)"
                class="btn btn-sm btn-outline-danger"
                title="Delete"
              >
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
          
          <div class="variation-details">
            <div class="price-info">
              <div class="price-row">
                <span class="label">Base Price:</span>
                <span class="value">${{ formatCurrency(product.basePrice) }}</span>
              </div>
              <div v-if="variation.additionalPrice > 0" class="price-row">
                <span class="label">Additional:</span>
                <span class="value">+${{ formatCurrency(variation.additionalPrice) }}</span>
              </div>
              <div class="price-row total">
                <span class="label">Total Price:</span>
                <span class="value">${{ formatCurrency(product.basePrice + variation.additionalPrice) }}</span>
              </div>
            </div>
            
            <div class="stock-info">
              <div class="stock-quantity">
                <span class="label">Stock:</span>
                <span class="value" :class="getStockClass(variation.stockQuantity)">
                  {{ variation.stockQuantity }}
                </span>
              </div>
              <div class="stock-actions">
                <button
                  @click="adjustStock(variation, -1)"
                  class="btn btn-sm btn-outline-secondary"
                  :disabled="variation.stockQuantity === 0"
                >
                  <i class="fas fa-minus"></i>
                </button>
                <input
                  v-model.number="stockEdits[variation.id]"
                  type="number"
                  class="stock-input"
                  @change="updateStock(variation)"
                  min="0"
                />
                <button
                  @click="adjustStock(variation, 1)"
                  class="btn btn-sm btn-outline-secondary"
                >
                  <i class="fas fa-plus"></i>
                </button>
              </div>
            </div>
            
            <div class="status-info">
              <span class="badge" :class="variation.isActive ? 'badge-success' : 'badge-danger'">
                {{ variation.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>
          </div>
        </div>
      </div>

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

    <!-- Create/Edit Modal -->
    <VariationModal
      v-if="showCreateModal || showEditModal"
      :show="showCreateModal || showEditModal"
      :variation="selectedVariation"
      :product="product"
      :existing-variations="variations"
      :is-edit="showEditModal"
      @save="handleSaveVariation"
      @cancel="closeModal"
    />

    <!-- Delete Confirmation Modal -->
    <DeleteConfirmationModal
      v-if="showDeleteModal"
      :show="showDeleteModal"
      :item="selectedVariation"
      item-type="variation"
      @confirm="handleDeleteVariation"
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
import { ref, computed, onMounted, watch } from 'vue'
import { useProductCatalogStore } from '@/stores/productCatalogStore'
import VariationModal from './VariationModal.vue'
import DeleteConfirmationModal from '@/components/common/DeleteConfirmationModal.vue'
import ToastNotification from '@/components/common/ToastNotification.vue'

export default {
  name: 'ProductVariationManager',
  components: {
    VariationModal,
    DeleteConfirmationModal,
    ToastNotification
  },
  props: {
    productId: {
      type: String,
      required: true
    }
  },
  setup(props) {
    const store = useProductCatalogStore()
    
    // Reactive state
    const loading = ref(false)
    const selectedSize = ref('')
    const selectedColor = ref('')
    const stockFilter = ref('')
    const showInactive = ref(false)
    const currentPage = ref(1)
    const pageSize = ref(12)
    const stockEdits = ref({})
    
    // Modal states
    const showCreateModal = ref(false)
    const showEditModal = ref(false)
    const showDeleteModal = ref(false)
    const selectedVariation = ref(null)
    const toasts = ref([])

    // Computed properties
    const product = computed(() => store.products.find(p => p.id === props.productId) || {})
    const variations = computed(() => store.productVariations[props.productId] || [])
    
    const availableSizes = computed(() => {
      const sizes = [...new Set(variations.value.map(v => v.size))]
      return sizes.sort()
    })
    
    const availableColors = computed(() => {
      const colors = [...new Set(variations.value.map(v => v.color))]
      return colors.sort()
    })
    
    const filteredVariations = computed(() => {
      let filtered = variations.value

      // Filter by size
      if (selectedSize.value) {
        filtered = filtered.filter(v => v.size === selectedSize.value)
      }

      // Filter by color
      if (selectedColor.value) {
        filtered = filtered.filter(v => v.color === selectedColor.value)
      }

      // Filter by stock status
      if (stockFilter.value) {
        filtered = filtered.filter(v => {
          if (stockFilter.value === 'in-stock') return v.stockQuantity > 10
          if (stockFilter.value === 'low-stock') return v.stockQuantity > 0 && v.stockQuantity <= 10
          if (stockFilter.value === 'out-of-stock') return v.stockQuantity === 0
          return true
        })
      }

      // Filter by active status
      if (!showInactive.value) {
        filtered = filtered.filter(v => v.isActive)
      }

      return filtered
    })
    
    const totalPages = computed(() => {
      return Math.ceil(filteredVariations.value.length / pageSize.value)
    })
    
    const paginatedVariations = computed(() => {
      const start = (currentPage.value - 1) * pageSize.value
      const end = start + pageSize.value
      return filteredVariations.value.slice(start, end)
    })
    
    const totalStock = computed(() => {
      return variations.value.reduce((total, v) => total + (v.stockQuantity || 0), 0)
    })
    
    const lowStockCount = computed(() => {
      return variations.value.filter(v => v.stockQuantity > 0 && v.stockQuantity <= 10).length
    })

    // Watch for stock edits
    watch(() => variations.value, (newVariations) => {
      const edits = {}
      newVariations.forEach(v => {
        edits[v.id] = v.stockQuantity
      })
      stockEdits.value = edits
    }, { immediate: true })

    // Methods
    const refreshVariations = async () => {
      loading.value = true
      try {
        await store.fetchProductVariations(props.productId)
      } catch (error) {
        addToast('Failed to load variations', 'error')
        console.error('Error loading variations:', error)
      } finally {
        loading.value = false
      }
    }

    const handleFilterChange = () => {
      currentPage.value = 1
    }

    const getStockClass = (stock) => {
      if (stock === 0) return 'stock-out'
      if (stock <= 10) return 'stock-low'
      return 'stock-good'
    }

    const formatCurrency = (amount) => {
      return amount.toFixed(2)
    }

    const adjustStock = (variation, delta) => {
      const newStock = Math.max(0, variation.stockQuantity + delta)
      stockEdits.value[variation.id] = newStock
      updateStock(variation)
    }

    const updateStock = async (variation) => {
      const newStock = stockEdits.value[variation.id]
      if (newStock === undefined || newStock === variation.stockQuantity) return

      try {
        await store.updateVariationStock(variation.id, newStock)
        addToast('Stock updated successfully', 'success')
      } catch (error) {
        addToast('Failed to update stock', 'error')
        console.error('Error updating stock:', error)
        // Revert the edit
        stockEdits.value[variation.id] = variation.stockQuantity
      }
    }

    const editVariation = (variation) => {
      selectedVariation.value = { ...variation }
      showEditModal.value = true
    }

    const deleteVariation = (variation) => {
      selectedVariation.value = variation
      showDeleteModal.value = true
    }

    const handleSaveVariation = async (variationData) => {
      try {
        if (showEditModal.value) {
          await store.updateProductVariation(selectedVariation.value.id, variationData)
          addToast('Variation updated successfully', 'success')
        } else {
          await store.createProductVariation(props.productId, variationData)
          addToast('Variation created successfully', 'success')
        }
        closeModal()
        await refreshVariations()
      } catch (error) {
        addToast('Failed to save variation', 'error')
        console.error('Error saving variation:', error)
      }
    }

    const handleDeleteVariation = async () => {
      try {
        await store.deleteProductVariation(selectedVariation.value.id)
        addToast('Variation deleted successfully', 'success')
        showDeleteModal.value = false
        selectedVariation.value = null
        await refreshVariations()
      } catch (error) {
        addToast('Failed to delete variation', 'error')
        console.error('Error deleting variation:', error)
      }
    }

    const closeModal = () => {
      showCreateModal.value = false
      showEditModal.value = false
      selectedVariation.value = null
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

    // Lifecycle
    onMounted(() => {
      refreshVariations()
    })

    return {
      // State
      loading,
      selectedSize,
      selectedColor,
      stockFilter,
      showInactive,
      currentPage,
      pageSize,
      stockEdits,
      showCreateModal,
      showEditModal,
      showDeleteModal,
      selectedVariation,
      toasts,
      
      // Computed
      product,
      variations,
      availableSizes,
      availableColors,
      filteredVariations,
      totalPages,
      paginatedVariations,
      totalStock,
      lowStockCount,
      
      // Methods
      refreshVariations,
      handleFilterChange,
      getStockClass,
      formatCurrency,
      adjustStock,
      updateStock,
      editVariation,
      deleteVariation,
      handleSaveVariation,
      handleDeleteVariation,
      closeModal,
      addToast,
      removeToast
    }
  }
}
</script>

<style scoped>
.variation-manager {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.variation-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 30px;
  padding-bottom: 15px;
  border-bottom: 1px solid #e0e0e0;
}

.product-info h3 {
  margin: 0 0 5px 0;
  color: #333;
  font-size: 20px;
  font-weight: 600;
}

.product-sku {
  margin: 0;
  color: #666;
  font-size: 14px;
}

.header-actions {
  display: flex;
  gap: 10px;
}

.variation-filters {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 25px;
  padding: 15px;
  background: #f8f9fa;
  border-radius: 8px;
  flex-wrap: wrap;
  gap: 20px;
}

.filter-controls {
  display: flex;
  gap: 15px;
  align-items: center;
  flex-wrap: wrap;
}

.filter-group {
  display: flex;
  align-items: center;
  gap: 8px;
}

.filter-group label {
  font-size: 14px;
  font-weight: 500;
  color: #333;
  white-space: nowrap;
}

.filter-group .form-control {
  min-width: 120px;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  cursor: pointer;
}

.stock-summary {
  display: flex;
  gap: 20px;
  align-items: center;
}

.summary-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.summary-item .label {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
}

.summary-item .value {
  font-size: 18px;
  font-weight: 600;
  color: #333;
}

.summary-item.low-stock .value {
  color: #ffc107;
}

.variations-container {
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

.variations-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
}

.variation-card {
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 20px;
  transition: all 0.2s ease;
}

.variation-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

.variation-card.inactive {
  opacity: 0.6;
  background: #f8f9fa;
}

.variation-card.low-stock {
  border-left: 4px solid #ffc107;
}

.variation-card.out-of-stock {
  border-left: 4px solid #dc3545;
}

.variation-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 15px;
}

.variation-info h4 {
  margin: 0 0 5px 0;
  color: #333;
  font-size: 16px;
  font-weight: 600;
}

.variation-info .sku {
  font-size: 12px;
  color: #666;
  font-family: monospace;
}

.variation-actions {
  display: flex;
  gap: 8px;
}

.variation-details {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.price-info {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.price-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.price-row .label {
  font-size: 12px;
  color: #666;
}

.price-row .value {
  font-size: 14px;
  font-weight: 500;
  color: #333;
}

.price-row.total {
  border-top: 1px solid #e0e0e0;
  padding-top: 5px;
  margin-top: 5px;
}

.price-row.total .value {
  font-weight: 600;
  color: #007bff;
}

.stock-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 15px;
}

.stock-quantity {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-width: 60px;
}

.stock-quantity .label {
  font-size: 12px;
  color: #666;
}

.stock-quantity .value {
  font-size: 16px;
  font-weight: 600;
}

.stock-good {
  color: #28a745;
}

.stock-low {
  color: #ffc107;
}

.stock-out {
  color: #dc3545;
}

.stock-actions {
  display: flex;
  align-items: center;
  gap: 5px;
}

.stock-input {
  width: 60px;
  padding: 4px 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
  text-align: center;
  font-size: 14px;
}

.stock-input:focus {
  outline: none;
  border-color: #007bff;
}

.status-info {
  text-align: right;
}

.badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
  text-transform: uppercase;
}

.badge-success {
  background: #28a745;
  color: white;
}

.badge-danger {
  background: #dc3545;
  color: white;
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
@media (max-width: 768px) {
  .variation-header {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }
  
  .variation-filters {
    flex-direction: column;
    gap: 15px;
  }
  
  .filter-controls {
    width: 100%;
    justify-content: space-between;
  }
  
  .stock-summary {
    width: 100%;
    justify-content: space-around;
  }
  
  .header-actions {
    justify-content: stretch;
  }
  
  .header-actions .btn {
    flex: 1;
  }
  
  .variations-grid {
    grid-template-columns: 1fr;
  }
  
  .stock-info {
    flex-direction: column;
    gap: 10px;
    align-items: stretch;
  }
  
  .stock-actions {
    justify-content: center;
  }
}
</style>

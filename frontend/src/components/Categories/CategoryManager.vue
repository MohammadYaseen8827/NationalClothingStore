<template>
  <div class="category-manager">
    <!-- Header -->
    <div class="category-header">
      <h2>Category Management</h2>
      <div class="header-actions">
        <button 
          @click="showCreateModal = true" 
          class="btn btn-primary"
          :disabled="loading"
        >
          <i class="fas fa-plus"></i>
          Add Category
        </button>
        <button 
          @click="refreshCategories" 
          class="btn btn-secondary"
          :disabled="loading"
        >
          <i class="fas fa-sync-alt" :class="{ 'fa-spin': loading }"></i>
          Refresh
        </button>
      </div>
    </div>

    <!-- Search and Filters -->
    <div class="category-filters">
      <div class="search-box">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search categories..."
          class="form-control"
          @input="handleSearch"
        />
        <i class="fas fa-search"></i>
      </div>
      <div class="filter-options">
        <label class="checkbox-label">
          <input
            v-model="showInactive"
            type="checkbox"
            @change="refreshCategories"
          />
          Show Inactive
        </label>
        <label class="checkbox-label">
          <input
            v-model="showHierarchy"
            type="checkbox"
            @change="refreshCategories"
          />
          Show Hierarchy
        </label>
      </div>
    </div>

    <!-- Categories List -->
    <div class="categories-container">
      <div v-if="loading && categories.length === 0" class="loading-spinner">
        <i class="fas fa-spinner fa-spin"></i>
        Loading categories...
      </div>
      
      <div v-else-if="filteredCategories.length === 0" class="empty-state">
        <i class="fas fa-folder-open"></i>
        <h3>No categories found</h3>
        <p>Get started by adding your first category.</p>
        <button @click="showCreateModal = true" class="btn btn-primary">
          Add First Category
        </button>
      </div>

      <div v-else class="categories-grid">
        <div
          v-for="category in filteredCategories"
          :key="category.id"
          class="category-card"
          :class="{ inactive: !category.isActive }"
        >
          <div class="category-header-card">
            <h3>{{ category.name }}</h3>
            <div class="category-actions">
              <button
                @click="editCategory(category)"
                class="btn btn-sm btn-outline-primary"
                title="Edit"
              >
                <i class="fas fa-edit"></i>
              </button>
              <button
                @click="deleteCategory(category)"
                class="btn btn-sm btn-outline-danger"
                title="Delete"
                :disabled="hasChildrenOrProducts(category)"
              >
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
          
          <div class="category-details">
            <p v-if="category.description" class="description">
              {{ category.description }}
            </p>
            <div class="category-meta">
              <span v-if="category.code" class="badge badge-secondary">
                {{ category.code }}
              </span>
              <span class="badge" :class="category.isActive ? 'badge-success' : 'badge-danger'">
                {{ category.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>
            <div v-if="showHierarchy && category.children" class="category-children">
              <small class="text-muted">
                {{ category.children.length }} sub-categories
              </small>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <CategoryModal
      v-if="showCreateModal || showEditModal"
      :show="showCreateModal || showEditModal"
      :category="selectedCategory"
      :categories="categories"
      :is-edit="showEditModal"
      @save="handleSaveCategory"
      @cancel="closeModal"
    />

    <!-- Delete Confirmation Modal -->
    <DeleteConfirmationModal
      v-if="showDeleteModal"
      :show="showDeleteModal"
      :item="selectedCategory"
      item-type="category"
      @confirm="handleDeleteCategory"
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
import CategoryModal from './CategoryModal.vue'
import DeleteConfirmationModal from '@/components/common/DeleteConfirmationModal.vue'
import ToastNotification from '@/components/common/ToastNotification.vue'

export default {
  name: 'CategoryManager',
  components: {
    CategoryModal,
    DeleteConfirmationModal,
    ToastNotification
  },
  setup() {
    const store = useProductCatalogStore()
    
    // Reactive state
    const loading = ref(false)
    const searchQuery = ref('')
    const showInactive = ref(false)
    const showHierarchy = ref(false)
    const showCreateModal = ref(false)
    const showEditModal = ref(false)
    const showDeleteModal = ref(false)
    const selectedCategory = ref(null)
    const toasts = ref([])

    // Computed properties
    const categories = computed(() => store.categories)
    
    const filteredCategories = computed(() => {
      let filtered = categories.value

      // Filter by active status
      if (!showInactive.value) {
        filtered = filtered.filter(cat => cat.isActive)
      }

      // Filter by search query
      if (searchQuery.value) {
        const query = searchQuery.value.toLowerCase()
        filtered = filtered.filter(cat => 
          cat.name.toLowerCase().includes(query) ||
          cat.description?.toLowerCase().includes(query) ||
          cat.code?.toLowerCase().includes(query)
        )
      }

      return filtered
    })

    // Methods
    const refreshCategories = async () => {
      loading.value = true
      try {
        await store.fetchCategories({ includeHierarchy: showHierarchy.value })
      } catch (error) {
        addToast('Failed to load categories', 'error')
        console.error('Error loading categories:', error)
      } finally {
        loading.value = false
      }
    }

    const handleSearch = debounce(() => {
      // Search is handled by computed property
    }, 300)

    const editCategory = (category) => {
      selectedCategory.value = { ...category }
      showEditModal.value = true
    }

    const deleteCategory = (category) => {
      selectedCategory.value = category
      showDeleteModal.value = true
    }

    const hasChildrenOrProducts = (category) => {
      return (category.children && category.children.length > 0) || 
             (category.productCount && category.productCount > 0)
    }

    const handleSaveCategory = async (categoryData) => {
      try {
        if (showEditModal.value) {
          await store.updateCategory(selectedCategory.value.id, categoryData)
          addToast('Category updated successfully', 'success')
        } else {
          await store.createCategory(categoryData)
          addToast('Category created successfully', 'success')
        }
        closeModal()
        await refreshCategories()
      } catch (error) {
        addToast('Failed to save category', 'error')
        console.error('Error saving category:', error)
      }
    }

    const handleDeleteCategory = async () => {
      try {
        await store.deleteCategory(selectedCategory.value.id)
        addToast('Category deleted successfully', 'success')
        showDeleteModal.value = false
        selectedCategory.value = null
        await refreshCategories()
      } catch (error) {
        addToast('Failed to delete category', 'error')
        console.error('Error deleting category:', error)
      }
    }

    const closeModal = () => {
      showCreateModal.value = false
      showEditModal.value = false
      selectedCategory.value = null
    }

    const addToast = (message, type = 'info') => {
      const toast = {
        id: Date.now(),
        message,
        type
      }
      toasts.value.push(toast)
      
      // Auto remove after 5 seconds
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
      refreshCategories()
    })

    return {
      // State
      loading,
      searchQuery,
      showInactive,
      showHierarchy,
      showCreateModal,
      showEditModal,
      showDeleteModal,
      selectedCategory,
      toasts,
      
      // Computed
      categories,
      filteredCategories,
      
      // Methods
      refreshCategories,
      handleSearch,
      editCategory,
      deleteCategory,
      hasChildrenOrProducts,
      handleSaveCategory,
      handleDeleteCategory,
      closeModal,
      addToast,
      removeToast
    }
  }
}
</script>

<style scoped>
.category-manager {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.category-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  padding-bottom: 15px;
  border-bottom: 1px solid #e0e0e0;
}

.category-header h2 {
  margin: 0;
  color: #333;
  font-size: 24px;
  font-weight: 600;
}

.header-actions {
  display: flex;
  gap: 10px;
}

.category-filters {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 25px;
  padding: 15px;
  background: #f8f9fa;
  border-radius: 8px;
}

.search-box {
  position: relative;
  flex: 1;
  max-width: 400px;
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

.filter-options {
  display: flex;
  gap: 20px;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  cursor: pointer;
}

.checkbox-label input[type="checkbox"] {
  margin: 0;
}

.categories-container {
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

.categories-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
}

.category-card {
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 20px;
  transition: all 0.2s ease;
}

.category-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

.category-card.inactive {
  opacity: 0.6;
  background: #f8f9fa;
}

.category-header-card {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 15px;
}

.category-header-card h3 {
  margin: 0;
  color: #333;
  font-size: 18px;
  font-weight: 600;
  flex: 1;
}

.category-actions {
  display: flex;
  gap: 8px;
}

.category-details {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.description {
  color: #666;
  font-size: 14px;
  line-height: 1.4;
  margin: 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.category-meta {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
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

.category-children {
  margin-top: 8px;
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
  width: 100%;
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
  .category-header {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }
  
  .category-filters {
    flex-direction: column;
    gap: 15px;
  }
  
  .search-box {
    max-width: none;
  }
  
  .categories-grid {
    grid-template-columns: 1fr;
  }
  
  .header-actions {
    justify-content: stretch;
  }
  
  .header-actions .btn {
    flex: 1;
  }
}
</style>

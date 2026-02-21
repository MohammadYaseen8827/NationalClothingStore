<template>
  <div class="category-details">
    <div class="page-header">
      <div class="header-left">
        <button @click="goBack" class="btn-back">
          <i class="fas fa-arrow-left"></i>
        </button>
        <h1>Category Details</h1>
      </div>
      <div class="header-actions">
        <button @click="editCategory" class="btn btn-primary" :disabled="loading">
          <i class="fas fa-edit"></i>
          Edit
        </button>
        <button @click="deleteCategory" class="btn btn-danger" :disabled="loading || deleting">
          <i class="fas fa-trash"></i>
          {{ deleting ? 'Deleting...' : 'Delete' }}
        </button>
      </div>
    </div>

    <div v-if="loading" class="loading">Loading category...</div>
    <div v-else-if="error" class="error-message">{{ error }}</div>
    <div v-else-if="category" class="category-content">
      <!-- Basic Info -->
      <div class="detail-card">
        <h3>Basic Information</h3>
        <div class="detail-grid">
          <div class="detail-item">
            <label>ID</label>
            <span>{{ category.id }}</span>
          </div>
          <div class="detail-item">
            <label>Name</label>
            <span>{{ category.name }}</span>
          </div>
          <div class="detail-item">
            <label>Code</label>
            <span>{{ category.code || 'N/A' }}</span>
          </div>
          <div class="detail-item">
            <label>Description</label>
            <span>{{ category.description || 'N/A' }}</span>
          </div>
          <div class="detail-item">
            <label>Status</label>
            <span :class="['status-badge', category.isActive ? 'active' : 'inactive']">
              {{ category.isActive ? 'Active' : 'Inactive' }}
            </span>
          </div>
          <div class="detail-item">
            <label>Sort Order</label>
            <span>{{ category.sortOrder || 0 }}</span>
          </div>
        </div>
      </div>

      <!-- Hierarchy Info -->
      <div v-if="category.parentId || category.children?.length" class="detail-card">
        <h3>Hierarchy</h3>
        <div class="detail-grid">
          <div v-if="category.parentId" class="detail-item">
            <label>Parent Category</label>
            <router-link :to="`/catalog/categories/${category.parentId}`">
              {{ category.parentName || category.parentId }}
            </router-link>
          </div>
          <div v-if="category.children?.length" class="detail-item">
            <label>Child Categories</label>
            <div class="child-categories">
              <router-link 
                v-for="child in category.children" 
                :key="child.id"
                :to="`/catalog/categories/${child.id}`"
                class="child-tag"
              >
                {{ child.name }}
              </router-link>
            </div>
          </div>
        </div>
      </div>

      <!-- Timestamps -->
      <div class="detail-card">
        <h3>Timestamps</h3>
        <div class="detail-grid">
          <div class="detail-item">
            <label>Created At</label>
            <span>{{ formatDate(category.createdAt) }}</span>
          </div>
          <div class="detail-item">
            <label>Updated At</label>
            <span>{{ formatDate(category.updatedAt) }}</span>
          </div>
        </div>
      </div>

      <!-- Validation Info -->
      <div v-if="deletionValidation" class="detail-card">
        <h3>Deletion Validation</h3>
        <div class="validation-info">
          <div :class="['validation-item', deletionValidation.canDelete ? 'success' : 'warning']">
            <i :class="['fas', deletionValidation.canDelete ? 'fa-check-circle' : 'fa-exclamation-circle']"></i>
            <span>{{ deletionValidation.canDelete ? 'Can be deleted' : 'Cannot be deleted' }}</span>
          </div>
          <div v-if="deletionValidation.reasons?.length" class="validation-reasons">
            <p v-for="(reason, idx) in deletionValidation.reasons" :key="idx">{{ reason }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit Modal -->
    <div v-if="showEditModal" class="modal-overlay" @click.self="showEditModal = false">
      <div class="modal">
        <div class="modal-header">
          <h2>Edit Category</h2>
          <button @click="showEditModal = false" class="modal-close">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <form @submit.prevent="saveCategory" class="modal-body">
          <div class="form-group">
            <label>Name *</label>
            <input v-model="editForm.name" type="text" required />
          </div>
          <div class="form-group">
            <label>Code</label>
            <input v-model="editForm.code" type="text" />
          </div>
          <div class="form-group">
            <label>Description</label>
            <textarea v-model="editForm.description" rows="3"></textarea>
          </div>
          <div class="form-group">
            <label>Sort Order</label>
            <input v-model.number="editForm.sortOrder" type="number" min="0" />
          </div>
          <div class="form-group">
            <label class="checkbox-label">
              <input v-model="editForm.isActive" type="checkbox" />
              Active
            </label>
          </div>
          <div class="modal-actions">
            <button type="button" @click="showEditModal = false" class="btn btn-secondary">Cancel</button>
            <button type="submit" class="btn btn-primary" :disabled="saving">
              {{ saving ? 'Saving...' : 'Save Changes' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import productCatalogService from '@/services/productCatalogService'

const props = defineProps<{
  id: string
}>()

const router = useRouter()
const route = useRoute()

const loading = ref(false)
const error = ref('')
const deleting = ref(false)
const saving = ref(false)
const category = ref<any>(null)
const deletionValidation = ref<any>(null)
const showEditModal = ref(false)

const editForm = ref({
  name: '',
  code: '',
  description: '',
  sortOrder: 0,
  isActive: true
})

const loadCategory = async () => {
  loading.value = true
  error.value = ''
  try {
    category.value = await productCatalogService.category.getCategory(props.id)
    editForm.value = {
      name: category.value.name || '',
      code: category.value.code || '',
      description: category.value.description || '',
      sortOrder: category.value.sortOrder || 0,
      isActive: category.value.isActive ?? true
    }
    await loadDeletionValidation()
  } catch (e: any) {
    error.value = e.message || 'Failed to load category'
  } finally {
    loading.value = false
  }
}

const loadDeletionValidation = async () => {
  try {
    deletionValidation.value = await productCatalogService.category.validateCategoryDeletion(props.id)
  } catch (e) {
    // Ignore validation errors
  }
}

const goBack = () => {
  router.push('/catalog/categories')
}

const editCategory = () => {
  showEditModal.value = true
}

const saveCategory = async () => {
  saving.value = true
  try {
    await productCatalogService.category.updateCategory(props.id, editForm.value)
    showEditModal.value = false
    await loadCategory()
  } catch (e: any) {
    error.value = e.message || 'Failed to save category'
  } finally {
    saving.value = false
  }
}

const deleteCategory = async () => {
  if (!confirm('Are you sure you want to delete this category?')) return
  
  deleting.value = true
  try {
    await productCatalogService.category.deleteCategory(props.id)
    router.push('/catalog/categories')
  } catch (e: any) {
    error.value = e.message || 'Failed to delete category'
    deleting.value = false
  }
}

const formatDate = (date?: string) => {
  if (!date) return 'N/A'
  return new Date(date).toLocaleString()
}

onMounted(loadCategory)

watch(() => props.id, loadCategory)
</script>

<style scoped>
.category-details {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.header-left h1 {
  margin: 0;
}

.btn-back {
  background: none;
  border: none;
  font-size: 18px;
  cursor: pointer;
  padding: 8px;
}

.header-actions {
  display: flex;
  gap: 8px;
}

.loading, .error-message {
  text-align: center;
  padding: 40px;
}

.error-message {
  color: #dc3545;
}

.category-content {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.detail-card {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.detail-card h3 {
  margin: 0 0 16px 0;
  padding-bottom: 12px;
  border-bottom: 1px solid #dee2e6;
}

.detail-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.detail-item label {
  font-size: 12px;
  color: #6c757d;
  text-transform: uppercase;
}

.status-badge {
  display: inline-block;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.status-badge.active {
  background: #d4edda;
  color: #155724;
}

.status-badge.inactive {
  background: #f8d7da;
  color: #721c24;
}

.child-categories {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.child-tag {
  padding: 4px 8px;
  background: #e9ecef;
  border-radius: 4px;
  text-decoration: none;
  font-size: 12px;
}

.validation-info {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.validation-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px;
  border-radius: 4px;
}

.validation-item.success {
  background: #d4edda;
  color: #155724;
}

.validation-item.warning {
  background: #fff3cd;
  color: #856404;
}

.validation-reasons p {
  margin: 8px 0 0 0;
  color: #856404;
  font-size: 14px;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal {
  background: white;
  border-radius: 8px;
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  border-bottom: 1px solid #dee2e6;
}

.modal-header h2 {
  margin: 0;
}

.modal-close {
  background: none;
  border: none;
  font-size: 18px;
  cursor: pointer;
}

.modal-body {
  padding: 20px;
}

.form-group {
  margin-bottom: 16px;
}

.form-group label {
  display: block;
  margin-bottom: 4px;
  font-weight: 500;
}

.form-group input,
.form-group textarea {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #ced4da;
  border-radius: 4px;
  font-size: 14px;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  margin-top: 20px;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
}

.btn-primary {
  background: #4a90d9;
  color: white;
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-danger {
  background: #dc3545;
  color: white;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>

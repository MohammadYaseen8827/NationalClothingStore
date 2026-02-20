<template>
  <div class="modal-overlay" @click="handleOverlayClick">
    <div class="modal-content" @click.stop>
      <div class="modal-header">
        <h3>{{ isEdit ? 'Edit Category' : 'Add New Category' }}</h3>
        <button @click="$emit('cancel')" class="btn-close">
          <i class="fas fa-times"></i>
        </button>
      </div>

      <form @submit.prevent="handleSubmit" class="modal-body">
        <div class="form-group">
          <label for="name">Category Name *</label>
          <input
            id="name"
            v-model="formData.name"
            type="text"
            class="form-control"
            :class="{ 'is-invalid': errors.name }"
            placeholder="Enter category name"
            required
          />
          <span v-if="errors.name" class="error-message">{{ errors.name }}</span>
        </div>

        <div class="form-group">
          <label for="description">Description</label>
          <textarea
            id="description"
            v-model="formData.description"
            class="form-control"
            :class="{ 'is-invalid': errors.description }"
            placeholder="Enter category description"
            rows="3"
          ></textarea>
          <span v-if="errors.description" class="error-message">{{ errors.description }}</span>
        </div>

        <div class="form-group">
          <label for="code">Category Code</label>
          <input
            id="code"
            v-model="formData.code"
            type="text"
            class="form-control"
            :class="{ 'is-invalid': errors.code }"
            placeholder="Enter category code (optional)"
          />
          <span v-if="errors.code" class="error-message">{{ errors.code }}</span>
        </div>

        <div class="form-group">
          <label for="parentId">Parent Category</label>
          <select
            id="parentId"
            v-model="formData.parentId"
            class="form-control"
            :class="{ 'is-invalid': errors.parentId }"
          >
            <option value="">None (Root Category)</option>
            <option
              v-for="cat in availableParentCategories"
              :key="cat.id"
              :value="cat.id"
              :disabled="isEdit && cat.id === category?.id"
            >
              {{ cat.name }}
            </option>
          </select>
          <span v-if="errors.parentId" class="error-message">{{ errors.parentId }}</span>
        </div>

        <div class="form-group">
          <label for="sortOrder">Sort Order</label>
          <input
            id="sortOrder"
            v-model.number="formData.sortOrder"
            type="number"
            class="form-control"
            :class="{ 'is-invalid': errors.sortOrder }"
            placeholder="Enter sort order"
            min="0"
          />
          <span v-if="errors.sortOrder" class="error-message">{{ errors.sortOrder }}</span>
        </div>

        <div class="form-group">
          <label class="checkbox-label">
            <input
              v-model="formData.isActive"
              type="checkbox"
            />
            Active
          </label>
        </div>
      </form>

      <div class="modal-footer">
        <button @click="$emit('cancel')" class="btn btn-secondary">
          Cancel
        </button>
        <button
          @click="handleSubmit"
          class="btn btn-primary"
          :disabled="loading"
        >
          <i v-if="loading" class="fas fa-spinner fa-spin"></i>
          {{ isEdit ? 'Update Category' : 'Create Category' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, watch } from 'vue'

export default {
  name: 'CategoryModal',
  props: {
    show: {
      type: Boolean,
      required: true
    },
    category: {
      type: Object,
      default: null
    },
    categories: {
      type: Array,
      default: () => []
    },
    isEdit: {
      type: Boolean,
      default: false
    }
  },
  emits: ['save', 'cancel'],
  setup(props, { emit }) {
    const loading = ref(false)
    const errors = ref({})
    
    const formData = ref({
      name: '',
      description: '',
      code: '',
      parentId: '',
      sortOrder: 0,
      isActive: true
    })

    // Computed properties
    const availableParentCategories = computed(() => {
      if (!props.isEdit || !props.category) {
        return props.categories
      }
      
      // For editing, exclude current category and its descendants
      const excludeIds = [props.category.id]
      const findDescendants = (category) => {
        if (category.children) {
          category.children.forEach(child => {
            excludeIds.push(child.id)
            findDescendants(child)
          })
        }
      }
      findDescendants(props.category)
      
      return props.categories.filter(cat => !excludeIds.includes(cat.id))
    })

    // Watch for category prop changes
    watch(() => props.category, (newCategory) => {
      if (newCategory) {
        formData.value = {
          name: newCategory.name || '',
          description: newCategory.description || '',
          code: newCategory.code || '',
          parentId: newCategory.parentCategoryId || '',
          sortOrder: newCategory.sortOrder || 0,
          isActive: newCategory.isActive ?? true
        }
      } else {
        resetForm()
      }
    }, { immediate: true })

    // Methods
    const resetForm = () => {
      formData.value = {
        name: '',
        description: '',
        code: '',
        parentId: '',
        sortOrder: 0,
        isActive: true
      }
      errors.value = {}
    }

    const validateForm = () => {
      const newErrors = {}
      
      // Name validation
      if (!formData.value.name.trim()) {
        newErrors.name = 'Category name is required'
      } else if (formData.value.name.trim().length < 2) {
        newErrors.name = 'Category name must be at least 2 characters'
      } else if (formData.value.name.trim().length > 200) {
        newErrors.name = 'Category name cannot exceed 200 characters'
      }

      // Description validation
      if (formData.value.description && formData.value.description.length > 1000) {
        newErrors.description = 'Description cannot exceed 1000 characters'
      }

      // Code validation
      if (formData.value.code) {
        if (formData.value.code.length > 50) {
          newErrors.code = 'Code cannot exceed 50 characters'
        }
        if (!/^[A-Z0-9_-]+$/.test(formData.value.code)) {
          newErrors.code = 'Code can only contain uppercase letters, numbers, underscores, and hyphens'
        }
      }

      // Sort order validation
      if (formData.value.sortOrder < 0) {
        newErrors.sortOrder = 'Sort order cannot be negative'
      }

      // Parent category validation
      if (props.isEdit && props.category && formData.value.parentId === props.category.id) {
        newErrors.parentId = 'Category cannot be its own parent'
      }

      errors.value = newErrors
      return Object.keys(newErrors).length === 0
    }

    const handleSubmit = async () => {
      if (!validateForm()) {
        return
      }

      loading.value = true
      try {
        const submitData = {
          name: formData.value.name.trim(),
          description: formData.value.description?.trim() || null,
          code: formData.value.code?.trim() || null,
          parentId: formData.value.parentId || null,
          sortOrder: formData.value.sortOrder,
          isActive: formData.value.isActive
        }

        emit('save', submitData)
      } catch (error) {
        console.error('Error submitting category:', error)
      } finally {
        loading.value = false
      }
    }

    const handleOverlayClick = () => {
      emit('cancel')
    }

    return {
      loading,
      errors,
      formData,
      availableParentCategories,
      handleSubmit,
      handleOverlayClick
    }
  }
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 20px;
}

.modal-content {
  background: white;
  border-radius: 8px;
  width: 100%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid #e0e0e0;
}

.modal-header h3 {
  margin: 0;
  color: #333;
  font-size: 18px;
  font-weight: 600;
}

.btn-close {
  background: none;
  border: none;
  font-size: 18px;
  color: #666;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.2s ease;
}

.btn-close:hover {
  background: #f0f0f0;
  color: #333;
}

.modal-body {
  padding: 20px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
  color: #333;
  font-size: 14px;
}

.form-control {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  transition: all 0.2s ease;
}

.form-control:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.25);
}

.form-control.is-invalid {
  border-color: #dc3545;
}

.form-control.is-invalid:focus {
  border-color: #dc3545;
  box-shadow: 0 0 0 2px rgba(220, 53, 69, 0.25);
}

.error-message {
  display: block;
  margin-top: 5px;
  color: #dc3545;
  font-size: 12px;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 500;
  color: #333;
  font-size: 14px;
  cursor: pointer;
}

.checkbox-label input[type="checkbox"] {
  margin: 0;
  width: auto;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  padding: 20px;
  border-top: 1px solid #e0e0e0;
}

.btn {
  padding: 10px 20px;
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

/* Responsive Design */
@media (max-width: 768px) {
  .modal-overlay {
    padding: 10px;
  }
  
  .modal-content {
    max-width: none;
    margin: 0;
  }
  
  .modal-header,
  .modal-body,
  .modal-footer {
    padding: 15px;
  }
  
  .modal-footer {
    flex-direction: column;
  }
  
  .modal-footer .btn {
    width: 100%;
    justify-content: center;
  }
}
</style>

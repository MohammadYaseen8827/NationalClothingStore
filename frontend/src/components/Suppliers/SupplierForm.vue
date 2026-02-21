<template>
  <div class="supplier-form">
    <div class="header">
      <h1>{{ isEditing ? 'Edit Supplier' : 'New Supplier' }}</h1>
      <router-link to="/procurement/suppliers" class="btn btn-secondary">Back to List</router-link>
    </div>

    <div v-if="store.isLoading" class="loading">Loading...</div>
    <div v-else-if="store.error" class="error">{{ store.error }}</div>

    <form v-else @submit.prevent="handleSubmit" class="form">
      <div class="form-row">
        <div class="form-group">
          <label for="name">Name *</label>
          <input id="name" v-model="formData.name" type="text" required />
        </div>
        <div class="form-group">
          <label for="code">Code *</label>
          <input id="code" v-model="formData.code" type="text" required />
        </div>
      </div>

      <div class="form-row">
        <div class="form-group">
          <label for="contactName">Contact Name</label>
          <input id="contactName" v-model="formData.contactName" type="text" />
        </div>
        <div class="form-group">
          <label for="email">Email</label>
          <input id="email" v-model="formData.email" type="email" />
        </div>
      </div>

      <div class="form-row">
        <div class="form-group">
          <label for="phone">Phone</label>
          <input id="phone" v-model="formData.phone" type="tel" />
        </div>
        <div class="form-group">
          <label for="country">Country</label>
          <input id="country" v-model="formData.country" type="text" />
        </div>
      </div>

      <div class="form-group full-width">
        <label for="address">Address</label>
        <input id="address" v-model="formData.address" type="text" />
      </div>

      <div class="form-row">
        <div class="form-group">
          <label for="city">City</label>
          <input id="city" v-model="formData.city" type="text" />
        </div>
        <div class="form-group">
          <label for="state">State</label>
          <input id="state" v-model="formData.state" type="text" />
        </div>
        <div class="form-group">
          <label for="zipCode">Zip Code</label>
          <input id="zipCode" v-model="formData.zipCode" type="text" />
        </div>
      </div>

      <div class="form-group full-width">
        <label for="notes">Notes</label>
        <textarea id="notes" v-model="formData.notes" rows="3"></textarea>
      </div>

      <div v-if="isEditing" class="form-group">
        <label class="checkbox-label">
          <input type="checkbox" v-model="formData.isActive" />
          Active
        </label>
      </div>

      <div class="form-actions">
        <button type="submit" class="btn btn-primary" :disabled="isSaving">
          {{ isSaving ? 'Saving...' : (isEditing ? 'Update Supplier' : 'Create Supplier') }}
        </button>
        <router-link to="/procurement/suppliers" class="btn btn-secondary">Cancel</router-link>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProcurementStore } from '@/stores/procurementStore'

const props = defineProps<{ id?: string }>()

const route = useRoute()
const router = useRouter()
const store = useProcurementStore()

const isSaving = ref(false)
const isEditing = computed(() => !!props.id || !!route.params.id)
const supplierId = computed(() => props.id || route.params.id as string)

const formData = reactive({
  name: '',
  code: '',
  contactName: '',
  email: '',
  phone: '',
  address: '',
  city: '',
  state: '',
  zipCode: '',
  country: '',
  notes: '',
  isActive: true
})

watch(() => store.currentSupplier, (supplier) => {
  if (supplier && isEditing.value) {
    Object.assign(formData, {
      name: supplier.name,
      code: supplier.code,
      contactName: supplier.contactName || '',
      email: supplier.email || '',
      phone: supplier.phone || '',
      address: supplier.address || '',
      city: supplier.city || '',
      state: supplier.state || '',
      zipCode: supplier.zipCode || '',
      country: supplier.country || '',
      notes: supplier.notes || '',
      isActive: supplier.isActive
    })
  }
}, { immediate: true })

async function handleSubmit() {
  isSaving.value = true
  try {
    if (isEditing.value) {
      await store.updateSupplier(supplierId.value, formData)
    } else {
      await store.createSupplier(formData)
    }
    router.push('/procurement/suppliers')
  } catch (e) {
    console.error('Failed to save supplier:', e)
  } finally {
    isSaving.value = false
  }
}

onMounted(() => {
  if (isEditing.value && supplierId.value) {
    store.fetchSupplier(supplierId.value)
  }
})
</script>

<style scoped>
.supplier-form {
  padding: 20px;
  max-width: 800px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.form {
  background: white;
  padding: 24px;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.form-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  margin-bottom: 16px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group.full-width {
  grid-column: 1 / -1;
}

.form-group label {
  font-weight: 500;
  margin-bottom: 6px;
  color: #333;
}

.form-group input,
.form-group textarea {
  padding: 10px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #007bff;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}

.form-actions {
  display: flex;
  gap: 12px;
  margin-top: 24px;
  padding-top: 24px;
  border-top: 1px solid #eee;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  text-decoration: none;
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.loading, .error {
  padding: 40px;
  text-align: center;
}

.error {
  color: #dc3545;
}
</style>

<template>
  <div class="supplier-manager">
    <div class="header">
      <h1>Suppliers</h1>
      <router-link to="/procurement/suppliers/new" class="btn btn-primary">
        Add Supplier
      </router-link>
    </div>

    <div class="search-bar">
      <input
        v-model="searchQuery"
        type="text"
        placeholder="Search suppliers by name or code..."
        class="search-input"
      />
    </div>

    <div v-if="store.isLoading" class="loading">Loading suppliers...</div>
    <div v-else-if="store.error" class="error">{{ store.error }}</div>
    <div v-else-if="filteredSuppliers.length === 0" class="empty">
      No suppliers found. Create your first supplier.
    </div>

    <div v-else class="supplier-list">
      <table class="data-table">
        <thead>
          <tr>
            <th>Code</th>
            <th>Name</th>
            <th>Contact</th>
            <th>Email</th>
            <th>Phone</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="supplier in filteredSuppliers" :key="supplier.id">
            <td>{{ supplier.code }}</td>
            <td>{{ supplier.name }}</td>
            <td>{{ supplier.contactName || '-' }}</td>
            <td>{{ supplier.email || '-' }}</td>
            <td>{{ supplier.phone || '-' }}</td>
            <td>
              <span :class="['status-badge', supplier.isActive ? 'active' : 'inactive']">
                {{ supplier.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td class="actions">
              <router-link :to="`/procurement/suppliers/${supplier.id}`" class="btn btn-sm btn-secondary">
                Edit
              </router-link>
              <button @click="confirmDelete(supplier)" class="btn btn-sm btn-danger">
                Delete
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal" class="modal-overlay" @click.self="showDeleteModal = false">
      <div class="modal">
        <h3>Confirm Delete</h3>
        <p>Are you sure you want to delete supplier "{{ supplierToDelete?.name }}"?</p>
        <div class="modal-actions">
          <button @click="showDeleteModal = false" class="btn btn-secondary">Cancel</button>
          <button @click="handleDelete" class="btn btn-danger" :disabled="isDeleting">
            {{ isDeleting ? 'Deleting...' : 'Delete' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useProcurementStore } from '@/stores/procurementStore'
import type { Supplier } from '@/services/procurementService'

const store = useProcurementStore()

const searchQuery = ref('')
const showDeleteModal = ref(false)
const supplierToDelete = ref<Supplier | null>(null)
const isDeleting = ref(false)

const filteredSuppliers = computed(() => {
  if (!searchQuery.value) return store.suppliers
  const query = searchQuery.value.toLowerCase()
  return store.suppliers.filter(s =>
    s.name.toLowerCase().includes(query) ||
    s.code.toLowerCase().includes(query) ||
    s.contactName?.toLowerCase().includes(query) ||
    s.email?.toLowerCase().includes(query)
  )
})

function confirmDelete(supplier: Supplier) {
  supplierToDelete.value = supplier
  showDeleteModal.value = true
}

async function handleDelete() {
  if (!supplierToDelete.value) return
  isDeleting.value = true
  try {
    await store.deleteSupplier(supplierToDelete.value.id)
    showDeleteModal.value = false
    supplierToDelete.value = null
  } catch (e) {
    console.error('Failed to delete supplier:', e)
  } finally {
    isDeleting.value = false
  }
}

onMounted(() => {
  store.fetchSuppliers()
})
</script>

<style scoped>
.supplier-manager {
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.search-bar {
  margin-bottom: 20px;
}

.search-input {
  width: 100%;
  max-width: 400px;
  padding: 10px 15px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table th,
.data-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.data-table th {
  background-color: #f8f9fa;
  font-weight: 600;
}

.status-badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.status-badge.active {
  background-color: #d4edda;
  color: #155724;
}

.status-badge.inactive {
  background-color: #f8d7da;
  color: #721c24;
}

.actions {
  display: flex;
  gap: 8px;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  text-decoration: none;
  display: inline-block;
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
}

.btn-sm {
  padding: 4px 8px;
  font-size: 12px;
}

.loading, .error, .empty {
  padding: 40px;
  text-align: center;
  color: #666;
}

.error {
  color: #dc3545;
}

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
}

.modal {
  background: white;
  padding: 24px;
  border-radius: 8px;
  max-width: 400px;
  width: 90%;
}

.modal h3 {
  margin: 0 0 16px;
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 20px;
}
</style>

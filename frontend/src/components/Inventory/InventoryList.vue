<template>
  <div class="inventory-list">
    <div class="page-header">
      <h1>Inventory List</h1>
      <div class="header-actions">
        <button class="btn-primary" @click="showAddModal = true">
          + Add Inventory
        </button>
      </div>
    </div>

    <!-- Filters -->
    <div class="filters-section">
      <div class="filter-group">
        <label>Branch</label>
        <select v-model="filters.branchId" @change="applyFilters">
          <option value="">All Branches</option>
          <option v-for="branch in branches" :key="branch.id" :value="branch.id">
            {{ branch.name }}
          </option>
        </select>
      </div>
      <div class="filter-group">
        <label>Warehouse</label>
        <select v-model="filters.warehouseId" @change="applyFilters">
          <option value="">All Warehouses</option>
          <option v-for="warehouse in warehouses" :key="warehouse.id" :value="warehouse.id">
            {{ warehouse.name }}
          </option>
        </select>
      </div>
      <div class="filter-group">
        <label>Stock Status</label>
        <select v-model="filters.stockStatus" @change="applyFilters">
          <option value="">All</option>
          <option value="low">Low Stock</option>
          <option value="out">Out of Stock</option>
          <option value="normal">Normal</option>
        </select>
      </div>
      <div class="filter-group search">
        <label>Search</label>
        <input
          type="text"
          v-model="filters.search"
          placeholder="Search by product..."
          @input="debouncedSearch"
        />
      </div>
    </div>

    <!-- Inventory Table -->
    <div class="table-container">
      <div v-if="loading" class="loading">Loading inventory...</div>
      <div v-else-if="filteredInventory.length === 0" class="empty-state">
        No inventory items found
      </div>
      <table v-else>
        <thead>
          <tr>
            <th @click="sortBy('productId')" class="sortable">
              Product
              <span v-if="sortColumn === 'productId'">{{ sortDirection === 'asc' ? '▲' : '▼' }}</span>
            </th>
            <th>Location</th>
            <th @click="sortBy('quantity')" class="sortable">
              Quantity
              <span v-if="sortColumn === 'quantity'">{{ sortDirection === 'asc' ? '▲' : '▼' }}</span>
            </th>
            <th>Reserved</th>
            <th @click="sortBy('availableQuantity')" class="sortable">
              Available
              <span v-if="sortColumn === 'availableQuantity'">{{ sortDirection === 'asc' ? '▲' : '▼' }}</span>
            </th>
            <th>Unit Cost</th>
            <th>Total Value</th>
            <th>Last Updated</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in filteredInventory" :key="item.id" :class="getRowClass(item)">
            <td>
              <div class="product-info">
                <span class="product-id">{{ item.productId }}</span>
                <span v-if="item.productVariationId" class="variation-id">
                  Var: {{ item.productVariationId }}
                </span>
              </div>
            </td>
            <td>
              <div class="location-info">
                <span class="branch">Branch: {{ item.branchId }}</span>
                <span v-if="item.warehouseId" class="warehouse">
                  Warehouse: {{ item.warehouseId }}
                </span>
              </div>
            </td>
            <td class="quantity">{{ item.quantity }}</td>
            <td>{{ item.reservedQuantity }}</td>
            <td :class="getQuantityClass(item)">{{ item.availableQuantity }}</td>
            <td>{{ formatCurrency(item.unitCost) }}</td>
            <td>{{ formatCurrency(item.quantity * item.unitCost) }}</td>
            <td>{{ formatDate(item.lastUpdated) }}</td>
            <td>
              <div class="actions">
                <button class="btn-icon" @click="editItem(item)" title="Edit">✏️</button>
                <button class="btn-icon" @click="adjustItem(item)" title="Adjust">📊</button>
                <button class="btn-icon" @click="transferItem(item)" title="Transfer">🔄</button>
                <button class="btn-icon delete" @click="deleteItem(item)" title="Delete">🗑️</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Add/Edit Modal would go here -->
    <div v-if="showAddModal" class="modal-overlay" @click.self="showAddModal = false">
      <div class="modal">
        <h2>{{ editingItem ? 'Edit Inventory' : 'Add Inventory' }}</h2>
        <form @submit.prevent="saveItem">
          <div class="form-group">
            <label>Product ID</label>
            <input type="text" v-model="formData.productId" required :disabled="!!editingItem" />
          </div>
          <div class="form-group">
            <label>Branch ID</label>
            <input type="text" v-model="formData.branchId" required :disabled="!!editingItem" />
          </div>
          <div class="form-group">
            <label>Quantity</label>
            <input type="number" v-model.number="formData.quantity" required min="0" />
          </div>
          <div class="form-group">
            <label>Unit Cost</label>
            <input type="number" v-model.number="formData.unitCost" required min="0" step="0.01" />
          </div>
          <div class="form-group">
            <label>Reason</label>
            <input type="text" v-model="formData.reason" required />
          </div>
          <div class="form-actions">
            <button type="button" class="btn-secondary" @click="showAddModal = false">Cancel</button>
            <button type="submit" class="btn-primary">Save</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useInventoryStore } from '@/stores/inventoryStore'
import type { Inventory } from '@/services/inventoryService'

const inventoryStore = useInventoryStore()
const loading = ref(true)
const showAddModal = ref(false)
const editingItem = ref<Inventory | null>(null)

const filters = ref({
  branchId: '',
  warehouseId: '',
  stockStatus: '',
  search: ''
})

const sortColumn = ref('productId')
const sortDirection = ref<'asc' | 'desc'>('asc')

const formData = ref({
  productId: '',
  branchId: '',
  quantity: 0,
  unitCost: 0,
  reason: ''
})

// Mock data for branches/warehouses
const branches = ref([
  { id: '1', name: 'Main Store' },
  { id: '2', name: 'Downtown Branch' }
])

const warehouses = ref([
  { id: '1', name: 'Central Warehouse' },
  { id: '2', name: 'East Warehouse' }
])

const filteredInventory = computed(() => {
  let items = [...inventoryStore.inventoryItems]

  if (filters.value.branchId) {
    items = items.filter(item => item.branchId === filters.value.branchId)
  }
  if (filters.value.warehouseId) {
    items = items.filter(item => item.warehouseId === filters.value.warehouseId)
  }
  if (filters.value.stockStatus === 'low') {
    items = items.filter(item => item.availableQuantity > 0 && item.availableQuantity <= 10)
  } else if (filters.value.stockStatus === 'out') {
    items = items.filter(item => item.availableQuantity === 0)
  } else if (filters.value.stockStatus === 'normal') {
    items = items.filter(item => item.availableQuantity > 10)
  }
  if (filters.value.search) {
    const search = filters.value.search.toLowerCase()
    items = items.filter(item =>
      item.productId.toLowerCase().includes(search) ||
      item.branchId.toLowerCase().includes(search)
    )
  }

  // Sort
  items.sort((a, b) => {
    const aVal = a[sortColumn.value as keyof Inventory] as string | number | undefined
    const bVal = b[sortColumn.value as keyof Inventory] as string | number | undefined
    if (aVal === undefined || bVal === undefined) return 0
    if (aVal < bVal) return sortDirection.value === 'asc' ? -1 : 1
    if (aVal > bVal) return sortDirection.value === 'asc' ? 1 : -1
    return 0
  })

  return items
})

onMounted(async () => {
  try {
    await inventoryStore.fetchInventoryByBranch('1')
  } catch (error) {
    console.error('Failed to load inventory:', error)
  } finally {
    loading.value = false
  }
})

function applyFilters() {
  // Filters are reactive, computed will update automatically
}

let searchTimeout: ReturnType<typeof setTimeout>
function debouncedSearch() {
  clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => {
    applyFilters()
  }, 300)
}

function sortBy(column: string) {
  if (sortColumn.value === column) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortColumn.value = column
    sortDirection.value = 'asc'
  }
}

function getRowClass(item: Inventory) {
  if (item.availableQuantity === 0) return 'out-of-stock'
  if (item.availableQuantity <= 10) return 'low-stock'
  return ''
}

function getQuantityClass(item: Inventory) {
  if (item.availableQuantity === 0) return 'danger'
  if (item.availableQuantity <= 10) return 'warning'
  return ''
}

function editItem(item: Inventory) {
  editingItem.value = item
  formData.value = {
    productId: item.productId,
    branchId: item.branchId,
    quantity: item.quantity,
    unitCost: item.unitCost,
    reason: 'Manual update'
  }
  showAddModal.value = true
}

function adjustItem(item: Inventory) {
  // Navigate to adjustment page or show adjustment modal
  console.log('Adjust:', item)
}

function transferItem(item: Inventory) {
  // Navigate to transfer page or show transfer modal
  console.log('Transfer:', item)
}

async function deleteItem(item: Inventory) {
  if (confirm('Are you sure you want to delete this inventory record?')) {
    try {
      await inventoryStore.deleteInventory(item.id)
    } catch (error) {
      console.error('Failed to delete:', error)
    }
  }
}

async function saveItem() {
  try {
    if (editingItem.value) {
      await inventoryStore.updateInventory(editingItem.value.id, {
        quantity: formData.value.quantity,
        unitCost: formData.value.unitCost,
        reason: formData.value.reason
      })
    } else {
      await inventoryStore.createInventory({
        productId: formData.value.productId,
        branchId: formData.value.branchId,
        quantity: formData.value.quantity,
        unitCost: formData.value.unitCost,
        reason: formData.value.reason
      })
    }
    showAddModal.value = false
    editingItem.value = null
    formData.value = { productId: '', branchId: '', quantity: 0, unitCost: 0, reason: '' }
  } catch (error) {
    console.error('Failed to save:', error)
  }
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(value)
}

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString()
}
</script>

<style scoped>
.inventory-list {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h1 {
  margin: 0;
  font-size: 28px;
  color: #1a1a2e;
}

.btn-primary {
  background: #4a90a4;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
}

.btn-primary:hover {
  background: #3d7a8c;
}

.filters-section {
  display: flex;
  gap: 20px;
  margin-bottom: 20px;
  padding: 20px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  flex-wrap: wrap;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.filter-group.search {
  flex: 1;
  min-width: 200px;
}

.filter-group label {
  font-size: 12px;
  font-weight: 600;
  color: #666;
}

.filter-group select,
.filter-group input {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.table-container {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.loading,
.empty-state {
  text-align: center;
  padding: 40px;
  color: #666;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #eee;
}

th {
  background: #f8f9fa;
  font-weight: 600;
  color: #666;
}

th.sortable {
  cursor: pointer;
}

th.sortable:hover {
  background: #e9ecef;
}

tr.low-stock {
  background: #fff3cd;
}

tr.out-of-stock {
  background: #f8d7da;
}

.product-info,
.location-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.product-id,
.branch {
  font-weight: 500;
}

.variation-id,
.warehouse {
  font-size: 12px;
  color: #666;
}

.quantity {
  font-weight: 600;
}

.warning {
  color: #856404;
}

.danger {
  color: #721c24;
}

.actions {
  display: flex;
  gap: 5px;
}

.btn-icon {
  background: none;
  border: none;
  cursor: pointer;
  padding: 5px;
  font-size: 16px;
  opacity: 0.7;
  transition: opacity 0.2s;
}

.btn-icon:hover {
  opacity: 1;
}

.btn-icon.delete:hover {
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
  border-radius: 12px;
  padding: 30px;
  width: 100%;
  max-width: 500px;
}

.modal h2 {
  margin: 0 0 20px;
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: 500;
}

.form-group input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 14px;
}

.form-actions {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
  margin-top: 20px;
}

.btn-secondary {
  background: white;
  color: #666;
  border: 1px solid #ddd;
  padding: 10px 20px;
  border-radius: 6px;
  cursor: pointer;
}

.btn-secondary:hover {
  background: #f8f9fa;
}
</style>

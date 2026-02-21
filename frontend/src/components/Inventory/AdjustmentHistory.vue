<template>
  <div class="adjustment-history">
    <div class="page-header">
      <h1>Adjustment History</h1>
      <p class="subtitle">Track all inventory adjustments and audit trail</p>
    </div>

    <!-- Summary Stats -->
    <div class="stats-row">
      <div class="stat-card">
        <span class="stat-value">{{ totalAdjustments }}</span>
        <span class="stat-label">Total Adjustments</span>
      </div>
      <div class="stat-card positive">
        <span class="stat-value">+{{ positiveAdjustments }}</span>
        <span class="stat-label">Stock Increases</span>
      </div>
      <div class="stat-card negative">
        <span class="stat-value">{{ negativeAdjustments }}</span>
        <span class="stat-label">Stock Decreases</span>
      </div>
      <div class="stat-card">
        <span class="stat-value">{{ formatCurrency(netValueChange) }}</span>
        <span class="stat-label">Net Value Change</span>
      </div>
    </div>

    <!-- Filters -->
    <div class="filters-section">
      <div class="filter-group">
        <label>Date Range</label>
        <div class="date-inputs">
          <input type="date" v-model="filters.startDate" />
          <span>to</span>
          <input type="date" v-model="filters.endDate" />
        </div>
      </div>
      <div class="filter-group">
        <label>Type</label>
        <select v-model="filters.type">
          <option value="">All Types</option>
          <option value="adjustment">Manual Adjustment</option>
          <option value="in">Stock In</option>
          <option value="out">Stock Out</option>
          <option value="transfer">Transfer</option>
        </select>
      </div>
      <div class="filter-group">
        <label>Branch</label>
        <select v-model="filters.branchId">
          <option value="">All Branches</option>
          <option v-for="branch in branches" :key="branch.id" :value="branch.id">
            {{ branch.name }}
          </option>
        </select>
      </div>
      <div class="filter-group">
        <label>Search</label>
        <input
          type="text"
          v-model="filters.search"
          placeholder="Search by product or reason..."
        />
      </div>
      <button class="btn-filter" @click="applyFilters">Apply</button>
      <button class="btn-export" @click="exportHistory">Export CSV</button>
    </div>

    <!-- Transactions Table -->
    <div class="table-container">
      <div v-if="loading" class="loading">Loading history...</div>
      <div v-else-if="filteredTransactions.length === 0" class="empty-state">
        No adjustments found for the selected criteria
      </div>
      <table v-else>
        <thead>
          <tr>
            <th @click="sortBy('createdAt')" class="sortable">
              Date
              <span v-if="sortColumn === 'createdAt'">{{ sortDirection === 'asc' ? '▲' : '▼' }}</span>
            </th>
            <th>Type</th>
            <th>Product</th>
            <th>Location</th>
            <th @click="sortBy('quantity')" class="sortable">
              Quantity
              <span v-if="sortColumn === 'quantity'">{{ sortDirection === 'asc' ? '▲' : '▼' }}</span>
            </th>
            <th>Reason</th>
            <th>User</th>
            <th>Reference</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="tx in paginatedTransactions" :key="tx.id">
            <td>{{ formatDateTime(tx.createdAt) }}</td>
            <td>
              <span class="type-badge" :class="tx.transactionType">
                {{ formatType(tx.transactionType) }}
              </span>
            </td>
            <td>
              <div class="product-cell">
                <span class="product-id">{{ tx.productId }}</span>
                <span v-if="tx.productVariationId" class="variation">
                  Var: {{ tx.productVariationId }}
                </span>
              </div>
            </td>
            <td>
              <div class="location-cell">
                <span v-if="tx.fromBranchId && tx.toBranchId">
                  {{ tx.fromBranchId }} → {{ tx.toBranchId }}
                </span>
                <span v-else>
                  {{ tx.fromBranchId || tx.toBranchId || 'N/A' }}
                </span>
              </div>
            </td>
            <td :class="tx.quantity > 0 ? 'positive' : 'negative'">
              {{ tx.quantity > 0 ? '+' : '' }}{{ tx.quantity }}
            </td>
            <td class="reason-cell">{{ tx.reason }}</td>
            <td>{{ tx.createdBy }}</td>
            <td>
              <span v-if="tx.referenceId" class="reference">{{ tx.referenceId }}</span>
              <span v-else class="no-ref">-</span>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="pagination">
        <button
          class="page-btn"
          :disabled="currentPage === 1"
          @click="currentPage--"
        >
          Previous
        </button>
        <span class="page-info">
          Page {{ currentPage }} of {{ totalPages }}
        </span>
        <button
          class="page-btn"
          :disabled="currentPage === totalPages"
          @click="currentPage++"
        >
          Next
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useInventoryStore } from '@/stores/inventoryStore'
import type { InventoryTransaction } from '@/services/inventoryService'

const inventoryStore = useInventoryStore()
const loading = ref(true)
const currentPage = ref(1)
const pageSize = 20

const sortColumn = ref('createdAt')
const sortDirection = ref<'asc' | 'desc'>('desc')

const filters = ref({
  startDate: '',
  endDate: '',
  type: '',
  branchId: '',
  search: ''
})

const branches = ref([
  { id: '1', name: 'Main Store' },
  { id: '2', name: 'Downtown Branch' },
  { id: '3', name: 'Central Warehouse' }
])

const transactions = computed(() => inventoryStore.transactions)

const filteredTransactions = computed(() => {
  let result = [...transactions.value]

  if (filters.value.startDate) {
    const start = new Date(filters.value.startDate)
    result = result.filter(tx => new Date(tx.createdAt) >= start)
  }

  if (filters.value.endDate) {
    const end = new Date(filters.value.endDate)
    end.setHours(23, 59, 59)
    result = result.filter(tx => new Date(tx.createdAt) <= end)
  }

  if (filters.value.type) {
    result = result.filter(tx => tx.transactionType === filters.value.type)
  }

  if (filters.value.branchId) {
    result = result.filter(tx => 
      tx.fromBranchId === filters.value.branchId || 
      tx.toBranchId === filters.value.branchId
    )
  }

  if (filters.value.search) {
    const search = filters.value.search.toLowerCase()
    result = result.filter(tx =>
      tx.productId.toLowerCase().includes(search) ||
      tx.reason.toLowerCase().includes(search)
    )
  }

  // Sort
  result.sort((a, b) => {
    let aVal: any = a[sortColumn.value as keyof InventoryTransaction]
    let bVal: any = b[sortColumn.value as keyof InventoryTransaction]
    
    if (sortColumn.value === 'createdAt') {
      aVal = new Date(aVal).getTime()
      bVal = new Date(bVal).getTime()
    }
    
    if (aVal < bVal) return sortDirection.value === 'asc' ? -1 : 1
    if (aVal > bVal) return sortDirection.value === 'asc' ? 1 : -1
    return 0
  })

  return result
})

const totalPages = computed(() => 
  Math.ceil(filteredTransactions.value.length / pageSize)
)

const paginatedTransactions = computed(() => {
  const start = (currentPage.value - 1) * pageSize
  return filteredTransactions.value.slice(start, start + pageSize)
})

const totalAdjustments = computed(() => filteredTransactions.value.length)

const positiveAdjustments = computed(() => 
  filteredTransactions.value.filter(tx => tx.quantity > 0).reduce((sum, tx) => sum + tx.quantity, 0)
)

const negativeAdjustments = computed(() => 
  filteredTransactions.value.filter(tx => tx.quantity < 0).reduce((sum, tx) => sum + tx.quantity, 0)
)

const netValueChange = computed(() => {
  // Simplified calculation - in real app would use actual unit costs
  return positiveAdjustments.value * 10 + negativeAdjustments.value * 10
})

onMounted(async () => {
  try {
    await inventoryStore.fetchTransactions()
  } finally {
    loading.value = false
  }
})

function sortBy(column: string) {
  if (sortColumn.value === column) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortColumn.value = column
    sortDirection.value = 'desc'
  }
}

function applyFilters() {
  currentPage.value = 1
}

function exportHistory() {
  const headers = ['Date', 'Type', 'Product', 'Quantity', 'Reason', 'User', 'Reference']
  const rows = filteredTransactions.value.map(tx => [
    formatDateTime(tx.createdAt),
    tx.transactionType,
    tx.productId,
    tx.quantity,
    tx.reason,
    tx.createdBy,
    tx.referenceId || ''
  ])

  const csv = [headers.join(','), ...rows.map(r => r.join(','))].join('\n')
  const blob = new Blob([csv], { type: 'text/csv' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `inventory-adjustments-${new Date().toISOString().split('T')[0]}.csv`
  a.click()
  URL.revokeObjectURL(url)
}

function formatType(type: string): string {
  const types: Record<string, string> = {
    'in': 'Stock In',
    'out': 'Stock Out',
    'adjustment': 'Adjustment',
    'transfer': 'Transfer'
  }
  return types[type] || type
}

function formatDateTime(dateString: string): string {
  return new Date(dateString).toLocaleString()
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(value)
}
</script>

<style scoped>
.adjustment-history {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 30px;
}

.page-header h1 {
  margin: 0;
  font-size: 28px;
  color: #1a1a2e;
}

.subtitle {
  margin: 5px 0 0;
  color: #666;
}

.stats-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.stat-card {
  background: white;
  border-radius: 10px;
  padding: 20px;
  text-align: center;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.stat-card.positive {
  border-top: 3px solid #28a745;
}

.stat-card.negative {
  border-top: 3px solid #dc3545;
}

.stat-value {
  display: block;
  font-size: 28px;
  font-weight: bold;
  color: #1a1a2e;
}

.stat-card.positive .stat-value {
  color: #28a745;
}

.stat-card.negative .stat-value {
  color: #dc3545;
}

.stat-label {
  color: #666;
  font-size: 14px;
}

.filters-section {
  display: flex;
  gap: 15px;
  align-items: flex-end;
  margin-bottom: 20px;
  padding: 20px;
  background: white;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  flex-wrap: wrap;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.filter-group label {
  font-size: 12px;
  font-weight: 600;
  color: #666;
}

.filter-group input,
.filter-group select {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.date-inputs {
  display: flex;
  align-items: center;
  gap: 10px;
}

.date-inputs input {
  width: 140px;
}

.btn-filter,
.btn-export {
  padding: 10px 20px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
}

.btn-filter {
  background: #4a90a4;
  color: white;
  border: none;
}

.btn-filter:hover {
  background: #3d7a8c;
}

.btn-export {
  background: white;
  color: #666;
  border: 1px solid #ddd;
}

.btn-export:hover {
  background: #f8f9fa;
}

.table-container {
  background: white;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.loading,
.empty-state {
  text-align: center;
  padding: 60px 20px;
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

.type-badge {
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
}

.type-badge.in { background: #d4edda; color: #155724; }
.type-badge.out { background: #f8d7da; color: #721c24; }
.type-badge.adjustment { background: #fff3cd; color: #856404; }
.type-badge.transfer { background: #cce5ff; color: #004085; }

.product-cell,
.location-cell {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.product-id {
  font-weight: 500;
}

.variation {
  font-size: 12px;
  color: #666;
}

.positive {
  color: #28a745;
  font-weight: 600;
}

.negative {
  color: #dc3545;
  font-weight: 600;
}

.reason-cell {
  max-width: 200px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.reference {
  font-family: monospace;
  font-size: 12px;
  color: #666;
}

.no-ref {
  color: #ccc;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 20px;
  padding: 20px;
  border-top: 1px solid #eee;
}

.page-btn {
  padding: 8px 16px;
  border: 1px solid #ddd;
  border-radius: 6px;
  background: white;
  cursor: pointer;
}

.page-btn:hover:not(:disabled) {
  background: #f8f9fa;
}

.page-btn:disabled {
  color: #ccc;
  cursor: not-allowed;
}

.page-info {
  color: #666;
}
</style>

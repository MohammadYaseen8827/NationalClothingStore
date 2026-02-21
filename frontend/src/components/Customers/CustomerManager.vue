<template>
  <div class="customer-manager">
    <div class="page-header">
      <h1>Customers</h1>
      <router-link to="/customers/new" class="btn-primary">
        + New Customer
      </router-link>
    </div>

    <!-- Search and Filters -->
    <div class="filters-section">
      <div class="search-box">
        <input
          type="text"
          v-model="searchQuery"
          placeholder="Search by name, email, or phone..."
          @input="debouncedSearch"
        />
      </div>
      <div class="filter-group">
        <label>Status</label>
        <select v-model="statusFilter">
          <option value="">All</option>
          <option value="active">Active</option>
          <option value="inactive">Inactive</option>
        </select>
      </div>
      <div class="filter-group">
        <label>Loyalty Tier</label>
        <select v-model="tierFilter">
          <option value="">All Tiers</option>
          <option value="Standard">Standard</option>
          <option value="Silver">Silver</option>
          <option value="Gold">Gold</option>
          <option value="Platinum">Platinum</option>
        </select>
      </div>
    </div>

    <!-- Customers Table -->
    <div class="table-container">
      <div v-if="loading" class="loading">Loading customers...</div>
      <div v-else-if="filteredCustomers.length === 0" class="empty-state">
        <span class="icon">👥</span>
        <p>No customers found</p>
      </div>
      <table v-else>
        <thead>
          <tr>
            <th @click="sortBy('lastName')" class="sortable">
              Name
              <span v-if="sortColumn === 'lastName'">{{ sortDirection === 'asc' ? '▲' : '▼' }}</span>
            </th>
            <th>Email</th>
            <th>Phone</th>
            <th @click="sortBy('loyaltyPoints')" class="sortable">
              Points
              <span v-if="sortColumn === 'loyaltyPoints'">{{ sortDirection === 'asc' ? '▲' : '▼' }}</span>
            </th>
            <th>Tier</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="customer in filteredCustomers" :key="customer.id">
            <td>
              <router-link :to="`/customers/${customer.id}`" class="customer-name">
                {{ customer.firstName }} {{ customer.lastName }}
              </router-link>
            </td>
            <td>{{ customer.email }}</td>
            <td>{{ customer.phone || '-' }}</td>
            <td>
              <span class="points-badge">{{ customer.loyaltyPoints }}</span>
            </td>
            <td>
              <span class="tier-badge" :class="(customer.loyaltyTier || 'Standard').toLowerCase()">
                {{ customer.loyaltyTier || 'Standard' }}
              </span>
            </td>
            <td>
              <span class="status-badge" :class="customer.isActive ? 'active' : 'inactive'">
                {{ customer.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td>
              <div class="actions">
                <router-link :to="`/customers/${customer.id}`" class="btn-icon" title="View">👁️</router-link>
                <router-link :to="`/customers/${customer.id}/edit`" class="btn-icon" title="Edit">✏️</router-link>
                <router-link :to="`/customers/${customer.id}/loyalty`" class="btn-icon" title="Loyalty">⭐</router-link>
                <button class="btn-icon delete" @click="confirmDelete(customer)" title="Delete">🗑️</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Stats Summary -->
    <div class="stats-summary">
      <div class="stat">
        <span class="stat-value">{{ totalCustomers }}</span>
        <span class="stat-label">Total Customers</span>
      </div>
      <div class="stat">
        <span class="stat-value">{{ activeCount }}</span>
        <span class="stat-label">Active</span>
      </div>
      <div class="stat">
        <span class="stat-value">{{ tierCounts.Gold || 0 }}</span>
        <span class="stat-label">Gold Members</span>
      </div>
      <div class="stat">
        <span class="stat-value">{{ tierCounts.Platinum || 0 }}</span>
        <span class="stat-label">Platinum Members</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useCustomerStore } from '@/stores/customerStore'
import type { Customer } from '@/services/customerService'

const customerStore = useCustomerStore()

const loading = ref(true)
const searchQuery = ref('')
const statusFilter = ref('')
const tierFilter = ref('')
const sortColumn = ref('lastName')
const sortDirection = ref<'asc' | 'desc'>('asc')

const customers = computed(() => customerStore.customers)

const filteredCustomers = computed(() => {
  let result = [...customers.value]

  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(c =>
      c.firstName.toLowerCase().includes(query) ||
      c.lastName.toLowerCase().includes(query) ||
      c.email.toLowerCase().includes(query) ||
      (c.phone && c.phone.includes(query))
    )
  }

  // Status filter
  if (statusFilter.value === 'active') {
    result = result.filter(c => c.isActive)
  } else if (statusFilter.value === 'inactive') {
    result = result.filter(c => !c.isActive)
  }

  // Tier filter
  if (tierFilter.value) {
    result = result.filter(c => (c.loyaltyTier || 'Standard') === tierFilter.value)
  }

  // Sort
  result.sort((a, b) => {
    const aVal = a[sortColumn.value as keyof Customer]
    const bVal = b[sortColumn.value as keyof Customer]
    if (aVal === undefined || aVal === null) return 1
    if (bVal === undefined || bVal === null) return -1
    if (aVal < bVal) return sortDirection.value === 'asc' ? -1 : 1
    if (aVal > bVal) return sortDirection.value === 'asc' ? 1 : -1
    return 0
  })

  return result
})

const totalCustomers = computed(() => customers.value.length)
const activeCount = computed(() => customers.value.filter(c => c.isActive).length)
const tierCounts = computed(() => {
  const counts: Record<string, number> = {}
  customers.value.forEach(c => {
    const tier = c.loyaltyTier || 'Standard'
    counts[tier] = (counts[tier] || 0) + 1
  })
  return counts
})

onMounted(async () => {
  try {
    await customerStore.fetchCustomers()
  } finally {
    loading.value = false
  }
})

let searchTimeout: ReturnType<typeof setTimeout>
function debouncedSearch() {
  clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => {
    // Filtering is reactive
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

async function confirmDelete(customer: Customer) {
  if (confirm(`Are you sure you want to delete ${customer.firstName} ${customer.lastName}?`)) {
    try {
      await customerStore.deleteCustomer(customer.id)
    } catch (error) {
      console.error('Failed to delete customer:', error)
    }
  }
}
</script>

<style scoped>
.customer-manager {
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
  padding: 10px 20px;
  border-radius: 6px;
  text-decoration: none;
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

.search-box {
  flex: 1;
  min-width: 300px;
}

.search-box input {
  width: 100%;
  padding: 10px 15px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 14px;
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

.filter-group select {
  padding: 10px 15px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 14px;
}

.table-container {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  margin-bottom: 20px;
}

.loading,
.empty-state {
  text-align: center;
  padding: 60px 20px;
  color: #666;
}

.empty-state .icon {
  font-size: 48px;
  display: block;
  margin-bottom: 15px;
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

.customer-name {
  color: #4a90a4;
  text-decoration: none;
  font-weight: 500;
}

.customer-name:hover {
  text-decoration: underline;
}

.points-badge {
  background: #e8f4f8;
  color: #4a90a4;
  padding: 4px 10px;
  border-radius: 20px;
  font-weight: 500;
  font-size: 13px;
}

.tier-badge {
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
}

.tier-badge.standard { background: #e9ecef; color: #495057; }
.tier-badge.silver { background: #e3e3e3; color: #666; }
.tier-badge.gold { background: #fff3cd; color: #856404; }
.tier-badge.platinum { background: #cce5ff; color: #004085; }

.status-badge {
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
}

.status-badge.active { background: #d4edda; color: #155724; }
.status-badge.inactive { background: #f8d7da; color: #721c24; }

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
  text-decoration: none;
}

.btn-icon:hover {
  opacity: 1;
}

.btn-icon.delete:hover {
  color: #dc3545;
}

.stats-summary {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 15px;
}

.stat {
  background: white;
  padding: 20px;
  border-radius: 8px;
  text-align: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.stat-value {
  display: block;
  font-size: 28px;
  font-weight: bold;
  color: #1a1a2e;
}

.stat-label {
  color: #666;
  font-size: 14px;
}
</style>

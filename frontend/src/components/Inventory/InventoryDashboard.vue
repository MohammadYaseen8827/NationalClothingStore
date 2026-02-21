<template>
  <div class="inventory-dashboard">
    <div class="page-header">
      <h1>Inventory Overview</h1>
      <p class="subtitle">Monitor stock levels across all locations</p>
    </div>

    <!-- Statistics Cards -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon total">
          <span>📦</span>
        </div>
        <div class="stat-content">
          <span class="stat-value">{{ statistics?.totalItems || 0 }}</span>
          <span class="stat-label">Total Items</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon value">
          <span>💰</span>
        </div>
        <div class="stat-content">
          <span class="stat-value">{{ formatCurrency(statistics?.totalValue || 0) }}</span>
          <span class="stat-label">Total Value</span>
        </div>
      </div>

      <div class="stat-card warning">
        <div class="stat-icon low-stock">
          <span>⚠️</span>
        </div>
        <div class="stat-content">
          <span class="stat-value">{{ statistics?.lowStockCount || 0 }}</span>
          <span class="stat-label">Low Stock Items</span>
        </div>
      </div>

      <div class="stat-card danger">
        <div class="stat-icon out-of-stock">
          <span>🚫</span>
        </div>
        <div class="stat-content">
          <span class="stat-value">{{ statistics?.outOfStockCount || 0 }}</span>
          <span class="stat-label">Out of Stock</span>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="quick-actions">
      <router-link to="/inventory/list" class="action-btn">
        <span class="action-icon">📋</span>
        <span>View All Inventory</span>
      </router-link>
      <router-link to="/inventory/low-stock" class="action-btn warning">
        <span class="action-icon">⚠️</span>
        <span>Low Stock Alerts</span>
      </router-link>
      <router-link to="/inventory/transfers" class="action-btn">
        <span class="action-icon">🔄</span>
        <span>Stock Transfers</span>
      </router-link>
      <router-link to="/inventory/adjustments" class="action-btn">
        <span class="action-icon">📝</span>
        <span>Adjustments</span>
      </router-link>
    </div>

    <!-- Recent Transactions -->
    <div class="section">
      <h2>Recent Transactions</h2>
      <div v-if="loading" class="loading">Loading...</div>
      <div v-else-if="recentTransactions.length === 0" class="empty-state">
        No recent transactions
      </div>
      <table v-else class="transactions-table">
        <thead>
          <tr>
            <th>Date</th>
            <th>Type</th>
            <th>Product</th>
            <th>Quantity</th>
            <th>Reason</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="tx in recentTransactions" :key="tx.id">
            <td>{{ formatDate(tx.createdAt) }}</td>
            <td>
              <span class="tx-type" :class="tx.transactionType">
                {{ tx.transactionType }}
              </span>
            </td>
            <td>{{ tx.productId }}</td>
            <td :class="tx.quantity > 0 ? 'positive' : 'negative'">
              {{ tx.quantity > 0 ? '+' : '' }}{{ tx.quantity }}
            </td>
            <td>{{ tx.reason }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Branch Distribution -->
    <div class="section">
      <h2>Branch Distribution</h2>
      <div v-if="Object.keys(branchCounts).length === 0" class="empty-state">
        No branch data available
      </div>
      <div v-else class="branch-grid">
        <div v-for="(count, branchId) in branchCounts" :key="branchId" class="branch-card">
          <div class="branch-name">Branch {{ branchId }}</div>
          <div class="branch-count">{{ count }} items</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useInventoryStore } from '@/stores/inventoryStore'

const inventoryStore = useInventoryStore()
const loading = ref(true)

const statistics = computed(() => inventoryStore.statistics)
const recentTransactions = computed(() => inventoryStore.recentTransactions)
const branchCounts = computed(() => inventoryStore.statistics?.branchCounts || {})

onMounted(async () => {
  try {
    await Promise.all([
      inventoryStore.fetchStatistics(),
      inventoryStore.fetchTransactions()
    ])
  } finally {
    loading.value = false
  }
})

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
.inventory-dashboard {
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

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.stat-card {
  background: white;
  border-radius: 12px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 15px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.stat-card.warning {
  border-left: 4px solid #f0ad4e;
}

.stat-card.danger {
  border-left: 4px solid #d9534f;
}

.stat-icon {
  width: 50px;
  height: 50px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
}

.stat-icon.total { background: #e3f2fd; }
.stat-icon.value { background: #e8f5e9; }
.stat-icon.low-stock { background: #fff3e0; }
.stat-icon.out-of-stock { background: #ffebee; }

.stat-content {
  display: flex;
  flex-direction: column;
}

.stat-value {
  font-size: 24px;
  font-weight: bold;
  color: #1a1a2e;
}

.stat-label {
  font-size: 14px;
  color: #666;
}

.quick-actions {
  display: flex;
  gap: 15px;
  margin-bottom: 30px;
  flex-wrap: wrap;
}

.action-btn {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 20px;
  background: white;
  border-radius: 8px;
  text-decoration: none;
  color: #1a1a2e;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  transition: all 0.2s;
}

.action-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
}

.action-btn.warning {
  border-left: 3px solid #f0ad4e;
}

.action-icon {
  font-size: 20px;
}

.section {
  background: white;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.section h2 {
  margin: 0 0 20px;
  font-size: 20px;
  color: #1a1a2e;
}

.loading,
.empty-state {
  text-align: center;
  padding: 40px;
  color: #666;
}

.transactions-table {
  width: 100%;
  border-collapse: collapse;
}

.transactions-table th,
.transactions-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.transactions-table th {
  font-weight: 600;
  color: #666;
  background: #f8f9fa;
}

.tx-type {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
  text-transform: capitalize;
}

.tx-type.in { background: #d4edda; color: #155724; }
.tx-type.out { background: #f8d7da; color: #721c24; }
.tx-type.adjustment { background: #fff3cd; color: #856404; }
.tx-type.transfer { background: #cce5ff; color: #004085; }

.positive { color: #28a745; }
.negative { color: #dc3545; }

.branch-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 15px;
}

.branch-card {
  padding: 15px;
  background: #f8f9fa;
  border-radius: 8px;
  text-align: center;
}

.branch-name {
  font-weight: 600;
  color: #1a1a2e;
  margin-bottom: 5px;
}

.branch-count {
  color: #666;
  font-size: 14px;
}
</style>

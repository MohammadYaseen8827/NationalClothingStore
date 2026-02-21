<template>
  <div class="low-stock-alerts">
    <div class="page-header">
      <h1>Low Stock Alerts</h1>
      <p class="subtitle">Monitor and manage inventory below threshold levels</p>
    </div>

    <!-- Summary Cards -->
    <div class="summary-cards">
      <div class="summary-card danger">
        <span class="count">{{ criticalCount }}</span>
        <span class="label">Critical (0 stock)</span>
      </div>
      <div class="summary-card warning">
        <span class="count">{{ lowStockCount }}</span>
        <span class="label">Low Stock</span>
      </div>
      <div class="summary-card info">
        <span class="count">{{ unresolvedCount }}</span>
        <span class="label">Unresolved Alerts</span>
      </div>
    </div>

    <!-- Filters -->
    <div class="filters">
      <div class="filter-group">
        <label>Location</label>
        <select v-model="selectedBranch" @change="loadAlerts">
          <option value="">All Locations</option>
          <option v-for="branch in branches" :key="branch.id" :value="branch.id">
            {{ branch.name }}
          </option>
        </select>
      </div>
      <div class="filter-group">
        <label>Status</label>
        <select v-model="showResolved">
          <option value="false">Unresolved Only</option>
          <option value="true">All Alerts</option>
        </select>
      </div>
    </div>

    <!-- Alerts List -->
    <div class="alerts-container">
      <div v-if="loading" class="loading">Loading alerts...</div>
      <div v-else-if="filteredAlerts.length === 0" class="empty-state">
        <span class="icon">✅</span>
        <p>No low stock alerts. All inventory levels are healthy!</p>
      </div>
      <div v-else class="alerts-list">
        <div
          v-for="alert in filteredAlerts"
          :key="alert.inventoryId"
          class="alert-card"
          :class="{ critical: alert.currentQuantity === 0, resolved: alert.isResolved }"
        >
          <div class="alert-header">
            <div class="product-info">
              <span class="product-name">{{ alert.productName }}</span>
              <span class="product-sku">SKU: {{ alert.productSku }}</span>
            </div>
            <div class="alert-badge" :class="alert.currentQuantity === 0 ? 'critical' : 'warning'">
              {{ alert.currentQuantity === 0 ? 'Out of Stock' : 'Low Stock' }}
            </div>
          </div>

          <div class="alert-body">
            <div class="info-grid">
              <div class="info-item">
                <span class="label">Current Stock</span>
                <span class="value" :class="{ danger: alert.currentQuantity === 0 }">
                  {{ alert.currentQuantity }}
                </span>
              </div>
              <div class="info-item">
                <span class="label">Threshold</span>
                <span class="value">{{ alert.lowStockThreshold }}</span>
              </div>
              <div class="info-item">
                <span class="label">Reorder Point</span>
                <span class="value">{{ alert.reorderPoint }}</span>
              </div>
              <div class="info-item">
                <span class="label">Location</span>
                <span class="value">{{ alert.locationName }}</span>
              </div>
            </div>

            <div v-if="alert.productVariationSize || alert.productVariationColor" class="variation-info">
              <span v-if="alert.productVariationSize">Size: {{ alert.productVariationSize }}</span>
              <span v-if="alert.productVariationColor">Color: {{ alert.productVariationColor }}</span>
            </div>
          </div>

          <div class="alert-footer">
            <span class="alert-date">Alert: {{ formatDate(alert.alertDate) }}</span>
            <div class="alert-actions">
              <button
                v-if="!alert.isResolved"
                class="btn-action"
                @click="createPurchaseOrder(alert)"
              >
                Create PO
              </button>
              <button
                v-if="!alert.isResolved"
                class="btn-action secondary"
                @click="markResolved(alert)"
              >
                Mark Resolved
              </button>
              <span v-if="alert.isResolved" class="resolved-badge">
                ✓ Resolved
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useInventoryStore } from '@/stores/inventoryStore'
import type { LowStockAlert } from '@/services/inventoryService'

const router = useRouter()
const inventoryStore = useInventoryStore()

const loading = ref(true)
const selectedBranch = ref('')
const showResolved = ref('false')

const branches = ref([
  { id: '1', name: 'Main Store' },
  { id: '2', name: 'Downtown Branch' },
  { id: '3', name: 'Central Warehouse' }
])

const alerts = computed(() => inventoryStore.lowStockAlerts)

const filteredAlerts = computed(() => {
  let result = [...alerts.value]
  
  if (showResolved.value === 'false') {
    result = result.filter(alert => !alert.isResolved)
  }
  
  // Sort by severity (out of stock first) then by date
  result.sort((a, b) => {
    if (a.currentQuantity === 0 && b.currentQuantity > 0) return -1
    if (b.currentQuantity === 0 && a.currentQuantity > 0) return 1
    return new Date(b.alertDate).getTime() - new Date(a.alertDate).getTime()
  })
  
  return result
})

const criticalCount = computed(() => 
  alerts.value.filter(a => a.currentQuantity === 0 && !a.isResolved).length
)

const lowStockCount = computed(() => 
  alerts.value.filter(a => a.currentQuantity > 0 && !a.isResolved).length
)

const unresolvedCount = computed(() => 
  alerts.value.filter(a => !a.isResolved).length
)

onMounted(async () => {
  await loadAlerts()
})

async function loadAlerts() {
  loading.value = true
  try {
    const params = selectedBranch.value ? { branchId: selectedBranch.value } : undefined
    await inventoryStore.fetchLowStockAlerts(params)
  } catch (error) {
    console.error('Failed to load alerts:', error)
  } finally {
    loading.value = false
  }
}

function createPurchaseOrder(alert: LowStockAlert) {
  // Navigate to purchase order creation with pre-filled data
  router.push({
    path: '/procurement/orders/new',
    query: {
      productId: alert.productId,
      quantity: String(alert.reorderPoint - alert.currentQuantity)
    }
  })
}

function markResolved(alert: LowStockAlert) {
  // In real app, would call API to mark resolved
  alert.isResolved = true
}

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString()
}
</script>

<style scoped>
.low-stock-alerts {
  padding: 20px;
  max-width: 1200px;
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

.summary-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.summary-card {
  background: white;
  border-radius: 12px;
  padding: 20px;
  text-align: center;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.summary-card.danger {
  border-top: 4px solid #dc3545;
}

.summary-card.warning {
  border-top: 4px solid #ffc107;
}

.summary-card.info {
  border-top: 4px solid #17a2b8;
}

.summary-card .count {
  display: block;
  font-size: 36px;
  font-weight: bold;
  color: #1a1a2e;
}

.summary-card .label {
  color: #666;
  font-size: 14px;
}

.filters {
  display: flex;
  gap: 20px;
  margin-bottom: 20px;
  padding: 15px 20px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
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
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  min-width: 180px;
}

.alerts-container {
  background: white;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
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

.alerts-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.alert-card {
  border: 1px solid #e0e0e0;
  border-radius: 10px;
  padding: 20px;
  transition: all 0.2s;
}

.alert-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.alert-card.critical {
  border-left: 4px solid #dc3545;
  background: #fff5f5;
}

.alert-card.resolved {
  opacity: 0.7;
  background: #f8f9fa;
}

.alert-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 15px;
}

.product-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.product-name {
  font-size: 18px;
  font-weight: 600;
  color: #1a1a2e;
}

.product-sku {
  font-size: 13px;
  color: #666;
}

.alert-badge {
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
}

.alert-badge.critical {
  background: #f8d7da;
  color: #721c24;
}

.alert-badge.warning {
  background: #fff3cd;
  color: #856404;
}

.alert-body {
  margin-bottom: 15px;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
  gap: 15px;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.info-item .label {
  font-size: 12px;
  color: #666;
}

.info-item .value {
  font-size: 16px;
  font-weight: 600;
  color: #1a1a2e;
}

.info-item .value.danger {
  color: #dc3545;
}

.variation-info {
  margin-top: 15px;
  padding: 10px;
  background: #f8f9fa;
  border-radius: 6px;
  font-size: 14px;
  color: #666;
  display: flex;
  gap: 20px;
}

.alert-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 15px;
  border-top: 1px solid #e0e0e0;
}

.alert-date {
  font-size: 13px;
  color: #666;
}

.alert-actions {
  display: flex;
  gap: 10px;
}

.btn-action {
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  background: #4a90a4;
  color: white;
  border: none;
}

.btn-action:hover {
  background: #3d7a8c;
}

.btn-action.secondary {
  background: white;
  color: #666;
  border: 1px solid #ddd;
}

.btn-action.secondary:hover {
  background: #f8f9fa;
}

.resolved-badge {
  color: #28a745;
  font-weight: 500;
}
</style>

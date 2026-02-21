<template>
  <div class="purchase-order-manager">
    <div class="header">
      <h1>Purchase Orders</h1>
      <router-link to="/procurement/orders/new" class="btn btn-primary">
        New Order
      </router-link>
    </div>

    <div class="filters">
      <select v-model="statusFilter" class="filter-select">
        <option value="">All Status</option>
        <option value="Draft">Draft</option>
        <option value="Pending">Pending</option>
        <option value="Approved">Approved</option>
        <option value="Ordered">Ordered</option>
        <option value="Received">Received</option>
        <option value="Cancelled">Cancelled</option>
      </select>
      <input
        v-model="searchQuery"
        type="text"
        placeholder="Search by order number..."
        class="search-input"
      />
    </div>

    <div v-if="store.isLoading" class="loading">Loading orders...</div>
    <div v-else-if="store.error" class="error">{{ store.error }}</div>
    <div v-else-if="filteredOrders.length === 0" class="empty">
      No purchase orders found.
    </div>

    <div v-else class="order-list">
      <table class="data-table">
        <thead>
          <tr>
            <th>Order #</th>
            <th>Supplier</th>
            <th>Status</th>
            <th>Order Date</th>
            <th>Expected Delivery</th>
            <th>Total</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="order in filteredOrders" :key="order.id">
            <td>{{ order.orderNumber }}</td>
            <td>{{ order.supplierName }}</td>
            <td>
              <span :class="['status-badge', order.status.toLowerCase()]">
                {{ order.status }}
              </span>
            </td>
            <td>{{ formatDate(order.orderDate) }}</td>
            <td>{{ order.expectedDeliveryDate ? formatDate(order.expectedDeliveryDate) : '-' }}</td>
            <td>{{ formatCurrency(order.totalAmount) }}</td>
            <td class="actions">
              <router-link :to="`/procurement/orders/${order.id}`" class="btn btn-sm btn-secondary">
                View
              </router-link>
              <button 
                v-if="order.status === 'Draft' || order.status === 'Pending'"
                @click="confirmDelete(order)" 
                class="btn btn-sm btn-danger"
              >
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
        <p>Are you sure you want to delete order #{{ orderToDelete?.orderNumber }}?</p>
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
import { ref, computed, onMounted, watch } from 'vue'
import { useProcurementStore } from '@/stores/procurementStore'
import type { PurchaseOrder } from '@/services/procurementService'

const store = useProcurementStore()

const searchQuery = ref('')
const statusFilter = ref('')
const showDeleteModal = ref(false)
const orderToDelete = ref<PurchaseOrder | null>(null)
const isDeleting = ref(false)

const filteredOrders = computed(() => {
  let orders = store.purchaseOrders
  
  if (statusFilter.value) {
    orders = orders.filter(o => o.status === statusFilter.value)
  }
  
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    orders = orders.filter(o =>
      o.orderNumber.toLowerCase().includes(query) ||
      o.supplierName.toLowerCase().includes(query)
    )
  }
  
  return orders
})

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(amount)
}

function confirmDelete(order: PurchaseOrder) {
  orderToDelete.value = order
  showDeleteModal.value = true
}

async function handleDelete() {
  if (!orderToDelete.value) return
  isDeleting.value = true
  try {
    await store.deletePurchaseOrder(orderToDelete.value.id)
    showDeleteModal.value = false
    orderToDelete.value = null
  } catch (e) {
    console.error('Failed to delete order:', e)
  } finally {
    isDeleting.value = false
  }
}

watch(statusFilter, (status) => {
  if (status) {
    store.fetchPurchaseOrdersByStatus(status)
  } else {
    store.fetchPurchaseOrders()
  }
})

onMounted(() => {
  store.fetchPurchaseOrders()
})
</script>

<style scoped>
.purchase-order-manager {
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.filters {
  display: flex;
  gap: 16px;
  margin-bottom: 20px;
}

.filter-select,
.search-input {
  padding: 10px 15px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.search-input {
  flex: 1;
  max-width: 300px;
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

.status-badge.draft { background: #e9ecef; color: #495057; }
.status-badge.pending { background: #fff3cd; color: #856404; }
.status-badge.approved { background: #cce5ff; color: #004085; }
.status-badge.ordered { background: #d4edda; color: #155724; }
.status-badge.received { background: #d1ecf1; color: #0c5460; }
.status-badge.cancelled { background: #f8d7da; color: #721c24; }

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

.btn-primary { background-color: #007bff; color: white; }
.btn-secondary { background-color: #6c757d; color: white; }
.btn-danger { background-color: #dc3545; color: white; }
.btn-sm { padding: 4px 8px; font-size: 12px; }

.loading, .error, .empty {
  padding: 40px;
  text-align: center;
  color: #666;
}

.error { color: #dc3545; }

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

.modal h3 { margin: 0 0 16px; }
.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 20px;
}
</style>

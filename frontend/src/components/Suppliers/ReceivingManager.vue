<template>
  <div class="receiving-manager">
    <div class="page-header">
      <h1>Receiving Manager</h1>
      <p class="subtitle">Receive and process incoming shipments</p>
    </div>

    <!-- Pending Orders Section -->
    <div class="section">
      <h2>Pending Purchase Orders</h2>
      <div v-if="loading" class="loading">Loading orders...</div>
      <div v-else-if="pendingOrders.length === 0" class="empty-state">
        No pending orders awaiting delivery
      </div>
      <div v-else class="orders-grid">
        <div
          v-for="order in pendingOrders"
          :key="order.id"
          class="order-card"
          :class="{ selected: selectedOrder?.id === order.id }"
          @click="selectOrder(order)"
        >
          <div class="order-header">
            <span class="order-number">PO-{{ order.id }}</span>
            <span class="order-status" :class="order.status.toLowerCase()">
              {{ order.status }}
            </span>
          </div>
          <div class="order-details">
            <p><strong>Supplier:</strong> {{ order.supplierName }}</p>
            <p><strong>Expected:</strong> {{ formatDate(order.expectedDeliveryDate) }}</p>
            <p><strong>Items:</strong> {{ order.items?.length || 0 }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Receiving Form Section -->
    <div v-if="selectedOrder" class="section receiving-form">
      <h2>Receive Order: PO-{{ selectedOrder.id }}</h2>
      
      <div class="order-info">
        <div class="info-row">
          <span class="label">Supplier:</span>
          <span class="value">{{ selectedOrder.supplierName }}</span>
        </div>
        <div class="info-row">
          <span class="label">Order Date:</span>
          <span class="value">{{ formatDate(selectedOrder.orderDate) }}</span>
        </div>
        <div class="info-row">
          <span class="label">Expected Delivery:</span>
          <span class="value">{{ formatDate(selectedOrder.expectedDeliveryDate) }}</span>
        </div>
      </div>

      <div class="items-table">
        <h3>Order Items</h3>
        <table>
          <thead>
            <tr>
              <th>Product</th>
              <th>SKU</th>
              <th>Ordered</th>
              <th>Previously Received</th>
              <th>Receive Now</th>
              <th>Notes</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, index) in receivingItems" :key="index">
              <td>{{ item.productName }}</td>
              <td>{{ item.productSku }}</td>
              <td>{{ item.orderedQuantity }}</td>
              <td>{{ item.receivedQuantity }}</td>
              <td>
                <input
                  type="number"
                  v-model.number="item.receiveNow"
                  :max="(item.orderedQuantity || 0) - (item.receivedQuantity || 0)"
                  min="0"
                  class="quantity-input"
                />
              </td>
              <td>
                <input
                  type="text"
                  v-model="item.notes"
                  placeholder="Optional notes"
                  class="notes-input"
                />
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="receiving-actions">
        <div class="receive-options">
          <label class="checkbox-label">
            <input type="checkbox" v-model="markAsComplete" />
            Mark order as complete after receiving
          </label>
        </div>
        <div class="action-buttons">
          <button class="btn-secondary" @click="cancelReceiving">Cancel</button>
          <button
            class="btn-primary"
            @click="processReceiving"
            :disabled="!hasItemsToReceive || processing"
          >
            {{ processing ? 'Processing...' : 'Receive Items' }}
          </button>
        </div>
      </div>
    </div>

    <!-- Recent Receipts Section -->
    <div class="section">
      <h2>Recent Receipts</h2>
      <div v-if="recentReceipts.length === 0" class="empty-state">
        No recent receipts
      </div>
      <table v-else class="receipts-table">
        <thead>
          <tr>
            <th>Receipt Date</th>
            <th>PO Number</th>
            <th>Supplier</th>
            <th>Items Received</th>
            <th>Received By</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="receipt in recentReceipts" :key="receipt.id">
            <td>{{ formatDate(receipt.receivedDate) }}</td>
            <td>PO-{{ receipt.purchaseOrderId }}</td>
            <td>{{ receipt.supplierName }}</td>
            <td>{{ receipt.itemCount }} items</td>
            <td>{{ receipt.receivedBy }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useProcurementStore } from '@/stores/procurementStore'

interface PurchaseOrder {
  id: string
  orderNumber?: string
  supplierName: string
  status: string
  orderDate: string
  expectedDeliveryDate?: string
  items?: PurchaseOrderItem[]
}

interface PurchaseOrderItem {
  id?: string
  productId: string
  productName: string
  productSku?: string
  quantity: number
  orderedQuantity?: number
  receivedQuantity?: number
  unitCost?: number
  totalCost?: number
  quantityReceived?: number
  notes?: string
}

interface OrderItem extends PurchaseOrderItem {
  receiveNow?: number
}

interface ReceivingItem extends OrderItem {}

interface Receipt {
  id: number
  receivedDate: string
  purchaseOrderId: number
  supplierName: string
  itemCount: number
  receivedBy: string
}

const procurementStore = useProcurementStore()

const loading = ref(true)
const processing = ref(false)
const selectedOrder = ref<PurchaseOrder | null>(null)
const receivingItems = ref<ReceivingItem[]>([])
const markAsComplete = ref(false)
const recentReceipts = ref<Receipt[]>([])

const pendingOrders = computed(() => {
  return procurementStore.purchaseOrders.filter(
    (order: PurchaseOrder) => order.status === 'Pending' || order.status === 'PartiallyReceived'
  )
})

const hasItemsToReceive = computed(() => {
  return receivingItems.value.some(item => (item.receiveNow || 0) > 0)
})

onMounted(async () => {
  try {
    await procurementStore.fetchPurchaseOrders()
    await loadRecentReceipts()
  } finally {
    loading.value = false
  }
})

function selectOrder(order: PurchaseOrder) {
  selectedOrder.value = order
  receivingItems.value = (order.items || []).map(item => ({
    ...item,
    receiveNow: 0,
    notes: ''
  }))
  markAsComplete.value = false
}

function cancelReceiving() {
  selectedOrder.value = null
  receivingItems.value = []
  markAsComplete.value = false
}

async function processReceiving() {
  if (!selectedOrder.value || !hasItemsToReceive.value) return

  processing.value = true
  try {
    const itemsToReceive = receivingItems.value
      .filter(item => (item.receiveNow || 0) > 0)
      .map(item => ({
        productId: item.productId,
        quantity: item.receiveNow,
        notes: item.notes
      }))

    await procurementStore.receivePurchaseOrder(selectedOrder.value.id, itemsToReceive)

    // Refresh data
    await procurementStore.fetchPurchaseOrders()
    await loadRecentReceipts()
    
    // Reset selection
    cancelReceiving()
  } catch (error) {
    console.error('Failed to process receiving:', error)
  } finally {
    processing.value = false
  }
}

async function loadRecentReceipts() {
  // Mock data - in real app would fetch from API
  recentReceipts.value = [
    {
      id: 1,
      receivedDate: new Date().toISOString(),
      purchaseOrderId: 101,
      supplierName: 'Fabric Supplier Inc.',
      itemCount: 5,
      receivedBy: 'John Doe'
    }
  ]
}

function formatDate(dateString?: string): string {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString()
}
</script>

<style scoped>
.receiving-manager {
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

.section {
  background: white;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 20px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
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

.orders-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 15px;
}

.order-card {
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  padding: 15px;
  cursor: pointer;
  transition: all 0.2s;
}

.order-card:hover {
  border-color: #4a90a4;
  background: #f8f9fa;
}

.order-card.selected {
  border-color: #4a90a4;
  background: #e8f4f8;
}

.order-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.order-number {
  font-weight: bold;
  font-size: 16px;
}

.order-status {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.order-status.pending {
  background: #fff3cd;
  color: #856404;
}

.order-status.partiallyreceived {
  background: #cce5ff;
  color: #004085;
}

.order-details p {
  margin: 5px 0;
  font-size: 14px;
  color: #555;
}

.order-info {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 15px;
  margin-bottom: 20px;
  padding: 15px;
  background: #f8f9fa;
  border-radius: 8px;
}

.info-row .label {
  font-weight: 500;
  color: #666;
  display: block;
  margin-bottom: 4px;
}

.info-row .value {
  color: #1a1a2e;
}

.items-table {
  margin-bottom: 20px;
}

.items-table h3 {
  margin: 0 0 15px;
  font-size: 16px;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #e0e0e0;
}

th {
  background: #f8f9fa;
  font-weight: 600;
  color: #1a1a2e;
}

.quantity-input {
  width: 80px;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.notes-input {
  width: 150px;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.receiving-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 20px;
  border-top: 1px solid #e0e0e0;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}

.action-buttons {
  display: flex;
  gap: 10px;
}

.btn-primary,
.btn-secondary {
  padding: 10px 20px;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary {
  background: #4a90a4;
  color: white;
  border: none;
}

.btn-primary:hover:not(:disabled) {
  background: #3d7a8c;
}

.btn-primary:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.btn-secondary {
  background: white;
  color: #666;
  border: 1px solid #ddd;
}

.btn-secondary:hover {
  background: #f8f9fa;
}

.receipts-table {
  width: 100%;
}
</style>

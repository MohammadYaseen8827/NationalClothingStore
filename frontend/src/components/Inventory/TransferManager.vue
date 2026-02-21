<template>
  <div class="transfer-manager">
    <div class="page-header">
      <h1>Stock Transfers</h1>
      <button class="btn-primary" @click="showTransferModal = true">
        + New Transfer
      </button>
    </div>

    <!-- Pending Transfers -->
    <div class="section">
      <h2>Pending Transfers</h2>
      <div v-if="pendingTransfers.length === 0" class="empty-state">
        No pending transfers
      </div>
      <div v-else class="transfers-grid">
        <div v-for="transfer in pendingTransfers" :key="transfer.id" class="transfer-card pending">
          <div class="transfer-header">
            <span class="transfer-id">#{{ transfer.id }}</span>
            <span class="status pending">Pending</span>
          </div>
          <div class="transfer-route">
            <div class="location from">
              <span class="label">From</span>
              <span class="name">{{ transfer.fromLocation }}</span>
            </div>
            <span class="arrow">→</span>
            <div class="location to">
              <span class="label">To</span>
              <span class="name">{{ transfer.toLocation }}</span>
            </div>
          </div>
          <div class="transfer-details">
            <p><strong>Product:</strong> {{ transfer.productName }}</p>
            <p><strong>Quantity:</strong> {{ transfer.quantity }}</p>
            <p><strong>Requested:</strong> {{ formatDate(transfer.requestDate) }}</p>
          </div>
          <div class="transfer-actions">
            <button class="btn-action" @click="approveTransfer(transfer)">Approve</button>
            <button class="btn-action secondary" @click="rejectTransfer(transfer)">Reject</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Transfer History -->
    <div class="section">
      <h2>Transfer History</h2>
      <div class="filters">
        <div class="filter-group">
          <label>Date Range</label>
          <select v-model="dateFilter">
            <option value="7">Last 7 days</option>
            <option value="30">Last 30 days</option>
            <option value="90">Last 90 days</option>
            <option value="all">All time</option>
          </select>
        </div>
        <div class="filter-group">
          <label>Status</label>
          <select v-model="statusFilter">
            <option value="">All</option>
            <option value="completed">Completed</option>
            <option value="cancelled">Cancelled</option>
          </select>
        </div>
      </div>

      <table class="history-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Date</th>
            <th>Product</th>
            <th>From</th>
            <th>To</th>
            <th>Qty</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="transfer in filteredHistory" :key="transfer.id">
            <td>#{{ transfer.id }}</td>
            <td>{{ formatDate(transfer.completedDate || transfer.requestDate) }}</td>
            <td>{{ transfer.productName }}</td>
            <td>{{ transfer.fromLocation }}</td>
            <td>{{ transfer.toLocation }}</td>
            <td>{{ transfer.quantity }}</td>
            <td>
              <span class="status" :class="transfer.status">
                {{ transfer.status }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Transfer Modal -->
    <div v-if="showTransferModal" class="modal-overlay" @click.self="closeModal">
      <div class="modal">
        <h2>Create Stock Transfer</h2>
        <form @submit.prevent="submitTransfer">
          <div class="form-group">
            <label>Source Inventory</label>
            <select v-model="transferForm.fromInventoryId" required>
              <option value="">Select source...</option>
              <option v-for="item in sourceInventory" :key="item.id" :value="item.id">
                {{ item.productId }} - {{ item.branchId }} ({{ item.availableQuantity }} available)
              </option>
            </select>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Destination Branch</label>
              <select v-model="transferForm.toBranchId" required>
                <option value="">Select branch...</option>
                <option v-for="branch in branches" :key="branch.id" :value="branch.id">
                  {{ branch.name }}
                </option>
              </select>
            </div>
            <div class="form-group">
              <label>Warehouse (optional)</label>
              <select v-model="transferForm.toWarehouseId">
                <option value="">No warehouse</option>
                <option v-for="wh in warehouses" :key="wh.id" :value="wh.id">
                  {{ wh.name }}
                </option>
              </select>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Quantity</label>
              <input type="number" v-model.number="transferForm.quantity" required min="1" />
            </div>
            <div class="form-group">
              <label>Unit Cost</label>
              <input type="number" v-model.number="transferForm.unitCost" required min="0" step="0.01" />
            </div>
          </div>

          <div class="form-group">
            <label>Reason</label>
            <textarea v-model="transferForm.reason" required rows="3" placeholder="Reason for transfer..."></textarea>
          </div>

          <div class="form-actions">
            <button type="button" class="btn-secondary" @click="closeModal">Cancel</button>
            <button type="submit" class="btn-primary" :disabled="submitting">
              {{ submitting ? 'Processing...' : 'Create Transfer' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useInventoryStore } from '@/stores/inventoryStore'

interface Transfer {
  id: string
  productId: string
  productName: string
  fromLocation: string
  toLocation: string
  quantity: number
  status: 'pending' | 'completed' | 'cancelled'
  requestDate: string
  completedDate?: string
}

const inventoryStore = useInventoryStore()

const showTransferModal = ref(false)
const submitting = ref(false)
const dateFilter = ref('30')
const statusFilter = ref('')

const transferForm = ref({
  fromInventoryId: '',
  toBranchId: '',
  toWarehouseId: '',
  quantity: 1,
  unitCost: 0,
  reason: ''
})

const pendingTransfers = ref<Transfer[]>([
  {
    id: 'TRF001',
    productId: 'PRD001',
    productName: 'Cotton T-Shirt',
    fromLocation: 'Main Store',
    toLocation: 'Downtown Branch',
    quantity: 50,
    status: 'pending',
    requestDate: new Date().toISOString()
  }
])

const transferHistory = ref<Transfer[]>([
  {
    id: 'TRF000',
    productId: 'PRD002',
    productName: 'Denim Jeans',
    fromLocation: 'Central Warehouse',
    toLocation: 'Main Store',
    quantity: 100,
    status: 'completed',
    requestDate: new Date(Date.now() - 86400000 * 5).toISOString(),
    completedDate: new Date(Date.now() - 86400000 * 4).toISOString()
  }
])

const sourceInventory = computed(() => inventoryStore.inventoryItems)

const branches = ref([
  { id: '1', name: 'Main Store' },
  { id: '2', name: 'Downtown Branch' },
  { id: '3', name: 'Mall Location' }
])

const warehouses = ref([
  { id: '1', name: 'Central Warehouse' },
  { id: '2', name: 'East Warehouse' }
])

const filteredHistory = computed(() => {
  let result = [...transferHistory.value]
  
  if (statusFilter.value) {
    result = result.filter(t => t.status === statusFilter.value)
  }
  
  if (dateFilter.value !== 'all') {
    const days = parseInt(dateFilter.value)
    const cutoff = new Date(Date.now() - days * 86400000)
    result = result.filter(t => new Date(t.requestDate) >= cutoff)
  }
  
  return result.sort((a, b) => 
    new Date(b.requestDate).getTime() - new Date(a.requestDate).getTime()
  )
})

onMounted(async () => {
  try {
    await inventoryStore.fetchInventoryByBranch('1')
  } catch (error) {
    console.error('Failed to load inventory:', error)
  }
})

function closeModal() {
  showTransferModal.value = false
  transferForm.value = {
    fromInventoryId: '',
    toBranchId: '',
    toWarehouseId: '',
    quantity: 1,
    unitCost: 0,
    reason: ''
  }
}

async function submitTransfer() {
  submitting.value = true
  try {
    await inventoryStore.transferInventory({
      fromInventoryId: transferForm.value.fromInventoryId,
      toBranchId: transferForm.value.toBranchId,
      toWarehouseId: transferForm.value.toWarehouseId || undefined,
      quantity: transferForm.value.quantity,
      unitCost: transferForm.value.unitCost,
      reason: transferForm.value.reason
    })
    closeModal()
  } catch (error) {
    console.error('Transfer failed:', error)
  } finally {
    submitting.value = false
  }
}

function approveTransfer(transfer: Transfer) {
  transfer.status = 'completed'
  transfer.completedDate = new Date().toISOString()
  transferHistory.value.unshift(transfer)
  pendingTransfers.value = pendingTransfers.value.filter(t => t.id !== transfer.id)
}

function rejectTransfer(transfer: Transfer) {
  transfer.status = 'cancelled'
  transferHistory.value.unshift(transfer)
  pendingTransfers.value = pendingTransfers.value.filter(t => t.id !== transfer.id)
}

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString()
}
</script>

<style scoped>
.transfer-manager {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
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

.btn-primary:disabled {
  background: #ccc;
  cursor: not-allowed;
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

.empty-state {
  text-align: center;
  padding: 40px;
  color: #666;
}

.transfers-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
}

.transfer-card {
  border: 1px solid #e0e0e0;
  border-radius: 10px;
  padding: 20px;
}

.transfer-card.pending {
  border-left: 4px solid #ffc107;
}

.transfer-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
}

.transfer-id {
  font-weight: 600;
  color: #1a1a2e;
}

.status {
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
  text-transform: capitalize;
}

.status.pending { background: #fff3cd; color: #856404; }
.status.completed { background: #d4edda; color: #155724; }
.status.cancelled { background: #f8d7da; color: #721c24; }

.transfer-route {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 15px;
  padding: 15px;
  background: #f8f9fa;
  border-radius: 8px;
}

.location {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.location .label {
  font-size: 11px;
  color: #666;
  text-transform: uppercase;
}

.location .name {
  font-weight: 500;
  color: #1a1a2e;
}

.arrow {
  font-size: 24px;
  color: #4a90a4;
}

.transfer-details p {
  margin: 5px 0;
  font-size: 14px;
  color: #555;
}

.transfer-actions {
  display: flex;
  gap: 10px;
  margin-top: 15px;
  padding-top: 15px;
  border-top: 1px solid #e0e0e0;
}

.btn-action {
  flex: 1;
  padding: 8px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
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

.filters {
  display: flex;
  gap: 20px;
  margin-bottom: 20px;
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
}

.history-table {
  width: 100%;
  border-collapse: collapse;
}

.history-table th,
.history-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.history-table th {
  background: #f8f9fa;
  font-weight: 600;
  color: #666;
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
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal h2 {
  margin: 0 0 20px;
}

.form-group {
  margin-bottom: 15px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 15px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: 500;
}

.form-group input,
.form-group select,
.form-group textarea {
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

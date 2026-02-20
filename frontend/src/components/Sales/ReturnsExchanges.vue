<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useSalesStore } from '@/stores/salesStore'
import { salesService } from '@/services/salesService'

const salesStore = useSalesStore()

// Form state
const returnTransactionNumber = ref('')
const returnReason = ref('')
const returnItems = ref<any[]>([])
const searchError = ref<string | null>(null)

// UI state
const isProcessing = ref(false)
const showSuccess = ref(false)

// Methods
const searchTransaction = async () => {
  if (!returnTransactionNumber.value.trim()) return
  
  searchError.value = null
  
  try {
    // Call actual API to search for transaction
    const transaction = await salesService.getTransactionByNumber(returnTransactionNumber.value)
    
    if (!transaction) {
      searchError.value = 'Transaction not found'
      returnItems.value = []
      return
    }
    
    // Map transaction items for return
    returnItems.value = transaction.items.map((item: any) => ({
      ...item,
      returnEligible: true // All items are eligible unless past return window
    }))
  } catch (error: any) {
    console.error('Error searching transaction:', error)
    searchError.value = error.message || 'Failed to find transaction'
    returnItems.value = []
  }
}

const processReturn = async () => {
  if (returnItems.value.length === 0) return
  
  isProcessing.value = true
  
  try {
    const returnRequest = {
      originalTransactionNumber: returnTransactionNumber.value,
      userId: 'current-user-id', // Would come from auth context
      items: returnItems.value.map(item => ({
        originalItemId: item.id,
        quantity: item.returnQuantity || 1,
        reason: returnReason.value
      }))
    }
    
    await salesStore.processReturn(returnRequest)
    showSuccess.value = true
    
    // Reset form
    returnTransactionNumber.value = ''
    returnReason.value = ''
    returnItems.value = []
    
    // Hide success message after 3 seconds
    setTimeout(() => {
      showSuccess.value = false
    }, 3000)
  } catch (error) {
    console.error('Error processing return:', error)
  } finally {
    isProcessing.value = false
  }
}

const updateReturnQuantity = (itemId: string, quantity: number) => {
  const item = returnItems.value.find(i => i.id === itemId)
  if (item) {
    item.returnQuantity = Math.max(0, Math.min(quantity, item.quantity))
  }
}
</script>

<template>
  <div class="returns-exchanges">
    <div class="header">
      <h1>Returns & Exchanges</h1>
    </div>

    <!-- Success Message -->
    <div v-if="showSuccess" class="alert alert-success">
      Return processed successfully!
    </div>

    <!-- Transaction Search -->
    <div class="search-section">
      <h2>Find Transaction</h2>
      <div class="search-form">
        <div class="form-group">
          <label>Transaction Number:</label>
          <div class="input-group">
            <input
              v-model="returnTransactionNumber"
              type="text"
              class="form-input"
              placeholder="Enter transaction number"
              @keyup.enter="searchTransaction"
            />
            <button 
              @click="searchTransaction"
              class="btn btn-primary"
            >
              Search
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Return Items -->
    <div v-if="returnItems.length > 0" class="return-items-section">
      <h2>Items to Return</h2>
      <div class="items-list">
        <div 
          v-for="item in returnItems" 
          :key="item.id"
          class="return-item"
        >
          <div class="item-info">
            <h3>{{ item.productName }}</h3>
            <p class="item-details">
              Price: ${{ item.unitPrice.toFixed(2) }} | 
              Purchased: {{ item.quantity }}
            </p>
          </div>
          
          <div class="return-controls">
            <div class="quantity-control">
              <label>Return Quantity:</label>
              <div class="input-group">
                <button 
                  @click="updateReturnQuantity(item.id, (item.returnQuantity || 0) - 1)"
                  class="qty-btn"
                  :disabled="(item.returnQuantity || 0) <= 0"
                >
                  -
                </button>
                <input
                  :value="item.returnQuantity || 0"
                  @change="updateReturnQuantity(item.id, parseInt(($event.target as HTMLInputElement).value) || 0)"
                  type="number"
                  min="0"
                  :max="item.quantity"
                  class="qty-input"
                />
                <button 
                  @click="updateReturnQuantity(item.id, (item.returnQuantity || 0) + 1)"
                  class="qty-btn"
                  :disabled="(item.returnQuantity || 0) >= item.quantity"
                >
                  +
                </button>
              </div>
            </div>
            
            <div class="reason-control">
              <label>Return Reason:</label>
              <select v-model="returnReason" class="form-select">
                <option value="">Select reason...</option>
                <option value="wrong_size">Wrong Size</option>
                <option value="wrong_color">Wrong Color</option>
                <option value="defective">Defective Item</option>
                <option value="not_as_described">Not as Described</option>
                <option value="changed_mind">Changed Mind</option>
                <option value="other">Other</option>
              </select>
            </div>
          </div>
        </div>
      </div>

      <div class="return-actions">
        <button 
          @click="processReturn"
          :disabled="isProcessing || !returnReason"
          class="btn btn-success process-btn"
        >
          {{ isProcessing ? 'Processing...' : 'Process Return' }}
        </button>
      </div>
    </div>

    <!-- Exchange Section -->
    <div class="exchange-section">
      <h2>Process Exchange</h2>
      <p>Exchange functionality would be implemented here...</p>
      <!-- Exchange form would go here -->
    </div>
  </div>
</template>

<style scoped>
.returns-exchanges {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.header {
  margin-bottom: 30px;
}

.header h1 {
  margin: 0;
  color: #333;
}

.alert {
  padding: 15px;
  border-radius: 4px;
  margin-bottom: 20px;
}

.alert-success {
  background-color: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
}

.search-section,
.return-items-section,
.exchange-section {
  background: white;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 20px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.search-section h2,
.return-items-section h2,
.exchange-section h2 {
  margin-top: 0;
  color: #333;
  border-bottom: 1px solid #eee;
  padding-bottom: 10px;
}

.search-form {
  max-width: 500px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: bold;
  color: #333;
}

.input-group {
  display: flex;
  gap: 10px;
}

.form-input,
.form-select {
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 16px;
  flex: 1;
}

.form-input:focus,
.form-select:focus {
  outline: none;
  border-color: #2c5aa0;
  box-shadow: 0 0 0 2px rgba(44, 90, 160, 0.2);
}

.items-list {
  margin-bottom: 20px;
}

.return-item {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 20px;
  border: 1px solid #eee;
  border-radius: 6px;
  margin-bottom: 15px;
  background: #f8f9fa;
}

.item-info h3 {
  margin: 0 0 10px 0;
  color: #333;
}

.item-details {
  margin: 0;
  color: #666;
}

.return-controls {
  display: flex;
  flex-direction: column;
  gap: 15px;
  min-width: 300px;
}

.quantity-control,
.reason-control {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.qty-btn {
  width: 35px;
  height: 35px;
  border: 1px solid #ddd;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
}

.qty-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.qty-input {
  width: 60px;
  height: 35px;
  text-align: center;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.return-actions {
  text-align: center;
  padding-top: 20px;
  border-top: 1px solid #eee;
}

.process-btn {
  padding: 12px 30px;
  font-size: 1.1em;
}

.btn {
  border: none;
  border-radius: 4px;
  padding: 8px 16px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-primary {
  background-color: #2c5aa0;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #1a3d7a;
}

.btn-success {
  background-color: #28a745;
  color: white;
}

.btn-success:hover:not(:disabled) {
  background-color: #218838;
}
</style>
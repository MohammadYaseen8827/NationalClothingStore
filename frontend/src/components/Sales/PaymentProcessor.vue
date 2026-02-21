<script setup lang="ts">
import { ref, computed } from 'vue'

const props = defineProps<{
  totalAmount: number
}>()

const emit = defineEmits<{
  (e: 'paymentComplete', paymentData: PaymentData): void
  (e: 'cancel'): void
}>()

// Form state
const paymentMethod = ref('cash')
const cashAmount = ref(props.totalAmount)
const cardNumber = ref('')
const cardExpiry = ref('')
const cardCvc = ref('')
const giftCardNumber = ref('')
const isProcessing = ref(false)

// Computed properties
const changeDue = computed(() => {
  if (paymentMethod.value === 'cash') {
    return Math.max(0, cashAmount.value - props.totalAmount)
  }
  return 0
})

const isValid = computed(() => {
  if (paymentMethod.value === 'cash') {
    return cashAmount.value >= props.totalAmount
  }
  if (paymentMethod.value === 'credit_card' || paymentMethod.value === 'debit_card') {
    return cardNumber.value.length >= 16 && cardExpiry.value.length === 5 && cardCvc.value.length >= 3
  }
  if (paymentMethod.value === 'gift_card') {
    return giftCardNumber.value.length >= 10
  }
  return false
})

// Methods
const formatCardNumber = (value: string) => {
  const cleaned = value.replace(/\D/g, '')
  const match = cleaned.match(/^(\d{0,4})(\d{0,4})(\d{0,4})(\d{0,4})$/)
  if (match) {
    return match.slice(1).filter(Boolean).join(' ')
  }
  return cleaned
}

const formatExpiry = (value: string) => {
  const cleaned = value.replace(/\D/g, '')
  if (cleaned.length >= 2) {
    return cleaned.substring(0, 2) + '/' + cleaned.substring(2, 4)
  }
  return cleaned
}

const processPayment = async () => {
  if (!isValid.value) return
  
  isProcessing.value = true
  
  try {
    // Simulate payment processing delay
    await new Promise(resolve => setTimeout(resolve, 1500))
    
    const paymentData: PaymentData = {
      method: paymentMethod.value,
      amount: props.totalAmount,
      cashAmount: paymentMethod.value === 'cash' ? cashAmount.value : undefined,
      change: changeDue.value,
      cardNumber: paymentMethod.value.includes('card') ? cardNumber.value : undefined,
      cardExpiry: paymentMethod.value.includes('card') ? cardExpiry.value : undefined,
      cardCvc: paymentMethod.value.includes('card') ? cardCvc.value : undefined,
      giftCardNumber: paymentMethod.value === 'gift_card' ? giftCardNumber.value : undefined,
      transactionId: `TXN${Date.now()}`
    }
    
    emit('paymentComplete', paymentData)
  } catch (error) {
    console.error('Payment processing error:', error)
    // Handle error appropriately
  } finally {
    isProcessing.value = false
  }
}

const cancelPayment = () => {
  emit('cancel')
}
</script>

<template>
  <div class="payment-processor">
    <div class="payment-header">
      <h2>Payment Processing</h2>
      <button @click="cancelPayment" class="close-btn">×</button>
    </div>
    
    <div class="payment-content">
      <div class="amount-display">
        <h3>Total Amount Due</h3>
        <div class="amount">${{ totalAmount.toFixed(2) }}</div>
      </div>
      
      <div class="payment-methods">
        <h3>Select Payment Method</h3>
        
        <div class="method-selector">
          <label class="method-option">
            <input 
              type="radio" 
              v-model="paymentMethod" 
              value="cash"
              class="method-radio"
            />
            <div class="method-content">
              <div class="method-icon">💵</div>
              <div class="method-info">
                <h4>Cash</h4>
                <p>Pay with cash</p>
              </div>
            </div>
          </label>
          
          <label class="method-option">
            <input 
              type="radio" 
              v-model="paymentMethod" 
              value="credit_card"
              class="method-radio"
            />
            <div class="method-content">
              <div class="method-icon">💳</div>
              <div class="method-info">
                <h4>Credit Card</h4>
                <p>All major credit cards accepted</p>
              </div>
            </div>
          </label>
          
          <label class="method-option">
            <input 
              type="radio" 
              v-model="paymentMethod" 
              value="debit_card"
              class="method-radio"
            />
            <div class="method-content">
              <div class="method-icon">💳</div>
              <div class="method-info">
                <h4>Debit Card</h4>
                <p>Debit card payment</p>
              </div>
            </div>
          </label>
          
          <label class="method-option">
            <input 
              type="radio" 
              v-model="paymentMethod" 
              value="gift_card"
              class="method-radio"
            />
            <div class="method-content">
              <div class="method-icon">🎁</div>
              <div class="method-info">
                <h4>Gift Card</h4>
                <p>Store gift card</p>
              </div>
            </div>
          </label>
        </div>
      </div>
      
      <!-- Cash Payment Form -->
      <div v-if="paymentMethod === 'cash'" class="payment-form">
        <h3>Cash Payment</h3>
        <div class="form-group">
          <label>Cash Amount Received:</label>
          <input
            v-model.number="cashAmount"
            type="number"
            step="0.01"
            min="0"
            class="form-input"
            placeholder="Enter amount received"
          />
        </div>
        <div v-if="changeDue > 0" class="change-display">
          <strong>Change Due: ${{ changeDue.toFixed(2) }}</strong>
        </div>
      </div>
      
      <!-- Card Payment Form -->
      <div v-else-if="paymentMethod.includes('card')" class="payment-form">
        <h3>Card Payment</h3>
        <div class="form-group">
          <label>Card Number:</label>
          <input
            v-model="cardNumber"
            @input="cardNumber = formatCardNumber(cardNumber)"
            type="text"
            class="form-input"
            placeholder="1234 5678 9012 3456"
            maxlength="19"
          />
        </div>
        <div class="card-details">
          <div class="form-group half-width">
            <label>Expiry Date:</label>
            <input
              v-model="cardExpiry"
              @input="cardExpiry = formatExpiry(cardExpiry)"
              type="text"
              class="form-input"
              placeholder="MM/YY"
              maxlength="5"
            />
          </div>
          <div class="form-group half-width">
            <label>CVC:</label>
            <input
              v-model="cardCvc"
              type="password"
              class="form-input"
              placeholder="123"
              maxlength="4"
            />
          </div>
        </div>
      </div>
      
      <!-- Gift Card Form -->
      <div v-else-if="paymentMethod === 'gift_card'" class="payment-form">
        <h3>Gift Card Payment</h3>
        <div class="form-group">
          <label>Gift Card Number:</label>
          <input
            v-model="giftCardNumber"
            type="text"
            class="form-input"
            placeholder="Enter gift card number"
          />
        </div>
      </div>
      
      <div class="payment-actions">
        <button 
          @click="cancelPayment"
          class="btn btn-secondary"
          :disabled="isProcessing"
        >
          Cancel
        </button>
        <button 
          @click="processPayment"
          :disabled="!isValid || isProcessing"
          class="btn btn-primary"
        >
          {{ isProcessing ? 'Processing...' : 'Complete Payment' }}
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.payment-processor {
  background: white;
  border-radius: 8px;
  overflow: hidden;
}

.payment-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  background: #2c5aa0;
  color: white;
}

.payment-header h2 {
  margin: 0;
  font-size: 1.5em;
}

.close-btn {
  background: none;
  border: none;
  color: white;
  font-size: 2em;
  cursor: pointer;
  padding: 0;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: background-color 0.2s;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.2);
}

.payment-content {
  padding: 20px;
}

.amount-display {
  text-align: center;
  margin-bottom: 30px;
  padding: 20px;
  background: #f8f9fa;
  border-radius: 8px;
}

.amount-display h3 {
  margin: 0 0 10px 0;
  color: #666;
  font-size: 1.1em;
}

.amount {
  font-size: 2.5em;
  font-weight: bold;
  color: #2c5aa0;
  margin: 0;
}

.payment-methods h3 {
  margin: 0 0 15px 0;
  color: #333;
}

.method-selector {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 15px;
  margin-bottom: 25px;
}

.method-option {
  display: block;
  border: 2px solid #ddd;
  border-radius: 8px;
  padding: 15px;
  cursor: pointer;
  transition: all 0.2s;
}

.method-option:hover {
  border-color: #2c5aa0;
}

.method-option input:checked + .method-content {
  border-color: #2c5aa0;
  background: #e3f2fd;
}

.method-content {
  display: flex;
  align-items: center;
  gap: 15px;
}

.method-icon {
  font-size: 2em;
}

.method-info h4 {
  margin: 0 0 5px 0;
  color: #333;
}

.method-info p {
  margin: 0;
  color: #666;
  font-size: 0.9em;
}

.method-radio {
  display: none;
}

.payment-form {
  background: #f8f9fa;
  padding: 20px;
  border-radius: 8px;
  margin-bottom: 25px;
}

.payment-form h3 {
  margin: 0 0 20px 0;
  color: #333;
}

.form-group {
  margin-bottom: 20px;
}

.form-group.half-width {
  display: inline-block;
  width: calc(50% - 10px);
  margin-right: 20px;
}

.form-group.half-width:last-child {
  margin-right: 0;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: bold;
  color: #333;
}

.form-input {
  width: 100%;
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 16px;
}

.form-input:focus {
  outline: none;
  border-color: #2c5aa0;
  box-shadow: 0 0 0 2px rgba(44, 90, 160, 0.2);
}

.card-details {
  display: flex;
  gap: 15px;
}

.change-display {
  padding: 15px;
  background: #d4edda;
  border-radius: 4px;
  text-align: center;
  color: #155724;
  font-size: 1.2em;
}

.payment-actions {
  display: flex;
  justify-content: space-between;
  gap: 15px;
}

.btn {
  padding: 12px 24px;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  cursor: pointer;
  transition: all 0.2s;
  flex: 1;
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

.btn-secondary {
  background-color: #6c757d;
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background-color: #5a6268;
}
</style>

<script lang="ts">
export interface PaymentData {
  method: string
  amount: number
  cashAmount?: number
  change: number
  cardNumber?: string
  cardExpiry?: string
  cardCvc?: string
  giftCardNumber?: string
  transactionId: string
}
</script>
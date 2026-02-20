<script setup lang="ts">
import { computed } from 'vue'
import { useSalesStore } from '@/stores/salesStore'

const salesStore = useSalesStore()

// Computed properties
const cartItems = computed(() => salesStore.cartItems)
const cartTotal = computed(() => salesStore.cartTotal)
const cartItemCount = computed(() => salesStore.cartItemCount)
const subtotal = computed(() => salesStore.subtotal)
const taxAmount = computed(() => salesStore.taxAmount)
const discountTotal = computed(() => salesStore.discountTotal)

// Methods
const removeItem = (itemId: string) => {
  salesStore.removeFromCart(itemId)
}

const updateQuantity = (itemId: string, quantity: number) => {
  if (quantity > 0) {
    salesStore.updateCartItemQuantity(itemId, quantity)
  }
}

const clearCart = () => {
  if (confirm('Are you sure you want to clear the cart?')) {
    salesStore.clearCart()
  }
}
</script>

<template>
  <div class="shopping-cart">
    <div class="cart-header">
      <h2>Shopping Cart</h2>
      <button 
        v-if="cartItemCount > 0"
        @click="clearCart"
        class="btn btn-danger clear-btn"
      >
        Clear Cart
      </button>
    </div>

    <div class="cart-content">
      <div v-if="cartItemCount === 0" class="empty-cart">
        <div class="empty-icon">🛒</div>
        <p>Your cart is empty</p>
        <p class="help-text">Add products to get started</p>
      </div>

      <div v-else class="cart-items">
        <div 
          v-for="item in cartItems" 
          :key="item.id"
          class="cart-item"
        >
          <div class="item-info">
            <h3 class="item-name">{{ item.productName }}</h3>
            <p class="item-sku">SKU: {{ item.sku }}</p>
            <div class="item-price">
              <span class="unit-price">${{ item.unitPrice.toFixed(2) }}</span>
              <span v-if="item.discountPercent > 0" class="discount-badge">
                {{ item.discountPercent }}% off
              </span>
            </div>
          </div>

          <div class="item-controls">
            <div class="quantity-control">
              <button 
                @click="updateQuantity(item.id, item.quantity - 1)"
                class="qty-btn"
              >
                -
              </button>
              <input
                :value="item.quantity"
                @change="updateQuantity(item.id, parseInt(($event.target as HTMLInputElement).value) || 1)"
                type="number"
                min="1"
                class="qty-input"
              />
              <button 
                @click="updateQuantity(item.id, item.quantity + 1)"
                class="qty-btn"
              >
                +
              </button>
            </div>

            <div class="item-total">
              <span class="total-label">Total:</span>
              <span class="total-amount">${{ item.total.toFixed(2) }}</span>
            </div>

            <button 
              @click="removeItem(item.id)"
              class="btn btn-outline-danger remove-btn"
              title="Remove item"
            >
              ×
            </button>
          </div>
        </div>
      </div>
    </div>

    <div v-if="cartItemCount > 0" class="cart-summary">
      <div class="summary-row">
        <span>Subtotal ({{ cartItemCount }} items):</span>
        <span>${{ subtotal.toFixed(2) }}</span>
      </div>
      
      <div v-if="discountTotal > 0" class="summary-row discount-row">
        <span>Discount:</span>
        <span>-${{ discountTotal.toFixed(2) }}</span>
      </div>
      
      <div class="summary-row tax-row">
        <span>Tax (8.5%):</span>
        <span>${{ taxAmount.toFixed(2) }}</span>
      </div>
      
      <div class="summary-row total-row">
        <span>Total:</span>
        <span class="total-amount">${{ cartTotal.toFixed(2) }}</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.shopping-cart {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  display: flex;
  flex-direction: column;
  height: 100%;
  max-height: 800px;
}

.cart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid #eee;
}

.cart-header h2 {
  margin: 0;
  color: #333;
}

.clear-btn {
  padding: 6px 12px;
  font-size: 0.9em;
}

.cart-content {
  flex: 1;
  overflow-y: auto;
  padding: 0 20px;
}

.empty-cart {
  text-align: center;
  padding: 60px 20px;
  color: #666;
}

.empty-icon {
  font-size: 3em;
  margin-bottom: 15px;
}

.empty-cart p {
  margin: 10px 0;
}

.help-text {
  font-size: 0.9em;
  color: #999;
}

.cart-items {
  padding: 10px 0;
}

.cart-item {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 15px 0;
  border-bottom: 1px solid #f0f0f0;
}

.cart-item:last-child {
  border-bottom: none;
}

.item-info {
  flex: 1;
  min-width: 0;
}

.item-name {
  margin: 0 0 5px 0;
  color: #333;
  font-size: 1.1em;
}

.item-sku {
  margin: 0 0 8px 0;
  color: #666;
  font-size: 0.9em;
}

.item-price {
  display: flex;
  align-items: center;
  gap: 10px;
}

.unit-price {
  font-weight: bold;
  color: #2c5aa0;
}

.discount-badge {
  background: #ffc107;
  color: #212529;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 0.8em;
  font-weight: bold;
}

.item-controls {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 10px;
  min-width: 120px;
}

.quantity-control {
  display: flex;
  align-items: center;
  gap: 5px;
}

.qty-btn {
  width: 30px;
  height: 30px;
  border: 1px solid #ddd;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
}

.qty-btn:hover {
  background: #f8f9fa;
}

.qty-input {
  width: 50px;
  height: 30px;
  text-align: center;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.item-total {
  text-align: right;
}

.total-label {
  display: block;
  font-size: 0.9em;
  color: #666;
}

.total-amount {
  display: block;
  font-weight: bold;
  font-size: 1.1em;
  color: #2c5aa0;
}

.remove-btn {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  padding: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 16px;
  font-weight: bold;
}

.cart-summary {
  border-top: 1px solid #eee;
  padding: 20px;
  background: #f8f9fa;
  border-radius: 0 0 8px 8px;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 10px;
  font-size: 1.1em;
}

.summary-row:last-child {
  margin-bottom: 0;
}

.discount-row {
  color: #28a745;
}

.tax-row {
  color: #666;
}

.total-row {
  font-weight: bold;
  font-size: 1.3em;
  padding-top: 10px;
  border-top: 1px solid #ddd;
  margin-top: 10px;
}

.total-row .total-amount {
  color: #2c5aa0;
  font-size: 1.3em;
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

.btn-danger {
  background-color: #dc3545;
  color: white;
}

.btn-danger:hover:not(:disabled) {
  background-color: #c82333;
}

.btn-outline-danger {
  border: 1px solid #dc3545;
  color: #dc3545;
  background: transparent;
}

.btn-outline-danger:hover {
  background-color: #dc3545;
  color: white;
}
</style>
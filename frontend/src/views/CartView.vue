<script setup lang="ts">
import { ref, computed } from 'vue'
import { RouterLink } from 'vue-router'

// Types
interface CartItem {
  id: string
  name: string
  price: number
  originalPrice?: number
  image: string
  color: string
  size: string
  quantity: number
  stock: number
}

interface UndoAction {
  type: 'remove' | 'update'
  item: CartItem
  previousQuantity?: number
}

// State
const cartItems = ref<CartItem[]>([
  {
    id: '1',
    name: 'Traditional Sarafan Dress',
    price: 459,
    originalPrice: 550,
    image: 'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=400&h=500&fit=crop',
    color: 'Burgundy',
    size: 'M',
    quantity: 1,
    stock: 15
  },
  {
    id: '2',
    name: 'Folk Patterned Shawl',
    price: 129,
    image: 'https://images.unsplash.com/photo-1601924994987-69e26d50dc26?w=400&h=500&fit=crop',
    color: 'Forest Green',
    size: 'One Size',
    quantity: 2,
    stock: 45
  }
])

const undoStack = ref<UndoAction[]>([])
const showUndoNotification = ref(false)
const undoMessage = ref('')

// Computed
const subtotal = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + item.price * item.quantity, 0)
})

const originalTotal = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + (item.originalPrice || item.price) * item.quantity, 0)
})

const discount = computed(() => originalTotal.value - subtotal.value)

const shipping = computed(() => subtotal.value >= 200 ? 0 : 15)

const total = computed(() => subtotal.value + shipping.value)

const itemCount = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + item.quantity, 0)
})

const canUndo = computed(() => undoStack.value.length > 0)

// Methods
const updateQuantity = (itemId: string, delta: number) => {
  const item = cartItems.value.find(i => i.id === itemId)
  if (item) {
    const newQty = item.quantity + delta
    if (newQty >= 1 && newQty <= item.stock) {
      // Save to undo stack
      undoStack.value.push({
        type: 'update',
        item: { ...item },
        previousQuantity: item.quantity
      })
      item.quantity = newQty
    }
  }
}

const removeItem = (itemId: string) => {
  const index = cartItems.value.findIndex(i => i.id === itemId)
  if (index > -1) {
    const removedItem = cartItems.value[index] as CartItem
    undoStack.value.push({
      type: 'remove',
      item: JSON.parse(JSON.stringify(removedItem)) as CartItem
    })
    cartItems.value.splice(index, 1)
    undoMessage.value = `Removed ${removedItem.name} from cart`
    showUndoNotification.value = true
    setTimeout(() => { showUndoNotification.value = false }, 4000)
  }
}

const undo = () => {
  const action = undoStack.value.pop()
  if (!action) return

  if (action.type === 'remove') {
    // Restore removed item
    cartItems.value.push(action.item)
    undoMessage.value = `Restored ${action.item.name}`
  } else if (action.type === 'update' && action.previousQuantity !== undefined) {
    // Restore previous quantity
    const item = cartItems.value.find(i => i.id === action.item.id)
    if (item) {
      item.quantity = action.previousQuantity
      undoMessage.value = `Restored quantity for ${item.name}`
    }
  }

  showUndoNotification.value = true
  setTimeout(() => { showUndoNotification.value = false }, 3000)
}

const proceedToCheckout = () => {
  console.log('Proceeding to checkout...')
}
</script>

<template>
  <div class="cart-page">
    <!-- Breadcrumb -->
    <div class="breadcrumb">
      <div class="container">
        <RouterLink to="/">Home</RouterLink>
        <span class="breadcrumb-separator">/</span>
        <span class="breadcrumb-current">Shopping Cart</span>
      </div>
    </div>

    <div class="cart-content">
      <div class="container">
        <!-- Page Header -->
        <div class="cart-header">
          <h1 class="cart-title">Shopping Cart</h1>
          <p class="cart-count">{{ itemCount }} {{ itemCount === 1 ? 'item' : 'items' }}</p>
        </div>

        <!-- Undo Notification -->
        <div v-if="showUndoNotification" class="undo-notification">
          <p>{{ undoMessage }}</p>
          <button v-if="canUndo" class="undo-btn" @click="undo" aria-label="Undo last action">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
              <path d="M3 7v6h6"></path>
              <path d="M21 17a9 9 0 00-9-9 9 9 0 00-6 2.3L3 13"></path>
            </svg>
            Undo
          </button>
        </div>

        <!-- Empty Cart -->
        <div v-if="cartItems.length === 0" class="cart-empty">
          <svg xmlns="http://www.w3.org/2000/svg" width="80" height="80" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1">
            <circle cx="9" cy="21" r="1"></circle>
            <circle cx="20" cy="21" r="1"></circle>
            <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
          </svg>
          <h2>Your cart is empty</h2>
          <p>Looks like you haven't added anything to your cart yet.</p>
          <RouterLink to="/products" class="btn btn-primary btn-lg">
            Continue Shopping
          </RouterLink>
        </div>

        <!-- Cart Layout -->
        <div v-else class="cart-layout">
          <!-- Cart Items -->
          <div class="cart-items">
            <div class="cart-items-header">
              <span class="header-product">Product</span>
              <span class="header-quantity">Quantity</span>
              <span class="header-total">Total</span>
            </div>

            <div 
              v-for="item in cartItems" 
              :key="item.id" 
              class="cart-item"
            >
              <div class="item-product">
                <RouterLink :to="`/products/${item.id}`" class="item-image">
                  <img :src="item.image" :alt="item.name" />
                </RouterLink>
                <div class="item-details">
                  <RouterLink :to="`/products/${item.id}`" class="item-name">
                    {{ item.name }}
                  </RouterLink>
                  <div class="item-variants">
                    <span>Color: {{ item.color }}</span>
                    <span class="variant-divider">|</span>
                    <span>Size: {{ item.size }}</span>
                  </div>
                  <div class="item-price">
                    <span class="current-price">${{ item.price.toLocaleString() }}</span>
                    <span v-if="item.originalPrice" class="original-price">
                      ${{ item.originalPrice.toLocaleString() }}
                    </span>
                  </div>
                </div>
              </div>

              <div class="item-quantity">
                <div class="quantity-selector">
                  <button 
                    class="qty-btn" 
                    @click="updateQuantity(item.id, -1)"
                    :disabled="item.quantity <= 1"
                    :aria-label="`Decrease quantity for ${item.name}`"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
                      <line x1="5" y1="12" x2="19" y2="12"></line>
                    </svg>
                  </button>
                  <span class="qty-value" :aria-label="`Quantity: ${item.quantity}`">{{ item.quantity }}</span>
                  <button 
                    class="qty-btn" 
                    @click="updateQuantity(item.id, 1)"
                    :disabled="item.quantity >= item.stock"
                    :aria-label="`Increase quantity for ${item.name}`"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
                      <line x1="12" y1="5" x2="12" y2="19"></line>
                      <line x1="5" y1="12" x2="19" y2="12"></line>
                    </svg>
                  </button>
                </div>
                <button class="remove-btn" @click="removeItem(item.id)" :aria-label="`Remove ${item.name} from cart`">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
                    <polyline points="3 6 5 6 21 6"></polyline>
                    <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                  </svg>
                  Remove
                </button>
              </div>

              <div class="item-total">
                <span class="total-price">${{ (item.price * item.quantity).toLocaleString() }}</span>
              </div>
            </div>

            <!-- Continue Shopping -->
            <RouterLink to="/products" class="continue-shopping">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <line x1="19" y1="12" x2="5" y2="12"></line>
                <polyline points="12 19 5 12 12 5"></polyline>
              </svg>
              Continue Shopping
            </RouterLink>
          </div>

          <!-- Order Summary -->
          <div class="order-summary">
            <h3 class="summary-title">Order Summary</h3>
            
            <div class="summary-row">
              <span>Subtotal</span>
              <span>${{ subtotal.toLocaleString() }}</span>
            </div>
            
            <div v-if="discount > 0" class="summary-row discount">
              <span>Discount</span>
              <span>-${{ discount.toLocaleString() }}</span>
            </div>
            
            <div class="summary-row">
              <span>Shipping</span>
              <span>
                {{ shipping === 0 ? 'Free' : `$${shipping}` }}
              </span>
            </div>

            <div v-if="subtotal < 200" class="free-shipping-note">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10"></circle>
                <line x1="12" y1="16" x2="12" y2="12"></line>
                <line x1="12" y1="8" x2="12.01" y2="8"></line>
              </svg>
              Add ${{ (200 - subtotal).toLocaleString() }} more for free shipping
            </div>

            <div class="summary-divider"></div>

            <div class="summary-row total">
              <span>Total</span>
              <span>${{ total.toLocaleString() }}</span>
            </div>

            <button class="btn btn-primary btn-lg checkout-btn" @click="proceedToCheckout">
              Proceed to Checkout
            </button>

            <!-- Trust Badges -->
            <div class="trust-badges">
              <div class="trust-item">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <rect x="3" y="11" width="18" height="11" rx="2" ry="2"></rect>
                  <path d="M7 11V7a5 5 0 0 1 10 0v4"></path>
                </svg>
                <span>Secure Checkout</span>
              </div>
              <div class="trust-item">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path>
                </svg>
                <span>Buyer Protection</span>
              </div>
              <div class="trust-item">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path>
                  <circle cx="12" cy="10" r="3"></circle>
                </svg>
                <span>Free Returns</span>
              </div>
            </div>

            <!-- Promo Code -->
            <div class="promo-section">
              <label class="promo-label">Promo Code</label>
              <div class="promo-input">
                <input type="text" placeholder="Enter code" />
                <button class="apply-btn">Apply</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* ========================================
   BREADCRUMB
   ======================================== */
.breadcrumb {
  padding: var(--space-4) 0;
  background: var(--color-cream);
}

.breadcrumb .container {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  font-size: var(--text-sm);
}

.breadcrumb a {
  color: var(--color-warm-gray);
  text-decoration: none;
  transition: color var(--transition-fast);
}

.breadcrumb a:hover {
  color: var(--color-burgundy);
}

.breadcrumb-separator {
  color: var(--color-warm-gray-light);
}

.breadcrumb-current {
  color: var(--color-text);
}

/* ========================================
   PAGE HEADER
   ======================================== */
.cart-content {
  padding: var(--space-10) 0 var(--space-16);
  background: var(--color-background);
  min-height: 60vh;
}

.cart-header {
  margin-bottom: var(--space-10);
}

.cart-title {
  font-family: var(--font-heading);
  font-size: var(--text-4xl);
  color: var(--color-charcoal);
  margin-bottom: var(--space-2);
}

.cart-count {
  font-size: var(--text-lg);
  color: var(--color-warm-gray);
}

/* ========================================
   EMPTY CART
   ======================================== */
.cart-empty {
  text-align: center;
  padding: var(--space-16) 0;
}

.cart-empty svg {
  color: var(--color-warm-gray-light);
  margin-bottom: var(--space-6);
}

.cart-empty h2 {
  font-family: var(--font-heading);
  font-size: var(--text-2xl);
  color: var(--color-charcoal);
  margin-bottom: var(--space-2);
}

.cart-empty p {
  color: var(--color-warm-gray);
  margin-bottom: var(--space-8);
}

/* ========================================
   CART LAYOUT
   ======================================== */
.cart-layout {
  display: grid;
  grid-template-columns: 1fr 400px;
  gap: var(--space-10);
  align-items: start;
}

/* ========================================
   CART ITEMS
   ======================================== */
.cart-items-header {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr;
  gap: var(--space-4);
  padding: var(--space-4);
  border-bottom: 1px solid var(--color-border-light);
  font-size: var(--text-sm);
  color: var(--color-warm-gray);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.cart-item {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr;
  gap: var(--space-4);
  padding: var(--space-6) var(--space-4);
  border-bottom: 1px solid var(--color-border-light);
  align-items: center;
}

.item-product {
  display: flex;
  gap: var(--space-4);
}

.item-image {
  width: 120px;
  height: 150px;
  border-radius: var(--radius-lg);
  overflow: hidden;
  flex-shrink: 0;
}

.item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.item-details {
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.item-name {
  font-family: var(--font-heading);
  font-size: var(--text-lg);
  color: var(--color-charcoal);
  text-decoration: none;
  margin-bottom: var(--space-2);
  transition: color var(--transition-fast);
}

.item-name:hover {
  color: var(--color-burgundy);
}

.item-variants {
  font-size: var(--text-sm);
  color: var(--color-warm-gray);
  margin-bottom: var(--space-2);
}

.variant-divider {
  margin: 0 var(--space-2);
  color: var(--color-border);
}

.item-price {
  display: flex;
  align-items: center;
  gap: var(--space-2);
}

.current-price {
  font-family: var(--font-heading);
  font-size: var(--text-lg);
  color: var(--color-burgundy);
}

.original-price {
  font-size: var(--text-sm);
  color: var(--color-warm-gray-light);
  text-decoration: line-through;
}

/* Quantity */
.item-quantity {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.quantity-selector {
  display: inline-flex;
  align-items: center;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
}

.qty-btn {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: none;
  cursor: pointer;
  color: var(--color-text);
  transition: all var(--transition-fast);
}

.qty-btn:hover:not(:disabled) {
  background: var(--color-background-soft);
  color: var(--color-burgundy);
}

.qty-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.qty-value {
  min-width: 40px;
  text-align: center;
  font-size: var(--text-base);
  font-weight: var(--font-medium);
}

.remove-btn {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  background: none;
  border: none;
  font-size: var(--text-sm);
  color: var(--color-warm-gray);
  cursor: pointer;
  transition: color var(--transition-fast);
  width: fit-content;
}

.remove-btn:hover {
  color: var(--color-burgundy);
}

/* Total */
.item-total {
  text-align: right;
}

.total-price {
  font-family: var(--font-heading);
  font-size: var(--text-xl);
  color: var(--color-charcoal);
  font-weight: var(--font-semibold);
}

/* Continue Shopping */
.continue-shopping {
  display: inline-flex;
  align-items: center;
  gap: var(--space-2);
  margin-top: var(--space-6);
  color: var(--color-burgundy);
  text-decoration: none;
  font-weight: var(--font-medium);
  transition: all var(--transition-fast);
}

.continue-shopping:hover {
  gap: var(--space-3);
}

/* ========================================
   ORDER SUMMARY
   ======================================== */
.order-summary {
  background: var(--color-cream);
  border-radius: var(--radius-xl);
  padding: var(--space-8);
  position: sticky;
  top: 100px;
}

.summary-title {
  font-family: var(--font-heading);
  font-size: var(--text-xl);
  color: var(--color-charcoal);
  margin-bottom: var(--space-6);
}

.summary-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--space-3);
  font-size: var(--text-base);
  color: var(--color-text);
}

.summary-row.discount {
  color: var(--color-forest-green);
}

.free-shipping-note {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-3);
  background: rgba(114, 47, 55, 0.08);
  border-radius: var(--radius-md);
  font-size: var(--text-sm);
  color: var(--color-burgundy);
  margin: var(--space-4) 0;
}

.summary-divider {
  height: 1px;
  background: var(--color-border-light);
  margin: var(--space-6) 0;
}

.summary-row.total {
  font-size: var(--text-xl);
  font-weight: var(--font-semibold);
  color: var(--color-charcoal);
}

.checkout-btn {
  width: 100%;
  margin-top: var(--space-6);
}

/* Trust Badges */
.trust-badges {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
  margin-top: var(--space-8);
  padding-top: var(--space-6);
  border-top: 1px solid var(--color-border-light);
}

.trust-item {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  font-size: var(--text-sm);
  color: var(--color-warm-gray);
}

.trust-item svg {
  color: var(--color-burgundy);
  flex-shrink: 0;
}

/* Promo Section */
.promo-section {
  margin-top: var(--space-6);
  padding-top: var(--space-6);
  border-top: 1px solid var(--color-border-light);
}

.promo-label {
  display: block;
  font-size: var(--text-sm);
  color: var(--color-text);
  margin-bottom: var(--space-2);
}

.promo-input {
  display: flex;
  gap: var(--space-2);
}

.promo-input input {
  flex: 1;
  padding: var(--space-3);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  font-size: var(--text-sm);
}

.promo-input .apply-btn {
  padding: var(--space-3) var(--space-4);
  background: var(--color-charcoal);
  color: var(--vt-c-white);
  border: none;
  border-radius: var(--radius-md);
  font-size: var(--text-sm);
  cursor: pointer;
  transition: background var(--transition-fast);
}

.promo-input .apply-btn:hover {
  background: var(--color-burgundy);
}

/* ========================================
   RESPONSIVE
   ======================================== */
@media (max-width: 1024px) {
  .cart-layout {
    grid-template-columns: 1fr;
  }
  
  .order-summary {
    position: static;
  }
}

@media (max-width: 640px) {
  .cart-items-header {
    display: none;
  }
  
  .cart-item {
    grid-template-columns: 1fr;
    gap: var(--space-4);
  }
  
  .item-quantity {
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
  }
  
  .item-total {
    text-align: left;
    padding-top: var(--space-3);
    border-top: 1px solid var(--color-border-light);
  }
}

/* Undo Notification */
.undo-notification {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: var(--space-4);
  padding: var(--space-3) var(--space-4);
  margin-bottom: var(--space-6);
  background: rgba(27, 77, 62, 0.1);
  border: 1px solid var(--color-forest-green);
  border-left: 4px solid var(--color-forest-green);
  border-radius: var(--radius-md);
  animation: slideUp 0.3s ease forwards;
}

.undo-notification p {
  margin: 0;
  color: var(--color-forest-green);
  font-size: var(--text-sm);
  font-weight: var(--font-medium);
}

.undo-btn {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-2) var(--space-3);
  background: var(--color-forest-green);
  color: white;
  border: none;
  border-radius: var(--radius-sm);
  font-size: var(--text-sm);
  font-weight: var(--font-medium);
  cursor: pointer;
  transition: all var(--transition-fast);
  white-space: nowrap;
}

.undo-btn:hover {
  background: var(--color-forest-green-deep);
  transform: translateX(-2px);
}

.undo-btn:focus-visible {
  outline: 2px solid var(--color-forest-green);
  outline-offset: 2px;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>

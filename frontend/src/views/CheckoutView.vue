<script setup lang="ts">
import { ref, computed } from 'vue'
import { RouterLink } from 'vue-router'

// Types
interface CartItem {
  id: string
  name: string
  price: number
  image: string
  color: string
  size: string
  quantity: number
}

// State
const cartItems = ref<CartItem[]>([
  {
    id: '1',
    name: 'Traditional Sarafan Dress',
    price: 459,
    image: 'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=400&h=500&fit=crop',
    color: 'Burgundy',
    size: 'M',
    quantity: 1
  },
  {
    id: '2',
    name: 'Folk Patterned Shawl',
    price: 129,
    image: 'https://images.unsplash.com/photo-1601924994987-69e26d50dc26?w=400&h=500&fit=crop',
    color: 'Forest Green',
    size: 'One Size',
    quantity: 2
  }
])

const currentStep = ref(1)
const processingPayment = ref(false)
const promoCode = ref('')
const promoApplied = ref(false)
const promoError = ref('')
const discount = ref(0)
const validPromos = {
  'WELCOME10': 0.10,
  'SAVE20': 0.20,
  'HERITAGE': 0.15,
  'SUMMER15': 0.15,
  'VIPSALE': 0.25
}

// Form data
const contactInfo = ref({
  email: '',
  phone: ''
})

const shippingAddress = ref({
  firstName: '',
  lastName: '',
  address: '',
  apartment: '',
  city: '',
  state: '',
  zipCode: '',
  country: 'United States'
})

const billingAddress = ref({
  sameAsShipping: true,
  firstName: '',
  lastName: '',
  address: '',
  apartment: '',
  city: '',
  state: '',
  zipCode: '',
  country: 'United States'
})

const paymentMethod = ref('card')
const cardInfo = ref({
  cardNumber: '',
  expiryDate: '',
  cvv: '',
  cardName: ''
})

// Computed
const subtotal = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + item.price * item.quantity, 0)
})

const shipping = computed(() => subtotal.value >= 200 ? 0 : 15)
const discountAmount = computed(() => Math.round(subtotal.value * discount.value))
const subtotalAfterDiscount = computed(() => subtotal.value - discountAmount.value)
const tax = computed(() => Math.round(subtotalAfterDiscount.value * 0.08))
const total = computed(() => subtotalAfterDiscount.value + shipping.value + tax.value)

// Methods
const nextStep = () => {
  if (currentStep.value < 3) {
    currentStep.value++
  }
}

const prevStep = () => {
  if (currentStep.value > 1) {
    currentStep.value--
  }
}

const applyPromo = () => {
  promoError.value = ''
  
  if (!promoCode.value.trim()) {
    promoError.value = 'Please enter a promo code'
    return
  }
  
  const code = promoCode.value.toUpperCase()
  const promoRate = (validPromos as Record<string, number>)[code]
  
  if (!promoRate) {
    promoError.value = 'Invalid promo code. Try: WELCOME10, SAVE20, HERITAGE, SUMMER15, VIPSALE'
    return
  }
  
  discount.value = promoRate
  promoApplied.value = true
  promoError.value = `Promo code applied! ${Math.round(promoRate * 100)}% discount`
}

const removePromo = () => {
  promoCode.value = ''
  discount.value = 0
  promoApplied.value = false
  promoError.value = ''
}

const processPayment = async () => {
  processingPayment.value = true
  // Simulate payment processing
  await new Promise(resolve => setTimeout(resolve, 2000))
  processingPayment.value = false
  alert('Order placed successfully!')
}
</script>

<template>
  <div class="checkout-page">
    <!-- Breadcrumb -->
    <div class="breadcrumb">
      <div class="container">
        <RouterLink to="/">Home</RouterLink>
        <span class="breadcrumb-separator">/</span>
        <RouterLink to="/cart">Cart</RouterLink>
        <span class="breadcrumb-separator">/</span>
        <span class="breadcrumb-current">Checkout</span>
      </div>
    </div>

    <div class="checkout-content">
      <div class="container">
        <!-- Progress Steps -->
        <div class="checkout-progress">
          <div class="progress-step" :class="{ 'progress-step--active': currentStep >= 1, 'progress-step--complete': currentStep > 1 }">
            <div class="step-number">1</div>
            <div class="step-label">Information</div>
          </div>
          <div class="progress-line" :class="{ 'progress-line--active': currentStep > 1 }"></div>
          <div class="progress-step" :class="{ 'progress-step--active': currentStep >= 2, 'progress-step--complete': currentStep > 2 }">
            <div class="step-number">2</div>
            <div class="step-label">Shipping</div>
          </div>
          <div class="progress-line" :class="{ 'progress-line--active': currentStep > 2 }"></div>
          <div class="progress-step" :class="{ 'progress-step--active': currentStep >= 3 }">
            <div class="step-number">3</div>
            <div class="step-label">Payment</div>
          </div>
        </div>

        <div class="checkout-layout">
          <!-- Checkout Form -->
          <div class="checkout-form">
            <!-- Step 1: Contact & Shipping -->
            <div v-if="currentStep === 1" class="checkout-step">
              <!-- Contact Information -->
              <div class="form-section">
                <h2 class="section-title">Contact Information</h2>
                <div class="form-grid">
                  <div class="form-group full-width">
                    <label>Email Address</label>
                    <input type="email" v-model="contactInfo.email" placeholder="your@email.com" />
                  </div>
                  <div class="form-group full-width">
                    <label>Phone Number</label>
                    <input type="tel" v-model="contactInfo.phone" placeholder="(555) 123-4567" />
                  </div>
                </div>
              </div>

              <!-- Shipping Address -->
              <div class="form-section">
                <h2 class="section-title">Shipping Address</h2>
                <div class="form-grid">
                  <div class="form-group">
                    <label>First Name</label>
                    <input type="text" v-model="shippingAddress.firstName" placeholder="John" />
                  </div>
                  <div class="form-group">
                    <label>Last Name</label>
                    <input type="text" v-model="shippingAddress.lastName" placeholder="Doe" />
                  </div>
                  <div class="form-group full-width">
                    <label>Street Address</label>
                    <input type="text" v-model="shippingAddress.address" placeholder="123 Main Street" />
                  </div>
                  <div class="form-group full-width">
                    <label>Apartment, suite, etc. (optional)</label>
                    <input type="text" v-model="shippingAddress.apartment" placeholder="Apt 4B" />
                  </div>
                  <div class="form-group">
                    <label>City</label>
                    <input type="text" v-model="shippingAddress.city" placeholder="New York" />
                  </div>
                  <div class="form-group">
                    <label>State</label>
                    <input type="text" v-model="shippingAddress.state" placeholder="NY" />
                  </div>
                  <div class="form-group">
                    <label>ZIP Code</label>
                    <input type="text" v-model="shippingAddress.zipCode" placeholder="10001" />
                  </div>
                  <div class="form-group">
                    <label>Country</label>
                    <select v-model="shippingAddress.country">
                      <option>United States</option>
                      <option>Canada</option>
                      <option>United Kingdom</option>
                      <option>Germany</option>
                    </select>
                  </div>
                </div>
              </div>

              <div class="form-actions">
                <RouterLink to="/cart" class="back-link">
                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <line x1="19" y1="12" x2="5" y2="12"></line>
                    <polyline points="12 19 5 12 12 5"></polyline>
                  </svg>
                  Return to Cart
                </RouterLink>
                <button class="btn btn-primary btn-lg" @click="nextStep">
                  Continue to Shipping
                </button>
              </div>
            </div>

            <!-- Step 2: Shipping Method -->
            <div v-if="currentStep === 2" class="checkout-step">
              <div class="form-section">
                <h2 class="section-title">Shipping Address</h2>
                <div class="saved-address">
                  <p><strong>{{ shippingAddress.firstName }} {{ shippingAddress.lastName }}</strong></p>
                  <p>{{ shippingAddress.address }}</p>
                  <p v-if="shippingAddress.apartment">{{ shippingAddress.apartment }}</p>
                  <p>{{ shippingAddress.city }}, {{ shippingAddress.state }} {{ shippingAddress.zipCode }}</p>
                  <p>{{ shippingAddress.country }}</p>
                  <button class="edit-btn">Edit</button>
                </div>
              </div>

              <div class="form-section">
                <h2 class="section-title">Shipping Method</h2>
                <div class="shipping-options">
                  <label class="shipping-option">
                    <input type="radio" name="shipping" value="standard" checked />
                    <div class="option-content">
                      <div class="option-info">
                        <span class="option-name">Standard Shipping</span>
                        <span class="option-desc">5-7 business days</span>
                      </div>
                      <span class="option-price">$15.00</span>
                    </div>
                  </label>
                  <label class="shipping-option">
                    <input type="radio" name="shipping" value="express" />
                    <div class="option-content">
                      <div class="option-info">
                        <span class="option-name">Express Shipping</span>
                        <span class="option-desc">2-3 business days</span>
                      </div>
                      <span class="option-price">$29.00</span>
                    </div>
                  </label>
                  <label class="shipping-option shipping-option--free">
                    <input type="radio" name="shipping" value="free" />
                    <div class="option-content">
                      <div class="option-info">
                        <span class="option-name">Free Shipping</span>
                        <span class="option-desc">On orders over $200</span>
                      </div>
                      <span class="option-price">$0.00</span>
                    </div>
                  </label>
                </div>
              </div>

              <div class="form-actions">
                <button class="back-link" @click="prevStep">
                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <line x1="19" y1="12" x2="5" y2="12"></line>
                    <polyline points="12 19 5 12 12 5"></polyline>
                  </svg>
                  Back
                </button>
                <button class="btn btn-primary btn-lg" @click="nextStep">
                  Continue to Payment
                </button>
              </div>
            </div>

            <!-- Step 3: Payment -->
            <div v-if="currentStep === 3" class="checkout-step">
              <div class="form-section">
                <h2 class="section-title">Payment Method</h2>
                <div class="payment-methods">
                  <label class="payment-method" :class="{ 'payment-method--active': paymentMethod === 'card' }">
                    <input type="radio" v-model="paymentMethod" value="card" />
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                      <rect x="1" y="4" width="22" height="16" rx="2" ry="2"></rect>
                      <line x1="1" y1="10" x2="23" y2="10"></line>
                    </svg>
                    <span>Credit/Debit Card</span>
                  </label>
                  <label class="payment-method" :class="{ 'payment-method--active': paymentMethod === 'paypal' }">
                    <input type="radio" v-model="paymentMethod" value="paypal" />
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                      <path d="M7.076 21.337H2.47a.641.641 0 0 1-.633-.74L4.944 3.72a.771.771 0 0 1 .76-.59h5.924a.77.77 0 0 1 .76.59l2.067 16.387a.641.641 0 0 1-.633.74H7.076z"/>
                    </svg>
                    <span>PayPal</span>
                  </label>
                </div>
              </div>

              <div v-if="paymentMethod === 'card'" class="form-section">
                <div class="form-grid">
                  <div class="form-group full-width">
                    <label>Card Number</label>
                    <input type="text" v-model="cardInfo.cardNumber" placeholder="1234 5678 9012 3456" />
                  </div>
                  <div class="form-group full-width">
                    <label>Name on Card</label>
                    <input type="text" v-model="cardInfo.cardName" placeholder="John Doe" />
                  </div>
                  <div class="form-group">
                    <label>Expiry Date</label>
                    <input type="text" v-model="cardInfo.expiryDate" placeholder="MM/YY" />
                  </div>
                  <div class="form-group">
                    <label>CVV</label>
                    <input type="text" v-model="cardInfo.cvv" placeholder="123" />
                  </div>
                </div>
              </div>

              <!-- Billing Address -->
              <div class="form-section">
                <label class="checkbox-field">
                  <input type="checkbox" v-model="billingAddress.sameAsShipping" />
                  <span>Billing address same as shipping</span>
                </label>
              </div>

              <div class="form-actions">
                <button class="back-link" @click="prevStep">
                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <line x1="19" y1="12" x2="5" y2="12"></line>
                    <polyline points="12 19 5 12 12 5"></polyline>
                  </svg>
                  Back
                </button>
                <button 
                  class="btn btn-primary btn-lg" 
                  :disabled="processingPayment"
                  @click="processPayment"
                >
                  <span v-if="processingPayment" class="spinner"></span>
                  {{ processingPayment ? 'Processing...' : `Pay $${total.toLocaleString()}` }}
                </button>
              </div>
            </div>
          </div>

          <!-- Order Summary -->
          <div class="order-summary">
            <h3 class="summary-title">Order Summary</h3>
            
            <div class="summary-items">
              <div v-for="item in cartItems" :key="item.id" class="summary-item">
                <div class="item-image">
                  <img :src="item.image" :alt="item.name" />
                  <span class="item-qty">{{ item.quantity }}</span>
                </div>
                <div class="item-info">
                  <span class="item-name">{{ item.name }}</span>
                  <span class="item-variant">{{ item.color }} / {{ item.size }}</span>
                </div>
                <span class="item-price">${{ (item.price * item.quantity).toLocaleString() }}</span>
              </div>
            </div>

            <!-- Promo Code -->
            <div class="promo-section">
              <h4>Have a Promo Code?</h4>
              <div v-if="!promoApplied" class="promo-input-group">
                <input 
                  v-model="promoCode" 
                  type="text" 
                  placeholder="Enter promo code"
                  class="promo-input"
                  @keyup.enter="applyPromo"
                  aria-label="Promo code input"
                />
                <button class="btn btn-outline btn-sm" @click="applyPromo" aria-label="Apply promo code">
                  Apply
                </button>
              </div>
              <div v-if="promoApplied" class="promo-applied">
                <span class="promo-success">✓ {{ promoCode.toUpperCase() }} applied</span>
                <button class="promo-remove" @click="removePromo" aria-label="Remove promo code">Remove</button>
              </div>
              <div v-if="promoError" :class="['promo-message', promoApplied ? 'promo-success' : 'promo-error']">
                {{ promoError }}
              </div>
            </div>

            <div class="summary-totals">
              <div class="total-row">
                <span>Subtotal</span>
                <span>${{ subtotal.toLocaleString() }}</span>
              </div>
              <div v-if="discount > 0" class="total-row discount">
                <span>Discount ({{ Math.round(discount * 100) }}%)</span>
                <span>-${{ discountAmount.toLocaleString() }}</span>
              </div>
              <div class="total-row">
                <span>Subtotal after discount</span>
                <span>${{ subtotalAfterDiscount.toLocaleString() }}</span>
              </div>
              <div class="total-row">
                <span>Shipping</span>
                <span>{{ shipping === 0 ? 'Free' : `$${shipping}` }}</span>
              </div>
              <div class="total-row">
                <span>Tax</span>
                <span>${{ tax.toLocaleString() }}</span>
              </div>
              <div class="total-row total">
                <span>Total</span>
                <span>${{ total.toLocaleString() }}</span>
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
}

.breadcrumb-separator {
  color: var(--color-warm-gray-light);
}

.breadcrumb-current {
  color: var(--color-text);
}

/* ========================================
   PROGRESS STEPS
   ======================================== */
.checkout-content {
  padding: var(--space-10) 0 var(--space-16);
  background: var(--color-background);
}

.checkout-progress {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: var(--space-12);
}

.progress-step {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--space-2);
}

.step-number {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: var(--radius-full);
  background: var(--color-background-soft);
  color: var(--color-warm-gray);
  font-weight: var(--font-semibold);
  transition: all var(--transition-base);
}

.progress-step--active .step-number {
  background: var(--color-burgundy);
  color: var(--vt-c-white);
}

.progress-step--complete .step-number {
  background: var(--color-forest-green);
  color: var(--vt-c-white);
}

.step-label {
  font-size: var(--text-sm);
  color: var(--color-warm-gray);
}

.progress-step--active .step-label {
  color: var(--color-charcoal);
  font-weight: var(--font-medium);
}

.progress-line {
  width: 100px;
  height: 2px;
  background: var(--color-border-light);
  margin: 0 var(--space-4);
  margin-bottom: var(--space-6);
  transition: background var(--transition-base);
}

.progress-line--active {
  background: var(--color-burgundy);
}

/* ========================================
   LAYOUT
   ======================================== */
.checkout-layout {
  display: grid;
  grid-template-columns: 1fr 400px;
  gap: var(--space-12);
  align-items: start;
}

/* ========================================
   CHECKOUT FORM
   ======================================== */
.checkout-step {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.form-section {
  margin-bottom: var(--space-8);
}

.section-title {
  font-family: var(--font-heading);
  font-size: var(--text-xl);
  color: var(--color-charcoal);
  margin-bottom: var(--space-6);
  padding-bottom: var(--space-4);
  border-bottom: 1px solid var(--color-border-light);
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-4);
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.form-group.full-width {
  grid-column: 1 / -1;
}

.form-group label {
  font-size: var(--text-sm);
  color: var(--color-text);
  font-weight: var(--font-medium);
}

.form-group input,
.form-group select {
  padding: var(--space-3) var(--space-4);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  font-size: var(--text-base);
  transition: border-color var(--transition-fast);
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: var(--color-burgundy);
}

/* Saved Address */
.saved-address {
  padding: var(--space-4);
  background: var(--color-cream);
  border-radius: var(--radius-lg);
  position: relative;
}

.saved-address p {
  font-size: var(--text-sm);
  color: var(--color-text);
  line-height: 1.6;
}

.edit-btn {
  position: absolute;
  top: var(--space-4);
  right: var(--space-4);
  background: none;
  border: none;
  color: var(--color-burgundy);
  font-size: var(--text-sm);
  cursor: pointer;
}

/* Shipping Options */
.shipping-options {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.shipping-option {
  display: block;
  cursor: pointer;
}

.shipping-option input {
  display: none;
}

.shipping-option .option-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-4);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  transition: all var(--transition-fast);
}

.shipping-option input:checked + .option-content {
  border-color: var(--color-burgundy);
  background: rgba(114, 47, 55, 0.04);
}

.option-info {
  display: flex;
  flex-direction: column;
  gap: var(--space-1);
}

.option-name {
  font-weight: var(--font-medium);
  color: var(--color-charcoal);
}

.option-desc {
  font-size: var(--text-sm);
  color: var(--color-warm-gray);
}

.option-price {
  font-weight: var(--font-medium);
  color: var(--color-charcoal);
}

.shipping-option--free .option-price {
  color: var(--color-forest-green);
}

/* Payment Methods */
.payment-methods {
  display: flex;
  gap: var(--space-4);
  margin-bottom: var(--space-6);
}

.payment-method {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--space-3);
  padding: var(--space-4);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  cursor: pointer;
  transition: all var(--transition-fast);
}

.payment-method input {
  display: none;
}

.payment-method--active {
  border-color: var(--color-burgundy);
  background: rgba(114, 47, 55, 0.04);
}

/* Checkbox */
.checkbox-field {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  cursor: pointer;
}

.checkbox-field input {
  width: 20px;
  height: 20px;
}

/* Form Actions */
.form-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: var(--space-8);
  padding-top: var(--space-6);
  border-top: 1px solid var(--color-border-light);
}

.back-link {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  color: var(--color-warm-gray);
  text-decoration: none;
  font-size: var(--text-base);
  transition: color var(--transition-fast);
  background: none;
  border: none;
  cursor: pointer;
}

.back-link:hover {
  color: var(--color-burgundy);
}

/* ========================================
   ORDER SUMMARY
   ======================================== */
.order-summary {
  background: var(--color-cream);
  border-radius: var(--radius-xl);
  padding: var(--space-6);
  position: sticky;
  top: 100px;
}

.summary-title {
  font-family: var(--font-heading);
  font-size: var(--text-xl);
  color: var(--color-charcoal);
  margin-bottom: var(--space-6);
}

.summary-items {
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
  margin-bottom: var(--space-6);
  padding-bottom: var(--space-6);
  border-bottom: 1px solid var(--color-border-light);
}

.summary-item {
  display: flex;
  align-items: center;
  gap: var(--space-3);
}

.item-image {
  position: relative;
  width: 64px;
  height: 80px;
  border-radius: var(--radius-md);
  overflow: hidden;
  flex-shrink: 0;
}

.item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.item-qty {
  position: absolute;
  top: -6px;
  right: -6px;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-charcoal);
  color: var(--vt-c-white);
  font-size: var(--text-xs);
  border-radius: var(--radius-full);
}

.item-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: var(--space-1);
}

.item-name {
  font-size: var(--text-sm);
  color: var(--color-charcoal);
}

.item-variant {
  font-size: var(--text-xs);
  color: var(--color-warm-gray);
}

.item-price {
  font-weight: var(--font-medium);
  color: var(--color-charcoal);
}

.summary-totals {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.total-row {
  display: flex;
  justify-content: space-between;
  font-size: var(--text-base);
  color: var(--color-text);
}

.total-row.total {
  font-size: var(--text-xl);
  font-weight: var(--font-semibold);
  color: var(--color-charcoal);
  padding-top: var(--space-4);
  border-top: 1px solid var(--color-border-light);
  margin-top: var(--space-4);
}

/* Promo Code Section */
.promo-section {
  padding: var(--space-4);
  margin-bottom: var(--space-6);
  background: rgba(27, 77, 62, 0.05);
  border-radius: var(--radius-lg);
  border: 1px solid rgba(27, 77, 62, 0.2);
}

.promo-section h4 {
  font-size: var(--text-sm);
  margin: 0 0 var(--space-3) 0;
  color: var(--color-forest-green);
  font-weight: var(--font-semibold);
}

.promo-input-group {
  display: flex;
  gap: var(--space-2);
  margin-bottom: var(--space-2);
}

.promo-input {
  flex: 1;
  padding: var(--space-2) var(--space-3);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  font-size: var(--text-sm);
  transition: all var(--transition-fast);
}

.promo-input:focus {
  outline: none;
  border-color: var(--color-forest-green);
  box-shadow: 0 0 0 3px rgba(27, 77, 62, 0.1);
}

.btn-sm {
  padding: var(--space-2) var(--space-4);
  font-size: var(--text-sm);
  white-space: nowrap;
}

.promo-applied {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--space-2) var(--space-3);
  background: rgba(27, 77, 62, 0.1);
  border-radius: var(--radius-md);
  margin-bottom: var(--space-2);
}

.promo-success {
  color: var(--color-forest-green);
  font-weight: var(--font-medium);
  font-size: var(--text-sm);
}

.promo-remove {
  background: none;
  border: none;
  color: var(--color-warm-gray);
  font-size: var(--text-xs);
  cursor: pointer;
  transition: color var(--transition-fast);
}

.promo-remove:hover {
  color: var(--color-burgundy);
}

.promo-message {
  font-size: var(--text-xs);
  padding: var(--space-2);
  border-radius: var(--radius-sm);
}

.promo-error {
  color: var(--color-burgundy);
  background: rgba(114, 47, 55, 0.1);
}

.promo-message.promo-success {
  color: var(--color-forest-green);
  background: rgba(27, 77, 62, 0.1);
}

.total-row.discount {
  color: var(--color-forest-green);
  font-weight: var(--font-medium);
}

/* Spinner */
.spinner {
  display: inline-block;
  width: 20px;
  height: 20px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  border-top-color: white;
  animation: spin 1s linear infinite;
  margin-right: var(--space-2);
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* ========================================
   RESPONSIVE
   ======================================== */
@media (max-width: 1024px) {
  .checkout-layout {
    grid-template-columns: 1fr;
  }
  
  .order-summary {
    position: static;
    order: -1;
  }
  
  .checkout-progress {
    flex-wrap: wrap;
    gap: var(--space-4);
  }
  
  .progress-line {
    display: none;
  }
}

@media (max-width: 640px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
  
  .form-group.full-width {
    grid-column: 1;
  }
  
  .payment-methods {
    flex-direction: column;
  }
  
  .form-actions {
    flex-direction: column-reverse;
    gap: var(--space-4);
  }
  
  .form-actions .btn {
    width: 100%;
  }
  
  .back-link {
    justify-content: center;
  }
}
</style>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useSalesStore } from '@/stores/salesStore'
import { useProductCatalogStore } from '@/stores/productCatalogStore'
import ShoppingCart from './ShoppingCart.vue'
import PaymentProcessor from './PaymentProcessor.vue'

const salesStore = useSalesStore()
const productStore = useProductCatalogStore()

// Form state
const searchQuery = ref('')
const selectedProduct = ref<any>(null)
const quantity = ref(1)
const discountPercent = ref(0)

// UI state
const showPaymentModal = ref(false)
const isProcessing = ref(false)

// Computed properties
const filteredProducts = computed(() => {
  if (!searchQuery.value.trim()) {
    return productStore.products.slice(0, 20) // Show first 20 products
  }
  
  return productStore.products.filter(product => 
    product.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    product.sku.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    product.description?.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
})

const cartTotal = computed(() => salesStore.cartTotal)
const cartItemCount = computed(() => salesStore.cartItemCount)

// Methods
const addToCart = () => {
  if (!selectedProduct.value) return
  
  const item = {
    productId: selectedProduct.value.id,
    productName: selectedProduct.value.name,
    sku: selectedProduct.value.sku,
    unitPrice: selectedProduct.value.price,
    quantity: quantity.value,
    discountPercent: discountPercent.value
  }
  
  salesStore.addToCart(item)
  resetForm()
}

const resetForm = () => {
  selectedProduct.value = null
  quantity.value = 1
  discountPercent.value = 0
  searchQuery.value = ''
}

const processSale = async () => {
  if (salesStore.cartItems.length === 0) return
  
  showPaymentModal.value = true
}

const completeSale = async (paymentData: any) => {
  isProcessing.value = true
  try {
    await salesStore.processSale(paymentData)
    showPaymentModal.value = false
  } catch (error) {
    console.error('Error processing sale:', error)
    // Handle error (show notification, etc.)
  } finally {
    isProcessing.value = false
  }
}

const cancelSale = () => {
  salesStore.clearCart()
  showPaymentModal.value = false
}

// Lifecycle
onMounted(async () => {
  await productStore.fetchProducts({ pageNumber: 1, pageSize: 100 })
})
</script>

<template>
  <div class="pos-interface">
    <div class="header">
      <h1>Point of Sale</h1>
      <div class="cart-summary">
        <span class="item-count">{{ cartItemCount }} items</span>
        <span class="total">Total: ${{ cartTotal.toFixed(2) }}</span>
        <button 
          @click="processSale" 
          :disabled="cartItemCount === 0 || isProcessing"
          class="btn btn-primary process-btn"
        >
          {{ isProcessing ? 'Processing...' : 'Process Sale' }}
        </button>
      </div>
    </div>

    <div class="pos-content">
      <!-- Product Search Section -->
      <div class="product-search-section">
        <h2>Add Products</h2>
        <div class="search-container">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search products by name, SKU, or description..."
            class="search-input"
          />
          
          <div class="product-list" v-if="filteredProducts.length > 0">
            <div
              v-for="product in filteredProducts"
              :key="product.id"
              @click="selectedProduct = product"
              :class="{ 'selected': selectedProduct?.id === product.id }"
              class="product-item"
            >
              <div class="product-info">
                <h3>{{ product.name }}</h3>
                <p class="sku">SKU: {{ product.sku }}</p>
                <p class="price">${{ product.basePrice.toFixed(2) }}</p>
                <p class="description">{{ product.description }}</p>
              </div>
              <div class="stock-info">
                <span :class="(product.totalStock || 0) > 10 ? 'in-stock' : (product.totalStock || 0) > 0 ? 'low-stock' : 'out-of-stock'">
                  {{ (product.totalStock || 0) > 0 ? `${product.totalStock} in stock` : 'Out of stock' }}
                </span>
              </div>
            </div>
          </div>
          
          <div v-else class="no-results">
            No products found
          </div>
        </div>
      </div>

      <!-- Selected Product Details -->
      <div class="product-details" v-if="selectedProduct">
        <h2>Selected Product</h2>
        <div class="details-card">
          <h3>{{ selectedProduct.name }}</h3>
          <p><strong>SKU:</strong> {{ selectedProduct.sku }}</p>
          <p><strong>Price:</strong> ${{ selectedProduct.price.toFixed(2) }}</p>
          <p><strong>Description:</strong> {{ selectedProduct.description }}</p>
          
          <div class="form-group">
            <label>Quantity:</label>
            <input
              v-model.number="quantity"
              type="number"
              min="1"
              :max="selectedProduct.stock"
              class="quantity-input"
            />
          </div>
          
          <div class="form-group">
            <label>Discount (%):</label>
            <input
              v-model.number="discountPercent"
              type="number"
              min="0"
              max="100"
              class="discount-input"
            />
          </div>
          
          <div class="subtotal">
            <strong>Subtotal:</strong> 
            ${{ (selectedProduct.price * quantity * (1 - discountPercent / 100)).toFixed(2) }}
          </div>
          
          <button 
            @click="addToCart" 
            :disabled="quantity > selectedProduct.stock || quantity <= 0"
            class="btn btn-success add-btn"
          >
            Add to Cart
          </button>
        </div>
      </div>

      <!-- Shopping Cart -->
      <div class="shopping-cart-section">
        <ShoppingCart />
      </div>
    </div>

    <!-- Payment Modal -->
    <div v-if="showPaymentModal" class="modal-overlay">
      <div class="modal-content">
        <PaymentProcessor 
          :total-amount="cartTotal"
          @payment-complete="completeSale"
          @cancel="cancelSale"
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
.pos-interface {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 2px solid #eee;
}

.header h1 {
  margin: 0;
  color: #333;
}

.cart-summary {
  display: flex;
  align-items: center;
  gap: 20px;
}

.item-count {
  font-weight: bold;
  color: #666;
}

.total {
  font-size: 1.5em;
  font-weight: bold;
  color: #2c5aa0;
}

.process-btn {
  padding: 12px 24px;
  font-size: 1.1em;
}

.pos-content {
  display: grid;
  grid-template-columns: 1fr 350px 400px;
  gap: 20px;
}

.product-search-section,
.product-details {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.product-search-section h2,
.product-details h2 {
  margin-top: 0;
  color: #333;
  border-bottom: 1px solid #eee;
  padding-bottom: 10px;
}

.search-input {
  width: 100%;
  padding: 12px;
  border: 2px solid #ddd;
  border-radius: 6px;
  font-size: 16px;
  margin-bottom: 15px;
}

.search-input:focus {
  outline: none;
  border-color: #2c5aa0;
}

.product-list {
  max-height: 400px;
  overflow-y: auto;
  border: 1px solid #eee;
  border-radius: 6px;
}

.product-item {
  padding: 15px;
  border-bottom: 1px solid #eee;
  cursor: pointer;
  transition: background-color 0.2s;
}

.product-item:hover {
  background-color: #f8f9fa;
}

.product-item.selected {
  background-color: #e3f2fd;
  border-left: 4px solid #2c5aa0;
}

.product-info h3 {
  margin: 0 0 5px 0;
  color: #333;
}

.sku {
  margin: 0;
  color: #666;
  font-size: 0.9em;
}

.price {
  margin: 5px 0;
  font-weight: bold;
  color: #2c5aa0;
  font-size: 1.1em;
}

.description {
  margin: 5px 0 0 0;
  color: #666;
  font-size: 0.9em;
}

.stock-info span {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.8em;
  font-weight: bold;
}

.in-stock {
  background-color: #d4edda;
  color: #155724;
}

.low-stock {
  background-color: #fff3cd;
  color: #856404;
}

.out-of-stock {
  background-color: #f8d7da;
  color: #721c24;
}

.details-card {
  background: #f8f9fa;
  padding: 20px;
  border-radius: 6px;
}

.form-group {
  margin: 15px 0;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
  color: #333;
}

.quantity-input,
.discount-input {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.subtotal {
  margin: 20px 0;
  padding: 10px;
  background: white;
  border-radius: 4px;
  font-size: 1.2em;
}

.add-btn {
  width: 100%;
  padding: 12px;
  font-size: 1.1em;
}

.no-results {
  text-align: center;
  padding: 40px;
  color: #666;
  font-style: italic;
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

.modal-content {
  background: white;
  border-radius: 8px;
  max-width: 500px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
}

.btn {
  border: none;
  border-radius: 4px;
  padding: 8px 16px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.2s;
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

.shopping-cart-section {
  height: fit-content;
}
</style>
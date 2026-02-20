import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { salesService } from '@/services/salesService'
import type { CartItem, PaymentData } from '@/types/sales'

export const useSalesStore = defineStore('sales', () => {
  // State
  const cartItems = ref<CartItem[]>([])
  const currentTransaction = ref<any>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const cartItemCount = computed(() => 
    cartItems.value.reduce((total, item) => total + item.quantity, 0)
  )

  const subtotal = computed(() => 
    cartItems.value.reduce((total, item) => total + (item.unitPrice * item.quantity), 0)
  )

  const discountTotal = computed(() => 
    cartItems.value.reduce((total, item) => {
      const itemDiscount = item.unitPrice * item.quantity * (item.discountPercent / 100)
      return total + itemDiscount
    }, 0)
  )

  const taxableAmount = computed(() => subtotal.value - discountTotal.value)
  const taxAmount = computed(() => taxableAmount.value * 0.085) // 8.5% tax rate
  const cartTotal = computed(() => taxableAmount.value + taxAmount.value)

  // Actions
  function addToCart(item: Omit<CartItem, 'id' | 'total'>) {
    // Check if item already exists in cart
    const existingItem = cartItems.value.find(cartItem => 
      cartItem.productId === item.productId
    )

    if (existingItem) {
      // Update quantity if same product
      existingItem.quantity += item.quantity
      existingItem.total = calculateItemTotal(existingItem)
    } else {
      // Add new item
      const newItem: CartItem = {
        ...item,
        id: generateId(),
        total: calculateItemTotal({ ...item, id: generateId(), total: 0 })
      }
      cartItems.value.push(newItem)
    }

    clearError()
  }

  function removeFromCart(itemId: string) {
    const index = cartItems.value.findIndex(item => item.id === itemId)
    if (index !== -1) {
      cartItems.value.splice(index, 1)
    }
    clearError()
  }

  function updateCartItemQuantity(itemId: string, quantity: number) {
    const item = cartItems.value.find(cartItem => cartItem.id === itemId)
    if (item && quantity > 0) {
      item.quantity = quantity
      item.total = calculateItemTotal(item)
    }
    clearError()
  }

  function clearCart() {
    cartItems.value = []
    currentTransaction.value = null
    clearError()
  }

  async function processSale(paymentData: PaymentData) {
    if (cartItems.value.length === 0) {
      setError('Cannot process sale: Cart is empty')
      return
    }

    isLoading.value = true
    error.value = null

    try {
      const saleRequest = {
        branchId: getCurrentBranchId(), // This would come from auth/user context
        userId: getCurrentUserId(), // This would come from auth context
        customerId: null, // Optional - could be added later
        items: cartItems.value.map(item => ({
          productId: item.productId,
          productVariationId: null, // Optional
          inventoryId: '', // Would need to be determined
          quantity: item.quantity,
          unitPrice: item.unitPrice,
          discountAmount: item.unitPrice * item.quantity * (item.discountPercent / 100),
          taxRate: 8.5
        })),
        payments: [{
          paymentMethod: paymentData.method,
          amount: cartTotal.value,
          currency: 'USD',
          referenceNumber: paymentData.transactionId,
          cardLastFour: paymentData.cardNumber ? paymentData.cardNumber.slice(-4) : undefined,
          cardType: paymentData.cardNumber ? detectCardType(paymentData.cardNumber) : undefined,
          giftCardNumber: paymentData.giftCardNumber
        }]
      }

      const transaction = await salesService.processSale(saleRequest)
      currentTransaction.value = transaction
      
      // Clear cart after successful sale
      clearCart()
      
      return transaction
    } catch (err: any) {
      setError(err.message || 'Failed to process sale')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function processReturn(returnData: any) {
    isLoading.value = true
    error.value = null

    try {
      const transaction = await salesService.processReturn(returnData)
      currentTransaction.value = transaction
      return transaction
    } catch (err: any) {
      setError(err.message || 'Failed to process return')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Helper functions
  function calculateItemTotal(item: CartItem): number {
    const subtotal = item.unitPrice * item.quantity
    const discount = subtotal * (item.discountPercent / 100)
    return subtotal - discount
  }

  function generateId(): string {
    return 'cart_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9)
  }

  function getCurrentBranchId(): string {
    // This would come from auth context or user settings
    return '00000000-0000-0000-0000-000000000001'
  }

  function getCurrentUserId(): string {
    // This would come from auth context
    return '00000000-0000-0000-0000-000000000002'
  }

  function detectCardType(cardNumber: string): string {
    const cleaned = cardNumber.replace(/\s/g, '')
    
    // Visa
    if (/^4/.test(cleaned)) return 'VISA'
    // Mastercard
    if (/^5[1-5]/.test(cleaned)) return 'MASTERCARD'
    // American Express
    if (/^3[47]/.test(cleaned)) return 'AMEX'
    // Discover
    if (/^6(?:011|5)/.test(cleaned)) return 'DISCOVER'
    
    return 'UNKNOWN'
  }

  function setError(message: string) {
    error.value = message
  }

  function clearError() {
    error.value = null
  }

  return {
    // State
    cartItems,
    currentTransaction,
    isLoading,
    error,

    // Getters
    cartItemCount,
    subtotal,
    discountTotal,
    taxAmount,
    cartTotal,

    // Actions
    addToCart,
    removeFromCart,
    updateCartItemQuantity,
    clearCart,
    processSale,
    processReturn
  }
})
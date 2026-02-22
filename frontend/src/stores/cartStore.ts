import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface CartItem {
  id: string
  productId: string
  name: string
  price: number
  originalPrice?: number
  image: string
  color: string
  size: string
  quantity: number
  stock: number
}

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>([])
  const loading = ref(false)

  // Initialize from localStorage
  const initializeCart = () => {
    const saved = localStorage.getItem('cart')
    if (saved) {
      try {
        items.value = JSON.parse(saved)
      } catch (e) {
        console.error('Failed to load cart from localStorage:', e)
      }
    }
  }

  // Persist to localStorage
  const persistCart = () => {
    localStorage.setItem('cart', JSON.stringify(items.value))
  }

  // Add item
  const addItem = (item: CartItem) => {
    const existing = items.value.find(
      i => i.productId === item.productId && i.color === item.color && i.size === item.size
    )

    if (existing) {
      existing.quantity += item.quantity
    } else {
      items.value.push(item)
    }
    persistCart()
  }

  // Update quantity
  const updateQuantity = (productId: string, color: string, size: string, quantity: number) => {
    const item = items.value.find(
      i => i.productId === productId && i.color === color && i.size === size
    )
    if (item) {
      item.quantity = Math.max(1, Math.min(quantity, item.stock))
      persistCart()
    }
  }

  // Remove item
  const removeItem = (productId: string, color: string, size: string) => {
    items.value = items.value.filter(
      i => !(i.productId === productId && i.color === color && i.size === size)
    )
    persistCart()
  }

  // Clear cart
  const clearCart = () => {
    items.value = []
    persistCart()
  }

  // Computed
  const itemCount = computed(() =>
    items.value.reduce((sum, item) => sum + item.quantity, 0)
  )

  const subtotal = computed(() =>
    items.value.reduce((sum, item) => sum + item.price * item.quantity, 0)
  )

  const hasItems = computed(() => items.value.length > 0)

  return {
    items,
    loading,
    itemCount,
    subtotal,
    hasItems,
    initializeCart,
    addItem,
    updateQuantity,
    removeItem,
    clearCart
  }
})

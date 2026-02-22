import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface WishlistItem {
  id: string
  name: string
  price: number
  image: string
  rating: number
  reviews: number
  addedAt: number
}

export const useWishlistStore = defineStore('wishlist', () => {
  const items = ref<WishlistItem[]>([])

  // Initialize from localStorage
  const initializeWishlist = () => {
    const saved = localStorage.getItem('wishlist')
    if (saved) {
      try {
        items.value = JSON.parse(saved)
      } catch (e) {
        console.error('Failed to load wishlist from localStorage:', e)
      }
    }
  }

  // Persist to localStorage
  const persistWishlist = () => {
    localStorage.setItem('wishlist', JSON.stringify(items.value))
  }

  // Toggle item
  const toggleItem = (item: WishlistItem) => {
    const index = items.value.findIndex(i => i.id === item.id)
    if (index > -1) {
      items.value.splice(index, 1)
    } else {
      items.value.push({
        ...item,
        addedAt: Date.now()
      })
    }
    persistWishlist()
  }

  // Add item
  const addItem = (item: WishlistItem) => {
    if (!items.value.find(i => i.id === item.id)) {
      items.value.push({
        ...item,
        addedAt: Date.now()
      })
      persistWishlist()
    }
  }

  // Remove item
  const removeItem = (productId: string) => {
    items.value = items.value.filter(i => i.id !== productId)
    persistWishlist()
  }

  // Clear wishlist
  const clearWishlist = () => {
    items.value = []
    persistWishlist()
  }

  // Computed
  const itemCount = computed(() => items.value.length)

  const isInWishlist = (productId: string) =>
    computed(() => items.value.some(i => i.id === productId))

  return {
    items,
    itemCount,
    toggleItem,
    addItem,
    removeItem,
    clearWishlist,
    initializeWishlist,
    isInWishlist
  }
})

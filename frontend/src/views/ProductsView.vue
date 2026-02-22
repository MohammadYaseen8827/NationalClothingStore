<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter, RouterLink } from 'vue-router'
import { useCartStore } from '../stores/cartStore'
import { useWishlistStore } from '../stores/wishlistStore'

// Types
interface Product {
  id: string
  name: string
  price: number
  originalPrice?: number
  image: string
  hoverImage?: string
  category: string
  occasion: string
  rating: number
  reviews: number
  isNew: boolean
  isPremium: boolean
  isSale: boolean
  colors: string[]
  sizes: string[]
  stock: number
}

// Route & Router
const route = useRoute()
const router = useRouter()
const cartStore = useCartStore()
const wishlistStore = useWishlistStore()

// State
const loading = ref(true)
const products = ref<Product[]>([])
const isFilterOpen = ref(false)
const addingToCart = ref<string | null>(null)
const toastMessage = ref('')
const showToast = ref(false)

// Filters
const filters = ref({
  gender: route.query.gender?.toString() || '',
  category: route.query.category?.toString() || '',
  occasion: route.query.occasion?.toString() || '',
  priceMin: 0,
  priceMax: 2000,
  colors: [] as string[],
  sizes: [] as string[],
  premium: false,
  inStock: true,
  searchTerm: route.query.search?.toString() || '',
  newCollection: route.query.collection === 'new',
})

// Sorting
const sortBy = ref('newest')

// Pagination
const currentPage = ref(1)
const itemsPerPage = 12

// Filter options
const filterOptions = {
  genders: [
    { value: '', label: 'All Genders' },
    { value: 'women', label: 'Women' },
    { value: 'men', label: 'Men' },
    { value: 'unisex', label: 'Unisex' }
  ],
  categories: [
    { value: '', label: 'All Categories' },
    { value: 'dresses', label: 'Dresses' },
    { value: 'shirts', label: 'Shirts' },
    { value: 'outerwear', label: 'Outerwear' },
    { value: 'accessories', label: 'Accessories' },
    { value: 'folk', label: 'Folk Traditional' }
  ],
  occasions: [
    { value: '', label: 'All Occasions' },
    { value: 'daily', label: 'Daily Wear' },
    { value: 'wedding', label: 'Wedding' },
    { value: 'festival', label: 'Festival' },
    { value: 'formal', label: 'Formal' },
    { value: 'celebration', label: 'Celebration' }
  ],
  colors: [
    { value: 'red', label: 'Red', hex: '#722F37' },
    { value: 'blue', label: 'Blue', hex: '#1B4D3E' },
    { value: 'white', label: 'White', hex: '#FFFFFF' },
    { value: 'gold', label: 'Gold', hex: '#C9A962' },
    { value: 'black', label: 'Black', hex: '#1A1715' },
    { value: 'cream', label: 'Cream', hex: '#FDF8F3' }
  ],
  sizes: ['XS', 'S', 'M', 'L', 'XL', 'XXL']
}

// Mock products data
const mockProducts: Product[] = [
  {
    id: '1', name: 'Traditional Sarafan Dress', price: 459, image: 'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=600&h=800&fit=crop', category: 'dresses', occasion: 'wedding', rating: 4.9, reviews: 28, isNew: true, isPremium: true, isSale: false, colors: ['red', 'blue'], sizes: ['S', 'M', 'L', 'XL'], stock: 15
  },
  {
    id: '2', name: 'Embroidered Rubakha Shirt', price: 189, originalPrice: 220, image: 'https://images.unsplash.com/photo-1609192591984-2a6d953a1c2d?w=600&h=800&fit=crop', category: 'shirts', occasion: 'festival', rating: 4.8, reviews: 42, isNew: false, isPremium: false, isSale: true, colors: ['white', 'cream'], sizes: ['S', 'M', 'L', 'XL', 'XXL'], stock: 28
  },
  {
    id: '3', name: 'Wedding Tsaritsa Gown', price: 1299, image: 'https://images.unsplash.com/photo-1566174053879-31528523f8ae?w=600&h=800&fit=crop', category: 'dresses', occasion: 'wedding', rating: 5.0, reviews: 15, isNew: true, isPremium: true, isSale: false, colors: ['gold', 'red'], sizes: ['S', 'M', 'L'], stock: 5
  },
  {
    id: '4', name: 'Folk Patterned Shawl', price: 129, image: 'https://images.unsplash.com/photo-1601924994987-69e26d50dc26?w=600&h=800&fit=crop', category: 'accessories', occasion: 'daily', rating: 4.7, reviews: 56, isNew: false, isPremium: false, isSale: false, colors: ['red', 'blue', 'gold'], sizes: ['One Size'], stock: 45
  },
  {
    id: '5', name: 'Traditional Kosovorotka', price: 245, image: 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=600&h=800&fit=crop', category: 'shirts', occasion: 'formal', rating: 4.9, reviews: 33, isNew: false, isPremium: true, isSale: false, colors: ['white', 'cream'], sizes: ['S', 'M', 'L', 'XL', 'XXL'], stock: 20
  },
  {
    id: '6', name: 'Royal Peasant Blouse', price: 179, image: 'https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?w=600&h=800&fit=crop', category: 'dresses', occasion: 'daily', rating: 4.6, reviews: 67, isNew: true, isPremium: false, isSale: false, colors: ['white', 'cream', 'blue'], sizes: ['XS', 'S', 'M', 'L', 'XL'], stock: 35
  },
  {
    id: '7', name: 'Velvet Sarafan', price: 599, image: 'https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=600&h=800&fit=crop', category: 'dresses', occasion: 'celebration', rating: 4.8, reviews: 22, isNew: false, isPremium: true, isSale: false, colors: ['red', 'blue', 'gold'], sizes: ['S', 'M', 'L'], stock: 12
  },
  {
    id: '8', name: 'Woolen Matryoshka Coat', price: 459, originalPrice: 550, image: 'https://images.unsplash.com/photo-1539533113208-f6df8cc8b543?w=600&h=800&fit=crop', category: 'outerwear', occasion: 'daily', rating: 4.7, reviews: 38, isNew: true, isPremium: false, isSale: true, colors: ['red', 'blue', 'black'], sizes: ['S', 'M', 'L', 'XL'], stock: 18
  },
  {
    id: '9', name: 'Embroidered Apron', price: 89, image: 'https://images.unsplash.com/photo-1589310243389-96a5483213a8?w=600&h=800&fit=crop', category: 'accessories', occasion: 'festival', rating: 4.5, reviews: 44, isNew: false, isPremium: false, isSale: false, colors: ['red', 'gold', 'cream'], sizes: ['One Size'], stock: 50
  },
  {
    id: '10', name: 'Traditional Tulu Hat', price: 145, image: 'https://images.unsplash.com/photo-1514327605112-b887c0e61c0a?w=600&h=800&fit=crop', category: 'accessories', occasion: 'formal', rating: 4.9, reviews: 29, isNew: true, isPremium: true, isSale: false, colors: ['black', 'red', 'gold'], sizes: ['S', 'M', 'L'], stock: 22
  },
  {
    id: '11', name: 'Floral Pattern Dress', price: 329, image: 'https://images.unsplash.com/photo-1572804013427-4d7ca7268217?w=600&h=800&fit=crop', category: 'dresses', occasion: 'wedding', rating: 4.8, reviews: 51, isNew: false, isPremium: true, isSale: false, colors: ['red', 'blue', 'gold'], sizes: ['XS', 'S', 'M', 'L', 'XL'], stock: 25
  },
  {
    id: '12', name: 'Classic Russian Shirt', price: 165, image: 'https://images.unsplash.com/photo-1598033129183-c4f50c736f10?w=600&h=800&fit=crop', category: 'shirts', occasion: 'daily', rating: 4.6, reviews: 73, isNew: false, isPremium: false, isSale: false, colors: ['white', 'cream', 'blue'], sizes: ['S', 'M', 'L', 'XL', 'XXL'], stock: 40
  }
]

// Computed
const filteredProducts = computed(() => {
  let result = [...mockProducts]

  // Filter by gender
  if (filters.value.gender) {
    result = result.filter(p => {
      if (filters.value.gender === 'women') return p.category === 'dresses' || p.category === 'accessories'
      if (filters.value.gender === 'men') return p.category === 'shirts' || p.category === 'outerwear'
      return true
    })
  }

  // Filter by category
  if (filters.value.category) {
    result = result.filter(p => p.category === filters.value.category)
  }

  // Filter by occasion
  if (filters.value.occasion) {
    result = result.filter(p => p.occasion === filters.value.occasion)
  }

  // Filter by price
  result = result.filter(p => p.price >= filters.value.priceMin && p.price <= filters.value.priceMax)

  // Filter by colors
  if (filters.value.colors.length > 0) {
    result = result.filter(p => p.colors.some(c => filters.value.colors.includes(c)))
  }

  // Filter by premium
  if (filters.value.premium) {
    result = result.filter(p => p.isPremium)
  }

  // Filter by in stock
  if (filters.value.inStock) {
    result = result.filter(p => p.stock > 0)
  }

  // Sort
  switch (sortBy.value) {
    case 'newest':
      result.sort((a, b) => (b.isNew ? 1 : 0) - (a.isNew ? 1 : 0))
      break
    case 'price-low':
      result.sort((a, b) => a.price - b.price)
      break
    case 'price-high':
      result.sort((a, b) => b.price - a.price)
      break
    case 'rating':
      result.sort((a, b) => b.rating - a.rating)
      break
    case 'popular':
      result.sort((a, b) => b.reviews - a.reviews)
      break
  }

  return result
})

const paginatedProducts = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  return filteredProducts.value.slice(start, start + itemsPerPage)
})

const totalPages = computed(() => Math.ceil(filteredProducts.value.length / itemsPerPage))

const activeFilterCount = computed(() => {
  let count = 0
  if (filters.value.gender) count++
  if (filters.value.category) count++
  if (filters.value.occasion) count++
  if (filters.value.colors.length > 0) count++
  if (filters.value.premium) count++
  return count
})

// Methods
const toggleFilter = (filterName: string, value: string) => {
  if (filterName === 'colors') {
    const index = filters.value.colors.indexOf(value)
    if (index > -1) {
      filters.value.colors.splice(index, 1)
    } else {
      filters.value.colors.push(value)
    }
  }
}

const clearFilters = () => {
  filters.value = {
    gender: '',
    category: '',
    occasion: '',
    priceMin: 0,
    priceMax: 2000,
    colors: [],
    sizes: [],
    premium: false,
    inStock: true,
    searchTerm: '',
    newCollection: false
  }
  currentPage.value = 1
}

const addToCart = (product: Product) => {
  addingToCart.value = product.id
  
  // For quick add from grid, navigate to PDP for size/color selection
  // In a real scenario, you could show a modal for quick selection
  setTimeout(() => {
    router.push(`/products/${product.id}`)
    addingToCart.value = null
  }, 300)
}

const toggleWishlist = (product: Product, event: Event) => {
  event.preventDefault()
  wishlistStore.toggleItem({
    id: product.id,
    name: product.name,
    price: product.price,
    image: product.image,
    rating: product.rating,
    reviews: product.reviews,
    addedAt: Date.now()
  })
  
  // Show toast
  const isInWishlist = wishlistStore.items.some(i => i.id === product.id)
  toastMessage.value = isInWishlist ? 'Added to wishlist' : 'Removed from wishlist'
  showToast.value = true
  setTimeout(() => { showToast.value = false }, 2000)
}

// Lifecycle
onMounted(() => {
  // Simulate API call
  setTimeout(() => {
    products.value = mockProducts
    loading.value = false
  }, 500)
})

// Watch for route changes
watch(() => route.query, () => {
  // Update filter values from query params
  filters.value.gender = route.query.gender?.toString() || ''
  filters.value.category = route.query.category?.toString() || ''
  filters.value.occasion = route.query.occasion?.toString() || ''
  
  // Handle search query
  if (route.query.search) {
    filters.value.searchTerm = route.query.search.toString()
  }
  
  // Handle collection filter
  if (route.query.collection) {
    filters.value.newCollection = route.query.collection === 'new'
  }
  
  // Handle premium filter
  if (route.query.premium) {
    filters.value.premium = route.query.premium === 'true'
  }
  
  // Reset to first page
  currentPage.value = 1
}, { deep: true })
</script>

<template>
  <div class="products-page">
    <!-- Breadcrumb -->
    <div class="breadcrumb">
      <div class="container">
        <RouterLink to="/">Home</RouterLink>
        <span class="breadcrumb-separator">/</span>
        <span class="breadcrumb-current">Products</span>
      </div>
    </div>

    <!-- Page Header -->
    <div class="page-header">
      <div class="container">
        <h1 class="page-title">
          {{ filters.category ? filterOptions.categories.find(c => c.value === filters.category)?.label : 'All Products' }}
        </h1>
        <p class="page-subtitle" v-if="filteredProducts.length">
          {{ filteredProducts.length }} pieces found
        </p>
      </div>
    </div>

    <div class="products-content">
      <div class="container">
        <div class="products-layout">
          <!-- Mobile Filter Toggle -->
          <button class="filter-toggle hide-desktop" @click="isFilterOpen = true">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <line x1="21" y1="10" x2="14" y2="10"></line>
              <line x1="10" y1="20" x2="21" y2="20"></line>
              <line x1="21" y1="6" x2="3" y2="6"></line>
              <line x1="3" y1="14" x2="21" y2="14"></line>
            </svg>
            Filters
            <span v-if="activeFilterCount" class="filter-count">{{ activeFilterCount }}</span>
          </button>

          <!-- Filter Sidebar -->
          <aside class="filters-sidebar" :class="{ 'filters-open': isFilterOpen }">
            <div class="filters-header hide-desktop">
              <h3>Filters</h3>
              <button class="filters-close" @click="isFilterOpen = false">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <line x1="18" y1="6" x2="6" y2="18"></line>
                  <line x1="6" y1="6" x2="18" y2="18"></line>
                </svg>
              </button>
            </div>

            <!-- Gender Filter -->
            <div class="filter-group">
              <h4 class="filter-title">Gender</h4>
              <div class="filter-options">
                <label v-for="option in filterOptions.genders" :key="option.value" class="filter-option">
                  <input 
                    type="radio" 
                    :value="option.value" 
                    v-model="filters.gender"
                    name="gender"
                  />
                  <span class="filter-label">{{ option.label }}</span>
                </label>
              </div>
            </div>

            <!-- Category Filter -->
            <div class="filter-group">
              <h4 class="filter-title">Category</h4>
              <div class="filter-options">
                <label v-for="option in filterOptions.categories" :key="option.value" class="filter-option">
                  <input 
                    type="radio" 
                    :value="option.value" 
                    v-model="filters.category"
                    name="category"
                  />
                  <span class="filter-label">{{ option.label }}</span>
                </label>
              </div>
            </div>

            <!-- Occasion Filter -->
            <div class="filter-group">
              <h4 class="filter-title">Occasion</h4>
              <div class="filter-options">
                <label v-for="option in filterOptions.occasions" :key="option.value" class="filter-option">
                  <input 
                    type="radio" 
                    :value="option.value" 
                    v-model="filters.occasion"
                    name="occasion"
                  />
                  <span class="filter-label">{{ option.label }}</span>
                </label>
              </div>
            </div>

            <!-- Price Filter -->
            <div class="filter-group">
              <h4 class="filter-title">Price Range</h4>
              <div class="price-inputs">
                <input 
                  type="number" 
                  v-model.number="filters.priceMin" 
                  placeholder="Min"
                  class="price-input"
                />
                <span class="price-separator">—</span>
                <input 
                  type="number" 
                  v-model.number="filters.priceMax" 
                  placeholder="Max"
                  class="price-input"
                />
              </div>
            </div>

            <!-- Color Filter -->
            <div class="filter-group">
              <h4 class="filter-title">Color</h4>
              <div class="color-options">
                <button 
                  v-for="color in filterOptions.colors" 
                  :key="color.value"
                  class="color-swatch"
                  :class="{ 'color-swatch--active': filters.colors.includes(color.value) }"
                  :style="{ backgroundColor: color.hex }"
                  :title="color.label"
                  @click="toggleFilter('colors', color.value)"
                >
                  <svg v-if="filters.colors.includes(color.value)" xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round">
                    <polyline points="20 6 9 17 4 12"></polyline>
                  </svg>
                </button>
              </div>
            </div>

            <!-- Checkbox Filters -->
            <div class="filter-group">
              <label class="filter-checkbox">
                <input type="checkbox" v-model="filters.premium" />
                <span class="checkbox-label">Premium Collection</span>
              </label>
              <label class="filter-checkbox">
                <input type="checkbox" v-model="filters.inStock" />
                <span class="checkbox-label">In Stock Only</span>
              </label>
            </div>

            <!-- Clear Filters -->
            <button v-if="activeFilterCount" class="clear-filters" @click="clearFilters">
              Clear All Filters
            </button>
          </aside>

          <!-- Products Grid -->
          <div class="products-main">
            <!-- Toolbar -->
            <div class="products-toolbar">
              <span class="results-count">{{ filteredProducts.length }} products</span>
              
              <div class="toolbar-right">
                <!-- Sort Dropdown -->
                <select v-model="sortBy" class="sort-select">
                  <option value="newest">Newest</option>
                  <option value="popular">Most Popular</option>
                  <option value="rating">Highest Rated</option>
                  <option value="price-low">Price: Low to High</option>
                  <option value="price-high">Price: High to Low</option>
                </select>
              </div>
            </div>

            <!-- Loading State -->
            <div v-if="loading" class="products-loading">
              <div class="skeleton-grid">
                <div v-for="n in 8" :key="n" class="skeleton-card">
                  <div class="skeleton-image"></div>
                  <div class="skeleton-text"></div>
                  <div class="skeleton-text short"></div>
                </div>
              </div>
            </div>

            <!-- Empty State -->
            <div v-else-if="filteredProducts.length === 0" class="products-empty">
              <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" stroke-linecap="round" stroke-linejoin="round">
                <circle cx="11" cy="11" r="8"></circle>
                <path d="m21 21-4.3-4.3"></path>
              </svg>
              <h3>No products found</h3>
              <p>Try adjusting your filters or search criteria</p>
              <button class="btn btn-primary" @click="clearFilters">Clear Filters</button>
            </div>

            <!-- Products Grid -->
            <div v-else class="products-grid">
              <article 
                v-for="product in paginatedProducts" 
                :key="product.id" 
                class="product-card"
              >
                <RouterLink :to="`/products/${product.id}`" class="product-card__link">
                  <div class="product-card__image">
                    <img :src="product.image" :alt="product.name" loading="lazy" />
                    <div class="product-card__badges">
                      <span v-if="product.isNew" class="badge badge-gold">New</span>
                      <span v-if="product.isPremium" class="badge badge-primary">Premium</span>
                      <span v-if="product.isSale" class="badge badge-outline">Sale</span>
                    </div>
                    <div class="product-card__overlay">
                      <button class="quick-add-btn" @click.prevent="addToCart(product)">
                        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                          <circle cx="9" cy="21" r="1"></circle>
                          <circle cx="20" cy="21" r="1"></circle>
                          <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
                        </svg>
                        Quick Add
                      </button>
                    </div>
                  </div>
                  <div class="product-card__info">
                    <span class="product-card__category">{{ product.category }}</span>
                    <h3 class="product-card__name">{{ product.name }}</h3>
                    <div class="product-card__meta">
                      <div class="product-card__rating">
                        <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2">
                          <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
                        </svg>
                        <span>{{ product.rating }}</span>
                        <span class="product-card__reviews">({{ product.reviews }})</span>
                      </div>
                      <span class="product-card__occasion">{{ product.occasion }}</span>
                    </div>
                    <div class="product-card__price">
                      <span class="current-price">${{ product.price.toLocaleString() }}</span>
                      <span v-if="product.originalPrice" class="original-price">
                        ${{ product.originalPrice.toLocaleString() }}
                      </span>
                    </div>
                  </div>
                </RouterLink>
                <button 
                  class="wishlist-btn"
                  :class="{ 'wishlist-btn--active': wishlistStore.items.some(i => i.id === product.id) }"
                  @click="toggleWishlist(product, $event)"
                  aria-label="Add to wishlist"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" :fill="wishlistStore.items.some(i => i.id === product.id) ? 'currentColor' : 'none'" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                    <path d="M19 14c1.49-1.46 3-3.21 3-5.5A5.5 5.5 0 0 0 16.5 3c-1.76 0-3 .5-4.5 2-1.5-1.5-2.74-2-4.5-2A5.5 5.5 0 0 0 2 8.5c0 2.3 1.5 4.05 3 5.5l7 7Z"></path>
                  </svg>
                </button>
              </article>
            </div>

            <!-- Pagination -->
            <div v-if="totalPages > 1" class="products-pagination">
              <button 
                class="pagination-btn" 
                :disabled="currentPage === 1"
                @click="currentPage--"
              >
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <polyline points="15 18 9 12 15 6"></polyline>
                </svg>
              </button>
              
              <div class="pagination-pages">
                <button 
                  v-for="page in totalPages" 
                  :key="page"
                  class="pagination-page"
                  :class="{ 'pagination-page--active': currentPage === page }"
                  @click="currentPage = page"
                >
                  {{ page }}
                </button>
              </div>

              <button 
                class="pagination-btn" 
                :disabled="currentPage === totalPages"
                @click="currentPage++"
              >
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <polyline points="9 18 15 12 9 6"></polyline>
                </svg>
              </button>
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
  transition: color var(--transition-fast) var(--ease-out);
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
.page-header {
  padding: var(--space-10) 0;
  background: var(--color-background);
  text-align: center;
  border-bottom: 1px solid var(--color-border-light);
}

.page-title {
  font-family: var(--font-heading);
  font-size: var(--text-4xl);
  color: var(--color-charcoal);
  margin-bottom: var(--space-2);
}

.page-subtitle {
  font-size: var(--text-lg);
  color: var(--color-warm-gray);
}

/* ========================================
   LAYOUT
   ======================================== */
.products-content {
  padding: var(--space-10) 0 var(--space-16);
  background: var(--color-background);
}

.products-layout {
  display: grid;
  grid-template-columns: 280px 1fr;
  gap: var(--space-10);
}

/* ========================================
   FILTERS SIDEBAR
   ======================================== */
.filters-sidebar {
  position: sticky;
  top: 100px;
  height: fit-content;
}

.filters-header {
  display: none;
}

.filter-group {
  padding: var(--space-5) 0;
  border-bottom: 1px solid var(--color-border-light);
}

.filter-title {
  font-family: var(--font-heading);
  font-size: var(--text-base);
  color: var(--color-charcoal);
  margin-bottom: var(--space-3);
  font-weight: var(--font-medium);
}

.filter-options {
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.filter-option {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  cursor: pointer;
}

.filter-option input {
  display: none;
}

.filter-label {
  font-size: var(--text-sm);
  color: var(--color-text);
  padding: var(--space-1) 0;
  transition: color var(--transition-fast) var(--ease-out);
}

.filter-option input:checked + .filter-label {
  color: var(--color-burgundy);
  font-weight: var(--font-medium);
}

/* Price Inputs */
.price-inputs {
  display: flex;
  align-items: center;
  gap: var(--space-2);
}

.price-input {
  width: 80px;
  padding: var(--space-2) var(--space-3);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  font-size: var(--text-sm);
}

.price-separator {
  color: var(--color-warm-gray-light);
}

/* Color Swatches */
.color-options {
  display: flex;
  gap: var(--space-2);
  flex-wrap: wrap;
}

.color-swatch {
  width: 32px;
  height: 32px;
  border-radius: var(--radius-full);
  border: 2px solid var(--color-border);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all var(--transition-fast) var(--ease-out);
}

.color-swatch:hover {
  transform: scale(1.1);
}

.color-swatch--active {
  border-color: var(--color-burgundy);
  box-shadow: 0 0 0 2px var(--color-background), 0 0 0 4px var(--color-burgundy);
}

.color-swatch--active svg {
  color: var(--color-background);
}

/* Checkboxes */
.filter-checkbox {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  cursor: pointer;
  margin-bottom: var(--space-3);
}

.filter-checkbox input {
  display: none;
}

.checkbox-label {
  font-size: var(--text-sm);
  color: var(--color-text);
  position: relative;
  padding-left: var(--space-6);
}

.checkbox-label::before {
  content: '';
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 18px;
  height: 18px;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-sm);
  transition: all var(--transition-fast) var(--ease-out);
}

.filter-checkbox input:checked + .checkbox-label::before {
  background: var(--color-burgundy);
  border-color: var(--color-burgundy);
}

.filter-checkbox input:checked + .checkbox-label::after {
  content: '✓';
  position: absolute;
  left: 3px;
  top: 50%;
  transform: translateY(-50%);
  color: var(--vt-c-white);
  font-size: 12px;
}

/* Clear Filters */
.clear-filters {
  width: 100%;
  padding: var(--space-3);
  margin-top: var(--space-4);
  background: transparent;
  border: 1px solid var(--color-burgundy);
  color: var(--color-burgundy);
  font-size: var(--text-sm);
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: all var(--transition-fast) var(--ease-out);
}

.clear-filters:hover {
  background: var(--color-burgundy);
  color: var(--vt-c-white);
}

/* Mobile Filter Toggle */
.filter-toggle {
  display: none;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-3) var(--space-4);
  background: var(--color-background);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  font-size: var(--text-sm);
  cursor: pointer;
  margin-bottom: var(--space-4);
}

.filter-count {
  background: var(--color-burgundy);
  color: var(--vt-c-white);
  padding: 2px 8px;
  border-radius: var(--radius-full);
  font-size: var(--text-xs);
}

/* ========================================
   TOOLBAR
   ======================================== */
.products-toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-6);
  padding-bottom: var(--space-4);
  border-bottom: 1px solid var(--color-border-light);
}

.results-count {
  font-size: var(--text-sm);
  color: var(--color-warm-gray);
}

.toolbar-right {
  display: flex;
  align-items: center;
  gap: var(--space-4);
}

.sort-select {
  padding: var(--space-2) var(--space-4);
  padding-right: var(--space-8);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  background: var(--color-background);
  font-size: var(--text-sm);
  cursor: pointer;
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%238B8178' stroke-width='2'%3E%3Cpolyline points='6 9 12 15 18 9'%3E%3C/polyline%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 12px center;
}

/* ========================================
   PRODUCTS GRID
   ======================================== */
.products-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: var(--space-6);
}

/* Product Card (same as homepage) */
.product-card {
  position: relative;
  background: var(--color-background);
  border-radius: var(--radius-xl);
  overflow: hidden;
  transition: all var(--transition-base) var(--ease-out);
}

.product-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-card-hover);
}

.product-card__link {
  display: block;
  text-decoration: none;
  color: inherit;
}

.product-card__image {
  position: relative;
  aspect-ratio: 3/4;
  overflow: hidden;
}

.product-card__image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s var(--ease-out);
}

.product-card:hover .product-card__image img {
  transform: scale(1.05);
}

.product-card__badges {
  position: absolute;
  top: var(--space-3);
  left: var(--space-3);
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.product-card__overlay {
  position: absolute;
  inset: 0;
  background: rgba(28, 25, 23, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity var(--transition-base) var(--ease-out);
}

.product-card:hover .product-card__overlay {
  opacity: 1;
}

.quick-add-btn {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-3) var(--space-5);
  background: var(--color-background);
  color: var(--color-charcoal);
  border: none;
  border-radius: var(--radius-lg);
  font-size: var(--text-sm);
  font-weight: var(--font-medium);
  cursor: pointer;
  transform: translateY(10px);
  transition: all var(--transition-base) var(--ease-out);
}

.product-card:hover .quick-add-btn {
  transform: translateY(0);
}

.quick-add-btn:hover {
  background: var(--color-burgundy);
  color: var(--vt-c-white);
}

.product-card__info {
  padding: var(--space-4);
}

.product-card__category {
  font-size: var(--text-xs);
  color: var(--color-warm-gray);
  text-transform: uppercase;
  letter-spacing: 0.1em;
}

.product-card__name {
  font-family: var(--font-heading);
  font-size: var(--text-lg);
  color: var(--color-charcoal);
  margin: var(--space-1) 0 var(--space-2);
}

.product-card__meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-3);
}

.product-card__rating {
  display: flex;
  align-items: center;
  gap: var(--space-1);
  color: var(--color-gold);
  font-size: var(--text-sm);
  font-weight: var(--font-medium);
}

.product-card__reviews {
  color: var(--color-warm-gray);
  font-weight: var(--font-regular);
}

.product-card__occasion {
  font-size: var(--text-xs);
  color: var(--color-burgundy);
  padding: var(--space-1) var(--space-2);
  background: rgba(114, 47, 55, 0.08);
  border-radius: var(--radius-sm);
}

.product-card__price {
  display: flex;
  align-items: center;
  gap: var(--space-2);
}

.current-price {
  font-family: var(--font-heading);
  font-size: var(--text-xl);
  color: var(--color-burgundy);
  font-weight: var(--font-semibold);
}

.original-price {
  font-size: var(--text-sm);
  color: var(--color-warm-gray-light);
  text-decoration: line-through;
}

.wishlist-btn {
  position: absolute;
  top: var(--space-3);
  right: var(--space-3);
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-background);
  border: none;
  border-radius: var(--radius-full);
  color: var(--color-warm-gray);
  cursor: pointer;
  opacity: 0;
  transform: scale(0.8);
  transition: all var(--transition-base) var(--ease-out);
}

.product-card:hover .wishlist-btn {
  opacity: 1;
  transform: scale(1);
}

.wishlist-btn:hover {
  color: var(--color-burgundy);
}

/* ========================================
   LOADING SKELETON
   ======================================== */
.products-loading {
  padding: var(--space-8) 0;
}

.skeleton-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: var(--space-6);
}

.skeleton-card {
  background: var(--color-background-soft);
  border-radius: var(--radius-xl);
  overflow: hidden;
}

.skeleton-image {
  aspect-ratio: 3/4;
  background: linear-gradient(90deg, var(--color-border-light) 25%, var(--color-background-soft) 50%, var(--color-border-light) 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
}

.skeleton-text {
  height: 20px;
  margin: var(--space-4);
  background: linear-gradient(90deg, var(--color-border-light) 25%, var(--color-background-soft) 50%, var(--color-border-light) 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  border-radius: var(--radius-md);
}

.skeleton-text.short {
  width: 60%;
}

@keyframes shimmer {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}

/* ========================================
   EMPTY STATE
   ======================================== */
.products-empty {
  text-align: center;
  padding: var(--space-16) var(--space-8);
}

.products-empty svg {
  color: var(--color-warm-gray-light);
  margin-bottom: var(--space-4);
}

.products-empty h3 {
  font-family: var(--font-heading);
  font-size: var(--text-2xl);
  color: var(--color-charcoal);
  margin-bottom: var(--space-2);
}

.products-empty p {
  color: var(--color-warm-gray);
  margin-bottom: var(--space-6);
}

/* ========================================
   PAGINATION
   ======================================== */
.products-pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--space-4);
  margin-top: var(--space-12);
  padding-top: var(--space-8);
  border-top: 1px solid var(--color-border-light);
}

.pagination-btn {
  width: 44px;
  height: 44px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-background);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  cursor: pointer;
  transition: all var(--transition-fast) var(--ease-out);
}

.pagination-btn:hover:not(:disabled) {
  border-color: var(--color-burgundy);
  color: var(--color-burgundy);
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-pages {
  display: flex;
  gap: var(--space-2);
}

.pagination-page {
  width: 44px;
  height: 44px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: 1px solid transparent;
  border-radius: var(--radius-lg);
  font-size: var(--text-sm);
  cursor: pointer;
  transition: all var(--transition-fast) var(--ease-out);
}

.pagination-page:hover {
  background: var(--color-background-soft);
}

.pagination-page--active {
  background: var(--color-burgundy);
  color: var(--vt-c-white);
}

/* ========================================
   RESPONSIVE
   ======================================== */
@media (max-width: 1280px) {
  .products-grid,
  .skeleton-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 1024px) {
  .products-layout {
    grid-template-columns: 1fr;
  }
  
  .filters-sidebar {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: var(--color-background);
    z-index: var(--z-modal);
    padding: var(--space-6);
    transform: translateX(-100%);
    transition: transform var(--transition-base) var(--ease-out);
    overflow-y: auto;
  }
  
  .filters-sidebar.filters-open {
    transform: translateX(0);
  }
  
  .filters-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: var(--space-6);
    padding-bottom: var(--space-4);
    border-bottom: 1px solid var(--color-border-light);
  }
  
  .filters-header h3 {
    font-family: var(--font-heading);
    font-size: var(--text-2xl);
  }
  
  .filters-close {
    background: none;
    border: none;
    cursor: pointer;
    padding: var(--space-2);
  }
  
  .filter-toggle {
    display: flex;
  }
}

@media (max-width: 640px) {
  .products-grid,
  .skeleton-grid {
    grid-template-columns: 1fr;
    max-width: 400px;
    margin: 0 auto;
  }
  
  .products-toolbar {
    flex-direction: column;
    gap: var(--space-3);
    align-items: flex-start;
  }
}
</style>

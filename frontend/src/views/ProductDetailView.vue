<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, RouterLink, useRouter } from 'vue-router'
import { useCartStore } from '../stores/cartStore'
import { useWishlistStore } from '../stores/wishlistStore'

// Types
interface Product {
  id: string
  name: string
  price: number
  originalPrice?: number
  description: string
  images: string[]
  category: string
  occasion: string
  material: string
  careInstructions: string
  rating: number
  reviews: number
  isNew: boolean
  isPremium: boolean
  isSale: boolean
  colors: { name: string; hex: string; }[]
  sizes: string[]
  stock: number
  deliveryDays: string
  features: string[]
}

// Route
const route = useRoute()
const router = useRouter()
const cartStore = useCartStore()
const wishlistStore = useWishlistStore()
const productId = route.params.id

// State
const loading = ref(true)
const selectedImage = ref(0)
const selectedColor = ref(0)
const selectedSize = ref('')
const quantity = ref(1)
const isWishlistActive = ref(false)
const showSizeGuide = ref(false)
const isAddingToCart = ref(false)
const toastMessage = ref('')
const showToast = ref(false)
const isZoomed = ref(false)
const zoomPosition = ref({ x: 0, y: 0 })
const activeTab = ref('description')

// Mock product data
const product = ref<Product>({
  id: productId as string,
  name: 'Traditional Sarafan Dress',
  price: 459,
  originalPrice: 550,
  description: 'This exquisite Traditional Sarafan Dress represents the pinnacle of Russian folk craftsmanship. Hand-embroidered with traditional floral patterns using techniques passed down through generations, this garment combines authentic heritage with modern elegance. Perfect for weddings, celebrations, and cultural events.',
  images: [
    'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=800&h=1000&fit=crop',
    'https://images.unsplash.com/photo-1566174053879-31528523f8ae?w=800&h=1000&fit=crop',
    'https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=800&h=1000&fit=crop',
    'https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?w=800&h=1000&fit=crop'
  ],
  category: 'Dresses',
  occasion: 'Wedding',
  material: '100% Premium Linen, Hand-embroidered with silk threads',
  careInstructions: 'Dry clean recommended. Store in a cool, dry place away from direct sunlight.',
  rating: 4.9,
  reviews: 28,
  isNew: true,
  isPremium: true,
  isSale: true,
  colors: [
    { name: 'Burgundy', hex: '#722F37' },
    { name: 'Forest Green', hex: '#1B4D3E' },
    { name: 'Navy Blue', hex: '#1A365D' }
  ],
  sizes: ['XS', 'S', 'M', 'L', 'XL'],
  stock: 15,
  deliveryDays: '3-5 business days',
  features: [
    'Hand-embroidered traditional patterns',
    'Premium Russian linen fabric',
    'Authentic folk art craftsmanship',
    'Perfect for weddings and celebrations',
    'Custom tailoring available'
  ]
})

// Related products
const relatedProducts = ref([
  {
    id: '2',
    name: 'Embroidered Rubakha Shirt',
    price: 189,
    image: 'https://images.unsplash.com/photo-1609192591984-2a6d953a1c2d?w=400&h=500&fit=crop'
  },
  {
    id: '3',
    name: 'Wedding Tsaritsa Gown',
    price: 1299,
    image: 'https://images.unsplash.com/photo-1566174053879-31528523f8ae?w=400&h=500&fit=crop'
  },
  {
    id: '4',
    name: 'Folk Patterned Shawl',
    price: 129,
    image: 'https://images.unsplash.com/photo-1601924994987-69e26d50dc26?w=400&h=500&fit=crop'
  }
])

// Computed
const totalPrice = computed(() => product.value.price * quantity.value)

const stockStatus = computed(() => {
  if (product.value.stock === 0) return 'Out of Stock'
  if (product.value.stock <= 5) return `Only ${product.value.stock} left`
  return 'In Stock'
})

const stockStatusClass = computed(() => {
  if (product.value.stock === 0) return 'out-of-stock'
  if (product.value.stock <= 5) return 'low-stock'
  return 'in-stock'
})

// Methods
const selectImage = (index: number) => {
  selectedImage.value = index
}

const selectColor = (index: number) => {
  selectedColor.value = index
}

const selectSize = (size: string) => {
  selectedSize.value = size
}

const previousImage = () => {
  selectedImage.value = (selectedImage.value - 1 + product.value.images.length) % product.value.images.length
}

const nextImage = () => {
  selectedImage.value = (selectedImage.value + 1) % product.value.images.length
}

const handleGalleryKeydown = (event: KeyboardEvent) => {
  if (event.key === 'ArrowLeft') {
    previousImage()
    event.preventDefault()
  } else if (event.key === 'ArrowRight') {
    nextImage()
    event.preventDefault()
  } else if (event.key === 'z' || event.key === 'Z') {
    isZoomed.value = !isZoomed.value
  } else if (event.key === 'Escape') {
    isZoomed.value = false
  }
}

const handleImageMouseMove = (event: MouseEvent) => {
  if (!isZoomed.value) return
  
  const rect = (event.target as HTMLElement).getBoundingClientRect()
  const x = ((event.clientX - rect.left) / rect.width) * 100
  const y = ((event.clientY - rect.top) / rect.height) * 100
  
  zoomPosition.value = { x, y }
}

const handleImageMouseLeave = () => {
  if (isZoomed.value) {
    zoomPosition.value = { x: 50, y: 50 }
  }
}

const incrementQuantity = () => {
  if (quantity.value < product.value.stock) {
    quantity.value++
  }
}

const decrementQuantity = () => {
  if (quantity.value > 1) {
    quantity.value--
  }
}

const addToCart = () => {
  // Validation
  if (!selectedSize.value) {
    toastMessage.value = 'Please select a size'
    showToast.value = true
    setTimeout(() => { showToast.value = false }, 2000)
    return
  }

  if (quantity.value <= 0) {
    toastMessage.value = 'Please select a valid quantity'
    showToast.value = true
    setTimeout(() => { showToast.value = false }, 2000)
    return
  }

  const colorIndex = selectedColor.value
  const color = product.value.colors[colorIndex]?.name || 'Default'
  const size = selectedSize.value as string // Now guaranteed to exist after validation

  // Check stock
  if (quantity.value > product.value.stock) {
    toastMessage.value = `Only ${product.value.stock} items available`
    showToast.value = true
    setTimeout(() => { showToast.value = false }, 2000)
    return
  }

  // Add to cart
  isAddingToCart.value = true
  
  setTimeout(() => {
    const imageUrl = product.value.images[0] || ''
    cartStore.addItem({
      id: `${product.value.id}-${color}-${size}`,
      productId: product.value.id,
      name: product.value.name,
      price: product.value.price,
      originalPrice: product.value.originalPrice,
      image: imageUrl,
      color: color,
      size: size,
      quantity: quantity.value,
      stock: product.value.stock
    })

    isAddingToCart.value = false
    toastMessage.value = 'Added to cart!'
    showToast.value = true
    setTimeout(() => { showToast.value = false }, 2000)
    
    // Reset form
    quantity.value = 1
    selectedSize.value = ''
  }, 300)
}

const toggleWishlist = () => {
  if (!isWishlistActive.value) {
    wishlistStore.addItem({
      id: product.value.id,
      name: product.value.name,
      price: product.value.price,
      image: product.value.images[0] || '',
      rating: product.value.rating,
      reviews: product.value.reviews,
      addedAt: Date.now()
    })
    toastMessage.value = 'Added to wishlist'
  } else {
    wishlistStore.removeItem(product.value.id)
    toastMessage.value = 'Removed from wishlist'
  }
  
  isWishlistActive.value = !isWishlistActive.value
  showToast.value = true
  setTimeout(() => { showToast.value = false }, 2000)
}

// Lifecycle
onMounted(() => {
  setTimeout(() => {
    loading.value = false
  }, 300)
})
</script>

<template>
  <div class="product-detail">
    <!-- Breadcrumb -->
    <div class="breadcrumb">
      <div class="container">
        <RouterLink to="/">Home</RouterLink>
        <span class="breadcrumb-separator">/</span>
        <RouterLink to="/products">Products</RouterLink>
        <span class="breadcrumb-separator">/</span>
        <RouterLink :to="`/products?category=${product.category.toLowerCase()}`">{{ product.category }}</RouterLink>
        <span class="breadcrumb-separator">/</span>
        <span class="breadcrumb-current">{{ product.name }}</span>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="product-loading">
      <div class="container">
        <div class="loading-layout">
          <div class="skeleton-gallery"></div>
          <div class="skeleton-info">
            <div class="skeleton-line wide"></div>
            <div class="skeleton-line"></div>
            <div class="skeleton-line medium"></div>
            <div class="skeleton-line short"></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Product Content -->
    <div v-else class="product-content">
      <div class="container">
        <div class="product-layout">
          <!-- Image Gallery -->
          <div class="product-gallery" @keydown="handleGalleryKeydown" tabindex="0" role="region" aria-label="Product image gallery">
            <div class="gallery-main" :class="{ 'gallery-main--zoomed': isZoomed }">
              <img 
                :src="product.images[selectedImage]" 
                :alt="product.name"
                class="gallery-image"
                :style="isZoomed ? {
                  transformOrigin: `${zoomPosition.x}% ${zoomPosition.y}%`,
                  cursor: 'zoom-in'
                } : {}"
                @mousemove="handleImageMouseMove"
                @mouseleave="handleImageMouseLeave"
              />
              
              <!-- Zoom indicator -->
              <div v-if="isZoomed" class="zoom-indicator">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
                  <circle cx="11" cy="11" r="8"></circle>
                  <path d="m21 21-4.3-4.3"></path>
                  <line x1="11" y1="8" x2="11" y2="14"></line>
                  <line x1="8" y1="11" x2="14" y2="11"></line>
                </svg>
                <span>Zoom active (ESC to exit)</span>
              </div>
              
              <!-- Navigation arrows -->
              <button 
                class="gallery-nav gallery-nav--prev" 
                @click="previousImage"
                aria-label="Previous image"
              >
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
                  <polyline points="15 18 9 12 15 6"></polyline>
                </svg>
              </button>
              <button 
                class="gallery-nav gallery-nav--next" 
                @click="nextImage"
                aria-label="Next image"
              >
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
                  <polyline points="9 18 15 12 9 6"></polyline>
                </svg>
              </button>
              
              <!-- Zoom toggle -->
              <button 
                class="gallery-zoom-btn" 
                @click="isZoomed = !isZoomed"
                :title="isZoomed ? 'Zoom out' : 'Zoom in (or press Z)'"
                aria-label="Toggle zoom"
              >
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
                  <circle cx="11" cy="11" r="8"></circle>
                  <path d="m21 21-4.3-4.3"></path>
                  <line x1="11" y1="8" x2="11" y2="14"></line>
                  <line x1="8" y1="11" x2="14" y2="11"></line>
                </svg>
              </button>
              
              <div class="gallery-badges">
                <span v-if="product.isNew" class="badge badge-gold">New Arrival</span>
                <span v-if="product.isPremium" class="badge badge-primary">Premium</span>
                <span v-if="product.isSale" class="badge badge-outline">Sale</span>
              </div>
            </div>
            <div class="gallery-thumbnails">
              <button 
                v-for="(image, index) in product.images" 
                :key="index"
                class="thumbnail"
                :class="{ 'thumbnail--active': selectedImage === index }"
                @click="selectImage(index)"
                :aria-label="`View ${product.name} image ${index + 1}`"
              >
                <img :src="image" :alt="`${product.name} view ${index + 1}`" />
              </button>
            </div>
            <div class="gallery-help-text">
              <p>← → Arrow keys to navigate • Z to zoom • ESC to close</p>
            </div>
          </div>

          <!-- Product Info -->
          <div class="product-info">
            <!-- Header -->
            <div class="product-header">
              <span class="product-category">{{ product.category }}</span>
              <h1 class="product-title">{{ product.name }}</h1>
              
              <div class="product-rating">
                <div class="rating-stars">
                  <svg v-for="n in 5" :key="n" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" :fill="n <= Math.floor(product.rating) ? 'currentColor' : 'none'" stroke="currentColor" stroke-width="2">
                    <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
                  </svg>
                </div>
                <span class="rating-text">{{ product.rating }} ({{ product.reviews }} reviews)</span>
              </div>
            </div>

            <!-- Price -->
            <div class="product-price">
              <span class="current-price">${{ product.price.toLocaleString() }}</span>
              <span v-if="product.originalPrice" class="original-price">
                ${{ product.originalPrice.toLocaleString() }}
              </span>
              <span v-if="product.isSale" class="discount-badge">
                {{ Math.round((1 - product.price / product.originalPrice!) * 100) }}% OFF
              </span>
            </div>

            <!-- Stock Status -->
            <div class="stock-status" :class="stockStatusClass">
              <span class="stock-dot"></span>
              {{ stockStatus }}
            </div>

            <!-- Color Selection -->
            <div class="product-option">
              <label class="option-label">
                Color: <strong>{{ product.colors[selectedColor]?.name }}</strong>
              </label>
              <div class="color-options">
                <button 
                  v-for="(color, index) in product.colors" 
                  :key="index"
                  class="color-swatch"
                  :class="{ 'color-swatch--active': selectedColor === index }"
                  :style="{ backgroundColor: color.hex }"
                  :title="color.name"
                  @click="selectColor(index)"
                >
                  <svg v-if="selectedColor === index" xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="3">
                    <polyline points="20 6 9 17 4 12"></polyline>
                  </svg>
                </button>
              </div>
            </div>

            <!-- Size Selection -->
            <div class="product-option">
              <label class="option-label">
                Size: <strong>{{ selectedSize || 'Select a size' }}</strong>
              </label>
              <div class="size-options">
                <button 
                  v-for="size in product.sizes" 
                  :key="size"
                  class="size-btn"
                  :class="{ 'size-btn--active': selectedSize === size }"
                  @click="selectSize(size)"
                >
                  {{ size }}
                </button>
              </div>
              <button class="size-guide-link" @click="showSizeGuide = true">
                Size Guide
              </button>
            </div>

            <!-- Quantity -->
            <div class="product-option">
              <label class="option-label">Quantity</label>
              <div class="quantity-selector">
                <button class="qty-btn" @click="decrementQuantity" :disabled="quantity <= 1">
                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <line x1="5" y1="12" x2="19" y2="12"></line>
                  </svg>
                </button>
                <span class="qty-value">{{ quantity }}</span>
                <button class="qty-btn" @click="incrementQuantity" :disabled="quantity >= product.stock">
                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <line x1="12" y1="5" x2="12" y2="19"></line>
                    <line x1="5" y1="12" x2="19" y2="12"></line>
                  </svg>
                </button>
              </div>
            </div>

            <!-- Actions -->
            <div class="product-actions">
              <button 
                class="btn btn-primary btn-lg add-to-cart-btn" 
                @click="addToCart"
                :disabled="isAddingToCart"
              >
                <svg v-if="!isAddingToCart" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <circle cx="9" cy="21" r="1"></circle>
                  <circle cx="20" cy="21" r="1"></circle>
                  <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
                </svg>
                <span v-else class="spinner spinner--sm spinner--white" style="margin-right: 8px"></span>
                {{ isAddingToCart ? 'Adding to Cart...' : `Add to Cart — $${totalPrice.toLocaleString()}` }}
              </button>
              <button 
                class="wishlist-btn"
                :class="{ 'wishlist-btn--active': isWishlistActive }"
                @click="toggleWishlist"
              >
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" :fill="isWishlistActive ? 'currentColor' : 'none'" stroke="currentColor" stroke-width="2">
                  <path d="M19 14c1.49-1.46 3-3.21 3-5.5A5.5 5.5 0 0 0 16.5 3c-1.76 0-3 .5-4.5 2-1.5-1.5-2.74-2-4.5-2A5.5 5.5 0 0 0 2 8.5c0 2.3 1.5 4.05 3 5.5l7 7Z"></path>
                </svg>
              </button>
            </div>

            <!-- Delivery Info -->
            <div class="delivery-info">
              <div class="delivery-item">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <rect x="1" y="3" width="15" height="13"></rect>
                  <polygon points="16 8 20 8 23 11 23 16 16 16 16 8"></polygon>
                  <circle cx="5.5" cy="18.5" r="2.5"></circle>
                  <circle cx="18.5" cy="18.5" r="2.5"></circle>
                </svg>
                <span>Estimated delivery: {{ product.deliveryDays }}</span>
              </div>
              <div class="delivery-item">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path>
                  <circle cx="12" cy="10" r="3"></circle>
                </svg>
                <span>Free shipping on orders over $200</span>
              </div>
              <div class="delivery-item">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"></path>
                  <polyline points="9 22 9 12 15 12 15 22"></polyline>
                </svg>
                <span>14-day return policy</span>
              </div>
            </div>

            <!-- Features -->
            <div class="product-features">
              <h4>Product Features</h4>
              <ul>
                <li v-for="feature in product.features" :key="feature">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <polyline points="20 6 9 17 4 12"></polyline>
                  </svg>
                  {{ feature }}
                </li>
              </ul>
            </div>
          </div>
        </div>

        <!-- Product Details Tabs -->
        <div class="product-tabs">
          <div class="tabs-header" role="tablist" aria-label="Product information tabs">
            <button 
              class="tab-btn"
              :class="{ 'tab-btn--active': activeTab === 'description' }"
              @click="activeTab = 'description'"
              role="tab"
              :aria-selected="activeTab === 'description'"
              aria-controls="tab-description"
            >
              Description
            </button>
            <button 
              class="tab-btn"
              :class="{ 'tab-btn--active': activeTab === 'material' }"
              @click="activeTab = 'material'"
              role="tab"
              :aria-selected="activeTab === 'material'"
              aria-controls="tab-material"
            >
              Material & Care
            </button>
            <button 
              class="tab-btn"
              :class="{ 'tab-btn--active': activeTab === 'reviews' }"
              @click="activeTab = 'reviews'"
              role="tab"
              :aria-selected="activeTab === 'reviews'"
              aria-controls="tab-reviews"
            >
              Reviews ({{ product.reviews }})
            </button>
          </div>
          <div class="tabs-content">
            <!-- Description Tab -->
            <div 
              v-show="activeTab === 'description'"
              id="tab-description"
              class="tab-panel"
              role="tabpanel"
              aria-labelledby="tab-description"
            >
              <h3>About This Piece</h3>
              <p>{{ product.description }}</p>
              <p>
                Each piece in our collection represents hours of meticulous handwork by skilled artisans 
                who have dedicated their lives to preserving traditional Russian embroidery techniques. 
                The intricate floral patterns are inspired by regional folk art from across Russia, 
                from the delicate white-on-white embroidery of Vladimir to the vibrant colors of Gzhel ceramics.
              </p>
              <div class="features-list">
                <h4>Key Features</h4>
                <ul>
                  <li v-for="(feature, index) in product.features" :key="index">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
                      <polyline points="20 6 9 17 4 12"></polyline>
                    </svg>
                    {{ feature }}
                  </li>
                </ul>
              </div>
            </div>

            <!-- Material & Care Tab -->
            <div 
              v-show="activeTab === 'material'"
              id="tab-material"
              class="tab-panel"
              role="tabpanel"
              aria-labelledby="tab-material"
            >
              <h3>Material Information</h3>
              <div class="material-info">
                <div class="info-item">
                  <h4>Fabric Composition</h4>
                  <p>{{ product.material }}</p>
                </div>
                <div class="info-item">
                  <h4>Care Instructions</h4>
                  <p>{{ product.careInstructions }}</p>
                </div>
                <div class="info-item">
                  <h4>Expected Delivery</h4>
                  <p>{{ product.deliveryDays }}</p>
                </div>
              </div>
              <div class="care-tips">
                <h4>Care Tips</h4>
                <ul>
                  <li>Dry clean recommended to preserve embroidery details</li>
                  <li>Store in a cool, dry place away from direct sunlight</li>
                  <li>Avoid folding for extended periods; hang on padded hangers</li>
                  <li>Keep away from moths using cedar blocks or lavender sachets</li>
                  <li>Handle with clean hands to prevent staining delicate fabrics</li>
                </ul>
              </div>
            </div>

            <!-- Reviews Tab -->
            <div 
              v-show="activeTab === 'reviews'"
              id="tab-reviews"
              class="tab-panel"
              role="tabpanel"
              aria-labelledby="tab-reviews"
            >
              <h3>Customer Reviews</h3>
              <div class="reviews-summary">
                <div class="rating-info">
                  <div class="rating-number">{{ product.rating }}</div>
                  <div class="rating-stars">
                    <svg v-for="n in 5" :key="n" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" :fill="n <= Math.floor(product.rating) ? 'currentColor' : 'none'" stroke="currentColor" stroke-width="2" aria-hidden="true">
                      <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
                    </svg>
                  </div>
                  <p class="review-count">Based on {{ product.reviews }} verified reviews</p>
                </div>
              </div>
              <div class="reviews-list">
                <div v-for="n in Math.min(3, product.reviews)" :key="n" class="review-item">
                  <div class="review-header">
                    <div class="review-author">Verified Customer</div>
                    <div class="review-rating">
                      <svg v-for="s in 5" :key="s" xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" :fill="s <= (5 - n % 2) ? 'currentColor' : 'none'" stroke="currentColor" stroke-width="2" aria-hidden="true">
                        <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
                      </svg>
                    </div>
                  </div>
                  <p class="review-text">This is absolutely beautiful! The craftsmanship is exceptional and the colors are even more stunning in person. Worth every penny!</p>
                </div>
              </div>
              <div class="reviews-cta">
                <p>Be the first to share your experience</p>
                <button class="btn btn-outline">Write a Review</button>
              </div>
            </div>
          </div>
        </div>

        <!-- Related Products -->
        <div class="related-products">
          <h3>You May Also Like</h3>
          <div class="related-grid">
            <RouterLink 
              v-for="item in relatedProducts" 
              :key="item.id"
              :to="`/products/${item.id}`"
              class="related-card"
            >
              <div class="related-image">
                <img :src="item.image" :alt="item.name" loading="lazy" />
              </div>
              <div class="related-info">
                <h4>{{ item.name }}</h4>
                <span class="related-price">${{ item.price.toLocaleString() }}</span>
              </div>
            </RouterLink>
          </div>
        </div>
      </div>
    </div>

    <!-- Size Guide Modal -->
    <div v-if="showSizeGuide" class="modal-overlay" @click.self="showSizeGuide = false">
      <div class="size-guide-modal">
        <button class="modal-close" @click="showSizeGuide = false">
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="18" y1="6" x2="6" y2="18"></line>
            <line x1="6" y1="6" x2="18" y2="18"></line>
          </svg>
        </button>
        <h3>Size Guide</h3>
        <table class="size-table">
          <thead>
            <tr>
              <th>Size</th>
              <th>Bust (inches)</th>
              <th>Waist (inches)</th>
              <th>Length (inches)</th>
            </tr>
          </thead>
          <tbody>
            <tr><td>XS</td><td>32</td><td>24</td><td>48</td></tr>
            <tr><td>S</td><td>34</td><td>26</td><td>49</td></tr>
            <tr><td>M</td><td>36</td><td>28</td><td>50</td></tr>
            <tr><td>L</td><td>38</td><td>30</td><td>51</td></tr>
            <tr><td>XL</td><td>40</td><td>32</td><td>52</td></tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Toast Notification -->
    <div v-if="showToast" class="pdp-toast" :class="toastMessage.includes('Added to cart') ? 'pdp-toast--success' : 'pdp-toast--info'">
      {{ toastMessage }}
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
  flex-wrap: wrap;
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
   LAYOUT
   ======================================== */
.product-content {
  padding: var(--space-10) 0 var(--space-16);
  background: var(--color-background);
}

.product-layout {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-12);
}

/* ========================================
   GALLERY
   ======================================== */
.product-gallery {
  position: sticky;
  top: 100px;
}

.gallery-main {
  position: relative;
  border-radius: var(--radius-xl);
  overflow: hidden;
  margin-bottom: var(--space-4);
  cursor: default;
}

.gallery-main--zoomed .gallery-image {
  transform: scale(2);
  cursor: zoom-out;
}

.gallery-image {
  width: 100%;
  aspect-ratio: 4/5;
  object-fit: cover;
  transition: transform var(--transition-fast) ease-out;
  user-select: none;
}

/* Gallery Navigation Controls */
.gallery-nav {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 48px;
  height: 48px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.9);
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--color-charcoal);
  transition: all var(--transition-fast);
  z-index: 10;
  backdrop-filter: blur(4px);
}

.gallery-nav:hover {
  background: white;
  box-shadow: var(--shadow-md);
  transform: translateY(-50%) scale(1.1);
}

.gallery-nav--prev {
  left: var(--space-4);
}

.gallery-nav--next {
  right: var(--space-4);
}

/* Zoom Button */
.gallery-zoom-btn {
  position: absolute;
  bottom: var(--space-4);
  right: var(--space-4);
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background: rgba(114, 47, 55, 0.9);
  border: none;
  cursor: pointer;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all var(--transition-fast);
  z-index: 10;
  backdrop-filter: blur(4px);
}

.gallery-zoom-btn:hover {
  background: var(--color-burgundy-deep);
  transform: scale(1.1);
  box-shadow: var(--shadow-md);
}

.gallery-zoom-btn:focus-visible {
  outline: 2px solid var(--color-burgundy);
  outline-offset: 2px;
}

/* Zoom Indicator */
.zoom-indicator {
  position: absolute;
  top: var(--space-4);
  right: var(--space-4);
  display: flex;
  align-items: center;
  gap: var(--space-2);
  background: rgba(114, 47, 55, 0.9);
  color: white;
  padding: var(--space-2) var(--space-3);
  border-radius: var(--radius-md);
  font-size: var(--text-xs);
  backdrop-filter: blur(4px);
  animation: fadeIn 0.3s ease;
}

.zoom-indicator svg {
  width: 16px;
  height: 16px;
}

/* Gallery Help Text */
.gallery-help-text {
  font-size: var(--text-xs);
  color: var(--color-warm-gray);
  text-align: center;
  margin-top: var(--space-3);
  padding: var(--space-2) var(--space-3);
  background: rgba(27, 77, 62, 0.05);
  border-radius: var(--radius-md);
  border-left: 2px solid var(--color-forest-green);
}

.gallery-help-text p {
  margin: 0;
}

.gallery-badges {
  position: absolute;
  top: var(--space-4);
  left: var(--space-4);
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.gallery-thumbnails {
  display: flex;
  gap: var(--space-3);
}

.thumbnail {
  width: 80px;
  height: 100px;
  border-radius: var(--radius-lg);
  overflow: hidden;
  border: 2px solid transparent;
  cursor: pointer;
  transition: all var(--transition-fast) var(--ease-out);
  background: none;
  padding: 0;
}

.thumbnail:hover {
  border-color: var(--color-warm-gray-light);
}

.thumbnail--active {
  border-color: var(--color-burgundy);
}

.thumbnail img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

/* ========================================
   PRODUCT INFO
   ======================================== */
.product-info {
  padding: var(--space-4) 0;
}

.product-header {
  margin-bottom: var(--space-6);
}

.product-category {
  font-size: var(--text-sm);
  color: var(--color-burgundy);
  text-transform: uppercase;
  letter-spacing: 0.1em;
}

.product-title {
  font-family: var(--font-heading);
  font-size: var(--text-4xl);
  color: var(--color-charcoal);
  line-height: 1.2;
  margin: var(--space-2) 0 var(--space-4);
}

.product-rating {
  display: flex;
  align-items: center;
  gap: var(--space-3);
}

.rating-stars {
  display: flex;
  color: var(--color-gold);
}

.rating-text {
  font-size: var(--text-sm);
  color: var(--color-warm-gray);
}

/* Price */
.product-price {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  margin-bottom: var(--space-4);
}

.current-price {
  font-family: var(--font-heading);
  font-size: var(--text-3xl);
  color: var(--color-burgundy);
  font-weight: var(--font-bold);
}

.original-price {
  font-size: var(--text-xl);
  color: var(--color-warm-gray-light);
  text-decoration: line-through;
}

.discount-badge {
  background: var(--color-burgundy);
  color: var(--vt-c-white);
  padding: var(--space-1) var(--space-3);
  border-radius: var(--radius-full);
  font-size: var(--text-xs);
  font-weight: var(--font-bold);
}

/* Stock Status */
.stock-status {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  font-size: var(--text-sm);
  margin-bottom: var(--space-6);
}

.stock-dot {
  width: 8px;
  height: 8px;
  border-radius: var(--radius-full);
}

.in-stock .stock-dot { background: var(--color-forest-green); }
.in-stock { color: var(--color-forest-green); }

.low-stock .stock-dot { background: var(--color-gold); }
.low-stock { color: var(--color-gold); }

.out-of-stock .stock-dot { background: var(--color-burgundy); }
.out-of-stock { color: var(--color-burgundy); }

/* Options */
.product-option {
  margin-bottom: var(--space-6);
}

.option-label {
  display: block;
  font-size: var(--text-sm);
  color: var(--color-text);
  margin-bottom: var(--space-3);
}

.option-label strong {
  color: var(--color-charcoal);
}

.color-options {
  display: flex;
  gap: var(--space-3);
}

.color-swatch {
  width: 40px;
  height: 40px;
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

.size-options {
  display: flex;
  gap: var(--space-2);
  flex-wrap: wrap;
}

.size-btn {
  min-width: 50px;
  height: 50px;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  background: var(--color-background);
  font-size: var(--text-sm);
  font-weight: var(--font-medium);
  cursor: pointer;
  transition: all var(--transition-fast) var(--ease-out);
}

.size-btn:hover {
  border-color: var(--color-burgundy);
}

.size-btn--active {
  background: var(--color-burgundy);
  border-color: var(--color-burgundy);
  color: var(--vt-c-white);
}

.size-guide-link {
  display: inline-block;
  margin-top: var(--space-2);
  font-size: var(--text-sm);
  color: var(--color-burgundy);
  text-decoration: underline;
  background: none;
  border: none;
  cursor: pointer;
}

/* Quantity */
.quantity-selector {
  display: inline-flex;
  align-items: center;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
}

.qty-btn {
  width: 44px;
  height: 44px;
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
  min-width: 50px;
  text-align: center;
  font-size: var(--text-lg);
  font-weight: var(--font-medium);
}

/* Actions */
.product-actions {
  display: flex;
  gap: var(--space-4);
  margin-bottom: var(--space-8);
}

.add-to-cart-btn {
  flex: 1;
}

.wishlist-btn {
  width: 56px;
  height: 56px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  background: var(--color-background);
  cursor: pointer;
  transition: all var(--transition-fast) var(--ease-out);
}

.wishlist-btn:hover {
  border-color: var(--color-burgundy);
  color: var(--color-burgundy);
}

.wishlist-btn--active {
  background: var(--color-burgundy);
  border-color: var(--color-burgundy);
  color: var(--vt-c-white);
}

/* Delivery Info */
.delivery-info {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
  padding: var(--space-4);
  background: var(--color-cream);
  border-radius: var(--radius-lg);
  margin-bottom: var(--space-6);
}

.delivery-item {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  font-size: var(--text-sm);
  color: var(--color-text);
}

.delivery-item svg {
  color: var(--color-burgundy);
  flex-shrink: 0;
}

/* Features */
.product-features h4 {
  font-family: var(--font-heading);
  font-size: var(--text-lg);
  margin-bottom: var(--space-3);
}

.product-features ul {
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.product-features li {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  font-size: var(--text-sm);
  color: var(--color-text);
}

.product-features li svg {
  color: var(--color-forest-green);
  flex-shrink: 0;
}

/* ========================================
   TABS
   ======================================== */
.product-tabs {
  margin-top: var(--space-16);
  padding-top: var(--space-10);
  border-top: 1px solid var(--color-border-light);
}

.tabs-header {
  display: flex;
  gap: var(--space-1);
  margin-bottom: var(--space-6);
  border-bottom: 1px solid var(--color-border-light);
}

.tab-btn {
  padding: var(--space-4) var(--space-6);
  background: transparent;
  border: none;
  font-size: var(--text-base);
  font-weight: var(--font-medium);
  color: var(--color-warm-gray);
  cursor: pointer;
  position: relative;
  transition: color var(--transition-fast);
}

.tab-btn:hover {
  color: var(--color-charcoal);
}

.tab-btn--active {
  color: var(--color-burgundy);
}

.tab-btn--active::after {
  content: '';
  position: absolute;
  bottom: -1px;
  left: 0;
  right: 0;
  height: 2px;
  background: var(--color-burgundy);
}

.tab-panel h3 {
  font-family: var(--font-heading);
  font-size: var(--text-xl);
  margin-bottom: var(--space-4);
}

.tab-panel p {
  line-height: var(--leading-relaxed);
  color: var(--color-text);
  margin-bottom: var(--space-4);
}

/* Features List */
.features-list {
  margin-top: var(--space-6);
  padding: var(--space-6);
  background: rgba(27, 77, 62, 0.05);
  border-radius: var(--radius-lg);
}

.features-list h4 {
  font-size: var(--text-lg);
  margin-bottom: var(--space-4);
  color: var(--color-charcoal);
}

.features-list ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.features-list li {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding: var(--space-2) 0;
  color: var(--color-text);
}

.features-list svg {
  color: var(--color-forest-green);
  flex-shrink: 0;
}

/* Material Info */
.material-info {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--space-6);
  margin-bottom: var(--space-6);
}

.info-item h4 {
  font-size: var(--text-base);
  margin-bottom: var(--space-2);
  color: var(--color-burgundy);
}

.info-item p {
  color: var(--color-text);
  line-height: var(--leading-relaxed);
}

/* Care Tips */
.care-tips {
  padding: var(--space-6);
  background: rgba(114, 47, 55, 0.05);
  border-radius: var(--radius-lg);
  border-left: 4px solid var(--color-burgundy);
}

.care-tips h4 {
  font-size: var(--text-lg);
  margin-bottom: var(--space-4);
  color: var(--color-charcoal);
}

.care-tips ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.care-tips li {
  padding: var(--space-2) 0;
  padding-left: var(--space-4);
  position: relative;
  color: var(--color-text);
}

.care-tips li::before {
  content: '•';
  position: absolute;
  left: 0;
  color: var(--color-burgundy);
  font-weight: bold;
}

/* Reviews Summary */
.reviews-summary {
  padding: var(--space-6);
  background: rgba(27, 77, 62, 0.05);
  border-radius: var(--radius-lg);
  margin-bottom: var(--space-6);
}

.rating-info {
  display: flex;
  align-items: center;
  gap: var(--space-4);
}

.rating-number {
  font-family: var(--font-display);
  font-size: var(--text-5xl);
  font-weight: var(--font-bold);
  color: var(--color-burgundy);
  line-height: 1;
}

.rating-stars {
  display: flex;
  gap: var(--space-1);
  margin-bottom: var(--space-2);
}

.rating-stars svg {
  color: var(--color-gold);
}

.review-count {
  font-size: var(--text-sm);
  color: var(--color-warm-gray);
  margin: 0;
}

/* Reviews List */
.reviews-list {
  margin-bottom: var(--space-8);
}

.review-item {
  padding: var(--space-6);
  border: 1px solid var(--color-border-light);
  border-radius: var(--radius-lg);
  margin-bottom: var(--space-4);
}

.review-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--space-3);
}

.review-author {
  font-weight: var(--font-medium);
  color: var(--color-charcoal);
}

.review-rating {
  display: flex;
  gap: var(--space-1);
}

.review-rating svg {
  color: var(--color-gold);
}

.review-text {
  color: var(--color-text);
  line-height: var(--leading-relaxed);
  margin: 0;
}

/* Reviews CTA */
.reviews-cta {
  padding: var(--space-6);
  background: rgba(27, 77, 62, 0.05);
  border-radius: var(--radius-lg);
  text-align: center;
}

.reviews-cta p {
  margin-bottom: var(--space-4);
  color: var(--color-text);
}

.reviews-cta .btn {
  margin: 0 auto;
}

/* ========================================
   RELATED PRODUCTS
   ======================================== */
.related-products {
  margin-top: var(--space-16);
}

.related-products h3 {
  font-family: var(--font-heading);
  font-size: var(--text-2xl);
  text-align: center;
  margin-bottom: var(--space-8);
}

.related-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: var(--space-6);
}

.related-card {
  display: block;
  text-decoration: none;
  border-radius: var(--radius-xl);
  overflow: hidden;
  transition: all var(--transition-base) var(--ease-out);
}

.related-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-card-hover);
}

.related-image {
  aspect-ratio: 4/5;
  overflow: hidden;
}

.related-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s var(--ease-out);
}

.related-card:hover .related-image img {
  transform: scale(1.05);
}

.related-info {
  padding: var(--space-4);
  background: var(--color-cream);
}

.related-info h4 {
  font-family: var(--font-heading);
  font-size: var(--text-base);
  color: var(--color-charcoal);
  margin-bottom: var(--space-1);
}

.related-price {
  font-family: var(--font-heading);
  font-size: var(--text-lg);
  color: var(--color-burgundy);
}

/* ========================================
   SIZE GUIDE MODAL
   ======================================== */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: var(--z-modal);
}

.size-guide-modal {
  background: var(--color-background);
  border-radius: var(--radius-xl);
  padding: var(--space-8);
  max-width: 600px;
  width: 90%;
  position: relative;
}

.modal-close {
  position: absolute;
  top: var(--space-4);
  right: var(--space-4);
  background: none;
  border: none;
  cursor: pointer;
  padding: var(--space-2);
}

.size-guide-modal h3 {
  font-family: var(--font-heading);
  font-size: var(--text-2xl);
  margin-bottom: var(--space-6);
}

.size-table {
  width: 100%;
  border-collapse: collapse;
}

.size-table th,
.size-table td {
  padding: var(--space-3);
  text-align: center;
  border-bottom: 1px solid var(--color-border-light);
}

.size-table th {
  font-size: var(--text-sm);
  color: var(--color-warm-gray);
  font-weight: var(--font-medium);
}

/* ========================================
   LOADING STATE
   ======================================== */
.product-loading {
  padding: var(--space-10) 0;
}

.loading-layout {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-12);
}

.skeleton-gallery {
  aspect-ratio: 4/5;
  background: linear-gradient(90deg, var(--color-border-light) 25%, var(--color-background-soft) 50%, var(--color-border-light) 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  border-radius: var(--radius-xl);
}

.skeleton-info {
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
}

.skeleton-line {
  height: 24px;
  background: linear-gradient(90deg, var(--color-border-light) 25%, var(--color-background-soft) 50%, var(--color-border-light) 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  border-radius: var(--radius-md);
}

.skeleton-line.wide { width: 100%; height: 48px; }
.skeleton-line.medium { width: 70%; }
.skeleton-line.short { width: 40%; }

@keyframes shimmer {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}

/* Toast Notification */
.pdp-toast {
  position: fixed;
  bottom: 24px;
  left: 24px;
  right: 24px;
  max-width: 400px;
  padding: 16px 20px;
  background: var(--color-background);
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
  font-size: var(--text-sm);
  animation: slideUp 0.3s ease forwards;
  z-index: 1000;
}

.pdp-toast--success {
  border-left: 4px solid var(--color-forest-green);
}

.pdp-toast--info {
  border-left: 4px solid var(--color-gold);
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

/* ========================================
   RESPONSIVE
   ======================================== */
@media (max-width: 1024px) {
  .product-layout {
    grid-template-columns: 1fr;
    gap: var(--space-8);
  }
  
  .product-gallery {
    position: static;
  }
  
  .related-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 640px) {
  .product-title {
    font-size: var(--text-3xl);
  }
  
  .gallery-thumbnails {
    overflow-x: auto;
    padding-bottom: var(--space-2);
  }
  
  .thumbnail {
    width: 60px;
    flex-shrink: 0;
  }
  
  .product-actions {
    flex-direction: column;
  }
  
  .wishlist-btn {
    width: 100%;
  }
  
  .related-grid {
    grid-template-columns: 1fr;
    max-width: 300px;
    margin: 0 auto;
  }
}
</style>

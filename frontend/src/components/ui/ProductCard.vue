<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import BaseBadge from './BaseBadge.vue'
import { useCartStore } from '../../stores/cartStore'
import { useWishlistStore } from '../../stores/wishlistStore'

interface Props {
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
  isNew?: boolean
  isPremium?: boolean
  isSale?: boolean
}

const props = defineProps<Props>()

const cartStore = useCartStore()
const wishlistStore = useWishlistStore()

const isHovered = ref(false)
const isAddingToCart = ref(false)

const addToCart = () => {
  isAddingToCart.value = true
  setTimeout(() => {
    cartStore.addItem({
      id: props.id,
      productId: props.id,
      name: props.name,
      price: props.price,
      image: props.image,
      color: 'Default',
      size: 'M',
      quantity: 1,
      stock: 10
    })
    isAddingToCart.value = false
  }, 300)
}

const toggleWishlist = () => {
  wishlistStore.toggleItem({
    id: props.id,
    name: props.name,
    price: props.price,
    image: props.image,
    rating: props.rating,
    reviews: props.reviews,
    addedAt: Date.now()
  })
}

const isInWishlist = () => wishlistStore.items.some((item: { id: string }) => item.id === props.id)
</script>

<template>
  <article 
    class="product-card"
    @mouseenter="isHovered = true"
    @mouseleave="isHovered = false"
  >
    <RouterLink :to="`/products/${id}`" class="product-card__link">
      <!-- Image Container -->
      <div class="product-card__image-container">
        <img 
          :src="isHovered && hoverImage ? hoverImage : image" 
          :alt="name"
          class="product-card__image"
          loading="lazy"
        />
        
        <!-- Badges -->
        <div class="product-card__badges">
          <BaseBadge v-if="isNew" variant="new" size="sm" />
          <BaseBadge v-if="isPremium" variant="premium" size="sm" />
          <BaseBadge v-if="isSale" variant="sale" size="sm" />
        </div>

        <!-- Quick Actions Overlay -->
        <div class="product-card__actions" :class="{ 'product-card__actions--visible': isHovered }">
          <button 
            class="product-card__action-btn"
            :class="{ 'product-card__action-btn--active': isInWishlist() }"
            @click.prevent="toggleWishlist"
            :aria-label="isInWishlist() ? 'Remove from wishlist' : 'Add to wishlist'"
          >
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" :fill="isInWishlist() ? 'currentColor' : 'none'" stroke="currentColor" stroke-width="2">
              <path d="M19 14c1.49-1.46 3-3.21 3-5.5A5.5 5.5 0 0 0 16.5 3c-1.76 0-3 .5-4.5 2-1.5-1.5-2.74-2-4.5-2A5.5 5.5 0 0 0 2 8.5c0 2.3 1.5 4.05 3 5.5l7 7Z"></path>
            </svg>
          </button>
        </div>
      </div>

      <!-- Product Info -->
      <div class="product-card__info">
        <span class="product-card__category">{{ category }}</span>
        <h3 class="product-card__title">{{ name }}</h3>
        
        <!-- Rating -->
        <div class="product-card__rating">
          <div class="product-card__stars">
            <svg v-for="n in 5" :key="n" xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" :fill="n <= Math.floor(rating) ? 'currentColor' : 'none'" stroke="currentColor" stroke-width="2">
              <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
            </svg>
          </div>
          <span class="product-card__reviews">{{ reviews }} reviews</span>
        </div>

        <!-- Price -->
        <div class="product-card__price">
          <span class="product-card__price-current">${{ price.toLocaleString() }}</span>
          <span v-if="originalPrice" class="product-card__price-original">${{ originalPrice.toLocaleString() }}</span>
        </div>
      </div>
    </RouterLink>

    <!-- Add to Cart Button -->
    <button 
      class="product-card__add-btn"
      :class="{ 'product-card__add-btn--loading': isAddingToCart }"
      @click.prevent="addToCart"
      :disabled="isAddingToCart"
    >
      <span v-if="!isAddingToCart">Add to Cart</span>
      <span v-else class="product-card__spinner"></span>
    </button>
  </article>
</template>

<style scoped>
.product-card {
  display: flex;
  flex-direction: column;
  background: var(--color-background, white);
  border-radius: var(--radius-lg, 12px);
  overflow: hidden;
  transition: all var(--transition-base, 0.3s) var(--ease-out, cubic-bezier(0.4, 0, 0.2, 1));
}

.product-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 32px rgba(44, 40, 37, 0.12);
}

.product-card__link {
  text-decoration: none;
  color: inherit;
}

.product-card__image-container {
  position: relative;
  aspect-ratio: 3/4;
  overflow: hidden;
  background: var(--color-background-soft, #FDF8F3);
}

.product-card__image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform var(--transition-slow, 0.5s) var(--ease-out);
}

.product-card:hover .product-card__image {
  transform: scale(1.05);
}

.product-card__badges {
  position: absolute;
  top: var(--space-3, 12px);
  left: var(--space-3, 12px);
  display: flex;
  flex-direction: column;
  gap: var(--space-2, 8px);
}

.product-card__actions {
  position: absolute;
  top: var(--space-3, 12px);
  right: var(--space-3, 12px);
  display: flex;
  flex-direction: column;
  gap: var(--space-2, 8px);
  opacity: 0;
  transform: translateX(8px);
  transition: all var(--transition-fast, 0.2s) var(--ease-out);
}

.product-card__actions--visible {
  opacity: 1;
  transform: translateX(0);
}

.product-card__action-btn {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
  border: none;
  border-radius: 50%;
  cursor: pointer;
  color: var(--color-warm-gray, #8B8178);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: all var(--transition-fast, 0.2s);
}

.product-card__action-btn:hover {
  color: var(--color-burgundy, #722F37);
  transform: scale(1.1);
}

.product-card__action-btn--active {
  color: var(--color-burgundy, #722F37);
}

.product-card__info {
  padding: var(--space-4, 16px);
  display: flex;
  flex-direction: column;
  gap: var(--space-2, 8px);
}

.product-card__category {
  font-size: var(--text-xs, 0.75rem);
  color: var(--color-warm-gray, #8B8178);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.product-card__title {
  font-family: var(--font-heading, 'Cormorant Garamond', serif);
  font-size: var(--text-lg, 1.125rem);
  font-weight: var(--font-medium, 500);
  color: var(--color-heading, #2C2825);
  margin: 0;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.product-card__rating {
  display: flex;
  align-items: center;
  gap: var(--space-2, 8px);
}

.product-card__stars {
  display: flex;
  color: var(--color-gold, #C9A962);
}

.product-card__reviews {
  font-size: var(--text-xs, 0.75rem);
  color: var(--color-warm-gray, #8B8178);
}

.product-card__price {
  display: flex;
  align-items: center;
  gap: var(--space-2, 8px);
  margin-top: var(--space-1, 4px);
}

.product-card__price-current {
  font-family: var(--font-heading, 'Cormorant Garamond', serif);
  font-size: var(--text-xl, 1.25rem);
  font-weight: var(--font-semibold, 600);
  color: var(--color-burgundy, #722F37);
}

.product-card__price-original {
  font-size: var(--text-sm, 0.875rem);
  color: var(--color-warm-gray-light, #A39B94);
  text-decoration: line-through;
}

.product-card__add-btn {
  margin: 0 var(--space-4, 16px) var(--space-4, 16px);
  padding: var(--space-3, 12px);
  background: transparent;
  border: 1.5px solid var(--color-burgundy, #722F37);
  color: var(--color-burgundy, #722F37);
  font-family: var(--font-body, 'Inter', sans-serif);
  font-size: var(--text-sm, 0.875rem);
  font-weight: var(--font-medium, 500);
  border-radius: var(--radius-md, 8px);
  cursor: pointer;
  transition: all var(--transition-fast, 0.2s);
}

.product-card__add-btn:hover:not(:disabled) {
  background: var(--color-burgundy, #722F37);
  color: white;
}

.product-card__add-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.product-card__spinner {
  display: inline-block;
  width: 16px;
  height: 16px;
  border: 2px solid rgba(114, 47, 55, 0.3);
  border-radius: 50%;
  border-top-color: var(--color-burgundy, #722F37);
  animation: spin 0.6s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>

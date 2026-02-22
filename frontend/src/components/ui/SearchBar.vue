<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRouter } from 'vue-router'

interface SearchResult {
  id: string
  name: string
  price: number
  image: string
  category: string
}

const router = useRouter()
const query = ref('')
const showSuggestions = ref(false)
const isSearching = ref(false)

// Mock search data
const allProducts: SearchResult[] = [
  { id: '1', name: 'Traditional Sarafan Dress', price: 459, image: 'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=100&h=100&fit=crop', category: 'Dresses' },
  { id: '2', name: 'Embroidered Rubakha Shirt', price: 189, image: 'https://images.unsplash.com/photo-1609192591984-2a6d953a1c2d?w=100&h=100&fit=crop', category: 'Shirts' },
  { id: '3', name: 'Wedding Tsaritsa Gown', price: 1299, image: 'https://images.unsplash.com/photo-1566174053879-31528523f8ae?w=100&h=100&fit=crop', category: 'Dresses' },
  { id: '4', name: 'Folk Patterned Shawl', price: 129, image: 'https://images.unsplash.com/photo-1601924994987-69e26d50dc26?w=100&h=100&fit=crop', category: 'Accessories' },
  { id: '5', name: 'Traditional Kosovorotka', price: 245, image: 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=100&h=100&fit=crop', category: 'Shirts' },
  { id: '6', name: 'Royal Peasant Blouse', price: 179, image: 'https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?w=100&h=100&fit=crop', category: 'Dresses' },
  { id: '7', name: 'Velvet Sarafan', price: 599, image: 'https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=100&h=100&fit=crop', category: 'Dresses' },
  { id: '8', name: 'Woolen Matryoshka Coat', price: 459, image: 'https://images.unsplash.com/photo-1539533113208-f6df8cc8b543?w=100&h=100&fit=crop', category: 'Outerwear' },
  { id: '9', name: 'Embroidered Apron', price: 89, image: 'https://images.unsplash.com/photo-1589310243389-96a5483213a8?w=100&h=100&fit=crop', category: 'Accessories' },
  { id: '10', name: 'Traditional Tulu Hat', price: 145, image: 'https://images.unsplash.com/photo-1514327605112-b887c0e61c0a?w=100&h=100&fit=crop', category: 'Accessories' },
]

const suggestions = computed(() => {
  if (!query.value.trim()) return []
  
  const searchTerm = query.value.toLowerCase()
  return allProducts
    .filter(product => 
      product.name.toLowerCase().includes(searchTerm) ||
      product.category.toLowerCase().includes(searchTerm)
    )
    .slice(0, 5)
})

const hasResults = computed(() => suggestions.value.length > 0)

const performSearch = async () => {
  if (!query.value.trim()) return
  
  isSearching.value = true
  setTimeout(() => {
    isSearching.value = false
    router.push(`/products?search=${encodeURIComponent(query.value)}`)
    showSuggestions.value = false
    query.value = ''
  }, 300)
}

const selectProduct = (productId: string) => {
  router.push(`/products/${productId}`)
  showSuggestions.value = false
  query.value = ''
}

const handleKeydown = (event: KeyboardEvent) => {
  if (event.key === 'Enter') {
    performSearch()
  } else if (event.key === 'Escape') {
    showSuggestions.value = false
  }
}

watch(query, () => {
  showSuggestions.value = true
})
</script>

<template>
  <div class="search-bar">
    <div class="search-input-wrapper">
      <svg class="search-icon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
        <circle cx="11" cy="11" r="8"></circle>
        <path d="m21 21-4.3-4.3"></path>
      </svg>
      <input 
        v-model="query"
        type="search"
        class="search-input"
        placeholder="Search products..."
        @keydown="handleKeydown"
        @focus="showSuggestions = true"
        aria-label="Search for products"
        aria-autocomplete="list"
        :aria-expanded="showSuggestions"
        aria-controls="search-suggestions"
      />
      <button 
        v-if="query"
        class="search-clear"
        @click="query = ''"
        aria-label="Clear search"
      >
        ✕
      </button>
    </div>

    <!-- Suggestions Dropdown -->
    <div 
      v-if="showSuggestions && query.trim()"
      id="search-suggestions"
      class="search-suggestions"
      role="listbox"
    >
      <div v-if="isSearching" class="search-loading">
        <div class="spinner spinner--sm"></div>
      </div>

      <div v-else-if="hasResults" class="suggestions-list">
        <button 
          v-for="product in suggestions"
          :key="product.id"
          class="suggestion-item"
          @click="selectProduct(product.id)"
          role="option"
        >
          <img :src="product.image" :alt="product.name" class="suggestion-image" />
          <div class="suggestion-content">
            <div class="suggestion-name">{{ product.name }}</div>
            <div class="suggestion-meta">
              <span class="suggestion-category">{{ product.category }}</span>
              <span class="suggestion-price">${{ product.price }}</span>
            </div>
          </div>
        </button>
        
        <button 
          class="suggestion-footer"
          @click="performSearch"
          role="option"
        >
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
            <circle cx="11" cy="11" r="8"></circle>
            <path d="m21 21-4.3-4.3"></path>
          </svg>
          View all results for "{{ query }}"
        </button>
      </div>

      <div v-else class="search-empty">
        <p>No products found for "{{ query }}"</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.search-bar {
  position: relative;
  flex: 1;
  max-width: 400px;
}

.search-input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.search-icon {
  position: absolute;
  left: var(--space-3);
  color: var(--color-warm-gray);
  pointer-events: none;
}

.search-input {
  width: 100%;
  padding: var(--space-2) var(--space-3) var(--space-2) var(--space-10);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  font-size: var(--text-sm);
  transition: all var(--transition-fast);
  background: var(--color-background);
}

.search-input:focus {
  outline: none;
  border-color: var(--color-burgundy);
  box-shadow: 0 0 0 3px rgba(114, 47, 55, 0.1);
}

.search-clear {
  position: absolute;
  right: var(--space-3);
  background: none;
  border: none;
  color: var(--color-warm-gray);
  font-size: var(--text-lg);
  cursor: pointer;
  padding: var(--space-2);
  transition: color var(--transition-fast);
}

.search-clear:hover {
  color: var(--color-burgundy);
}

/* Suggestions Dropdown */
.search-suggestions {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  margin-top: var(--space-2);
  background: var(--color-background);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-lg);
  z-index: 100;
  max-height: 400px;
  overflow-y: auto;
}

.search-loading,
.search-empty {
  padding: var(--space-6) var(--space-4);
  text-align: center;
  color: var(--color-warm-gray);
  font-size: var(--text-sm);
}

.search-loading {
  display: flex;
  align-items: center;
  justify-content: center;
}

.suggestions-list {
  display: flex;
  flex-direction: column;
}

.suggestion-item {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding: var(--space-3) var(--space-4);
  border: none;
  background: transparent;
  cursor: pointer;
  transition: background-color var(--transition-fast);
  text-align: left;
  border-bottom: 1px solid var(--color-border-light);
}

.suggestion-item:hover {
  background-color: var(--color-background-soft);
}

.suggestion-item:focus {
  outline: 2px solid var(--color-burgundy);
  outline-offset: -2px;
}

.suggestion-image {
  width: 48px;
  height: 48px;
  border-radius: var(--radius-md);
  object-fit: cover;
  flex-shrink: 0;
}

.suggestion-content {
  flex: 1;
  min-width: 0;
}

.suggestion-name {
  font-weight: var(--font-medium);
  color: var(--color-charcoal);
  font-size: var(--text-sm);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  margin-bottom: var(--space-1);
}

.suggestion-meta {
  display: flex;
  gap: var(--space-3);
  font-size: var(--text-xs);
}

.suggestion-category {
  color: var(--color-warm-gray);
}

.suggestion-price {
  color: var(--color-burgundy);
  font-weight: var(--font-medium);
}

.suggestion-footer {
  padding: var(--space-3) var(--space-4);
  border: none;
  background: var(--color-background-soft);
  color: var(--color-burgundy);
  font-size: var(--text-sm);
  font-weight: var(--font-medium);
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: var(--space-2);
  transition: all var(--transition-fast);
  text-align: left;
}

.suggestion-footer:hover {
  background: var(--color-background-mute);
}

.suggestion-footer:focus {
  outline: 2px solid var(--color-burgundy);
  outline-offset: -2px;
}

@media (max-width: 640px) {
  .search-bar {
    max-width: 100%;
  }

  .search-suggestions {
    max-height: 300px;
  }
}
</style>

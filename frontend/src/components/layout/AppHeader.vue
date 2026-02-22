<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { RouterLink } from 'vue-router'
import { useCartStore } from '../../stores/cartStore'
import SearchBar from '../ui/SearchBar.vue'

// State
const isScrolled = ref(false)
const isMobileMenuOpen = ref(false)
const searchQuery = ref('')
const cartStore = useCartStore()

// Navigation items
const navItems = [
  { name: 'Women', path: '/products?gender=women' },
  { name: 'Men', path: '/products?gender=men' },
  { name: 'Wedding', path: '/products?occasion=wedding' },
  { name: 'Folk', path: '/products?category=folk' },
  { name: 'Accessories', path: '/products?category=accessories' },
  { name: 'New Collection', path: '/products?collection=new' },
]

// Cart state (now from store)
const isWishlistActive = ref(false)

// Scroll handler
const handleScroll = () => {
  isScrolled.value = window.scrollY > 50
}

// Search handler
const handleSearch = () => {
  if (searchQuery.value.trim()) {
    // Navigate to search results
    window.location.href = `/products?search=${encodeURIComponent(searchQuery.value)}`
  }
}

// Toggle mobile menu
const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}

// Lifecycle
onMounted(() => {
  window.addEventListener('scroll', handleScroll)
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
})
</script>

<template>
  <header class="header" :class="{ 'header--scrolled': isScrolled }">
    <div class="header__container">
      <!-- Mobile Menu Toggle -->
      <button 
        class="header__mobile-toggle hide-desktop" 
        @click="toggleMobileMenu"
        aria-label="Toggle menu"
      >
        <span class="hamburger" :class="{ 'hamburger--active': isMobileMenuOpen }">
          <span></span>
          <span></span>
          <span></span>
        </span>
      </button>

      <!-- Logo -->
      <RouterLink to="/" class="header__logo">
        <span class="logo-text">NATIONAL</span>
        <span class="logo-subtext">CLOTHING STORE</span>
      </RouterLink>

      <!-- Desktop Navigation -->
      <nav class="header__nav hide-mobile" aria-label="Main Navigation">
        <RouterLink 
          v-for="item in navItems" 
          :key="item.path"
          :to="item.path" 
          class="header__nav-link"
          :aria-current="false"
        >
          {{ item.name }}
        </RouterLink>
      </nav>

      <!-- Actions -->
      <div class="header__actions">
        <!-- Search Bar -->
        <SearchBar class="hide-mobile" />

        <!-- Account -->
        <RouterLink to="/account" class="header__action-btn" aria-label="Account">
          <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2"></path>
            <circle cx="12" cy="7" r="4"></circle>
          </svg>
        </RouterLink>

        <!-- Wishlist -->
        <button 
          class="header__action-btn" 
          :class="{ 'header__action-btn--active': isWishlistActive }"
          aria-label="Wishlist"
        >
          <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M19 14c1.49-1.46 3-3.21 3-5.5A5.5 5.5 0 0 0 16.5 3c-1.76 0-3 .5-4.5 2-1.5-1.5-2.74-2-4.5-2A5.5 5.5 0 0 0 2 8.5c0 2.3 1.5 4.05 3 5.5l7 7Z"></path>
          </svg>
        </button>

        <!-- Cart -->
        <RouterLink to="/cart" class="header__cart-btn" aria-label="Cart">
          <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <circle cx="8" cy="21" r="1"></circle>
            <circle cx="19" cy="21" r="1"></circle>
            <path d="M2.05 2.05h2l2.66 12.42a2 2 0 0 0 2 1.58h9.78a2 2 0 0 0 1.95-1.57l1.65-7.43H5.12"></path>
          </svg>
          <span v-if="cartStore.itemCount > 0" class="cart-badge">
            {{ cartStore.itemCount }}
          </span>
        </RouterLink>
      </div>
    </div>

    <!-- Mobile Menu -->
    <div class="mobile-menu" :class="{ 'mobile-menu--open': isMobileMenuOpen }">
      <nav class="mobile-menu__nav">
        <RouterLink 
          v-for="item in navItems" 
          :key="item.path"
          :to="item.path" 
          class="mobile-menu__link"
          @click="isMobileMenuOpen = false"
        >
          {{ item.name }}
        </RouterLink>
      </nav>
    </div>
  </header>
</template>

<style scoped>
/* ========================================
   HEADER - Premium Heritage Style
   ======================================== */
.header {
  position: sticky;
  top: 0;
  left: 0;
  right: 0;
  z-index: var(--z-fixed);
  background: var(--color-cream);
  border-bottom: 1px solid transparent;
  transition: all var(--transition-base) var(--ease-out);
  padding: var(--space-3) 0;
}

.header--scrolled {
  background: var(--color-background);
  border-bottom-color: var(--color-border-light);
  box-shadow: var(--shadow-md);
}

.header__container {
  max-width: var(--container-xl);
  margin: 0 auto;
  padding: var(--space-4) var(--space-6);
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--space-8);
}

/* Logo */
.header__logo {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-decoration: none;
  line-height: 1;
}

.logo-text {
  font-family: var(--font-heading);
  font-size: var(--text-2xl);
  font-weight: var(--font-bold);
  color: var(--color-burgundy);
  letter-spacing: 0.15em;
}

.logo-subtext {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  color: var(--color-gold);
  letter-spacing: 0.3em;
  margin-top: 2px;
}

/* Navigation */
.header__nav {
  display: flex;
  align-items: center;
  gap: var(--space-8);
}

.header__nav-link {
  font-family: var(--font-body);
  font-size: var(--text-sm);
  font-weight: var(--font-medium);
  color: var(--color-text);
  text-decoration: none;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  padding: var(--space-2) 0;
  position: relative;
  transition: color var(--transition-fast) var(--ease-out);
}

.header__nav-link::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 0;
  height: 1px;
  background: var(--color-gold);
  transition: width var(--transition-base) var(--ease-out);
}

.header__nav-link:hover {
  color: var(--color-burgundy);
}

.header__nav-link:hover::after {
  width: 100%;
}

.header__nav-link:focus {
  outline: none;
  color: var(--color-burgundy);
}

.header__nav-link:focus::after {
  width: 100%;
}

.header__nav-link:focus-visible::after {
  width: 100%;
}

/* Actions */
.header__actions {
  display: flex;
  align-items: center;
  gap: var(--space-4);
}

.header__action-btn,
.header__cart-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  color: var(--color-text);
  border-radius: var(--radius-full);
  transition: all var(--transition-fast) var(--ease-out);
  position: relative;
  background: none;
  border: none;
  cursor: pointer;
}

.header__action-btn:hover,
.header__cart-btn:hover {
  background: var(--color-background-soft);
  color: var(--color-burgundy);
}

.header__action-btn:focus,
.header__cart-btn:focus {
  outline: none;
  box-shadow: inset 0 0 0 2px var(--color-burgundy);
}

.header__action-btn:focus-visible,
.header__cart-btn:focus-visible {
  outline: 2px solid var(--color-burgundy);
  outline-offset: 2px;
}

.header__action-btn--active {
  color: var(--color-burgundy);
}

/* Cart Badge */
.cart-badge {
  position: absolute;
  top: 2px;
  right: 2px;
  min-width: 18px;
  height: 18px;
  padding: 0 4px;
  background: var(--color-burgundy);
  color: var(--vt-c-white);
  font-size: 10px;
  font-weight: var(--font-bold);
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  animation: badge-pop 0.3s var(--ease-out);
}

@keyframes badge-pop {
  0% { transform: scale(0); }
  50% { transform: scale(1.2); }
  100% { transform: scale(1); }
}

/* Search */
.header__search {
  position: relative;
}

.header__search-dropdown {
  position: absolute;
  top: 100%;
  right: 0;
  margin-top: var(--space-2);
  background: var(--color-background);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-xl);
  display: none;
  overflow: hidden;
}

.header__search:hover .header__search-dropdown,
.header__search:focus-within .header__search-dropdown {
  display: flex;
}

.search-input {
  padding: var(--space-3) var(--space-4);
  border: none;
  background: transparent;
  width: 280px;
  font-size: var(--text-sm);
}

.search-input:focus {
  outline: none;
}

.search-submit {
  padding: var(--space-3) var(--space-4);
  background: var(--color-burgundy);
  color: var(--vt-c-white);
  border: none;
  cursor: pointer;
  transition: background var(--transition-fast) var(--ease-out);
}

.search-submit:hover {
  background: var(--color-burgundy-deep);
}

/* Mobile Menu Toggle */
.header__mobile-toggle {
  background: none;
  border: none;
  cursor: pointer;
  padding: var(--space-2);
}

.hamburger {
  display: flex;
  flex-direction: column;
  gap: 5px;
  width: 24px;
}

.hamburger span {
  display: block;
  height: 2px;
  background: var(--color-text);
  transition: all var(--transition-base) var(--ease-out);
}

.hamburger--active span:nth-child(1) {
  transform: rotate(45deg) translate(5px, 5px);
}

.hamburger--active span:nth-child(2) {
  opacity: 0;
}

.hamburger--active span:nth-child(3) {
  transform: rotate(-45deg) translate(5px, -5px);
}

/* Mobile Menu */
.mobile-menu {
  position: fixed;
  top: 73px;
  left: 0;
  right: 0;
  background: var(--color-background);
  border-bottom: 1px solid var(--color-border-light);
  transform: translateY(-100%);
  opacity: 0;
  visibility: hidden;
  transition: all var(--transition-base) var(--ease-out);
}

.mobile-menu--open {
  transform: translateY(0);
  opacity: 1;
  visibility: visible;
}

.mobile-menu__nav {
  display: flex;
  flex-direction: column;
  padding: var(--space-4);
}

.mobile-menu__link {
  padding: var(--space-4);
  font-size: var(--text-lg);
  color: var(--color-text);
  text-decoration: none;
  border-bottom: 1px solid var(--color-border-light);
  transition: all var(--transition-fast) var(--ease-out);
}

.mobile-menu__link:hover {
  color: var(--color-burgundy);
  padding-left: var(--space-6);
}

/* Hide on Desktop */
.hide-desktop {
  display: none;
}

@media (min-width: 769px) {
  .hide-desktop {
    display: flex;
  }
  
  .hide-mobile {
    display: flex;
  }
}

@media (max-width: 768px) {
  .hide-mobile {
    display: none;
  }
  
  .header__container {
    padding: var(--space-3) var(--space-4);
  }
  
  .logo-text {
    font-size: var(--text-xl);
  }
  
  .logo-subtext {
    font-size: 8px;
  }
  
  .header__actions {
    gap: var(--space-2);
  }
  
  .header__action-btn,
  .header__cart-btn {
    width: 36px;
    height: 36px;
  }
}
</style>

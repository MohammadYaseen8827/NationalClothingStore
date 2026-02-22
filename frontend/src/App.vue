<script setup lang="ts">
import { onMounted } from 'vue'
import { RouterView } from 'vue-router'
import AppHeader from './components/layout/AppHeader.vue'
import AppFooter from './components/layout/AppFooter.vue'
import { useCartStore } from './stores/cartStore'
import { useWishlistStore } from './stores/wishlistStore'

// Initialize stores from localStorage
onMounted(() => {
  const cartStore = useCartStore()
  const wishlistStore = useWishlistStore()
  cartStore.initializeCart()
  wishlistStore.initializeWishlist()
})
</script>

<template>
  <div class="app">
    <AppHeader />
    <main class="main-content">
      <RouterView />
    </main>
    <AppFooter />
  </div>
</template>

<style scoped>
.app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-content {
  flex: 1;
  margin-top: 73px; /* Header height */
}
</style>

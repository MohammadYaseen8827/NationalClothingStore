<template>
  <div class="product-details">
    <h1>Product Details</h1>
    <p>Product ID: {{ id }}</p>
    <!-- To be implemented -->
  </div>
</template>

<script setup lang="ts">
defineProps<{
  id: string
}>()
</script>

<style scoped>
.product-details {
  padding: 20px;
}
</style>

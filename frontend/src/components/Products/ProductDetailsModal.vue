<template>
  <div v-if="show" class="modal-overlay" @click.self="close">
    <div class="modal">
      <div class="modal-header">
        <h2>Product Details</h2>
        <button class="close-btn" @click="close">&times;</button>
      </div>
      <div class="modal-body">
        <p v-if="product">{{ product.name }}</p>
        <p v-else>No product selected</p>
      </div>
      <div class="modal-footer">
        <button @click="close" class="btn-secondary">Close</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ProductDetailsModal',
  props: {
    show: Boolean,
    product: Object
  },
  emits: ['close'],
  methods: {
    close() {
      this.$emit('close')
    }
  }
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
}
.modal {
  background: white;
  border-radius: 8px;
  padding: 20px;
  min-width: 400px;
}
</style>

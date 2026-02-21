<template>
  <div v-if="show" class="modal-overlay" @click.self="close">
    <div class="modal">
      <div class="modal-header">
        <h2>{{ variation ? 'Edit Variation' : 'Add Variation' }}</h2>
        <button class="close-btn" @click="close">&times;</button>
      </div>
      <div class="modal-body">
        <p>Variation form placeholder</p>
      </div>
      <div class="modal-footer">
        <button @click="close" class="btn-secondary">Cancel</button>
        <button @click="save" class="btn-primary">Save</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'VariationModal',
  props: {
    show: Boolean,
    variation: Object,
    productId: String
  },
  emits: ['close', 'save'],
  methods: {
    close() {
      this.$emit('close')
    },
    save() {
      this.$emit('save', {})
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

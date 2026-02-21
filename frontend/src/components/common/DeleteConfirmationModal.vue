<template>
  <div v-if="show" class="modal-overlay" @click.self="cancel">
    <div class="modal">
      <div class="modal-header">
        <h2>{{ title || 'Confirm Delete' }}</h2>
      </div>
      <div class="modal-body">
        <p>{{ message || 'Are you sure you want to delete this item?' }}</p>
      </div>
      <div class="modal-footer">
        <button @click="cancel" class="btn-secondary">Cancel</button>
        <button @click="confirm" class="btn-danger">Delete</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'DeleteConfirmationModal',
  props: {
    show: Boolean,
    title: String,
    message: String
  },
  emits: ['cancel', 'confirm'],
  methods: {
    cancel() {
      this.$emit('cancel')
    },
    confirm() {
      this.$emit('confirm')
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
  min-width: 350px;
}
.btn-danger {
  background: #dc3545;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
}
</style>

<template>
  <div v-if="show" class="toast" :class="type">
    <span class="message">{{ message }}</span>
    <button @click="close" class="close-btn">&times;</button>
  </div>
</template>

<script>
export default {
  name: 'ToastNotification',
  props: {
    show: Boolean,
    message: String,
    type: {
      type: String,
      default: 'info'
    }
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
.toast {
  position: fixed;
  top: 20px;
  right: 20px;
  padding: 15px 20px;
  border-radius: 4px;
  display: flex;
  align-items: center;
  gap: 10px;
  z-index: 9999;
}
.toast.info { background: #17a2b8; color: white; }
.toast.success { background: #28a745; color: white; }
.toast.error { background: #dc3545; color: white; }
.toast.warning { background: #ffc107; color: #333; }
.close-btn {
  background: none;
  border: none;
  color: inherit;
  font-size: 20px;
  cursor: pointer;
}
</style>

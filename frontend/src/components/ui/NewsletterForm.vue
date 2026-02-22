<script setup lang="ts">
import { ref } from 'vue'

const email = ref('')
const isLoading = ref(false)
const toastMessage = ref('')
const toastType = ref<'success' | 'error'>('success')
const showToast = ref(false)

const validateEmail = (value: string) => {
  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return regex.test(value)
}

const submitNewsletter = async (event: Event) => {
  event.preventDefault()

  // Validation
  if (!email.value.trim()) {
    toastMessage.value = 'Please enter your email'
    toastType.value = 'error'
    showToast.value = true
    setTimeout(() => { showToast.value = false }, 3000)
    return
  }

  if (!validateEmail(email.value)) {
    toastMessage.value = 'Please enter a valid email address'
    toastType.value = 'error'
    showToast.value = true
    setTimeout(() => { showToast.value = false }, 3000)
    return
  }

  // Submit
  isLoading.value = true

  try {
    // API call placeholder - in production, connect to backend
    await new Promise(resolve => setTimeout(resolve, 800))
    
    // Success
    toastMessage.value = 'Welcome! Check your email for exclusive offers'
    toastType.value = 'success'
    showToast.value = true
    email.value = ''
    setTimeout(() => { showToast.value = false }, 3000)
  } catch (error) {
    toastMessage.value = 'Something went wrong. Please try again.'
    toastType.value = 'error'
    showToast.value = true
    setTimeout(() => { showToast.value = false }, 3000)
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="newsletter-form">
    <form @submit="submitNewsletter" class="form">
      <div class="form-group">
        <input 
          v-model="email" 
          type="email"
          placeholder="Enter your email"
          class="form-input"
          :disabled="isLoading"
          aria-label="Email address"
          required
        />
        <button 
          type="submit" 
          class="btn btn-primary"
          :disabled="isLoading"
          aria-label="Subscribe to newsletter"
        >
          <span v-if="!isLoading">Subscribe</span>
          <span v-else class="spinner spinner--sm"></span>
        </button>
      </div>
      <p class="form-text">
        By subscribing, you agree to receive promotional emails and accept our 
        <a href="#privacy">Privacy Policy</a>
      </p>
    </form>

    <!-- Toast -->
    <div v-if="showToast" class="newsletter-toast" :class="`newsletter-toast--${toastType}`">
      {{ toastMessage }}
    </div>
  </div>
</template>

<style scoped>
.newsletter-form {
  width: 100%;
}

.form {
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
}

.form-group {
  display: flex;
  gap: var(--space-2);
}

.form-input {
  flex: 1;
  padding: var(--space-3) var(--space-4);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  font-size: var(--text-base);
  transition: all var(--transition-fast);
}

.form-input:focus {
  outline: none;
  border-color: var(--color-burgundy);
  box-shadow: 0 0 0 3px rgba(114, 47, 55, 0.1);
}

.form-input:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn {
  white-space: nowrap;
  min-width: 120px;
}

.form-text {
  font-size: var(--text-xs);
  color: var(--color-warm-gray);
  line-height: var(--leading-relaxed);
}

.form-text a {
  color: var(--color-burgundy);
  text-decoration: underline;
  transition: color var(--transition-fast);
}

.form-text a:hover {
  color: var(--color-burgundy-deep);
}

/* Toast */
.newsletter-toast {
  padding: 12px 16px;
  margin-top: var(--space-3);
  border-radius: var(--radius-md);
  font-size: var(--text-sm);
  animation: slideUp 0.3s ease forwards;
}

.newsletter-toast--success {
  background: rgba(27, 77, 62, 0.1);
  color: var(--color-forest-green);
  border-left: 3px solid var(--color-forest-green);
}

.newsletter-toast--error {
  background: rgba(114, 47, 55, 0.1);
  color: var(--color-burgundy);
  border-left: 3px solid var(--color-burgundy);
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@media (max-width: 640px) {
  .form-group {
    flex-direction: column;
  }

  .btn {
    width: 100%;
  }
}
</style>

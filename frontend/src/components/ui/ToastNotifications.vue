<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Toast notifications system
interface Toast {
  id: number
  message: string
  type: 'success' | 'error' | 'info'
}

const toasts = ref<Toast[]>([])
let toastId = 0

const showToast = (message: string, type: 'success' | 'error' | 'info' = 'success') => {
  const id = ++toastId
  toasts.value.push({ id, message, type })
  setTimeout(() => {
    toasts.value = toasts.value.filter(t => t.id !== id)
  }, 3000)
}

// Expose for global use
defineExpose({ showToast })

// Scroll animations
const scrollElements = ref<HTMLElement[]>([])

onMounted(() => {
  const observer = new IntersectionObserver(
    (entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('animate-in')
        }
      })
    },
    { threshold: 0.1 }
  )

  document.querySelectorAll('.scroll-animate').forEach(el => {
    observer.observe(el)
  })
})

// Button ripple effect
const createRipple = (event: MouseEvent) => {
  const button = event.currentTarget as HTMLElement
  const ripple = document.createElement('span')
  ripple.classList.add('ripple')
  
  const rect = button.getBoundingClientRect()
  const size = Math.max(rect.width, rect.height)
  
  ripple.style.width = ripple.style.height = `${size}px`
  ripple.style.left = `${event.clientX - rect.left - size / 2}px`
  ripple.style.top = `${event.clientY - rect.top - size / 2}px`
  
  button.appendChild(ripple)
  
  setTimeout(() => ripple.remove(), 600)
}

// Number counter animation
const animateNumber = (el: HTMLElement, target: number, duration: number = 2000) => {
  const start = 0
  const startTime = performance.now()
  
  const update = (currentTime: number) => {
    const elapsed = currentTime - startTime
    const progress = Math.min(elapsed / duration, 1)
    const eased = 1 - Math.pow(1 - progress, 3)
    
    el.textContent = Math.round(start + (target - start) * eased).toString()
    
    if (progress < 1) {
      requestAnimationFrame(update)
    }
  }
  
  requestAnimationFrame(update)
}

defineExpose({
  showToast,
  createRipple,
  animateNumber
})
</script>

<template>
  <Teleport to="body">
    <div class="toast-container">
      <TransitionGroup name="toast">
        <div 
          v-for="toast in toasts" 
          :key="toast.id" 
          class="toast"
          :class="`toast--${toast.type}`"
        >
          <svg v-if="toast.type === 'success'" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path>
            <polyline points="22 4 12 14.01 9 11.01"></polyline>
          </svg>
          <svg v-else-if="toast.type === 'error'" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="12" cy="12" r="10"></circle>
            <line x1="15" y1="9" x2="9" y2="15"></line>
            <line x1="9" y1="9" x2="15" y2="15"></line>
          </svg>
          <svg v-else xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="12" cy="12" r="10"></circle>
            <line x1="12" y1="16" x2="12" y2="12"></line>
            <line x1="12" y1="8" x2="12.01" y2="8"></line>
          </svg>
          <span>{{ toast.message }}</span>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<style scoped>
.toast-container {
  position: fixed;
  top: 100px;
  right: 20px;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.toast {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px 20px;
  background: var(--color-background);
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
  min-width: 300px;
  max-width: 400px;
}

.toast--success {
  border-left: 4px solid var(--color-forest-green);
}

.toast--success svg {
  color: var(--color-forest-green);
}

.toast--error {
  border-left: 4px solid var(--color-burgundy);
}

.toast--error svg {
  color: var(--color-burgundy);
}

.toast--info {
  border-left: 4px solid var(--color-gold);
}

.toast--info svg {
  color: var(--color-gold);
}

.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(100px);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(100px);
}
</style>

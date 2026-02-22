<script setup lang="ts">
interface Props {
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost'
  size?: 'sm' | 'md' | 'lg'
  disabled?: boolean
  loading?: boolean
  fullWidth?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  size: 'md',
  disabled: false,
  loading: false,
  fullWidth: false
})
</script>

<template>
  <button
    class="base-button"
    :class="[
      `base-button--${variant}`,
      `base-button--${size}`,
      { 'base-button--full': fullWidth },
      { 'base-button--loading': loading }
    ]"
    :disabled="disabled || loading"
  >
    <span v-if="loading" class="base-button__spinner"></span>
    <span class="base-button__content" :class="{ 'base-button__content--hidden': loading }">
      <slot />
    </span>
  </button>
</template>

<style scoped>
.base-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: var(--space-2);
  font-family: var(--font-body, 'Inter', sans-serif);
  font-weight: var(--font-medium, 500);
  border: none;
  border-radius: var(--radius-md, 8px);
  cursor: pointer;
  transition: all var(--transition-fast, 0.2s) var(--ease-out, cubic-bezier(0.4, 0, 0.2, 1));
  position: relative;
  overflow: hidden;
  white-space: nowrap;
}

.base-button:focus-visible {
  outline: 2px solid var(--color-burgundy, #722F37);
  outline-offset: 2px;
}

.base-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Sizes */
.base-button--sm {
  padding: var(--space-2) var(--space-4);
  font-size: var(--text-sm, 0.875rem);
}

.base-button--md {
  padding: var(--space-3) var(--space-6);
  font-size: var(--text-base, 1rem);
}

.base-button--lg {
  padding: var(--space-4) var(--space-8);
  font-size: var(--text-lg, 1.125rem);
}

/* Variants */
.base-button--primary {
  background: var(--color-burgundy, #722F37);
  color: white;
}

.base-button--primary:hover:not(:disabled) {
  background: var(--color-burgundy-deep, #4A1F24);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(114, 47, 55, 0.3);
}

.base-button--secondary {
  background: var(--color-forest-green, #1B4D3E);
  color: white;
}

.base-button--secondary:hover:not(:disabled) {
  background: var(--color-forest-green-deep, #0F2E25);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(27, 77, 62, 0.3);
}

.base-button--outline {
  background: transparent;
  color: var(--color-burgundy, #722F37);
  border: 1.5px solid var(--color-burgundy, #722F37);
}

.base-button--outline:hover:not(:disabled) {
  background: var(--color-burgundy, #722F37);
  color: white;
}

.base-button--ghost {
  background: transparent;
  color: var(--color-text, #2C2825);
}

.base-button--ghost:hover:not(:disabled) {
  background: var(--color-background-soft, #FDF8F3);
}

/* Full width */
.base-button--full {
  width: 100%;
}

/* Loading state */
.base-button--loading {
  pointer-events: none;
}

.base-button__spinner {
  width: 18px;
  height: 18px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  border-top-color: white;
  animation: spin 0.8s linear infinite;
  position: absolute;
}

.base-button__content--hidden {
  visibility: hidden;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>

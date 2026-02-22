<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  size?: 'sm' | 'md' | 'lg'
  color?: 'primary' | 'white' | 'gold'
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  color: 'primary'
})

const spinnerClasses = computed(() => [
  'spinner',
  `spinner--${props.size}`,
  `spinner--${props.color}`
])
</script>

<template>
  <div :class="spinnerClasses">
    <div class="spinner-ring"></div>
  </div>
</template>

<style scoped>
.spinner {
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.spinner-ring {
  border-radius: 50%;
  border-style: solid;
  animation: spin 0.8s linear infinite;
}

/* Sizes */
.spinner--sm .spinner-ring {
  width: 16px;
  height: 16px;
  border-width: 2px;
}

.spinner--md .spinner-ring {
  width: 24px;
  height: 24px;
  border-width: 3px;
}

.spinner--lg .spinner-ring {
  width: 40px;
  height: 40px;
  border-width: 4px;
}

/* Colors */
.spinner--primary .spinner-ring {
  border-color: var(--color-burgundy);
  border-top-color: transparent;
}

.spinner--white .spinner-ring {
  border-color: white;
  border-top-color: transparent;
}

.spinner--gold .spinner-ring {
  border-color: var(--color-gold);
  border-top-color: transparent;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>

<script setup lang="ts">
interface Props {
  variant?: 'text' | 'circular' | 'rectangular' | 'rounded'
  width?: string
  height?: string
  lines?: number
}

withDefaults(defineProps<Props>(), {
  variant: 'text',
  width: '100%',
  height: '20px',
  lines: 1
})
</script>

<template>
  <div 
    v-if="variant === 'text' && lines > 1" 
    class="skeleton-text"
  >
    <div 
      v-for="i in lines" 
      :key="i" 
      class="skeleton-text__line"
      :style="{ width: i === lines ? '60%' : '100%' }"
    ></div>
  </div>
  <div 
    v-else
    class="skeleton"
    :class="`skeleton--${variant}`"
    :style="{ width, height }"
  ></div>
</template>

<style scoped>
.skeleton {
  background: linear-gradient(
    90deg,
    var(--color-background-soft, #FDF8F3) 25%,
    var(--color-background-mute, #F5F0EB) 50%,
    var(--color-background-soft, #FDF8F3) 75%
  );
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
}

.skeleton--circular {
  border-radius: 50%;
}

.skeleton--rounded {
  border-radius: var(--radius-md, 8px);
}

.skeleton--rectangular {
  border-radius: 0;
}

.skeleton-text {
  display: flex;
  flex-direction: column;
  gap: var(--space-2, 8px);
  width: 100%;
}

.skeleton-text__line {
  height: 16px;
  background: linear-gradient(
    90deg,
    var(--color-background-soft, #FDF8F3) 25%,
    var(--color-background-mute, #F5F0EB) 50%,
    var(--color-background-soft, #FDF8F3) 75%
  );
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  border-radius: var(--radius-sm, 4px);
}

@keyframes shimmer {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}
</style>

<script setup lang="ts">
interface Props {
  title: string
  subtitle?: string
  alignment?: 'left' | 'center' | 'right'
  size?: 'sm' | 'md' | 'lg'
}

withDefaults(defineProps<Props>(), {
  alignment: 'center',
  size: 'md'
})
</script>

<template>
  <div 
    class="section-header"
    :class="[
      `section-header--${alignment}`,
      `section-header--${size}`
    ]"
  >
    <h2 v-if="title" class="section-header__title">{{ title }}</h2>
    <p v-if="subtitle" class="section-header__subtitle">{{ subtitle }}</p>
    <div v-if="$slots.default" class="section-header__actions">
      <slot />
    </div>
  </div>
</template>

<style scoped>
.section-header {
  display: flex;
  flex-direction: column;
  gap: var(--space-3, 12px);
  margin-bottom: var(--space-8, 32px);
}

.section-header--left {
  align-items: flex-start;
  text-align: left;
}

.section-header--center {
  align-items: center;
  text-align: center;
}

.section-header--right {
  align-items: flex-end;
  text-align: right;
}

.section-header__title {
  font-family: var(--font-heading, 'Cormorant Garamond', serif);
  color: var(--color-heading, #2C2825);
  margin: 0;
  line-height: 1.2;
}

.section-header--sm .section-header__title {
  font-size: var(--text-2xl, 1.5rem);
}

.section-header--md .section-header__title {
  font-size: var(--text-3xl, 1.875rem);
}

.section-header--lg .section-header__title {
  font-size: var(--text-4xl, 2.25rem);
}

.section-header__subtitle {
  font-family: var(--font-body, 'Inter', sans-serif);
  color: var(--color-text-light, #8B8178);
  margin: 0;
  max-width: 600px;
  line-height: 1.6;
}

.section-header--sm .section-header__subtitle {
  font-size: var(--text-sm, 0.875rem);
}

.section-header--md .section-header__subtitle {
  font-size: var(--text-base, 1rem);
}

.section-header--lg .section-header__subtitle {
  font-size: var(--text-lg, 1.125rem);
}

.section-header__actions {
  margin-top: var(--space-4, 16px);
}
</style>

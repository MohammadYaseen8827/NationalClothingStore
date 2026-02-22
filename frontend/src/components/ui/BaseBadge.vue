<script setup lang="ts">
interface Props {
  variant?: 'new' | 'wedding' | 'premium' | 'handcrafted' | 'sale' | 'limited'
  size?: 'sm' | 'md'
}

withDefaults(defineProps<Props>(), {
  variant: 'new',
  size: 'md'
})

const variantLabels: Record<string, string> = {
  new: 'New',
  wedding: 'Wedding',
  premium: 'Premium',
  handcrafted: 'Handcrafted',
  sale: 'Sale',
  limited: 'Limited'
}
</script>

<template>
  <span
    class="base-badge"
    :class="[
      `base-badge--${variant}`,
      `base-badge--${size}`
    ]"
  >
    {{ variantLabels[variant] }}
  </span>
</template>

<style scoped>
.base-badge {
  display: inline-flex;
  align-items: center;
  font-family: var(--font-body, 'Inter', sans-serif);
  font-weight: var(--font-medium, 500);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  border-radius: var(--radius-sm, 4px);
  white-space: nowrap;
}

/* Sizes */
.base-badge--sm {
  padding: 2px 6px;
  font-size: 10px;
}

.base-badge--md {
  padding: 4px 10px;
  font-size: 11px;
}

/* Variants */
.base-badge--new {
  background: var(--color-gold, #C9A962);
  color: var(--color-charcoal, #2C2825);
}

.base-badge--wedding {
  background: var(--color-burgundy, #722F37);
  color: white;
}

.base-badge--premium {
  background: linear-gradient(135deg, var(--color-gold, #C9A962) 0%, var(--color-gold-dark, #9E7F3E) 100%);
  color: var(--color-charcoal, #2C2825);
}

.base-badge--handcrafted {
  background: var(--color-forest-green, #1B4D3E);
  color: white;
}

.base-badge--sale {
  background: var(--color-burgundy-light, #8B3A42);
  color: white;
}

.base-badge--limited {
  background: var(--color-charcoal, #2C2825);
  color: white;
}
</style>

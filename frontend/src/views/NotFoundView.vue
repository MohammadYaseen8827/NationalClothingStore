<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>

<template>
  <div class="not-found">
    <div class="not-found__content">
      <div class="not-found__illustration">
        <svg xmlns="http://www.w3.org/2000/svg" width="120" height="120" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1">
          <circle cx="12" cy="12" r="10"></circle>
          <line x1="12" y1="8" x2="12" y2="12"></line>
          <line x1="12" y1="16" x2="12.01" y2="16"></line>
        </svg>
      </div>
      
      <h1 class="not-found__title">Page Not Found</h1>
      <p class="not-found__message">
        The page you're looking for doesn't exist or has been moved.
      </p>
      
      <div class="not-found__actions">
        <RouterLink to="/" class="btn btn-primary btn-lg">
          Return to Home
        </RouterLink>
        <RouterLink to="/products" class="btn btn-secondary btn-lg">
          Browse Products
        </RouterLink>
      </div>

      <div class="not-found__links">
        <h3>Quick Links</h3>
        <ul>
          <li><RouterLink to="/">Home</RouterLink></li>
          <li><RouterLink to="/products">Products</RouterLink></li>
          <li><RouterLink to="/cart">Cart</RouterLink></li>
          <li><RouterLink to="/about">About</RouterLink></li>
        </ul>
      </div>
    </div>
  </div>
</template>

<style scoped>
.not-found {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: var(--space-8);
  background: var(--color-background);
}

.not-found__content {
  text-align: center;
  max-width: 600px;
  width: 100%;
}

.not-found__illustration {
  margin-bottom: var(--space-8);
  color: var(--color-burgundy);
  opacity: 0.8;
}

.not-found__title {
  font-family: var(--font-heading);
  font-size: var(--text-5xl);
  color: var(--color-charcoal);
  margin-bottom: var(--space-4);
  line-height: 1.2;
}

.not-found__message {
  font-size: var(--text-lg);
  color: var(--color-warm-gray);
  margin-bottom: var(--space-8);
  line-height: var(--leading-relaxed);
}

.not-found__actions {
  display: flex;
  gap: var(--space-4);
  justify-content: center;
  margin-bottom: var(--space-12);
  flex-wrap: wrap;
}

.btn {
  min-width: 200px;
}

.not-found__links {
  padding-top: var(--space-8);
  border-top: 1px solid var(--color-border-light);
}

.not-found__links h3 {
  font-family: var(--font-heading);
  font-size: var(--text-lg);
  color: var(--color-charcoal);
  margin-bottom: var(--space-4);
}

.not-found__links ul {
  display: flex;
  gap: var(--space-4);
  justify-content: center;
  flex-wrap: wrap;
  list-style: none;
  padding: 0;
}

.not-found__links li a {
  color: var(--color-burgundy);
  text-decoration: none;
  transition: color var(--transition-fast);
  padding: var(--space-2);
}

.not-found__links li a:hover {
  color: var(--color-burgundy-deep);
  text-decoration: underline;
}

@media (max-width: 640px) {
  .not-found {
    padding: var(--space-6);
  }

  .not-found__title {
    font-size: var(--text-3xl);
  }

  .not-found__actions {
    flex-direction: column;
  }

  .btn {
    width: 100%;
  }
}
</style>

<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>

<template>
  <div class="coming-soon">
    <div class="container">
      <div class="coming-soon-content">
        <svg class="coming-soon-icon" xmlns="http://www.w3.org/2000/svg" width="120" height="120" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1">
          <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.42 0-8-3.58-8-8s3.58-8 8-8 8 3.58 8 8-3.58 8-8 8zm.5-13H11v6l5.25 3.15.75-1.23-4.5-2.67z"></path>
        </svg>
        
        <h1 class="coming-soon-title">Coming Soon</h1>
        
        <p class="coming-soon-subtitle">
          We're working on something beautiful. This page will be available soon.
        </p>

        <p class="coming-soon-description">
          In the meantime, explore our collection of exquisite traditional wear and heritage fashion pieces.
        </p>

        <div class="coming-soon-actions">
          <RouterLink to="/" class="btn btn-primary btn-lg">
            Back to Home
          </RouterLink>
          <RouterLink to="/products" class="btn btn-outline btn-lg">
            Browse Collection
          </RouterLink>
        </div>

        <div class="coming-soon-features">
          <div class="feature">
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
              <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"></path>
            </svg>
            <span>High-Quality Craftsmanship</span>
          </div>
          <div class="feature">
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
              <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"></path>
            </svg>
            <span>Authentic Heritage Design</span>
          </div>
          <div class="feature">
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
              <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"></path>
            </svg>
            <span>Premium Quality Assurance</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.coming-soon {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: var(--space-8) var(--space-4);
  background: linear-gradient(135deg, 
    rgba(27, 77, 62, 0.05) 0%,
    rgba(114, 47, 55, 0.05) 100%);
}

.container {
  max-width: 600px;
  width: 100%;
}

.coming-soon-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  gap: var(--space-6);
}

.coming-soon-icon {
  color: var(--color-burgundy);
  margin-bottom: var(--space-4);
}

.coming-soon-title {
  font-family: var(--font-display);
  font-size: var(--text-5xl);
  font-weight: var(--font-bold);
  color: var(--color-charcoal);
  margin: 0;
  line-height: var(--leading-tight);
}

.coming-soon-subtitle {
  font-size: var(--text-xl);
  color: var(--color-charcoal);
  margin: 0;
  font-weight: var(--font-medium);
}

.coming-soon-description {
  font-size: var(--text-base);
  color: var(--color-warm-gray);
  margin: 0;
  line-height: var(--leading-relaxed);
}

.coming-soon-actions {
  display: flex;
  gap: var(--space-4);
  width: 100%;
  justify-content: center;
  flex-wrap: wrap;
}

.btn {
  text-decoration: none;
  border: none;
  cursor: pointer;
  transition: all var(--transition-base);
  border-radius: var(--radius-md);
  padding: var(--space-3) var(--space-6);
  font-size: var(--text-base);
  font-weight: var(--font-medium);
}

.btn-primary {
  background: var(--color-burgundy);
  color: white;
}

.btn-primary:hover {
  background: var(--color-burgundy-deep);
  transform: translateY(-2px);
  box-shadow: var(--shadow-md);
}

.btn-outline {
  background: transparent;
  color: var(--color-burgundy);
  border: 2px solid var(--color-burgundy);
}

.btn-outline:hover {
  background: var(--color-burgundy);
  color: white;
}

.btn-lg {
  padding: var(--space-4) var(--space-8);
  font-size: var(--text-lg);
  min-width: 200px;
}

.coming-soon-features {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: var(--space-6);
  width: 100%;
  margin-top: var(--space-4);
}

.feature {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--space-3);
}

.feature svg {
  color: var(--color-forest-green);
  flex-shrink: 0;
}

.feature span {
  font-size: var(--text-sm);
  color: var(--color-charcoal);
  font-weight: var(--font-medium);
}

@media (max-width: 640px) {
  .coming-soon {
    padding: var(--space-6) var(--space-4);
  }

  .coming-soon-title {
    font-size: var(--text-4xl);
  }

  .coming-soon-actions {
    flex-direction: column;
  }

  .btn-lg {
    width: 100%;
    min-width: unset;
  }

  .coming-soon-features {
    grid-template-columns: 1fr;
  }
}
</style>

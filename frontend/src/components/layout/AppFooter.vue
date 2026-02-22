<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink } from 'vue-router'

// Newsletter
const email = ref('')
const isSubscribed = ref(false)

const subscribe = () => {
  if (email.value.trim()) {
    isSubscribed.value = true
    email.value = ''
    // TODO: Connect to newsletter service
  }
}

// Footer links
const footerLinks = {
  shop: [
    { name: 'Women', path: '/products?gender=women' },
    { name: 'Men', path: '/products?gender=men' },
    { name: 'Wedding Collection', path: '/products?occasion=wedding' },
    { name: 'Traditional Folk', path: '/products?category=folk' },
    { name: 'Accessories', path: '/products?category=accessories' },
  ],
  help: [
    { name: 'Size Guide', path: '/coming-soon' },
    { name: 'Shipping & Delivery', path: '/coming-soon' },
    { name: 'Returns & Exchanges', path: '/coming-soon' },
    { name: 'Order Tracking', path: '/coming-soon' },
    { name: 'Contact Us', path: '/coming-soon' },
  ],
  about: [
    { name: 'Our Story', path: '/coming-soon' },
    { name: 'Craftsmanship', path: '/coming-soon' },
    { name: 'Sustainability', path: '/coming-soon' },
    { name: 'Careers', path: '/coming-soon' },
    { name: 'Press', path: '/coming-soon' },
  ],
}

const currentYear = new Date().getFullYear()
</script>

<template>
  <footer class="footer">
    <!-- Main Footer -->
    <div class="footer__main">
      <div class="footer__container">
        <div class="footer__grid">
          <!-- Brand Column -->
          <div class="footer__brand">
            <RouterLink to="/" class="footer__logo">
              <span class="logo-text">NATIONAL</span>
              <span class="logo-subtext">CLOTHING STORE</span>
            </RouterLink>
            <p class="footer__tagline">
              Celebrating Russian heritage through exquisite traditional wear. 
              Each piece tells a story of craftsmanship, culture, and timeless elegance.
            </p>
            <div class="footer__trust">
              <div class="trust-item">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <rect x="3" y="11" width="18" height="11" rx="2" ry="2"></rect>
                  <path d="M7 11V7a5 5 0 0 1 10 0v4"></path>
                </svg>
                <span>Secure Payment</span>
              </div>
              <div class="trust-item">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path>
                </svg>
                <span>Quality Guarantee</span>
              </div>
              <div class="trust-item">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path>
                  <circle cx="12" cy="10" r="3"></circle>
                </svg>
                <span>Free Returns</span>
              </div>
            </div>
          </div>

          <!-- Shop Links -->
          <nav class="footer__column" aria-label="Shop Navigation">
            <h4 class="footer__heading">Shop</h4>
            <ul class="footer__links">
              <li v-for="link in footerLinks.shop" :key="link.path">
                <RouterLink :to="link.path" class="footer__link">
                  {{ link.name }}
                </RouterLink>
              </li>
            </ul>
          </nav>

          <!-- Help Links -->
          <nav class="footer__column" aria-label="Customer Support Navigation">
            <h4 class="footer__heading">Customer Care</h4>
            <ul class="footer__links">
              <li v-for="link in footerLinks.help" :key="link.path">
                <RouterLink :to="link.path" class="footer__link">
                  {{ link.name }}
                </RouterLink>
              </li>
            </ul>
          </nav>

          <!-- About Links -->
          <nav class="footer__column" aria-label="Company Information Navigation">
            <h4 class="footer__heading">About</h4>
            <ul class="footer__links">
              <li v-for="link in footerLinks.about" :key="link.path">
                <RouterLink :to="link.path" class="footer__link">
                  {{ link.name }}
                </RouterLink>
              </li>
            </ul>
          </nav>

          <!-- Newsletter -->
          <div class="footer__newsletter">
            <h4 class="footer__heading">Stay Inspired</h4>
            <p class="footer__newsletter-text">
              Subscribe for exclusive access to new collections, 
              cultural stories, and special offers.
            </p>
            <form v-if="!isSubscribed" @submit.prevent="subscribe" class="newsletter-form">
              <input 
                v-model="email"
                type="email" 
                placeholder="Your email address" 
                class="newsletter-input"
                required
              />
              <button type="submit" class="newsletter-btn">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="m22 2-7 20-4-9-9-4 20Z"></path>
                  <path d="M22 2 11 13"></path>
                </svg>
              </button>
            </form>
            <p v-else class="newsletter-success">
              Thank you for subscribing!
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Cultural Banner -->
    <div class="footer__cultural">
      <div class="footer__container">
        <div class="cultural-content">
          <div class="cultural-icon">
            <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
              <path d="M3 21h18"></path>
              <path d="M5 21V7l8-4 8 4v14"></path>
              <path d="M9 21v-4a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2v4"></path>
            </svg>
          </div>
          <div class="cultural-text">
            <h5>Handcrafted with Love</h5>
            <p>Each garment reflects centuries of Russian folk traditions, 
               featuring authentic embroidery and premium natural fabrics.</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Bottom Bar -->
    <div class="footer__bottom">
      <div class="footer__container">
        <div class="footer__bottom-content">
          <p class="footer__copyright">
            &copy; {{ currentYear }} National Clothing Store. All rights reserved.
          </p>
          <div class="footer__legal">
            <RouterLink to="/privacy" class="footer__legal-link">Privacy Policy</RouterLink>
            <RouterLink to="/terms" class="footer__legal-link">Terms of Service</RouterLink>
          </div>
          <div class="footer__social">
            <a href="#" class="social-link" aria-label="Instagram">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <rect x="2" y="2" width="20" height="20" rx="5" ry="5"></rect>
                <path d="M16 11.37A4 4 0 1 1 12.63 8 4 4 0 0 1 16 11.37z"></path>
                <line x1="17.5" y1="6.5" x2="17.51" y2="6.5"></line>
              </svg>
            </a>
            <a href="#" class="social-link" aria-label="Facebook">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M18 2h-3a5 5 0 0 0-5 5v3H7v4h3v8h4v-8h3l1-4h-4V7a1 1 0 0 1 1-1h3z"></path>
              </svg>
            </a>
            <a href="#" class="social-link" aria-label="Pinterest">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M8 12a4 4 0 1 1 8 0 4 4 0 0 1-8 0z"></path>
                <path d="M12 2v20"></path>
                <path d="M8 22c0-4 4-6 8-6"></path>
              </svg>
            </a>
            <a href="#" class="social-link" aria-label="YouTube">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M22.54 6.42a2.78 2.78 0 0 0-1.94-2C18.88 4 12 4 12 4s-6.88 0-8.6.46a2.78 2.78 0 0 0-1.94 2A29 29 0 0 0 1 11.75a29 29 0 0 0 .46 5.33A2.78 2.78 0 0 0 3.4 19c1.72.46 8.6.46 8.6.46s6.88 0 8.6-.46a2.78 2.78 0 0 0 1.94-2 29 29 0 0 0 .46-5.25 29 29 0 0 0-.46-5.33z"></path>
                <polygon points="9.75 15.02 15.5 11.75 9.75 8.48 9.75 15.02"></polygon>
              </svg>
            </a>
          </div>
        </div>
      </div>
    </div>
  </footer>
</template>

<style scoped>
/* ========================================
   FOOTER - Heritage Premium Style
   ======================================== */
.footer {
  background: var(--color-charcoal);
  color: var(--color-cream);
  margin-top: auto;
}

/* Main Footer */
.footer__main {
  padding: var(--space-16) 0;
}

.footer__container {
  max-width: var(--container-xl);
  margin: 0 auto;
  padding: 0 var(--space-6);
}

.footer__grid {
  display: grid;
  grid-template-columns: 1.5fr repeat(3, 1fr) 1.5fr;
  gap: var(--space-10);
}

/* Brand Column */
.footer__brand {
  padding-right: var(--space-8);
}

.footer__logo {
  display: flex;
  flex-direction: column;
  text-decoration: none;
  line-height: 1;
  margin-bottom: var(--space-4);
}

.footer__logo .logo-text {
  font-family: var(--font-heading);
  font-size: var(--text-2xl);
  font-weight: var(--font-bold);
  color: var(--color-gold);
  letter-spacing: 0.15em;
}

.footer__logo .logo-subtext {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  color: var(--color-warm-gray-light);
  letter-spacing: 0.3em;
  margin-top: 2px;
}

.footer__tagline {
  font-size: var(--text-sm);
  color: var(--color-warm-gray-light);
  line-height: var(--leading-relaxed);
  margin-bottom: var(--space-6);
}

/* Trust Items */
.footer__trust {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.trust-item {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  font-size: var(--text-sm);
  color: var(--color-warm-gray-light);
}

.trust-item svg {
  color: var(--color-gold);
  flex-shrink: 0;
}

/* Column Links */
.footer__column {
  min-width: 0;
}

.footer__heading {
  font-family: var(--font-heading);
  font-size: var(--text-base);
  font-weight: var(--font-semibold);
  color: var(--color-cream);
  margin-bottom: var(--space-5);
  text-transform: uppercase;
  letter-spacing: 0.1em;
}

.footer__links {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.footer__link {
  font-size: var(--text-sm);
  color: var(--color-warm-gray-light);
  text-decoration: none;
  transition: all var(--transition-fast) var(--ease-out);
}

.footer__link:hover {
  color: var(--color-gold);
  padding-left: var(--space-1);
}

/* Newsletter */
.footer__newsletter {
  padding-left: var(--space-6);
  border-left: 1px solid var(--color-warm-gray-dark);
}

.footer__newsletter-text {
  font-size: var(--text-sm);
  color: var(--color-warm-gray-light);
  margin-bottom: var(--space-4);
  line-height: var(--leading-relaxed);
}

.newsletter-form {
  display: flex;
  gap: 0;
}

.newsletter-input {
  flex: 1;
  padding: var(--space-3) var(--space-4);
  background: var(--color-warm-gray-dark);
  border: 1px solid var(--color-warm-gray-dark);
  border-right: none;
  color: var(--color-cream);
  font-size: var(--text-sm);
}

.newsletter-input::placeholder {
  color: var(--color-warm-gray);
}

.newsletter-input:focus {
  outline: none;
  border-color: var(--color-gold);
}

.newsletter-btn {
  padding: var(--space-3) var(--space-4);
  background: var(--color-burgundy);
  color: var(--vt-c-white);
  border: none;
  cursor: pointer;
  transition: background var(--transition-fast) var(--ease-out);
}

.newsletter-btn:hover {
  background: var(--color-burgundy-deep);
}

.newsletter-success {
  font-size: var(--text-sm);
  color: var(--color-gold);
  padding: var(--space-3);
  background: rgba(201, 169, 98, 0.1);
  border-radius: var(--radius-md);
  text-align: center;
}

/* Cultural Banner */
.footer__cultural {
  background: var(--color-burgundy-deep);
  padding: var(--space-6) 0;
}

.cultural-content {
  display: flex;
  align-items: center;
  gap: var(--space-6);
}

.cultural-icon {
  color: var(--color-gold);
  flex-shrink: 0;
}

.cultural-text h5 {
  font-family: var(--font-heading);
  font-size: var(--text-lg);
  color: var(--color-cream);
  margin-bottom: var(--space-1);
}

.cultural-text p {
  font-size: var(--text-sm);
  color: var(--color-warm-gray-light);
  line-height: var(--leading-relaxed);
}

/* Bottom Bar */
.footer__bottom {
  background: var(--color-black);
  padding: var(--space-4) 0;
}

.footer__bottom-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: var(--space-4);
}

.footer__copyright {
  font-size: var(--text-sm);
  color: var(--color-warm-gray);
}

.footer__legal {
  display: flex;
  gap: var(--space-6);
}

.footer__legal-link {
  font-size: var(--text-sm);
  color: var(--color-warm-gray);
  text-decoration: none;
  transition: color var(--transition-fast) var(--ease-out);
}

.footer__legal-link:hover {
  color: var(--color-gold);
}

.footer__social {
  display: flex;
  gap: var(--space-4);
}

.social-link {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  color: var(--color-warm-gray-light);
  border-radius: var(--radius-full);
  transition: all var(--transition-fast) var(--ease-out);
}

.social-link:hover {
  background: var(--color-warm-gray-dark);
  color: var(--color-gold);
}

/* Responsive */
@media (max-width: 1024px) {
  .footer__grid {
    grid-template-columns: repeat(3, 1fr);
    gap: var(--space-8);
  }
  
  .footer__brand {
    grid-column: 1 / -1;
    padding-right: 0;
    text-align: center;
  }
  
  .footer__trust {
    flex-direction: row;
    justify-content: center;
    flex-wrap: wrap;
  }
  
  .footer__newsletter {
    grid-column: 1 / -1;
    padding-left: 0;
    border-left: none;
    text-align: center;
    max-width: 400px;
    margin: 0 auto;
  }
}

@media (max-width: 768px) {
  .footer__main {
    padding: var(--space-10) 0;
  }
  
  .footer__grid {
    grid-template-columns: 1fr 1fr;
    gap: var(--space-6);
  }
  
  .footer__column {
    text-align: center;
  }
  
  .cultural-content {
    flex-direction: column;
    text-align: center;
  }
  
  .footer__bottom-content {
    flex-direction: column;
    text-align: center;
  }
}
</style>

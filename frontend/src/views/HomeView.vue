<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { RouterLink } from 'vue-router'

// Mock data - in production, connect to product service
const featuredProducts = ref([
  {
    id: '1',
    name: 'Traditional Sarafan Dress',
    price: 459,
    image: 'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=600&h=800&fit=crop',
    category: 'Women',
    occasion: 'Wedding',
    rating: 4.9,
    reviews: 28,
    isNew: true,
    isPremium: true
  },
  {
    id: '2',
    name: 'Embroidered Rubakha Shirt',
    price: 189,
    image: 'https://images.unsplash.com/photo-1609192591984-2a6d953a1c2d?w=600&h=800&fit=crop',
    category: 'Men',
    occasion: 'Festival',
    rating: 4.8,
    reviews: 42,
    isNew: false,
    isPremium: false
  },
  {
    id: '3',
    name: 'Wedding Tsaritsa Gown',
    price: 1299,
    image: 'https://images.unsplash.com/photo-1566174053879-31528523f8ae?w=600&h=800&fit=crop',
    category: 'Women',
    occasion: 'Wedding',
    rating: 5.0,
    reviews: 15,
    isNew: true,
    isPremium: true
  },
  {
    id: '4',
    name: 'Folk Patterned Shawl',
    price: 129,
    image: 'https://images.unsplash.com/photo-1601924994987-69e26d50dc26?w=600&h=800&fit=crop',
    category: 'Accessories',
    occasion: 'Daily',
    rating: 4.7,
    reviews: 56,
    isNew: false,
    isPremium: false
  },
  {
    id: '5',
    name: 'Traditional Kosovorotka',
    price: 245,
    image: 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=600&h=800&fit=crop',
    category: 'Men',
    occasion: 'Formal',
    rating: 4.9,
    reviews: 33,
    isNew: false,
    isPremium: true
  },
  {
    id: '6',
    name: 'Royal Peasant Blouse',
    price: 179,
    image: 'https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?w=600&h=800&fit=crop',
    category: 'Women',
    occasion: 'Daily',
    rating: 4.6,
    reviews: 67,
    isNew: true,
    isPremium: false
  }
])

// Collections data
const collections = [
  {
    id: 'new',
    title: 'New Collection',
    subtitle: 'Spring-Summer 2026',
    description: 'Discover the latest expressions of Russian heritage fashion',
    image: 'https://images.unsplash.com/photo-1469334031218-e382a71b716b?w=800&h=600&fit=crop',
    link: '/products?collection=new',
    tag: 'New Arrivals'
  },
  {
    id: 'wedding',
    title: 'Wedding & Celebration',
    subtitle: 'Tie the Knot in Tradition',
    description: 'Exquisite garments for your most precious moments',
    image: 'https://images.unsplash.com/photo-1519741497674-611481863552?w=800&h=600&fit=crop',
    link: '/products?occasion=wedding',
    tag: 'Wedding Collection'
  },
  {
    id: 'folk',
    title: 'Traditional Classics',
    subtitle: 'Timeless Heritage',
    description: 'Authentic pieces passed down through generations',
    image: 'https://images.unsplash.com/photo-1509631179647-0177331693ae?w=800&h=600&fit=crop',
    link: '/products?category=folk',
    tag: 'Classic'
  },
  {
    id: 'premium',
    title: 'Premium Handcrafted',
    subtitle: 'Artisan Excellence',
    description: 'Masterfully crafted by skilled artisans',
    image: 'https://images.unsplash.com/photo-1558171813-4c088753af8f?w=800&h=600&fit=crop',
    link: '/products?premium=true',
    tag: 'Premium'
  }
]

// Trust signals
const trustSignals = [
  {
    icon: 'handcrafted',
    title: 'Handcrafted Excellence',
    description: 'Each piece is meticulously crafted by skilled artisans'
  },
  {
    icon: 'fabrics',
    title: 'Premium Fabrics',
    description: 'Only the finest natural materials'
  },
  {
    icon: 'embroidery',
    title: 'Traditional Embroidery',
    description: 'Authentic patterns with centuries of history'
  },
  {
    icon: 'limited',
    title: 'Limited Production',
    description: 'Exclusive pieces in small batches'
  }
]

// Testimonials
const testimonials = ref([
  {
    id: 1,
    name: 'Maria Petrov',
    location: 'Moscow',
    text: 'The embroidery on my wedding dress was absolutely breathtaking. Every detail told a story of Russian heritage.',
    rating: 5,
    image: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=100&h=100&fit=crop'
  },
  {
    id: 2,
    name: 'Anna Sokolova',
    location: 'Saint Petersburg',
    text: 'Finally found authentic folk wear that feels modern and elegant. The quality is exceptional.',
    rating: 5,
    image: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=100&h=100&fit=crop'
  },
  {
    id: 3,
    name: 'Elena Volkov',
    location: 'Novosibirsk',
    text: 'My husband\'s traditional shirt received so many compliments at our celebration. Truly special.',
    rating: 5,
    image: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=100&h=100&fit=crop'
  }
])

// Add to cart animation
const addToCart = (productId: string) => {
  // TODO: Connect to cart store
  console.log('Added to cart:', productId)
}
</script>

<template>
  <div class="home">
    <!-- Hero Section -->
    <section class="hero">
      <div class="hero__background">
        <div class="hero__overlay"></div>
      </div>
      <div class="hero__content">
        <span class="hero__tag">Heritage Collection 2026</span>
        <h1 class="hero__title">
          Embrace the<br/>
          <span class="hero__title-accent">Timeless Beauty</span><br/>
          of Russian Tradition
        </h1>
        <p class="hero__subtitle">
          Discover exquisite handcrafted garments that celebrate centuries 
          of cultural heritage with modern elegance
        </p>
        <div class="hero__actions">
          <RouterLink to="/products" class="btn btn-primary btn-lg">
            Explore Collection
          </RouterLink>
          <RouterLink to="/products?category=folk" class="btn btn-outline btn-lg">
            Traditional Wear
          </RouterLink>
        </div>
      </div>
      <div class="hero__scroll">
        <span>Scroll to discover</span>
        <div class="hero__scroll-line"></div>
      </div>
    </section>

    <!-- Trust Banner -->
    <section class="trust-banner">
      <div class="container">
        <div class="trust-banner__grid">
          <div v-for="signal in trustSignals" :key="signal.icon" class="trust-banner__item">
            <div class="trust-banner__icon">
              <svg v-if="signal.icon === 'handcrafted'" xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                <path d="M12 2.69l5.66 5.66a8 8 0 1 1-11.31 0z"></path>
              </svg>
              <svg v-else-if="signal.icon === 'fabrics'" xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                <path d="M20.38 3.46L16 2a4 4 0 0 1-8 0L3.62 3.46a2 2 0 0 0-1.34 2.23l.58 3.47a1 1 0 0 0 .99.84H6v10c0 1.1.9 2 2 2h8a2 2 0 0 0 2-2V10h2.15a1 1 0 0 0 .99-.84l.58-3.47a2 2 0 0 0-1.34-2.23z"></path>
              </svg>
              <svg v-else-if="signal.icon === 'embroidery'" xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                <path d="M20.59 13.41l-7.17 7.17a2 2 0 0 1-2.83 0L2 12V2h10l8.59 8.59a2 2 0 0 1 0 2.82z"></path>
                <line x1="7" y1="7" x2="7.01" y2="7"></line>
              </svg>
              <svg v-else xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                <rect x="2" y="7" width="20" height="14" rx="2" ry="2"></rect>
                <path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16"></path>
              </svg>
            </div>
            <div class="trust-banner__text">
              <h4>{{ signal.title }}</h4>
              <p>{{ signal.description }}</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Collections Grid -->
    <section class="collections">
      <div class="container">
        <div class="collections__header">
          <h2 class="section-title">Curated Collections</h2>
          <p class="section-subtitle">
            Each collection tells a story of Russian heritage and craftsmanship
          </p>
        </div>
        <div class="collections__grid">
          <RouterLink 
            v-for="collection in collections" 
            :key="collection.id"
            :to="collection.link" 
            class="collection-card"
          >
            <div class="collection-card__image">
              <img :src="collection.image" :alt="collection.title" loading="lazy" />
              <div class="collection-card__overlay"></div>
            </div>
            <div class="collection-card__content">
              <span class="collection-card__tag">{{ collection.tag }}</span>
              <h3 class="collection-card__title">{{ collection.title }}</h3>
              <p class="collection-card__subtitle">{{ collection.subtitle }}</p>
              <p class="collection-card__description">{{ collection.description }}</p>
              <span class="collection-card__cta">
                Discover
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M5 12h14"></path>
                  <path d="m12 5 7 7-7 7"></path>
                </svg>
              </span>
            </div>
          </RouterLink>
        </div>
      </div>
    </section>

    <!-- Featured Products -->
    <section class="featured">
      <div class="container">
        <div class="featured__header">
          <h2 class="section-title">Featured Pieces</h2>
          <p class="section-subtitle">
            Handpicked favorites from our collection
          </p>
        </div>
        <div class="featured__grid">
          <article 
            v-for="product in featuredProducts" 
            :key="product.id" 
            class="product-card"
          >
            <RouterLink :to="`/products/${product.id}`" class="product-card__link">
              <div class="product-card__image">
                <img :src="product.image" :alt="product.name" loading="lazy" />
                <div class="product-card__badges">
                  <span v-if="product.isNew" class="badge badge-gold">New</span>
                  <span v-if="product.isPremium" class="badge badge-primary">Premium</span>
                </div>
                <div class="product-card__overlay">
                  <button class="quick-add-btn" @click.prevent="addToCart(product.id)">
                    <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                      <circle cx="9" cy="21" r="1"></circle>
                      <circle cx="20" cy="21" r="1"></circle>
                      <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
                    </svg>
                    Quick Add
                  </button>
                </div>
              </div>
              <div class="product-card__info">
                <span class="product-card__category">{{ product.category }}</span>
                <h3 class="product-card__name">{{ product.name }}</h3>
                <div class="product-card__meta">
                  <div class="product-card__rating">
                    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2">
                      <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
                    </svg>
                    <span>{{ product.rating }}</span>
                    <span class="product-card__reviews">({{ product.reviews }})</span>
                  </div>
                  <span class="product-card__occasion">{{ product.occasion }}</span>
                </div>
                <div class="product-card__price">
                  ${{ product.price.toLocaleString() }}
                </div>
              </div>
            </RouterLink>
            <button class="wishlist-btn" aria-label="Add to wishlist">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M19 14c1.49-1.46 3-3.21 3-5.5A5.5 5.5 0 0 0 16.5 3c-1.76 0-3 .5-4.5 2-1.5-1.5-2.74-2-4.5-2A5.5 5.5 0 0 0 2 8.5c0 2.3 1.5 4.05 3 5.5l7 7Z"></path>
              </svg>
            </button>
          </article>
        </div>
        <div class="featured__cta">
          <RouterLink to="/products" class="btn btn-outline">
            View All Products
          </RouterLink>
        </div>
      </div>
    </section>

    <!-- Storytelling Section -->
    <section class="story">
      <div class="story__image">
        <img 
          src="https://images.unsplash.com/photo-1558171813-4c088753af8f?w=1200&h=800&fit=crop" 
          alt="Artisan at work" 
          loading="lazy"
        />
      </div>
      <div class="story__content">
        <span class="story__label">Our Craftsmanship</span>
        <h2 class="story__title">Threads of Heritage</h2>
        <p class="story__text">
          Each garment in our collection carries forward the legacy of Russian folk art. 
          Our skilled artisans pour their hearts into every stitch, preserving techniques 
          passed down through generations.
        </p>
        <p class="story__text">
          From the intricate floral patterns of Vladimir embroidery to the bold colors 
          of Gzhel ceramics, every design element tells a story of Russia's rich 
          cultural tapestry.
        </p>
        <div class="story__features">
          <div class="story__feature">
            <span class="story__feature-number">150+</span>
            <span class="story__feature-label">Years of Tradition</span>
          </div>
          <div class="story__feature">
            <span class="story__feature-number">50+</span>
            <span class="story__feature-label">Master Artisans</span>
          </div>
          <div class="story__feature">
            <span class="story__feature-number">100%</span>
            <span class="story__feature-label">Handcrafted</span>
          </div>
        </div>
        <RouterLink to="/about" class="btn btn-primary">
          Discover Our Story
        </RouterLink>
      </div>
    </section>

    <!-- Testimonials -->
    <section class="testimonials">
      <div class="container">
        <div class="testimonials__header">
          <h2 class="section-title">Voices of Tradition</h2>
          <p class="section-subtitle">
            What our valued customers say
          </p>
        </div>
        <div class="testimonials__grid">
          <article v-for="testimonial in testimonials" :key="testimonial.id" class="testimonial-card">
            <div class="testimonial-card__rating">
              <svg v-for="n in testimonial.rating" :key="n" xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
                <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
              </svg>
            </div>
            <blockquote class="testimonial-card__text">
              "{{ testimonial.text }}"
            </blockquote>
            <div class="testimonial-card__author">
              <img :src="testimonial.image" :alt="testimonial.name" />
              <div>
                <strong>{{ testimonial.name }}</strong>
                <span>{{ testimonial.location }}</span>
              </div>
            </div>
          </article>
        </div>
      </div>
    </section>

    <!-- Newsletter CTA -->
    <section class="newsletter-cta">
      <div class="container">
        <div class="newsletter-cta__content">
          <span class="newsletter-cta__label">Stay Inspired</span>
          <h2 class="newsletter-cta__title">Join Our Heritage</h2>
          <p class="newsletter-cta__text">
            Subscribe to receive exclusive access to new collections, 
            cultural stories, and special offers
          </p>
          <form class="newsletter-cta__form">
            <input type="email" placeholder="Your email address" required />
            <button type="submit" class="btn btn-primary">
              Subscribe
            </button>
          </form>
          <p class="newsletter-cta__privacy">
            We respect your privacy. Unsubscribe anytime.
          </p>
        </div>
      </div>
    </section>
  </div>
</template>

<style scoped>
/* ========================================
   HERO SECTION
   ======================================== */
.hero {
  position: relative;
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  margin-top: -73px;
  padding-top: 73px;
}

.hero__background {
  position: absolute;
  inset: 0;
  background: url('https://images.unsplash.com/photo-1558171813-4c088753af8f?w=1920&h=1080&fit=crop') center/cover no-repeat;
}

.hero__overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    135deg,
    rgba(44, 40, 37, 0.85) 0%,
    rgba(44, 40, 37, 0.7) 50%,
    rgba(44, 40, 37, 0.6) 100%
  );
}

.hero__content {
  position: relative;
  z-index: 2;
  text-align: center;
  max-width: 800px;
  padding: var(--space-8);
}

.hero__tag {
  display: inline-block;
  font-family: var(--font-accent);
  font-size: var(--text-sm);
  color: var(--color-gold);
  text-transform: uppercase;
  letter-spacing: 0.3em;
  margin-bottom: var(--space-6);
  animation: fadeInUp 0.8s var(--ease-out) 0.2s both;
}

.hero__title {
  font-family: var(--font-heading);
  font-size: clamp(2.5rem, 8vw, 5rem);
  font-weight: var(--font-medium);
  color: var(--color-cream);
  line-height: 1.1;
  margin-bottom: var(--space-6);
  animation: fadeInUp 0.8s var(--ease-out) 0.4s both;
}

.hero__title-accent {
  color: var(--color-gold);
  font-style: italic;
}

.hero__subtitle {
  font-size: var(--text-lg);
  color: var(--color-warm-gray-light);
  line-height: var(--leading-relaxed);
  margin-bottom: var(--space-8);
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
  animation: fadeInUp 0.8s var(--ease-out) 0.6s both;
}

.hero__actions {
  display: flex;
  gap: var(--space-4);
  justify-content: center;
  flex-wrap: wrap;
  animation: fadeInUp 0.8s var(--ease-out) 0.8s both;
}

.hero__scroll {
  position: absolute;
  bottom: var(--space-8);
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--space-2);
  color: var(--color-warm-gray-light);
  font-size: var(--text-xs);
  text-transform: uppercase;
  letter-spacing: 0.1em;
  animation: fadeIn 1s var(--ease-out) 1s both;
}

.hero__scroll-line {
  width: 1px;
  height: 40px;
  background: linear-gradient(to bottom, var(--color-gold), transparent);
  animation: scrollLine 2s ease-in-out infinite;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes scrollLine {
  0%, 100% { transform: scaleY(1); opacity: 1; }
  50% { transform: scaleY(0.5); opacity: 0.5; }
}

/* ========================================
   TRUST BANNER
   ======================================== */
.trust-banner {
  background: var(--color-cream);
  padding: var(--space-10) 0;
  border-bottom: 1px solid var(--color-border-light);
}

.trust-banner__grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: var(--space-8);
}

.trust-banner__item {
  display: flex;
  gap: var(--space-4);
  align-items: flex-start;
}

.trust-banner__icon {
  color: var(--color-burgundy);
  flex-shrink: 0;
  padding: var(--space-2);
  background: rgba(114, 47, 55, 0.08);
  border-radius: var(--radius-lg);
}

.trust-banner__text h4 {
  font-family: var(--font-heading);
  font-size: var(--text-base);
  color: var(--color-charcoal);
  margin-bottom: var(--space-1);
}

.trust-banner__text p {
  font-size: var(--text-sm);
  color: var(--color-warm-gray);
  line-height: var(--leading-relaxed);
}

@media (max-width: 1024px) {
  .trust-banner__grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 640px) {
  .trust-banner__grid {
    grid-template-columns: 1fr;
    text-align: center;
  }
  
  .trust-banner__item {
    flex-direction: column;
    align-items: center;
  }
}

/* ========================================
   SECTION STYLES
   ======================================== */
.section-title {
  font-family: var(--font-heading);
  font-size: var(--text-4xl);
  color: var(--color-charcoal);
  text-align: center;
  margin-bottom: var(--space-3);
}

.section-subtitle {
  font-size: var(--text-lg);
  color: var(--color-warm-gray);
  text-align: center;
  max-width: 600px;
  margin: 0 auto var(--space-12);
}

/* ========================================
   COLLECTIONS
   ======================================== */
.collections {
  padding: var(--space-24) 0;
  background: var(--color-background);
}

.collections__header {
  margin-bottom: var(--space-12);
}

.collections__grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: var(--space-6);
}

.collection-card {
  position: relative;
  display: block;
  overflow: hidden;
  border-radius: var(--radius-xl);
  text-decoration: none;
  aspect-ratio: 4/3;
}

.collection-card__image {
  position: absolute;
  inset: 0;
}

.collection-card__image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.6s var(--ease-out);
}

.collection-card:hover .collection-card__image img {
  transform: scale(1.05);
}

.collection-card__overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    to top,
    rgba(28, 25, 23, 0.8) 0%,
    rgba(28, 25, 23, 0.3) 50%,
    transparent 100%
  );
}

.collection-card__content {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: var(--space-8);
  color: var(--color-cream);
}

.collection-card__tag {
  display: inline-block;
  font-size: var(--text-xs);
  text-transform: uppercase;
  letter-spacing: 0.15em;
  color: var(--color-gold);
  margin-bottom: var(--space-2);
}

.collection-card__title {
  font-family: var(--font-heading);
  font-size: var(--text-3xl);
  margin-bottom: var(--space-1);
}

.collection-card__subtitle {
  font-size: var(--text-sm);
  opacity: 0.8;
  margin-bottom: var(--space-2);
}

.collection-card__description {
  font-size: var(--text-sm);
  opacity: 0.7;
  margin-bottom: var(--space-4);
  display: none;
}

.collection-card__cta {
  display: inline-flex;
  align-items: center;
  gap: var(--space-2);
  font-size: var(--text-sm);
  font-weight: var(--font-medium);
  text-transform: uppercase;
  letter-spacing: 0.1em;
  color: var(--color-gold);
  transition: gap var(--transition-base) var(--ease-out);
}

.collection-card:hover .collection-card__cta {
  gap: var(--space-3);
}

@media (max-width: 768px) {
  .collections__grid {
    grid-template-columns: 1fr;
  }
  
  .collection-card__description {
    display: block;
  }
}

/* ========================================
   FEATURED PRODUCTS
   ======================================== */
.featured {
  padding: var(--space-24) 0;
  background: var(--color-background-soft);
}

.featured__header {
  margin-bottom: var(--space-12);
}

.featured__grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: var(--space-8);
}

.featured__cta {
  text-align: center;
  margin-top: var(--space-12);
}

/* Product Card */
.product-card {
  position: relative;
  background: var(--color-background);
  border-radius: var(--radius-xl);
  overflow: hidden;
  transition: all var(--transition-base) var(--ease-out);
}

.product-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-card-hover);
}

.product-card__link {
  display: block;
  text-decoration: none;
  color: inherit;
}

.product-card__image {
  position: relative;
  aspect-ratio: 3/4;
  overflow: hidden;
}

.product-card__image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s var(--ease-out);
}

.product-card:hover .product-card__image img {
  transform: scale(1.05);
}

.product-card__badges {
  position: absolute;
  top: var(--space-3);
  left: var(--space-3);
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.product-card__overlay {
  position: absolute;
  inset: 0;
  background: rgba(28, 25, 23, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity var(--transition-base) var(--ease-out);
}

.product-card:hover .product-card__overlay {
  opacity: 1;
}

.quick-add-btn {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-3) var(--space-5);
  background: var(--color-background);
  color: var(--color-charcoal);
  border: none;
  border-radius: var(--radius-lg);
  font-size: var(--text-sm);
  font-weight: var(--font-medium);
  cursor: pointer;
  transform: translateY(10px);
  transition: all var(--transition-base) var(--ease-out);
}

.product-card:hover .quick-add-btn {
  transform: translateY(0);
}

.quick-add-btn:hover {
  background: var(--color-burgundy);
  color: var(--vt-c-white);
}

.product-card__info {
  padding: var(--space-4);
}

.product-card__category {
  font-size: var(--text-xs);
  color: var(--color-warm-gray);
  text-transform: uppercase;
  letter-spacing: 0.1em;
}

.product-card__name {
  font-family: var(--font-heading);
  font-size: var(--text-lg);
  color: var(--color-charcoal);
  margin: var(--space-1) 0 var(--space-2);
}

.product-card__meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-3);
}

.product-card__rating {
  display: flex;
  align-items: center;
  gap: var(--space-1);
  color: var(--color-gold);
  font-size: var(--text-sm);
  font-weight: var(--font-medium);
}

.product-card__reviews {
  color: var(--color-warm-gray);
  font-weight: var(--font-regular);
}

.product-card__occasion {
  font-size: var(--text-xs);
  color: var(--color-burgundy);
  padding: var(--space-1) var(--space-2);
  background: rgba(114, 47, 55, 0.08);
  border-radius: var(--radius-sm);
}

.product-card__price {
  font-family: var(--font-heading);
  font-size: var(--text-xl);
  color: var(--color-burgundy);
  font-weight: var(--font-semibold);
}

.wishlist-btn {
  position: absolute;
  top: var(--space-3);
  right: var(--space-3);
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-background);
  border: none;
  border-radius: var(--radius-full);
  color: var(--color-warm-gray);
  cursor: pointer;
  opacity: 0;
  transform: scale(0.8);
  transition: all var(--transition-base) var(--ease-out);
}

.product-card:hover .wishlist-btn {
  opacity: 1;
  transform: scale(1);
}

.wishlist-btn:hover {
  color: var(--color-burgundy);
}

@media (max-width: 1024px) {
  .featured__grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 640px) {
  .featured__grid {
    grid-template-columns: 1fr;
    max-width: 400px;
    margin: 0 auto;
  }
}

/* ========================================
   STORY SECTION
   ======================================== */
.story {
  display: grid;
  grid-template-columns: 1fr 1fr;
  min-height: 80vh;
}

.story__image {
  position: relative;
  overflow: hidden;
}

.story__image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.story__content {
  display: flex;
  flex-direction: column;
  justify-content: center;
  padding: var(--space-16);
  background: var(--color-burgundy-deep);
  color: var(--color-cream);
}

.story__label {
  font-size: var(--text-sm);
  text-transform: uppercase;
  letter-spacing: 0.2em;
  color: var(--color-gold);
  margin-bottom: var(--space-4);
}

.story__title {
  font-family: var(--font-heading);
  font-size: var(--text-4xl);
  margin-bottom: var(--space-6);
}

.story__text {
  font-size: var(--text-base);
  line-height: var(--leading-relaxed);
  opacity: 0.9;
  margin-bottom: var(--space-4);
}

.story__features {
  display: flex;
  gap: var(--space-8);
  margin: var(--space-8) 0;
  padding: var(--space-6) 0;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.story__feature {
  display: flex;
  flex-direction: column;
}

.story__feature-number {
  font-family: var(--font-heading);
  font-size: var(--text-3xl);
  color: var(--color-gold);
  font-weight: var(--font-bold);
}

.story__feature-label {
  font-size: var(--text-sm);
  opacity: 0.7;
}

@media (max-width: 1024px) {
  .story {
    grid-template-columns: 1fr;
  }
  
  .story__image {
    min-height: 50vh;
  }
  
  .story__content {
    padding: var(--space-10);
  }
}

/* ========================================
   TESTIMONIALS
   ======================================== */
.testimonials {
  padding: var(--space-24) 0;
  background: var(--color-background);
}

.testimonials__header {
  margin-bottom: var(--space-12);
}

.testimonials__grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: var(--space-8);
}

.testimonial-card {
  background: var(--color-cream);
  padding: var(--space-8);
  border-radius: var(--radius-xl);
  text-align: center;
}

.testimonial-card__rating {
  display: flex;
  justify-content: center;
  gap: var(--space-1);
  color: var(--color-gold);
  margin-bottom: var(--space-4);
}

.testimonial-card__text {
  font-family: var(--font-accent);
  font-size: var(--text-lg);
  font-style: italic;
  color: var(--color-charcoal);
  line-height: var(--leading-relaxed);
  margin-bottom: var(--space-6);
}

.testimonial-card__author {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--space-3);
}

.testimonial-card__author img {
  width: 48px;
  height: 48px;
  border-radius: var(--radius-full);
  object-fit: cover;
}

.testimonial-card__author strong {
  display: block;
  color: var(--color-charcoal);
  font-size: var(--text-sm);
}

.testimonial-card__author span {
  font-size: var(--text-xs);
  color: var(--color-warm-gray);
}

@media (max-width: 1024px) {
  .testimonials__grid {
    grid-template-columns: 1fr;
    max-width: 500px;
    margin: 0 auto;
  }
}

/* ========================================
   NEWSLETTER CTA
   ======================================== */
.newsletter-cta {
  padding: var(--space-24) 0;
  background: var(--color-forest-green);
}

.newsletter-cta__content {
  max-width: 600px;
  margin: 0 auto;
  text-align: center;
  color: var(--color-cream);
}

.newsletter-cta__label {
  display: block;
  font-size: var(--text-sm);
  text-transform: uppercase;
  letter-spacing: 0.2em;
  color: var(--color-gold);
  margin-bottom: var(--space-4);
}

.newsletter-cta__title {
  font-family: var(--font-heading);
  font-size: var(--text-4xl);
  margin-bottom: var(--space-4);
}

.newsletter-cta__text {
  font-size: var(--text-lg);
  opacity: 0.9;
  margin-bottom: var(--space-8);
}

.newsletter-cta__form {
  display: flex;
  gap: var(--space-3);
  max-width: 450px;
  margin: 0 auto var(--space-4);
}

.newsletter-cta__form input {
  flex: 1;
  padding: var(--space-4);
  border: none;
  border-radius: var(--radius-lg);
  font-size: var(--text-base);
}

.newsletter-cta__privacy {
  font-size: var(--text-sm);
  opacity: 0.6;
}

@media (max-width: 640px) {
  .newsletter-cta__form {
    flex-direction: column;
  }
}
</style>

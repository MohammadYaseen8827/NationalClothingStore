import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import ProductsView from '../views/ProductsView.vue'
import ProductDetailView from '../views/ProductDetailView.vue'
import CartView from '../views/CartView.vue'
import CheckoutView from '../views/CheckoutView.vue'
import NotFoundView from '../views/NotFoundView.vue'
import ComingSoonView from '../views/ComingSoonView.vue'
import { productRoutes } from './productRoutes'
import { salesRoutes } from './salesRoutes'
import { reportingRoutes } from './reportingRoutes'
import { procurementRoutes } from './procurementRoutes'
import { inventoryRoutes } from './inventoryRoutes'
import { customerRoutes } from './customerRoutes'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/products',
      name: 'products',
      component: ProductsView,
    },
    {
      path: '/products/:id',
      name: 'product-detail',
      component: ProductDetailView,
    },
    {
      path: '/cart',
      name: 'cart',
      component: CartView,
    },
    {
      path: '/checkout',
      name: 'checkout',
      component: CheckoutView,
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue'),
    },
    {
      path: '/system/health',
      name: 'SystemHealth',
      component: () => import('../views/SystemHealthView.vue'),
      meta: {
        title: 'System Health',
        requiresAuth: true,
        icon: 'fas fa-heartbeat',
        breadcrumb: 'System Health'
      }
    },
    ...productRoutes,
    ...salesRoutes,
    ...reportingRoutes,
    ...procurementRoutes,
    ...inventoryRoutes,
    ...customerRoutes,
    {
      path: '/coming-soon',
      name: 'coming-soon',
      component: ComingSoonView
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: NotFoundView
    }
  ],
})

export default router

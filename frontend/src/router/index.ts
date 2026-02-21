import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
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
  ],
})

export default router

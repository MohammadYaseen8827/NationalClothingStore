import type { RouteRecordRaw } from 'vue-router'

export const customerRoutes: RouteRecordRaw[] = [
  {
    path: '/customers',
    name: 'customer-list',
    component: () => import('@/components/Customers/CustomerManager.vue'),
    meta: { title: 'Customers', requiresAuth: true }
  },
  {
    path: '/customers/new',
    name: 'customer-new',
    component: () => import('@/components/Customers/CustomerForm.vue'),
    meta: { title: 'New Customer', requiresAuth: true }
  },
  {
    path: '/customers/:id',
    name: 'customer-profile',
    component: () => import('@/components/Customers/CustomerProfile.vue'),
    meta: { title: 'Customer Profile', requiresAuth: true }
  },
  {
    path: '/customers/:id/edit',
    name: 'customer-edit',
    component: () => import('@/components/Customers/CustomerForm.vue'),
    meta: { title: 'Edit Customer', requiresAuth: true }
  },
  {
    path: '/customers/:id/loyalty',
    name: 'customer-loyalty',
    component: () => import('@/components/Customers/LoyaltyPanel.vue'),
    meta: { title: 'Customer Loyalty', requiresAuth: true }
  }
]

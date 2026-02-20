import type { RouteRecordRaw } from 'vue-router'
import PointOfSale from '@/components/Sales/PointOfSale.vue'
import ReturnsExchanges from '@/components/Sales/ReturnsExchanges.vue'

export const salesRoutes: RouteRecordRaw[] = [
  {
    path: '/sales',
    name: 'Sales',
    component: PointOfSale,
    meta: {
      requiresAuth: true,
      roles: ['Cashier', 'SalesAssociate', 'Manager', 'Admin']
    }
  },
  {
    path: '/sales/returns',
    name: 'ReturnsExchanges',
    component: ReturnsExchanges,
    meta: {
      requiresAuth: true,
      roles: ['Cashier', 'SalesAssociate', 'Manager', 'Admin']
    }
  },
  {
    path: '/sales/history',
    name: 'SalesHistory',
    component: () => import('@/views/Sales/SalesHistory.vue'),
    meta: {
      requiresAuth: true,
      roles: ['Manager', 'Admin']
    }
  },
  {
    path: '/sales/reports',
    name: 'SalesReports',
    component: () => import('@/views/Sales/SalesReports.vue'),
    meta: {
      requiresAuth: true,
      roles: ['Manager', 'Admin']
    }
  }
]

// Export individual route names for easy reference
export const SALES_ROUTE_NAMES = {
  POS: 'Sales',
  RETURNS: 'ReturnsExchanges',
  HISTORY: 'SalesHistory',
  REPORTS: 'SalesReports'
} as const

export type SalesRouteName = typeof SALES_ROUTE_NAMES[keyof typeof SALES_ROUTE_NAMES]
import type { RouteRecordRaw } from 'vue-router'

export const inventoryRoutes: RouteRecordRaw[] = [
  {
    path: '/inventory',
    name: 'inventory-dashboard',
    component: () => import('@/components/Inventory/InventoryDashboard.vue'),
    meta: { title: 'Inventory Overview', requiresAuth: true }
  },
  {
    path: '/inventory/list',
    name: 'inventory-list',
    component: () => import('@/components/Inventory/InventoryList.vue'),
    meta: { title: 'Inventory List', requiresAuth: true }
  },
  {
    path: '/inventory/low-stock',
    name: 'inventory-low-stock',
    component: () => import('@/components/Inventory/LowStockAlerts.vue'),
    meta: { title: 'Low Stock Alerts', requiresAuth: true }
  },
  {
    path: '/inventory/transfers',
    name: 'inventory-transfers',
    component: () => import('@/components/Inventory/TransferManager.vue'),
    meta: { title: 'Stock Transfers', requiresAuth: true }
  },
  {
    path: '/inventory/adjustments',
    name: 'inventory-adjustments',
    component: () => import('@/components/Inventory/AdjustmentHistory.vue'),
    meta: { title: 'Adjustment History', requiresAuth: true }
  }
]

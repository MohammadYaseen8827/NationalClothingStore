import type { RouteRecordRaw } from 'vue-router'

export const procurementRoutes: RouteRecordRaw[] = [
  {
    path: '/procurement',
    name: 'procurement',
    redirect: '/procurement/suppliers',
    meta: { title: 'Procurement' }
  },
  {
    path: '/procurement/suppliers',
    name: 'suppliers',
    component: () => import('@/components/Suppliers/SupplierManager.vue'),
    meta: { title: 'Suppliers' }
  },
  {
    path: '/procurement/suppliers/:id',
    name: 'supplier-details',
    component: () => import('@/components/Suppliers/SupplierForm.vue'),
    props: true,
    meta: { title: 'Supplier Details' }
  },
  {
    path: '/procurement/suppliers/new',
    name: 'supplier-new',
    component: () => import('@/components/Suppliers/SupplierForm.vue'),
    meta: { title: 'New Supplier' }
  },
  {
    path: '/procurement/orders',
    name: 'purchase-orders',
    component: () => import('@/components/Suppliers/PurchaseOrderManager.vue'),
    meta: { title: 'Purchase Orders' }
  },
  {
    path: '/procurement/orders/:id',
    name: 'purchase-order-details',
    component: () => import('@/components/Suppliers/PurchaseOrderForm.vue'),
    props: true,
    meta: { title: 'Purchase Order Details' }
  },
  {
    path: '/procurement/orders/new',
    name: 'purchase-order-new',
    component: () => import('@/components/Suppliers/PurchaseOrderForm.vue'),
    meta: { title: 'New Purchase Order' }
  },
  {
    path: '/procurement/receiving',
    name: 'receiving',
    component: () => import('@/components/Suppliers/ReceivingManager.vue'),
    meta: { title: 'Receiving' }
  }
]

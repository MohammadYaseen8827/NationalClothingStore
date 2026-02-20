import type { RouteRecordRaw } from 'vue-router'

// Lazy load components
const CategoryManager = () => import('@/components/Categories/CategoryManager.vue')
const ProductManager = () => import('@/components/Products/ProductManager.vue')
const ProductVariationManager = () => import('@/components/Products/ProductVariationManager.vue')
const ProductDetails = () => import('@/components/Products/ProductDetails.vue')
const CategoryDetails = () => import('@/components/Categories/CategoryDetails.vue')

// Product catalog routes
export const productRoutes: RouteRecordRaw[] = [
  {
    path: '/catalog',
    name: 'Catalog',
    redirect: '/catalog/products',
    meta: {
      title: 'Product Catalog',
      requiresAuth: true,
      icon: 'fas fa-boxes',
      breadcrumb: 'Catalog'
    }
  },
  
  // Category routes
  {
    path: '/catalog/categories',
    name: 'Categories',
    component: CategoryManager,
    meta: {
      title: 'Categories',
      requiresAuth: true,
      icon: 'fas fa-folder',
      breadcrumb: 'Catalog > Categories',
      permissions: ['categories.read']
    }
  },
  
  {
    path: '/catalog/categories/:id',
    name: 'CategoryDetails',
    component: CategoryDetails,
    props: true,
    meta: {
      title: 'Category Details',
      requiresAuth: true,
      icon: 'fas fa-folder-open',
      breadcrumb: 'Catalog > Categories > Details',
      permissions: ['categories.read']
    }
  },
  
  // Product routes
  {
    path: '/catalog/products',
    name: 'Products',
    component: ProductManager,
    meta: {
      title: 'Products',
      requiresAuth: true,
      icon: 'fas fa-box',
      breadcrumb: 'Catalog > Products',
      permissions: ['products.read']
    }
  },
  
  {
    path: '/catalog/products/:id',
    name: 'ProductDetails',
    component: ProductDetails,
    props: true,
    meta: {
      title: 'Product Details',
      requiresAuth: true,
      icon: 'fas fa-box-open',
      breadcrumb: 'Catalog > Products > Details',
      permissions: ['products.read']
    }
  },
  
  // Product variation routes
  {
    path: '/catalog/products/:productId/variations',
    name: 'ProductVariations',
    component: ProductVariationManager,
    props: true,
    meta: {
      title: 'Product Variations',
      requiresAuth: true,
      icon: 'fas fa-layer-group',
      breadcrumb: 'Catalog > Products > Variations',
      permissions: ['products.read', 'variations.read']
    }
  },
  
  {
    path: '/catalog/products/:productId/variations/:variationId',
    name: 'VariationDetails',
    component: ProductVariationManager,
    props: (route) => ({
      productId: route.params.productId,
      variationId: route.params.variationId
    }),
    meta: {
      title: 'Variation Details',
      requiresAuth: true,
      icon: 'fas fa-layer-group',
      breadcrumb: 'Catalog > Products > Variations > Details',
      permissions: ['products.read', 'variations.read']
    }
  },
  
  // Quick access routes
  {
    path: '/catalog/low-stock',
    name: 'LowStock',
    component: ProductManager,
    meta: {
      title: 'Low Stock Items',
      requiresAuth: true,
      icon: 'fas fa-exclamation-triangle',
      breadcrumb: 'Catalog > Low Stock',
      permissions: ['products.read', 'inventory.read'],
      filters: {
        stockStatus: 'low'
      }
    }
  },
  
  {
    path: '/catalog/out-of-stock',
    name: 'OutOfStock',
    component: ProductManager,
    meta: {
      title: 'Out of Stock Items',
      requiresAuth: true,
      icon: 'fas fa-times-circle',
      breadcrumb: 'Catalog > Out of Stock',
      permissions: ['products.read', 'inventory.read'],
      filters: {
        stockStatus: 'out'
      }
    }
  },
  
  // Search routes
  {
    path: '/catalog/search',
    name: 'ProductSearch',
    component: ProductManager,
    meta: {
      title: 'Search Products',
      requiresAuth: true,
      icon: 'fas fa-search',
      breadcrumb: 'Catalog > Search',
      permissions: ['products.read']
    }
  },
  
  {
    path: '/catalog/search/:query',
    name: 'ProductSearchResults',
    component: ProductManager,
    props: true,
    meta: {
      title: 'Search Results',
      requiresAuth: true,
      icon: 'fas fa-search',
      breadcrumb: 'Catalog > Search > Results',
      permissions: ['products.read']
    }
  },
  
  // Category-specific routes
  {
    path: '/catalog/category/:categoryId',
    name: 'CategoryProducts',
    component: ProductManager,
    props: true,
    meta: {
      title: 'Category Products',
      requiresAuth: true,
      icon: 'fas fa-folder',
      breadcrumb: 'Catalog > Category Products',
      permissions: ['categories.read', 'products.read']
    }
  },
  
  // Brand-specific routes
  {
    path: '/catalog/brand/:brand',
    name: 'BrandProducts',
    component: ProductManager,
    props: true,
    meta: {
      title: 'Brand Products',
      requiresAuth: true,
      icon: 'fas fa-tag',
      breadcrumb: 'Catalog > Brand Products',
      permissions: ['products.read']
    }
  },
  
  // Season-specific routes
  {
    path: '/catalog/season/:season',
    name: 'SeasonProducts',
    component: ProductManager,
    props: true,
    meta: {
      title: 'Season Products',
      requiresAuth: true,
      icon: 'fas fa-calendar',
      breadcrumb: 'Catalog > Season Products',
      permissions: ['products.read']
    }
  }
]

// Route guard for authentication
export const productRouteGuards = {
  requiresAuth: (to: any, from: any, next: any) => {
    const token = localStorage.getItem('authToken')
    if (!token) {
      next({
        name: 'Login',
        query: { redirect: to.fullPath }
      })
    } else {
      next()
    }
  },
  
  requiresPermission: (permission: string) => {
    return (to: any, from: any, next: any) => {
      const userPermissions = JSON.parse(localStorage.getItem('userPermissions') || '[]')
      if (!userPermissions.includes(permission)) {
        next({
          name: 'Unauthorized'
        })
      } else {
        next()
      }
    }
  }
}

// Navigation items for sidebar/menu
export const productNavigationItems = [
  {
    title: 'Product Catalog',
    icon: 'fas fa-boxes',
    children: [
      {
        title: 'Categories',
        icon: 'fas fa-folder',
        route: { name: 'Categories' },
        permissions: ['categories.read']
      },
      {
        title: 'Products',
        icon: 'fas fa-box',
        route: { name: 'Products' },
        permissions: ['products.read']
      },
      {
        title: 'Low Stock',
        icon: 'fas fa-exclamation-triangle',
        route: { name: 'LowStock' },
        permissions: ['inventory.read'],
        badge: 'warning'
      },
      {
        title: 'Out of Stock',
        icon: 'fas fa-times-circle',
        route: { name: 'OutOfStock' },
        permissions: ['inventory.read'],
        badge: 'danger'
      },
      {
        title: 'Search',
        icon: 'fas fa-search',
        route: { name: 'ProductSearch' },
        permissions: ['products.read']
      }
    ]
  }
]

// Quick actions for catalog management
export const productQuickActions = [
  {
    title: 'Add Category',
    icon: 'fas fa-plus',
    route: { name: 'Categories' },
    action: 'create',
    permissions: ['categories.create']
  },
  {
    title: 'Add Product',
    icon: 'fas fa-plus',
    route: { name: 'Products' },
    action: 'create',
    permissions: ['products.create']
  },
  {
    title: 'Import Products',
    icon: 'fas fa-upload',
    route: { name: 'Products' },
    action: 'import',
    permissions: ['products.import']
  },
  {
    title: 'Export Catalog',
    icon: 'fas fa-download',
    route: { name: 'Products' },
    action: 'export',
    permissions: ['products.export']
  }
]

// Route configuration for breadcrumbs
export const breadcrumbConfig: Record<string, any> = {
  Catalog: {
    title: 'Product Catalog',
    icon: 'fas fa-boxes'
  },
  Categories: {
    title: 'Categories',
    parent: 'Catalog',
    icon: 'fas fa-folder'
  },
  Products: {
    title: 'Products',
    parent: 'Catalog',
    icon: 'fas fa-box'
  },
  CategoryDetails: {
    title: 'Category Details',
    parent: 'Categories',
    icon: 'fas fa-folder-open',
    dynamic: true
  },
  ProductDetails: {
    title: 'Product Details',
    parent: 'Products',
    icon: 'fas fa-box-open',
    dynamic: true
  },
  ProductVariations: {
    title: 'Variations',
    parent: 'ProductDetails',
    icon: 'fas fa-layer-group',
    dynamic: true
  },
  Search: {
    title: 'Search',
    parent: 'Catalog',
    icon: 'fas fa-search'
  }
}

// Utility functions for route handling
export const routeUtils = {
  // Get route title with dynamic data
  getRouteTitle(route: any, store?: any): string {
    const config = breadcrumbConfig[route.name as string]
    if (!config) return route.meta?.title || 'Unknown'
    
    if (config?.dynamic) {
      // For dynamic routes, append entity name
      if (route.name === 'CategoryDetails' && store?.categories) {
        const category = store.categories.find((c: any) => c.id === route.params.id)
        return `${config.title} - ${category?.name || 'Unknown'}`
      }
      
      if (route.name === 'ProductDetails' && store?.products) {
        const product = store.products.find((p: any) => p.id === route.params.id)
        return `${config.title} - ${product?.name || 'Unknown'}`
      }
      
      if (route.name === 'ProductVariations' && store?.products) {
        const product = store.products.find((p: any) => p.id === route.params.productId)
        return `${config.title} - ${product?.name || 'Unknown'}`
      }
    }
    
    return config.title
  },
  
  // Get breadcrumb items
  getBreadcrumbs(route: any, store?: any): Array<{title: string, route?: any, icon?: string}> {
    const breadcrumbs: Array<{title: string, route?: any, icon?: string}> = []
    const currentConfig = breadcrumbConfig[route.name as string]
    
    // Build breadcrumb trail
    if (currentConfig?.parent) {
      const parentConfig = breadcrumbConfig[currentConfig.parent as string]
      if (parentConfig) {
        breadcrumbs.push({
          title: parentConfig.title,
          route: parentConfig.route,
          icon: parentConfig.icon
        })
      }
    }
    
    // Add current route
    breadcrumbs.push({
      title: this.getRouteTitle(route, store),
      icon: currentConfig?.icon
    })
    
    return breadcrumbs
  },
  
  // Check if user has required permissions
  hasPermission(permission: string): boolean {
    const userPermissions = JSON.parse(localStorage.getItem('userPermissions') || '[]')
    return userPermissions.includes(permission)
  },
  
  // Check if user has any of the required permissions
  hasAnyPermission(permissions: string[]): boolean {
    const userPermissions = JSON.parse(localStorage.getItem('userPermissions') || '[]')
    return permissions.some(permission => userPermissions.includes(permission))
  },
  
  // Get navigation items filtered by permissions
  getFilteredNavigationItems(): typeof productNavigationItems {
    return productNavigationItems.map(item => ({
      ...item,
      children: item.children.filter(child => 
        this.hasAnyPermission(child.permissions || [])
      )
    })).filter(item => item.children.length > 0)
  },
  
  // Get quick actions filtered by permissions
  getFilteredQuickActions(): typeof productQuickActions {
    return productQuickActions.filter(action => 
      this.hasAnyPermission(action.permissions || [])
    )
  },
  
  // Generate route params for dynamic routes
  generateRouteParams(routeName: string, entity: any): any {
    switch (routeName) {
      case 'CategoryDetails':
        return { id: entity.id }
      case 'ProductDetails':
        return { id: entity.id }
      case 'ProductVariations':
        return { productId: entity.id }
      case 'VariationDetails':
        return { 
          productId: entity.productId, 
          variationId: entity.id 
        }
      case 'CategoryProducts':
        return { categoryId: entity.id }
      case 'BrandProducts':
        return { brand: entity.brand }
      case 'SeasonProducts':
        return { season: entity.season }
      default:
        return {}
    }
  }
}

export default {
  routes: productRoutes,
  guards: productRouteGuards,
  navigation: productNavigationItems,
  quickActions: productQuickActions,
  breadcrumbs: breadcrumbConfig,
  utils: routeUtils
}

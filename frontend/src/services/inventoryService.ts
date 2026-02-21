import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '/api'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json'
  }
})

apiClient.interceptors.request.use(
  (config: any) => {
    const token = localStorage.getItem('authToken')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error: any) => Promise.reject(error)
)

apiClient.interceptors.response.use(
  (response: any) => response,
  (error: any) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('authToken')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export interface Inventory {
  id: string
  productId: string
  productVariationId?: string
  branchId: string
  warehouseId?: string
  quantity: number
  reservedQuantity: number
  availableQuantity: number
  unitCost: number
  lastUpdated: string
  createdAt: string
}

export interface InventoryTransaction {
  id: string
  inventoryId: string
  productId: string
  productVariationId?: string
  transactionType: 'in' | 'out' | 'adjustment' | 'transfer'
  quantity: number
  reason: string
  referenceId?: string
  createdAt: string
  createdBy: string
  fromBranchId?: string
  toBranchId?: string
  fromWarehouseId?: string
  toWarehouseId?: string
}

export interface CreateInventoryRequest {
  productId: string
  productVariationId?: string
  branchId: string
  warehouseId?: string
  quantity: number
  unitCost: number
  reason: string
}

export interface UpdateInventoryRequest {
  quantity: number
  unitCost: number
  reason: string
}

export interface TransferInventoryRequest {
  fromInventoryId: string
  toBranchId: string
  toWarehouseId?: string
  quantity: number
  unitCost: number
  reason: string
}

export interface AdjustInventoryRequest {
  quantity: number
  unitCost: number
  reason: string
}

export interface InventoryStatistics {
  totalItems: number
  totalValue: number
  lowStockCount: number
  outOfStockCount: number
  branchCounts: Record<string, number>
  warehouseCounts: Record<string, number>
}

export interface LowStockAlert {
  inventoryId: string
  productId: string
  productVariationId?: string
  productName: string
  productSku: string
  productVariationSize?: string
  productVariationColor?: string
  branchId: string
  warehouseId?: string
  locationName: string
  currentQuantity: number
  lowStockThreshold: number
  reorderPoint: number
  unitCost: number
  totalValue: number
  alertDate: string
  isResolved: boolean
}

function handleAxiosError(error: unknown, fallbackMessage: string): never {
  if (axios.isAxiosError(error)) {
    const msg = (error as any).response?.data?.message || (error as any).response?.data || error.message
    if (typeof msg === 'string' && msg.trim().length > 0) {
      throw new Error(msg)
    }
  }
  throw new Error(fallbackMessage)
}

export const inventoryService = {
  /**
   * Get inventory by ID
   */
  async getInventory(id: string): Promise<Inventory> {
    try {
      const response = await apiClient.get<Inventory>(`/inventory/${id}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve inventory')
    }
  },

  /**
   * Get inventory by product
   */
  async getInventoryByProduct(productId: string): Promise<Inventory[]> {
    try {
      const response = await apiClient.get<Inventory[]>(`/inventory/by-product/${productId}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve inventory by product')
    }
  },

  /**
   * Get inventory by product variation
   */
  async getInventoryByVariation(productVariationId: string): Promise<Inventory[]> {
    try {
      const response = await apiClient.get<Inventory[]>(`/inventory/by-variation/${productVariationId}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve inventory by variation')
    }
  },

  /**
   * Get inventory by branch
   */
  async getInventoryByBranch(branchId: string): Promise<Inventory[]> {
    try {
      const response = await apiClient.get<Inventory[]>(`/inventory/by-branch/${branchId}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve inventory by branch')
    }
  },

  /**
   * Get inventory by warehouse
   */
  async getInventoryByWarehouse(warehouseId: string): Promise<Inventory[]> {
    try {
      const response = await apiClient.get<Inventory[]>(`/inventory/by-warehouse/${warehouseId}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve inventory by warehouse')
    }
  },

  /**
   * Get inventory by location
   */
  async getInventoryByLocation(params: {
    productId: string
    productVariationId?: string
    branchId: string
    warehouseId?: string
  }): Promise<Inventory | null> {
    try {
      const response = await apiClient.get<Inventory | null>('/inventory/by-location', { params })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve inventory by location')
    }
  },

  /**
   * Create new inventory
   */
  async createInventory(inventory: CreateInventoryRequest): Promise<Inventory> {
    try {
      const response = await apiClient.post<Inventory>('/inventory', inventory)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to create inventory')
    }
  },

  /**
   * Update existing inventory
   */
  async updateInventory(id: string, inventory: UpdateInventoryRequest): Promise<Inventory> {
    try {
      const response = await apiClient.put<Inventory>(`/inventory/${id}`, inventory)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to update inventory')
    }
  },

  /**
   * Delete inventory
   */
  async deleteInventory(id: string): Promise<void> {
    try {
      await apiClient.delete(`/inventory/${id}`)
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to delete inventory')
    }
  },

  /**
   * Update stock quantity
   */
  async updateStock(id: string, quantity: number): Promise<void> {
    try {
      await apiClient.put(`/inventory/${id}/stock`, { quantity })
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to update stock')
    }
  },

  /**
   * Transfer inventory between locations
   */
  async transferInventory(transfer: TransferInventoryRequest): Promise<any> {
    try {
      const response = await apiClient.post('/inventory/transfer', transfer)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to transfer inventory')
    }
  },

  /**
   * Adjust inventory quantity
   */
  async adjustInventory(id: string, adjustment: AdjustInventoryRequest): Promise<Inventory> {
    try {
      const response = await apiClient.put<Inventory>(`/inventory/${id}/adjust`, adjustment)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to adjust inventory')
    }
  },

  /**
   * Get inventory transactions
   */
  async getTransactions(params?: {
    inventoryId?: string
    productId?: string
    branchId?: string
    startDate?: string
    endDate?: string
  }): Promise<InventoryTransaction[]> {
    try {
      const response = await apiClient.get<InventoryTransaction[]>('/inventory/transaction/recent', { params })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve transactions')
    }
  },

  /**
   * Get inventory statistics
   */
  async getStatistics(params?: {
    branchId?: string
    warehouseId?: string
  }): Promise<InventoryStatistics> {
    try {
      const response = await apiClient.get<InventoryStatistics>('/inventory/statistics', { params })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve inventory statistics')
    }
  },

  /**
   * Get low stock items
   */
  async getLowStock(threshold: number = 10): Promise<Inventory[]> {
    try {
      const response = await apiClient.get<Inventory[]>('/inventory/low-stock', {
        params: { threshold }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve low stock items')
    }
  },

  /**
   * Get out of stock items
   */
  async getOutOfStock(): Promise<Inventory[]> {
    try {
      const response = await apiClient.get<Inventory[]>('/inventory/out-of-stock')
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve out of stock items')
    }
  },

  /**
   * Get low stock alerts
   */
  async getLowStockAlerts(params?: {
    branchId?: string
    warehouseId?: string
  }): Promise<LowStockAlert[]> {
    try {
      const response = await apiClient.get<LowStockAlert[]>('/inventory/alerts/low-stock', { params })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve low stock alerts')
    }
  },

  /**
   * Reserve inventory
   */
  async reserveInventory(id: string, quantity: number): Promise<void> {
    try {
      await apiClient.post(`/inventory/${id}/reserve`, { quantity })
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to reserve inventory')
    }
  },

  /**
   * Release inventory reservation
   */
  async releaseInventory(id: string, quantity: number): Promise<void> {
    try {
      await apiClient.post(`/inventory/${id}/release`, { quantity })
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to release inventory')
    }
  },

  /**
   * Bulk transfer inventory
   */
  async bulkTransferInventory(transfers: TransferInventoryRequest[]): Promise<any> {
    try {
      const response = await apiClient.post('/inventory/bulk-transfer', transfers)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to bulk transfer inventory')
    }
  },

  /**
   * Bulk adjust inventory
   */
  async bulkAdjustInventory(adjustments: AdjustInventoryRequest[]): Promise<Inventory[]> {
    try {
      const response = await apiClient.put<Inventory[]>('/inventory/bulk-adjust', adjustments)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to bulk adjust inventory')
    }
  },

  /**
   * Search inventory
   */
  async searchInventory(query: any): Promise<any> {
    try {
      const response = await apiClient.post('/inventory/search', query)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to search inventory')
    }
  },

  /**
   * Get inventory value by location
   */
  async getInventoryValueByLocation(): Promise<any[]> {
    try {
      const response = await apiClient.get<any[]>('/inventory/value-by-location')
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to get inventory value by location')
    }
  },

  /**
   * Generate inventory report
   */
  async generateInventoryReport(): Promise<any> {
    try {
      const response = await apiClient.get('/inventory/report')
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to generate inventory report')
    }
  },

  /**
   * Create inventory transaction
   */
  async createTransaction(data: any): Promise<InventoryTransaction> {
    try {
      const response = await apiClient.post<InventoryTransaction>('/inventory/transaction', data)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to create transaction')
    }
  },

  /**
   * Get transaction by ID
   */
  async getTransactionById(id: string): Promise<InventoryTransaction> {
    try {
      const response = await apiClient.get<InventoryTransaction>(`/inventory/transaction/${id}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to get transaction')
    }
  },

  /**
   * Search transactions
   */
  async searchTransactions(query: any): Promise<any> {
    try {
      const response = await apiClient.post('/inventory/transaction/search', query)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to search transactions')
    }
  },

  /**
   * Send low stock alerts
   */
  async sendLowStockAlerts(): Promise<void> {
    try {
      await apiClient.post('/inventory/alerts/low-stock/send')
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to send low stock alerts')
    }
  }
}

export default inventoryService

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

export interface Supplier {
  id: string
  name: string
  code: string
  contactName?: string
  email?: string
  phone?: string
  address?: string
  city?: string
  state?: string
  zipCode?: string
  country?: string
  notes?: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateSupplierRequest {
  name: string
  code: string
  contactName?: string
  email?: string
  phone?: string
  address?: string
  city?: string
  state?: string
  zipCode?: string
  country?: string
  notes?: string
}

export interface UpdateSupplierRequest {
  name: string
  code: string
  contactName?: string
  email?: string
  phone?: string
  address?: string
  city?: string
  state?: string
  zipCode?: string
  country?: string
  notes?: string
  isActive: boolean
}

export interface PurchaseOrder {
  id: string
  orderNumber: string
  supplierId: string
  supplierName: string
  status: 'Draft' | 'Pending' | 'Approved' | 'Ordered' | 'Received' | 'Cancelled'
  orderDate: string
  expectedDeliveryDate?: string
  receivedDate?: string
  totalAmount: number
  taxAmount: number
  discountAmount: number
  notes?: string
  items: PurchaseOrderItem[]
  createdAt: string
  updatedAt: string
}

export interface PurchaseOrderItem {
  id: string
  productId: string
  productName: string
  productSku: string
  quantity: number
  unitCost: number
  totalCost: number
  quantityReceived: number
}

export interface CreatePurchaseOrderRequest {
  supplierId: string
  expectedDeliveryDate?: string
  notes?: string
  items: CreatePurchaseOrderItemRequest[]
}

export interface CreatePurchaseOrderItemRequest {
  productId: string
  quantity: number
  unitCost: number
}

export interface UpdatePurchaseOrderRequest {
  status?: string
  expectedDeliveryDate?: string
  notes?: string
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

export const procurementService = {
  // Supplier endpoints
  /**
   * Get supplier by ID
   */
  async getSupplier(id: string): Promise<Supplier> {
    try {
      const response = await apiClient.get<Supplier>(`/procurement/suppliers/${id}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve supplier')
    }
  },

  /**
   * Get supplier by code
   */
  async getSupplierByCode(code: string): Promise<Supplier> {
    try {
      const response = await apiClient.get<Supplier>(`/procurement/suppliers/by-code/${code}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve supplier by code')
    }
  },

  /**
   * Get all suppliers
   */
  async getSuppliers(params?: {
    page?: number
    pageSize?: number
  }): Promise<Supplier[]> {
    try {
      const response = await apiClient.get<Supplier[]>('/procurement/suppliers', { params })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve suppliers')
    }
  },

  /**
   * Create a new supplier
   */
  async createSupplier(supplier: CreateSupplierRequest): Promise<Supplier> {
    try {
      const response = await apiClient.post<Supplier>('/procurement/suppliers', supplier)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to create supplier')
    }
  },

  /**
   * Update an existing supplier
   */
  async updateSupplier(id: string, supplier: UpdateSupplierRequest): Promise<Supplier> {
    try {
      const response = await apiClient.put<Supplier>(`/procurement/suppliers/${id}`, supplier)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to update supplier')
    }
  },

  /**
   * Delete a supplier
   */
  async deleteSupplier(id: string): Promise<void> {
    try {
      await apiClient.delete(`/procurement/suppliers/${id}`)
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to delete supplier')
    }
  },

  // Purchase Order endpoints
  /**
   * Get purchase order by ID
   */
  async getPurchaseOrder(id: string): Promise<PurchaseOrder> {
    try {
      const response = await apiClient.get<PurchaseOrder>(`/procurement/purchase-orders/${id}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve purchase order')
    }
  },

  /**
   * Get purchase order by order number
   */
  async getPurchaseOrderByNumber(orderNumber: string): Promise<PurchaseOrder> {
    try {
      const response = await apiClient.get<PurchaseOrder>(`/procurement/purchase-orders/by-number/${orderNumber}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve purchase order by number')
    }
  },

  /**
   * Get all purchase orders
   */
  async getPurchaseOrders(params?: {
    page?: number
    pageSize?: number
  }): Promise<PurchaseOrder[]> {
    try {
      const response = await apiClient.get<PurchaseOrder[]>('/procurement/purchase-orders', { params })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve purchase orders')
    }
  },

  /**
   * Get purchase orders by supplier
   */
  async getPurchaseOrdersBySupplier(supplierId: string): Promise<PurchaseOrder[]> {
    try {
      const response = await apiClient.get<PurchaseOrder[]>(`/procurement/purchase-orders/by-supplier/${supplierId}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve purchase orders by supplier')
    }
  },

  /**
   * Get purchase orders by status
   */
  async getPurchaseOrdersByStatus(status: string): Promise<PurchaseOrder[]> {
    try {
      const response = await apiClient.get<PurchaseOrder[]>(`/procurement/purchase-orders/by-status/${status}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve purchase orders by status')
    }
  },

  /**
   * Create a new purchase order
   */
  async createPurchaseOrder(order: CreatePurchaseOrderRequest): Promise<PurchaseOrder> {
    try {
      const response = await apiClient.post<PurchaseOrder>('/procurement/purchase-orders', order)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to create purchase order')
    }
  },

  /**
   * Update an existing purchase order
   */
  async updatePurchaseOrder(id: string, order: UpdatePurchaseOrderRequest): Promise<PurchaseOrder> {
    try {
      const response = await apiClient.put<PurchaseOrder>(`/procurement/purchase-orders/${id}`, order)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to update purchase order')
    }
  },

  /**
   * Delete a purchase order
   */
  async deletePurchaseOrder(id: string): Promise<void> {
    try {
      await apiClient.delete(`/procurement/purchase-orders/${id}`)
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to delete purchase order')
    }
  }
}

export default procurementService

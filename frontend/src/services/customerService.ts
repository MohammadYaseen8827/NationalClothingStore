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

export interface Customer {
  id: string
  firstName: string
  lastName: string
  email: string
  phone?: string
  address?: string
  city?: string
  state?: string
  zipCode?: string
  country?: string
  dateOfBirth?: string
  loyaltyPoints: number
  loyaltyTier?: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateCustomerRequest {
  firstName: string
  lastName: string
  email: string
  phone?: string
  address?: string
  city?: string
  state?: string
  zipCode?: string
  country?: string
  dateOfBirth?: string
}

export interface UpdateCustomerRequest {
  firstName: string
  lastName: string
  email: string
  phone?: string
  address?: string
  city?: string
  state?: string
  zipCode?: string
  country?: string
  dateOfBirth?: string
  isActive: boolean
}

export interface AddLoyaltyPointsRequest {
  points: number
  reason: string
}

export interface RedeemLoyaltyPointsRequest {
  points: number
  reason: string
}

export interface CustomerLoyalty {
  customerId: string
  points: number
  tier: string
  totalPointsEarned: number
  totalPointsRedeemed: number
  transactionsCount: number
  totalSpent: number
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

export const customerService = {
  /**
   * Get customer by ID
   */
  async getCustomer(id: string): Promise<Customer> {
    try {
      const response = await apiClient.get<Customer>(`/customers/${id}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve customer')
    }
  },

  /**
   * Get all customers with pagination
   */
  async getCustomers(params?: {
    page?: number
    pageSize?: number
  }): Promise<Customer[]> {
    try {
      const response = await apiClient.get<Customer[]>('/customers', { params })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve customers')
    }
  },

  /**
   * Search customer by email or name
   */
  async searchCustomer(query: string): Promise<Customer> {
    try {
      const response = await apiClient.get<Customer>(`/customers/search`, {
        params: { query }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to search customer')
    }
  },

  /**
   * Create a new customer
   */
  async createCustomer(customer: CreateCustomerRequest): Promise<Customer> {
    try {
      const response = await apiClient.post<Customer>('/customers', customer)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to create customer')
    }
  },

  /**
   * Update an existing customer
   */
  async updateCustomer(id: string, customer: UpdateCustomerRequest): Promise<Customer> {
    try {
      const response = await apiClient.put<Customer>(`/customers/${id}`, customer)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to update customer')
    }
  },

  /**
   * Delete a customer
   */
  async deleteCustomer(id: string): Promise<void> {
    try {
      await apiClient.delete(`/customers/${id}`)
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to delete customer')
    }
  },

  /**
   * Get customer loyalty information
   */
  async getCustomerLoyalty(id: string): Promise<CustomerLoyalty> {
    try {
      const response = await apiClient.get<CustomerLoyalty>(`/customers/${id}/loyalty`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve customer loyalty')
    }
  },

  /**
   * Add loyalty points to customer
   */
  async addLoyaltyPoints(id: string, request: AddLoyaltyPointsRequest): Promise<CustomerLoyalty> {
    try {
      const response = await apiClient.post<CustomerLoyalty>(
        `/customers/${id}/loyalty/add-points`,
        request
      )
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to add loyalty points')
    }
  },

  /**
   * Redeem loyalty points from customer
   */
  async redeemLoyaltyPoints(id: string, request: RedeemLoyaltyPointsRequest): Promise<CustomerLoyalty> {
    try {
      const response = await apiClient.post<CustomerLoyalty>(
        `/customers/${id}/loyalty/redeem-points`,
        request
      )
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to redeem loyalty points')
    }
  }
}

export default customerService

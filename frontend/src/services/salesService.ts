import axios from 'axios'
import type { 
  ProcessSaleRequest, 
  SalesTransaction,
  ProcessReturnRequest 
} from '@/types/sales'

const API_BASE_URL = '/api'

export const salesService = {
  /**
   * Process a new sale transaction
   */
  async processSale(request: ProcessSaleRequest): Promise<SalesTransaction> {
    try {
      const response = await axios.post<SalesTransaction>(
        `${API_BASE_URL}/sales/process-sale`,
        request
      )
      return response.data
    } catch (error: unknown) {
      if (axios.isAxiosError(error) && (error as any).response?.data?.message) {
        throw new Error((error as any).response.data.message)
      }
      throw new Error('Failed to process sale')
    }
  },

  /**
   * Process a return transaction
   */
  async processReturn(request: ProcessReturnRequest): Promise<SalesTransaction> {
    try {
      const response = await axios.post<SalesTransaction>(
        `${API_BASE_URL}/sales/process-return`,
        request
      )
      return response.data
    } catch (error: unknown) {
      if (axios.isAxiosError(error) && (error as any).response?.data?.message) {
        throw new Error((error as any).response.data.message)
      }
      throw new Error('Failed to process return')
    }
  },

  /**
   * Process an exchange transaction
   */
  async processExchange(request: any): Promise<SalesTransaction> {
    try {
      const response = await axios.post<SalesTransaction>(
        `${API_BASE_URL}/sales/process-exchange`,
        request
      )
      return response.data
    } catch (error: unknown) {
      if (axios.isAxiosError(error) && (error as any).response?.data?.message) {
        throw new Error((error as any).response.data.message)
      }
      throw new Error('Failed to process exchange')
    }
  },

  /**
   * Get sales transaction by ID
   */
  async getTransactionById(id: string): Promise<SalesTransaction> {
    try {
      const response = await axios.get<SalesTransaction>(
        `${API_BASE_URL}/sales/${id}`
      )
      return response.data
    } catch (error: unknown) {
      if (axios.isAxiosError(error) && (error as any).response?.data?.message) {
        throw new Error((error as any).response.data.message)
      }
      throw new Error('Failed to retrieve transaction')
    }
  },

  /**
   * Get sales transaction by transaction number
   */
  async getTransactionByNumber(transactionNumber: string): Promise<SalesTransaction> {
    try {
      const response = await axios.get<SalesTransaction>(
        `${API_BASE_URL}/sales/by-number/${transactionNumber}`
      )
      return response.data
    } catch (error: unknown) {
      if (axios.isAxiosError(error) && (error as any).response?.data?.message) {
        throw new Error((error as any).response.data.message)
      }
      throw new Error('Failed to retrieve transaction')
    }
  },

  /**
   * Get paginated sales transactions
   */
  async getTransactions(params: {
    pageNumber?: number
    pageSize?: number
    branchId?: string
    customerId?: string
    transactionType?: string
    status?: string
    startDate?: string
    endDate?: string
  }): Promise<{ items: SalesTransaction[], totalCount: number }> {
    try {
      const response = await axios.get<{ Items: SalesTransaction[]; TotalCount: number }>(`${API_BASE_URL}/sales`, { params })
      // Map backend PagedResponse format (PascalCase) to frontend expected format (camelCase)
      const data = response.data
      return {
        items: data.Items || [],
        totalCount: data.TotalCount || 0
      }
    } catch (error: unknown) {
      if (axios.isAxiosError(error) && (error as any).response?.data?.message) {
        throw new Error((error as any).response.data.message)
      }
      throw new Error('Failed to retrieve transactions')
    }
  },

  /**
   * Get sales statistics for a period
   */
  async getSalesStatistics(startDate: string, endDate: string, branchId?: string): Promise<any> {
    try {
      const params: any = { startDate, endDate }
      if (branchId) params.branchId = branchId
      
      const response = await axios.get(`${API_BASE_URL}/sales/statistics`, { params })
      return response.data
    } catch (error: unknown) {
      if (axios.isAxiosError(error) && (error as any).response?.data?.message) {
        throw new Error((error as any).response.data.message)
      }
      throw new Error('Failed to retrieve sales statistics')
    }
  },

  /**
   * Get top selling products for a period
   */
  async getTopSellingProducts(
    startDate: string, 
    endDate: string, 
    limit: number = 10, 
    branchId?: string
  ): Promise<any[]> {
    try {
      const params: any = { startDate, endDate, limit }
      if (branchId) params.branchId = branchId
      
      const response = await axios.get<any[]>(`${API_BASE_URL}/sales/top-products`, { params })
      return response.data
    } catch (error: unknown) {
      if (axios.isAxiosError(error) && (error as any).response?.data?.message) {
        throw new Error((error as any).response.data.message)
      }
      throw new Error('Failed to retrieve top selling products')
    }
  },

  /**
   * Cancel a pending transaction
   */
  async cancelTransaction(id: string, reason: string): Promise<void> {
    try {
      await axios.put(`${API_BASE_URL}/sales/${id}/cancel`, { reason })
    } catch (error: unknown) {
      if (axios.isAxiosError(error) && (error as any).response?.data?.message) {
        throw new Error((error as any).response.data.message)
      }
      throw new Error('Failed to cancel transaction')
    }
  }
}
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

export type AnalyticsPeriod = 'Daily' | 'Weekly' | 'Monthly' | 'Quarterly' | 'Yearly'
export type PredictionType = 'SalesForecast' | 'DemandPrediction' | 'ChurnPrediction' | 'InventoryOptimization'

export interface ExportRequest {
  reportType: string
  startDate: string
  endDate: string
  format: string
  parameters?: Record<string, any>
}

export interface ExportResult {
  fileId: string
  fileName: string
  format: string
  status: string
  downloadUrl?: string
  generatedAt?: string
  estimatedCompletion?: string
}

function toIsoDate(value: string | Date): string {
  if (typeof value === 'string') return value
  return value.toISOString()
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

export const reportingService = {
  async getSalesReport(params: {
    startDate: string | Date
    endDate: string | Date
    branchId?: string | null
    warehouseId?: string | null
  }): Promise<any> {
    try {
      const response = await apiClient.get('/reporting/sales', {
        params: {
          startDate: toIsoDate(params.startDate),
          endDate: toIsoDate(params.endDate),
          branchId: params.branchId ?? undefined,
          warehouseId: params.warehouseId ?? undefined
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve sales report')
    }
  },

  async getInventoryReport(params?: {
    branchId?: string | null
    warehouseId?: string | null
  }): Promise<any> {
    try {
      const response = await apiClient.get('/reporting/inventory', {
        params: {
          branchId: params?.branchId ?? undefined,
          warehouseId: params?.warehouseId ?? undefined
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve inventory report')
    }
  },

  async getCustomerReport(params: {
    startDate: string | Date
    endDate: string | Date
  }): Promise<any> {
    try {
      const response = await apiClient.get('/reporting/customers', {
        params: {
          startDate: toIsoDate(params.startDate),
          endDate: toIsoDate(params.endDate)
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve customer report')
    }
  },

  async getProcurementReport(params: {
    startDate: string | Date
    endDate: string | Date
  }): Promise<any> {
    try {
      const response = await apiClient.get('/reporting/procurement', {
        params: {
          startDate: toIsoDate(params.startDate),
          endDate: toIsoDate(params.endDate)
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve procurement report')
    }
  },

  async getFinancialReport(params: {
    startDate: string | Date
    endDate: string | Date
  }): Promise<any> {
    try {
      const response = await apiClient.get('/reporting/financial', {
        params: {
          startDate: toIsoDate(params.startDate),
          endDate: toIsoDate(params.endDate)
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve financial report')
    }
  },

  async getSalesAnalytics(params: {
    startDate: string | Date
    endDate: string | Date
    period?: AnalyticsPeriod | string
    branchId?: string | null
    warehouseId?: string | null
  }): Promise<any> {
    try {
      const response = await apiClient.get('/reporting/analytics/sales', {
        params: {
          startDate: toIsoDate(params.startDate),
          endDate: toIsoDate(params.endDate),
          period: params.period ?? 'Daily',
          branchId: params.branchId ?? undefined,
          warehouseId: params.warehouseId ?? undefined
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve sales analytics')
    }
  },

  async getInventoryAnalytics(params?: {
    branchId?: string | null
    warehouseId?: string | null
  }): Promise<any> {
    try {
      const response = await apiClient.get('/reporting/analytics/inventory', {
        params: {
          branchId: params?.branchId ?? undefined,
          warehouseId: params?.warehouseId ?? undefined
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve inventory analytics')
    }
  },

  async getCustomerAnalytics(params: {
    startDate: string | Date
    endDate: string | Date
  }): Promise<any> {
    try {
      const response = await apiClient.get('/reporting/analytics/customers', {
        params: {
          startDate: toIsoDate(params.startDate),
          endDate: toIsoDate(params.endDate)
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve customer analytics')
    }
  },

  async getFinancialAnalytics(params: {
    startDate: string | Date
    endDate: string | Date
    period?: AnalyticsPeriod | string
  }): Promise<any> {
    try {
      const response = await apiClient.get('/reporting/analytics/financial', {
        params: {
          startDate: toIsoDate(params.startDate),
          endDate: toIsoDate(params.endDate),
          period: params.period ?? 'Monthly'
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve financial analytics')
    }
  },

  async getPredictiveAnalytics(params: {
    startDate: string | Date
    endDate: string | Date
    predictionType: PredictionType | string
  }): Promise<any> {
    try {
      const response = await apiClient.get('/reporting/analytics/predictive', {
        params: {
          startDate: toIsoDate(params.startDate),
          endDate: toIsoDate(params.endDate),
          predictionType: params.predictionType
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve predictive analytics')
    }
  },

  async getDashboardSummary(params?: {
    branchId?: string | null
    warehouseId?: string | null
  }): Promise<any> {
    try {
      const response = await apiClient.get('/reporting/dashboard/summary', {
        params: {
          branchId: params?.branchId ?? undefined,
          warehouseId: params?.warehouseId ?? undefined
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve dashboard summary')
    }
  },

  async getRealTimeMetrics(params?: {
    branchId?: string | null
    warehouseId?: string | null
  }): Promise<any> {
    try {
      const response = await apiClient.get('/reporting/dashboard/realtime', {
        params: {
          branchId: params?.branchId ?? undefined,
          warehouseId: params?.warehouseId ?? undefined
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve real-time metrics')
    }
  },

  async exportReport(request: ExportRequest): Promise<ExportResult> {
    try {
      const response = await apiClient.post<ExportResult>('/reporting/export', {
        reportType: request.reportType,
        startDate: toIsoDate(request.startDate),
        endDate: toIsoDate(request.endDate),
        format: request.format,
        parameters: request.parameters ?? {}
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to start export')
    }
  },

  async getExportStatus(fileId: string): Promise<ExportResult> {
    try {
      const response = await apiClient.get<ExportResult>(`/reporting/export/${fileId}/status`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to get export status')
    }
  },

  async downloadExport(fileId: string): Promise<Blob> {
    try {
      const response = await apiClient.get(`/reporting/export/${fileId}/download`, {
        responseType: 'blob'
      })
      return response.data as Blob
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to download export')
    }
  },

  async runCustomReport(payload: any): Promise<any> {
    try {
      const response = await apiClient.post('/reporting/custom/run', payload)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to run custom report')
    }
  },

  async exportCustomReport(payload: any): Promise<any> {
    try {
      const response = await apiClient.post('/reporting/custom/export', payload)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to export custom report')
    }
  }
}

export default reportingService

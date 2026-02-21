import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '/api'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
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

export interface HealthStatus {
  status: 'healthy' | 'degraded' | 'unhealthy'
  timestamp: string
  version?: string
  uptime?: number
}

export interface ReadinessStatus {
  ready: boolean
  checks: {
    database: boolean
    cache: boolean
    externalServices: boolean
  }
}

export interface DetailedHealth {
  status: 'healthy' | 'degraded' | 'unhealthy'
  timestamp: string
  version: string
  uptime: number
  checks: {
    database: { status: string; responseTime?: number; error?: string }
    cache: { status: string; responseTime?: number; error?: string }
    externalServices: { status: string; responseTime?: number; error?: string }
  }
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

export const healthService = {
  /**
   * Get basic health status
   */
  async getHealth(): Promise<HealthStatus> {
    try {
      const response = await apiClient.get<HealthStatus>('/v1/Health')
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to get health status')
    }
  },

  /**
   * Get readiness status
   */
  async getReadiness(): Promise<ReadinessStatus> {
    try {
      const response = await apiClient.get<ReadinessStatus>('/v1/Health/readiness')
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to get readiness status')
    }
  },

  /**
   * Get detailed health information
   */
  async getDetailedHealth(): Promise<DetailedHealth> {
    try {
      const response = await apiClient.get<DetailedHealth>('/v1/Health/detailed')
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to get detailed health')
    }
  }
}

export default healthService

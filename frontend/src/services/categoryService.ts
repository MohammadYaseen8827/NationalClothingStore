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

export interface Category {
  id: string
  name: string
  description?: string
  code?: string
  parentId?: string
  sortOrder?: number
  isActive: boolean
  createdAt: string
  updatedAt: string
  children?: Category[]
}

export interface CreateCategoryRequest {
  name: string
  description?: string
  code?: string
  parentId?: string
  sortOrder?: number
}

export interface UpdateCategoryRequest {
  name: string
  description?: string
  code?: string
  parentId?: string
  sortOrder?: number
  isActive: boolean
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

export const categoryService = {
  /**
   * Get all categories
   */
  async getCategories(params?: { includeInactive?: boolean }): Promise<Category[]> {
    try {
      const response = await apiClient.get<Category[]>('/categories', { params })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve categories')
    }
  },

  /**
   * Get category by ID
   */
  async getCategory(id: string): Promise<Category> {
    try {
      const response = await apiClient.get<Category>(`/categories/${id}`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve category')
    }
  },

  /**
   * Get root categories (no parent)
   */
  async getRootCategories(): Promise<Category[]> {
    try {
      const response = await apiClient.get<Category[]>('/categories/root')
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve root categories')
    }
  },

  /**
   * Get child categories of a parent
   */
  async getChildCategories(parentId: string): Promise<Category[]> {
    try {
      const response = await apiClient.get<Category[]>(`/categories/${parentId}/children`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to retrieve child categories')
    }
  },

  /**
   * Create a new category
   */
  async createCategory(category: CreateCategoryRequest): Promise<Category> {
    try {
      const response = await apiClient.post<Category>('/categories', category)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to create category')
    }
  },

  /**
   * Update an existing category
   */
  async updateCategory(id: string, category: UpdateCategoryRequest): Promise<Category> {
    try {
      const response = await apiClient.put<Category>(`/categories/${id}`, category)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to update category')
    }
  },

  /**
   * Delete a category
   */
  async deleteCategory(id: string): Promise<void> {
    try {
      await apiClient.delete(`/categories/${id}`)
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to delete category')
    }
  },

  /**
   * Validate category deletion (check if category can be safely deleted)
   */
  async validateCategoryDeletion(id: string): Promise<{ canDelete: boolean; reason?: string }> {
    try {
      const response = await apiClient.get<{ canDelete: boolean; reason?: string }>(`/categories/${id}/validate-deletion`)
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to validate category deletion')
    }
  }
}

export default categoryService

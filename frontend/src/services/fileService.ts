import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '/api'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 60000,
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

export interface FileUploadResult {
  fileName: string
  filePath: string
  fileSize: number
  contentType: string
  uploadedAt: string
}

export interface FileInfo {
  fileName: string
  filePath: string
  fileSize: number
  contentType: string
  uploadedAt: string
}

export interface DeleteFileRequest {
  filePath: string
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

export const fileService = {
  /**
   * Upload a single file
   */
  async uploadFile(file: File): Promise<FileUploadResult> {
    const formData = new FormData()
    formData.append('file', file)
    
    try {
      const response = await apiClient.post<FileUploadResult>('/files/upload', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to upload file')
    }
  },

  /**
   * Upload multiple files
   */
  async uploadFiles(files: File[]): Promise<FileUploadResult[]> {
    const formData = new FormData()
    files.forEach(file => {
      formData.append('files', file)
    })
    
    try {
      const response = await apiClient.post<FileUploadResult[]>('/files/upload-multiple', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to upload files')
    }
  },

  /**
   * Delete a file
   */
  async deleteFile(filePath: string): Promise<void> {
    try {
      await apiClient.delete('/files', { data: { filePath } })
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to delete file')
    }
  },

  /**
   * Get file information
   */
  async getFileInfo(filePath: string): Promise<FileInfo> {
    try {
      const response = await apiClient.get<FileInfo>('/files/info', {
        params: { filePath }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to get file info')
    }
  },

  /**
   * Check if file exists
   */
  async fileExists(filePath: string): Promise<boolean> {
    try {
      const response = await apiClient.get<boolean>('/files/exists', {
        params: { filePath }
      })
      return response.data
    } catch (error: unknown) {
      handleAxiosError(error, 'Failed to check file existence')
    }
  }
}

export default fileService

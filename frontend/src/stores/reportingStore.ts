import { defineStore } from 'pinia'
import { ref } from 'vue'
import reportingService, { type ExportRequest } from '@/services/reportingService'

export const useReportingStore = defineStore('reporting', () => {
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const recentReports = ref<any[]>([])

  const setError = (message: string | null) => {
    error.value = message
  }

  const clearError = () => {
    error.value = null
  }

  const mapLocationParams = (params?: { locationId?: string | null }) => {
    if (!params?.locationId) return { branchId: undefined as string | undefined }
    return { branchId: params.locationId }
  }

  const getDashboardSummary = async (params?: { locationId?: string | null }) => {
    isLoading.value = true
    clearError()

    try {
      const mapped = mapLocationParams(params)
      return await reportingService.getDashboardSummary({
        branchId: mapped.branchId ?? null
      })
    } catch (e: any) {
      setError(e?.message || 'Failed to retrieve dashboard summary')
      throw e
    } finally {
      isLoading.value = false
    }
  }

  const getRealTimeMetrics = async (params?: { locationId?: string | null }) => {
    isLoading.value = true
    clearError()

    try {
      const mapped = mapLocationParams(params)
      return await reportingService.getRealTimeMetrics({
        branchId: mapped.branchId ?? null
      })
    } catch (e: any) {
      setError(e?.message || 'Failed to retrieve real-time metrics')
      throw e
    } finally {
      isLoading.value = false
    }
  }

  const getSalesReport = async (params: {
    startDate: string | Date
    endDate: string | Date
    locationId?: string | null
  }) => {
    isLoading.value = true
    clearError()

    try {
      const mapped = mapLocationParams({ locationId: params.locationId ?? null })
      return await reportingService.getSalesReport({
        startDate: params.startDate,
        endDate: params.endDate,
        branchId: mapped.branchId ?? null
      })
    } catch (e: any) {
      setError(e?.message || 'Failed to retrieve sales report')
      throw e
    } finally {
      isLoading.value = false
    }
  }

  const getSalesAnalytics = async (params: {
    startDate: string | Date
    endDate: string | Date
    period?: string
    locationId?: string | null
  }) => {
    isLoading.value = true
    clearError()

    try {
      const mapped = mapLocationParams({ locationId: params.locationId ?? null })
      return await reportingService.getSalesAnalytics({
        startDate: params.startDate,
        endDate: params.endDate,
        period: params.period,
        branchId: mapped.branchId ?? null
      })
    } catch (e: any) {
      setError(e?.message || 'Failed to retrieve sales analytics')
      throw e
    } finally {
      isLoading.value = false
    }
  }

  const getInventoryAnalytics = async (params?: { locationId?: string | null }) => {
    isLoading.value = true
    clearError()

    try {
      const mapped = mapLocationParams(params)
      return await reportingService.getInventoryAnalytics({
        branchId: mapped.branchId ?? null
      })
    } catch (e: any) {
      setError(e?.message || 'Failed to retrieve inventory analytics')
      throw e
    } finally {
      isLoading.value = false
    }
  }

  const getCustomerAnalytics = async (params: { startDate: string | Date; endDate: string | Date }) => {
    isLoading.value = true
    clearError()

    try {
      return await reportingService.getCustomerAnalytics(params)
    } catch (e: any) {
      setError(e?.message || 'Failed to retrieve customer analytics')
      throw e
    } finally {
      isLoading.value = false
    }
  }

  const getFinancialAnalytics = async (params: {
    startDate: string | Date
    endDate: string | Date
    period?: string
  }) => {
    isLoading.value = true
    clearError()

    try {
      return await reportingService.getFinancialAnalytics(params)
    } catch (e: any) {
      setError(e?.message || 'Failed to retrieve financial analytics')
      throw e
    } finally {
      isLoading.value = false
    }
  }

  const getPredictiveAnalytics = async (params: {
    startDate: string | Date
    endDate: string | Date
    predictionType: string
  }) => {
    isLoading.value = true
    clearError()

    try {
      return await reportingService.getPredictiveAnalytics(params)
    } catch (e: any) {
      setError(e?.message || 'Failed to retrieve predictive analytics')
      throw e
    } finally {
      isLoading.value = false
    }
  }

  const exportAnalytics = async (params: {
    type: string
    format: string
    startDate: string
    endDate: string
    period: string
  }) => {
    const request: ExportRequest = {
      reportType: `analytics:${params.type}`,
      startDate: params.startDate,
      endDate: params.endDate,
      format: params.format,
      parameters: {
        period: params.period
      }
    }

    return await reportingService.exportReport(request)
  }

  const getRecentReports = async () => {
    // Placeholder until a backend endpoint exists.
    return recentReports.value
  }

  const downloadReport = async (fileId: string) => {
    const blob = await reportingService.downloadExport(fileId)
    const url = window.URL.createObjectURL(blob)

    const a = document.createElement('a')
    a.href = url
    a.download = `report_${fileId}`
    a.click()

    window.URL.revokeObjectURL(url)
  }

  const deleteReport = async (id: string) => {
    // Placeholder until a backend endpoint exists.
    recentReports.value = recentReports.value.filter(r => r.id !== id)
  }

  const runCustomReport = async (payload: any) => {
    return await reportingService.runCustomReport(payload)
  }

  const exportCustomReport = async (payload: any) => {
    return await reportingService.exportCustomReport(payload)
  }

  return {
    isLoading,
    error,
    getDashboardSummary,
    getRealTimeMetrics,
    getSalesReport,
    getSalesAnalytics,
    getInventoryAnalytics,
    getCustomerAnalytics,
    getFinancialAnalytics,
    getPredictiveAnalytics,
    exportAnalytics,
    getRecentReports,
    downloadReport,
    deleteReport,
    runCustomReport,
    exportCustomReport
  }
})

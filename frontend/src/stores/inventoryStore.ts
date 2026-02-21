import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { inventoryService } from '@/services/inventoryService'
import type {
  Inventory,
  InventoryTransaction,
  InventoryStatistics,
  LowStockAlert,
  CreateInventoryRequest,
  UpdateInventoryRequest,
  TransferInventoryRequest,
  AdjustInventoryRequest
} from '@/services/inventoryService'

export const useInventoryStore = defineStore('inventory', () => {
  // State
  const inventoryItems = ref<Inventory[]>([])
  const transactions = ref<InventoryTransaction[]>([])
  const statistics = ref<InventoryStatistics | null>(null)
  const lowStockAlerts = ref<LowStockAlert[]>([])
  const currentInventory = ref<Inventory | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const totalItems = computed(() => inventoryItems.value.length)
  const lowStockCount = computed(() => statistics.value?.lowStockCount || 0)
  const outOfStockCount = computed(() => statistics.value?.outOfStockCount || 0)
  const totalValue = computed(() => statistics.value?.totalValue || 0)

  const inventoryByBranch = computed(() => {
    const grouped: Record<string, Inventory[]> = {}
    inventoryItems.value.forEach(item => {
      if (!grouped[item.branchId]) {
        grouped[item.branchId] = []
      }
      grouped[item.branchId]!.push(item)
    })
    return grouped
  })

  const recentTransactions = computed(() => {
    return [...transactions.value]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 10)
  })

  // Actions
  async function fetchInventoryByBranch(branchId: string) {
    loading.value = true
    error.value = null
    try {
      inventoryItems.value = await inventoryService.getInventoryByBranch(branchId)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch inventory'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchInventoryByProduct(productId: string) {
    loading.value = true
    error.value = null
    try {
      inventoryItems.value = await inventoryService.getInventoryByProduct(productId)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch inventory'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchInventory(id: string) {
    loading.value = true
    error.value = null
    try {
      currentInventory.value = await inventoryService.getInventory(id)
      return currentInventory.value
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch inventory'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createInventory(data: CreateInventoryRequest) {
    loading.value = true
    error.value = null
    try {
      const newInventory = await inventoryService.createInventory(data)
      inventoryItems.value.push(newInventory)
      return newInventory
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create inventory'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateInventory(id: string, data: UpdateInventoryRequest) {
    loading.value = true
    error.value = null
    try {
      const updated = await inventoryService.updateInventory(id, data)
      const index = inventoryItems.value.findIndex(item => item.id === id)
      if (index !== -1) {
        inventoryItems.value[index] = updated
      }
      if (currentInventory.value?.id === id) {
        currentInventory.value = updated
      }
      return updated
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update inventory'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteInventory(id: string) {
    loading.value = true
    error.value = null
    try {
      await inventoryService.deleteInventory(id)
      inventoryItems.value = inventoryItems.value.filter(item => item.id !== id)
      if (currentInventory.value?.id === id) {
        currentInventory.value = null
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to delete inventory'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function transferInventory(data: TransferInventoryRequest) {
    loading.value = true
    error.value = null
    try {
      const result = await inventoryService.transferInventory(data)
      // Refresh inventory list after transfer
      await fetchStatistics()
      return result
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to transfer inventory'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function adjustInventory(id: string, data: AdjustInventoryRequest) {
    loading.value = true
    error.value = null
    try {
      const adjusted = await inventoryService.adjustInventory(id, data)
      const index = inventoryItems.value.findIndex(item => item.id === id)
      if (index !== -1) {
        inventoryItems.value[index] = adjusted
      }
      if (currentInventory.value?.id === id) {
        currentInventory.value = adjusted
      }
      return adjusted
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to adjust inventory'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchTransactions(params?: {
    inventoryId?: string
    productId?: string
    branchId?: string
    startDate?: string
    endDate?: string
  }) {
    loading.value = true
    error.value = null
    try {
      transactions.value = await inventoryService.getTransactions(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch transactions'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchStatistics(params?: { branchId?: string; warehouseId?: string }) {
    loading.value = true
    error.value = null
    try {
      statistics.value = await inventoryService.getStatistics(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch statistics'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchLowStockAlerts(params?: { branchId?: string; warehouseId?: string }) {
    loading.value = true
    error.value = null
    try {
      lowStockAlerts.value = await inventoryService.getLowStockAlerts(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch low stock alerts'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchLowStock(threshold: number = 10) {
    loading.value = true
    error.value = null
    try {
      inventoryItems.value = await inventoryService.getLowStock(threshold)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch low stock items'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchOutOfStock() {
    loading.value = true
    error.value = null
    try {
      inventoryItems.value = await inventoryService.getOutOfStock()
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch out of stock items'
      throw err
    } finally {
      loading.value = false
    }
  }

  function clearError() {
    error.value = null
  }

  function resetState() {
    inventoryItems.value = []
    transactions.value = []
    statistics.value = null
    lowStockAlerts.value = []
    currentInventory.value = null
    loading.value = false
    error.value = null
  }

  return {
    // State
    inventoryItems,
    transactions,
    statistics,
    lowStockAlerts,
    currentInventory,
    loading,
    error,

    // Getters
    totalItems,
    lowStockCount,
    outOfStockCount,
    totalValue,
    inventoryByBranch,
    recentTransactions,

    // Actions
    fetchInventoryByBranch,
    fetchInventoryByProduct,
    fetchInventory,
    createInventory,
    updateInventory,
    deleteInventory,
    transferInventory,
    adjustInventory,
    fetchTransactions,
    fetchStatistics,
    fetchLowStockAlerts,
    fetchLowStock,
    fetchOutOfStock,
    clearError,
    resetState
  }
})

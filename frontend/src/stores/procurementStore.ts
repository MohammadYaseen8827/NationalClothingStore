import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { procurementService, type Supplier, type PurchaseOrder, type CreateSupplierRequest, type UpdateSupplierRequest, type CreatePurchaseOrderRequest, type UpdatePurchaseOrderRequest } from '@/services/procurementService'

export const useProcurementStore = defineStore('procurement', () => {
  // State
  const suppliers = ref<Supplier[]>([])
  const currentSupplier = ref<Supplier | null>(null)
  const purchaseOrders = ref<PurchaseOrder[]>([])
  const currentOrder = ref<PurchaseOrder | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const activeSuppliers = computed(() => suppliers.value.filter(s => s.isActive))
  const pendingOrders = computed(() => purchaseOrders.value.filter(o => o.status === 'Pending'))
  const orderedOrders = computed(() => purchaseOrders.value.filter(o => o.status === 'Ordered'))

  // Supplier Actions
  async function fetchSuppliers() {
    isLoading.value = true
    error.value = null
    try {
      suppliers.value = await procurementService.getSuppliers()
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch suppliers'
      console.error('Error fetching suppliers:', e)
    } finally {
      isLoading.value = false
    }
  }

  async function fetchSupplier(id: string) {
    isLoading.value = true
    error.value = null
    try {
      currentSupplier.value = await procurementService.getSupplier(id)
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch supplier'
      console.error('Error fetching supplier:', e)
    } finally {
      isLoading.value = false
    }
  }

  async function createSupplier(data: CreateSupplierRequest) {
    isLoading.value = true
    error.value = null
    try {
      const newSupplier = await procurementService.createSupplier(data)
      suppliers.value.push(newSupplier)
      return newSupplier
    } catch (e: any) {
      error.value = e.message || 'Failed to create supplier'
      console.error('Error creating supplier:', e)
      throw e
    } finally {
      isLoading.value = false
    }
  }

  async function updateSupplier(id: string, data: UpdateSupplierRequest) {
    isLoading.value = true
    error.value = null
    try {
      const updated = await procurementService.updateSupplier(id, data)
      const index = suppliers.value.findIndex(s => s.id === id)
      if (index !== -1) {
        suppliers.value[index] = updated
      }
      if (currentSupplier.value?.id === id) {
        currentSupplier.value = updated
      }
      return updated
    } catch (e: any) {
      error.value = e.message || 'Failed to update supplier'
      console.error('Error updating supplier:', e)
      throw e
    } finally {
      isLoading.value = false
    }
  }

  async function deleteSupplier(id: string) {
    isLoading.value = true
    error.value = null
    try {
      await procurementService.deleteSupplier(id)
      suppliers.value = suppliers.value.filter(s => s.id !== id)
      if (currentSupplier.value?.id === id) {
        currentSupplier.value = null
      }
    } catch (e: any) {
      error.value = e.message || 'Failed to delete supplier'
      console.error('Error deleting supplier:', e)
      throw e
    } finally {
      isLoading.value = false
    }
  }

  // Purchase Order Actions
  async function fetchPurchaseOrders() {
    isLoading.value = true
    error.value = null
    try {
      purchaseOrders.value = await procurementService.getPurchaseOrders()
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch purchase orders'
      console.error('Error fetching purchase orders:', e)
    } finally {
      isLoading.value = false
    }
  }

  async function fetchPurchaseOrder(id: string) {
    isLoading.value = true
    error.value = null
    try {
      currentOrder.value = await procurementService.getPurchaseOrder(id)
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch purchase order'
      console.error('Error fetching purchase order:', e)
    } finally {
      isLoading.value = false
    }
  }

  async function fetchPurchaseOrdersBySupplier(supplierId: string) {
    isLoading.value = true
    error.value = null
    try {
      purchaseOrders.value = await procurementService.getPurchaseOrdersBySupplier(supplierId)
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch purchase orders by supplier'
      console.error('Error fetching purchase orders by supplier:', e)
    } finally {
      isLoading.value = false
    }
  }

  async function fetchPurchaseOrdersByStatus(status: string) {
    isLoading.value = true
    error.value = null
    try {
      purchaseOrders.value = await procurementService.getPurchaseOrdersByStatus(status)
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch purchase orders by status'
      console.error('Error fetching purchase orders by status:', e)
    } finally {
      isLoading.value = false
    }
  }

  async function createPurchaseOrder(data: CreatePurchaseOrderRequest) {
    isLoading.value = true
    error.value = null
    try {
      const newOrder = await procurementService.createPurchaseOrder(data)
      purchaseOrders.value.push(newOrder)
      return newOrder
    } catch (e: any) {
      error.value = e.message || 'Failed to create purchase order'
      console.error('Error creating purchase order:', e)
      throw e
    } finally {
      isLoading.value = false
    }
  }

  async function updatePurchaseOrder(id: string, data: UpdatePurchaseOrderRequest) {
    isLoading.value = true
    error.value = null
    try {
      const updated = await procurementService.updatePurchaseOrder(id, data)
      const index = purchaseOrders.value.findIndex(o => o.id === id)
      if (index !== -1) {
        purchaseOrders.value[index] = updated
      }
      if (currentOrder.value?.id === id) {
        currentOrder.value = updated
      }
      return updated
    } catch (e: any) {
      error.value = e.message || 'Failed to update purchase order'
      console.error('Error updating purchase order:', e)
      throw e
    } finally {
      isLoading.value = false
    }
  }

  async function deletePurchaseOrder(id: string) {
    isLoading.value = true
    error.value = null
    try {
      await procurementService.deletePurchaseOrder(id)
      purchaseOrders.value = purchaseOrders.value.filter(o => o.id !== id)
      if (currentOrder.value?.id === id) {
        currentOrder.value = null
      }
    } catch (e: any) {
      error.value = e.message || 'Failed to delete purchase order'
      console.error('Error deleting purchase order:', e)
      throw e
    } finally {
      isLoading.value = false
    }
  }

  async function receivePurchaseOrder(id: string, _items?: any[]) {
    isLoading.value = true
    error.value = null
    try {
      const updated = await procurementService.updatePurchaseOrder(id, {
        status: 'Received'
      })
      const index = purchaseOrders.value.findIndex(o => o.id === id)
      if (index !== -1) {
        purchaseOrders.value[index] = updated
      }
      if (currentOrder.value?.id === id) {
        currentOrder.value = updated
      }
      return updated
    } catch (e: any) {
      error.value = e.message || 'Failed to receive purchase order'
      console.error('Error receiving purchase order:', e)
      throw e
    } finally {
      isLoading.value = false
    }
  }

  function clearError() {
    error.value = null
  }

  function clearCurrentSupplier() {
    currentSupplier.value = null
  }

  function clearCurrentOrder() {
    currentOrder.value = null
  }

  return {
    // State
    suppliers,
    currentSupplier,
    purchaseOrders,
    currentOrder,
    isLoading,
    error,
    // Getters
    activeSuppliers,
    pendingOrders,
    orderedOrders,
    // Actions
    fetchSuppliers,
    fetchSupplier,
    createSupplier,
    updateSupplier,
    deleteSupplier,
    fetchPurchaseOrders,
    fetchPurchaseOrder,
    fetchPurchaseOrdersBySupplier,
    fetchPurchaseOrdersByStatus,
    createPurchaseOrder,
    updatePurchaseOrder,
    deletePurchaseOrder,
    receivePurchaseOrder,
    clearError,
    clearCurrentSupplier,
    clearCurrentOrder
  }
})

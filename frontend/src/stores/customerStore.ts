import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { customerService } from '@/services/customerService'
import type {
  Customer,
  CreateCustomerRequest,
  UpdateCustomerRequest,
  CustomerLoyalty,
  AddLoyaltyPointsRequest,
  RedeemLoyaltyPointsRequest
} from '@/services/customerService'

export const useCustomerStore = defineStore('customer', () => {
  // State
  const customers = ref<Customer[]>([])
  const currentCustomer = ref<Customer | null>(null)
  const customerLoyalty = ref<CustomerLoyalty | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const totalCustomers = computed(() => customers.value.length)
  
  const activeCustomers = computed(() => 
    customers.value.filter(c => c.isActive)
  )

  const customersByTier = computed(() => {
    const tiers: Record<string, Customer[]> = {}
    customers.value.forEach(customer => {
      const tier = customer.loyaltyTier || 'Standard'
      if (!tiers[tier]) {
        tiers[tier] = []
      }
      tiers[tier].push(customer)
    })
    return tiers
  })

  // Actions
  async function fetchCustomers(params?: { page?: number; pageSize?: number }) {
    loading.value = true
    error.value = null
    try {
      customers.value = await customerService.getCustomers(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch customers'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchCustomer(id: string) {
    loading.value = true
    error.value = null
    try {
      currentCustomer.value = await customerService.getCustomer(id)
      return currentCustomer.value
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch customer'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function searchCustomer(query: string) {
    loading.value = true
    error.value = null
    try {
      const customer = await customerService.searchCustomer(query)
      currentCustomer.value = customer
      return customer
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to search customer'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createCustomer(data: CreateCustomerRequest) {
    loading.value = true
    error.value = null
    try {
      const newCustomer = await customerService.createCustomer(data)
      customers.value.push(newCustomer)
      return newCustomer
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create customer'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateCustomer(id: string, data: UpdateCustomerRequest) {
    loading.value = true
    error.value = null
    try {
      const updated = await customerService.updateCustomer(id, data)
      const index = customers.value.findIndex(c => c.id === id)
      if (index !== -1) {
        customers.value[index] = updated
      }
      if (currentCustomer.value?.id === id) {
        currentCustomer.value = updated
      }
      return updated
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update customer'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteCustomer(id: string) {
    loading.value = true
    error.value = null
    try {
      await customerService.deleteCustomer(id)
      customers.value = customers.value.filter(c => c.id !== id)
      if (currentCustomer.value?.id === id) {
        currentCustomer.value = null
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to delete customer'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchCustomerLoyalty(id: string) {
    loading.value = true
    error.value = null
    try {
      customerLoyalty.value = await customerService.getCustomerLoyalty(id)
      return customerLoyalty.value
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch loyalty info'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function addLoyaltyPoints(id: string, data: AddLoyaltyPointsRequest) {
    loading.value = true
    error.value = null
    try {
      customerLoyalty.value = await customerService.addLoyaltyPoints(id, data)
      // Update customer's loyalty points in list
      const customer = customers.value.find(c => c.id === id)
      if (customer) {
        customer.loyaltyPoints = customerLoyalty.value.points
      }
      if (currentCustomer.value?.id === id) {
        currentCustomer.value.loyaltyPoints = customerLoyalty.value.points
      }
      return customerLoyalty.value
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to add points'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function redeemLoyaltyPoints(id: string, data: RedeemLoyaltyPointsRequest) {
    loading.value = true
    error.value = null
    try {
      customerLoyalty.value = await customerService.redeemLoyaltyPoints(id, data)
      // Update customer's loyalty points in list
      const customer = customers.value.find(c => c.id === id)
      if (customer) {
        customer.loyaltyPoints = customerLoyalty.value.points
      }
      if (currentCustomer.value?.id === id) {
        currentCustomer.value.loyaltyPoints = customerLoyalty.value.points
      }
      return customerLoyalty.value
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to redeem points'
      throw err
    } finally {
      loading.value = false
    }
  }

  function clearError() {
    error.value = null
  }

  function resetState() {
    customers.value = []
    currentCustomer.value = null
    customerLoyalty.value = null
    loading.value = false
    error.value = null
  }

  return {
    // State
    customers,
    currentCustomer,
    customerLoyalty,
    loading,
    error,

    // Getters
    totalCustomers,
    activeCustomers,
    customersByTier,

    // Actions
    fetchCustomers,
    fetchCustomer,
    searchCustomer,
    createCustomer,
    updateCustomer,
    deleteCustomer,
    fetchCustomerLoyalty,
    addLoyaltyPoints,
    redeemLoyaltyPoints,
    clearError,
    resetState
  }
})

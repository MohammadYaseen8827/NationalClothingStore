<template>
  <div class="purchase-order-form">
    <div class="header">
      <h1>{{ isEditing ? 'Edit Purchase Order' : 'New Purchase Order' }}</h1>
      <router-link to="/procurement/orders" class="btn btn-secondary">Back to List</router-link>
    </div>

    <div v-if="store.isLoading" class="loading">Loading...</div>
    <div v-else-if="store.error" class="error">{{ store.error }}</div>

    <form v-else @submit.prevent="handleSubmit" class="form">
      <div class="form-section">
        <h3>Order Details</h3>
        
        <div class="form-row">
          <div class="form-group">
            <label for="supplier">Supplier *</label>
            <select id="supplier" v-model="formData.supplierId" required :disabled="isEditing">
              <option value="">Select a supplier</option>
              <option v-for="s in store.suppliers" :key="s.id" :value="s.id">
                {{ s.name }} ({{ s.code }})
              </option>
            </select>
          </div>
          <div class="form-group">
            <label for="expectedDelivery">Expected Delivery</label>
            <input id="expectedDelivery" v-model="formData.expectedDeliveryDate" type="date" />
          </div>
        </div>

        <div v-if="isEditing" class="form-group">
          <label for="status">Status</label>
          <select id="status" v-model="formData.status">
            <option value="Draft">Draft</option>
            <option value="Pending">Pending</option>
            <option value="Approved">Approved</option>
            <option value="Ordered">Ordered</option>
            <option value="Received">Received</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>

        <div class="form-group full-width">
          <label for="notes">Notes</label>
          <textarea id="notes" v-model="formData.notes" rows="2"></textarea>
        </div>
      </div>

      <div v-if="!isEditing" class="form-section">
        <h3>Order Items</h3>
        
        <div v-for="(item, index) in formData.items" :key="index" class="item-row">
          <div class="form-group">
            <label>Product</label>
            <input v-model="item.productId" type="text" placeholder="Product ID" required />
          </div>
          <div class="form-group">
            <label>Quantity</label>
            <input v-model.number="item.quantity" type="number" min="1" required />
          </div>
          <div class="form-group">
            <label>Unit Cost</label>
            <input v-model.number="item.unitCost" type="number" min="0" step="0.01" required />
          </div>
          <button type="button" @click="removeItem(index)" class="btn btn-danger btn-sm">
            Remove
          </button>
        </div>

        <button type="button" @click="addItem" class="btn btn-secondary">
          + Add Item
        </button>
      </div>

      <div class="form-actions">
        <button type="submit" class="btn btn-primary" :disabled="isSaving">
          {{ isSaving ? 'Saving...' : (isEditing ? 'Update Order' : 'Create Order') }}
        </button>
        <router-link to="/procurement/orders" class="btn btn-secondary">Cancel</router-link>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProcurementStore } from '@/stores/procurementStore'

const props = defineProps<{ id?: string }>()

const route = useRoute()
const router = useRouter()
const store = useProcurementStore()

const isSaving = ref(false)
const isEditing = computed(() => !!props.id || !!route.params.id)
const orderId = computed(() => props.id || route.params.id as string)

const formData = reactive({
  supplierId: '',
  expectedDeliveryDate: '',
  notes: '',
  status: 'Draft',
  items: [{ productId: '', quantity: 1, unitCost: 0 }] as Array<{
    productId: string
    quantity: number
    unitCost: number
  }>
})

watch(() => store.currentOrder, (order) => {
  if (order && isEditing.value) {
    formData.supplierId = order.supplierId
    formData.expectedDeliveryDate = order.expectedDeliveryDate?.split('T')[0] || ''
    formData.notes = order.notes || ''
    formData.status = order.status
  }
}, { immediate: true })

function addItem() {
  formData.items.push({ productId: '', quantity: 1, unitCost: 0 })
}

function removeItem(index: number) {
  if (formData.items.length > 1) {
    formData.items.splice(index, 1)
  }
}

async function handleSubmit() {
  isSaving.value = true
  try {
    if (isEditing.value) {
      await store.updatePurchaseOrder(orderId.value, {
        status: formData.status,
        expectedDeliveryDate: formData.expectedDeliveryDate || undefined,
        notes: formData.notes || undefined
      })
    } else {
      await store.createPurchaseOrder({
        supplierId: formData.supplierId,
        expectedDeliveryDate: formData.expectedDeliveryDate || undefined,
        notes: formData.notes || undefined,
        items: formData.items.filter(i => i.productId)
      })
    }
    router.push('/procurement/orders')
  } catch (e) {
    console.error('Failed to save order:', e)
  } finally {
    isSaving.value = false
  }
}

onMounted(() => {
  store.fetchSuppliers()
  if (isEditing.value && orderId.value) {
    store.fetchPurchaseOrder(orderId.value)
  }
})
</script>

<style scoped>
.purchase-order-form {
  padding: 20px;
  max-width: 900px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.form {
  background: white;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.form-section {
  padding: 24px;
  border-bottom: 1px solid #eee;
}

.form-section:last-of-type {
  border-bottom: none;
}

.form-section h3 {
  margin: 0 0 16px;
  color: #333;
}

.form-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  margin-bottom: 16px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group.full-width {
  grid-column: 1 / -1;
}

.form-group label {
  font-weight: 500;
  margin-bottom: 6px;
  color: #333;
  font-size: 14px;
}

.form-group input,
.form-group select,
.form-group textarea {
  padding: 10px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.item-row {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr auto;
  gap: 12px;
  align-items: end;
  margin-bottom: 12px;
  padding: 12px;
  background: #f8f9fa;
  border-radius: 4px;
}

.form-actions {
  display: flex;
  gap: 12px;
  padding: 24px;
  border-top: 1px solid #eee;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  text-decoration: none;
}

.btn-primary { background-color: #007bff; color: white; }
.btn-secondary { background-color: #6c757d; color: white; }
.btn-danger { background-color: #dc3545; color: white; }
.btn-sm { padding: 6px 12px; font-size: 12px; }
.btn:disabled { opacity: 0.6; cursor: not-allowed; }

.loading, .error { padding: 40px; text-align: center; }
.error { color: #dc3545; }
</style>

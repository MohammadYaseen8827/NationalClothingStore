<template>
  <div class="customer-form">
    <div class="page-header">
      <router-link to="/customers" class="back-link">← Back to Customers</router-link>
      <h1>{{ isEditing ? 'Edit Customer' : 'New Customer' }}</h1>
    </div>

    <form @submit.prevent="submitForm" class="form-container">
      <div class="form-section">
        <h2>Basic Information</h2>
        <div class="form-row">
          <div class="form-group">
            <label for="firstName">First Name *</label>
            <input
              id="firstName"
              type="text"
              v-model="formData.firstName"
              required
              placeholder="Enter first name"
            />
          </div>
          <div class="form-group">
            <label for="lastName">Last Name *</label>
            <input
              id="lastName"
              type="text"
              v-model="formData.lastName"
              required
              placeholder="Enter last name"
            />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="email">Email *</label>
            <input
              id="email"
              type="email"
              v-model="formData.email"
              required
              placeholder="customer@example.com"
            />
          </div>
          <div class="form-group">
            <label for="phone">Phone</label>
            <input
              id="phone"
              type="tel"
              v-model="formData.phone"
              placeholder="+1 (555) 123-4567"
            />
          </div>
        </div>

        <div class="form-group">
          <label for="dateOfBirth">Date of Birth</label>
          <input
            id="dateOfBirth"
            type="date"
            v-model="formData.dateOfBirth"
          />
        </div>
      </div>

      <div class="form-section">
        <h2>Address</h2>
        <div class="form-group">
          <label for="address">Street Address</label>
          <input
            id="address"
            type="text"
            v-model="formData.address"
            placeholder="123 Main Street"
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="city">City</label>
            <input
              id="city"
              type="text"
              v-model="formData.city"
              placeholder="City"
            />
          </div>
          <div class="form-group">
            <label for="state">State/Province</label>
            <input
              id="state"
              type="text"
              v-model="formData.state"
              placeholder="State"
            />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="zipCode">ZIP/Postal Code</label>
            <input
              id="zipCode"
              type="text"
              v-model="formData.zipCode"
              placeholder="12345"
            />
          </div>
          <div class="form-group">
            <label for="country">Country</label>
            <select id="country" v-model="formData.country">
              <option value="">Select country</option>
              <option value="US">United States</option>
              <option value="CA">Canada</option>
              <option value="UK">United Kingdom</option>
              <option value="AU">Australia</option>
              <option value="OTHER">Other</option>
            </select>
          </div>
        </div>
      </div>

      <div v-if="isEditing" class="form-section">
        <h2>Account Status</h2>
        <div class="form-group checkbox-group">
          <label class="checkbox-label">
            <input type="checkbox" v-model="formData.isActive" />
            <span>Account is active</span>
          </label>
        </div>
      </div>

      <div v-if="error" class="error-message">
        {{ error }}
      </div>

      <div class="form-actions">
        <router-link to="/customers" class="btn-secondary">Cancel</router-link>
        <button type="submit" class="btn-primary" :disabled="submitting">
          {{ submitting ? 'Saving...' : (isEditing ? 'Update Customer' : 'Create Customer') }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useCustomerStore } from '@/stores/customerStore'

const route = useRoute()
const router = useRouter()
const customerStore = useCustomerStore()

const submitting = ref(false)
const error = ref<string | null>(null)

const isEditing = computed(() => !!route.params.id)
const customerId = computed(() => route.params.id as string)

const formData = ref({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  dateOfBirth: '',
  address: '',
  city: '',
  state: '',
  zipCode: '',
  country: '',
  isActive: true
})

onMounted(async () => {
  if (isEditing.value) {
    try {
      const customer = await customerStore.fetchCustomer(customerId.value)
      formData.value = {
        firstName: customer.firstName,
        lastName: customer.lastName,
        email: customer.email,
        phone: customer.phone || '',
        dateOfBirth: customer.dateOfBirth?.split('T')[0] || '',
        address: customer.address || '',
        city: customer.city || '',
        state: customer.state || '',
        zipCode: customer.zipCode || '',
        country: customer.country || '',
        isActive: customer.isActive
      }
    } catch (err) {
      error.value = 'Failed to load customer data'
    }
  }
})

async function submitForm() {
  submitting.value = true
  error.value = null

  try {
    if (isEditing.value) {
      await customerStore.updateCustomer(customerId.value, {
        firstName: formData.value.firstName,
        lastName: formData.value.lastName,
        email: formData.value.email,
        phone: formData.value.phone || undefined,
        dateOfBirth: formData.value.dateOfBirth || undefined,
        address: formData.value.address || undefined,
        city: formData.value.city || undefined,
        state: formData.value.state || undefined,
        zipCode: formData.value.zipCode || undefined,
        country: formData.value.country || undefined,
        isActive: formData.value.isActive
      })
      router.push(`/customers/${customerId.value}`)
    } else {
      const newCustomer = await customerStore.createCustomer({
        firstName: formData.value.firstName,
        lastName: formData.value.lastName,
        email: formData.value.email,
        phone: formData.value.phone || undefined,
        dateOfBirth: formData.value.dateOfBirth || undefined,
        address: formData.value.address || undefined,
        city: formData.value.city || undefined,
        state: formData.value.state || undefined,
        zipCode: formData.value.zipCode || undefined,
        country: formData.value.country || undefined
      })
      router.push(`/customers/${newCustomer.id}`)
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to save customer'
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.customer-form {
  padding: 20px;
  max-width: 800px;
  margin: 0 auto;
}

.back-link {
  color: #4a90a4;
  text-decoration: none;
  font-size: 14px;
}

.back-link:hover {
  text-decoration: underline;
}

.page-header h1 {
  margin: 10px 0 30px;
  font-size: 28px;
  color: #1a1a2e;
}

.form-container {
  background: white;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.form-section {
  padding: 25px;
  border-bottom: 1px solid #e0e0e0;
}

.form-section:last-of-type {
  border-bottom: none;
}

.form-section h2 {
  margin: 0 0 20px;
  font-size: 18px;
  color: #1a1a2e;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
  color: #333;
  font-size: 14px;
}

.form-group input,
.form-group select {
  width: 100%;
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 14px;
  transition: border-color 0.2s;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #4a90a4;
}

.checkbox-group {
  margin-bottom: 0;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
}

.checkbox-label input[type="checkbox"] {
  width: auto;
  cursor: pointer;
}

.error-message {
  margin: 0 25px;
  padding: 15px;
  background: #f8d7da;
  color: #721c24;
  border-radius: 6px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 15px;
  padding: 25px;
  border-top: 1px solid #e0e0e0;
}

.btn-primary,
.btn-secondary {
  padding: 12px 24px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  text-decoration: none;
}

.btn-primary {
  background: #4a90a4;
  color: white;
  border: none;
}

.btn-primary:hover:not(:disabled) {
  background: #3d7a8c;
}

.btn-primary:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.btn-secondary {
  background: white;
  color: #666;
  border: 1px solid #ddd;
}

.btn-secondary:hover {
  background: #f8f9fa;
}

@media (max-width: 600px) {
  .form-row {
    grid-template-columns: 1fr;
  }
}
</style>

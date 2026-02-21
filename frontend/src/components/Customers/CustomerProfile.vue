<template>
  <div class="customer-profile">
    <div v-if="loading" class="loading">Loading customer...</div>
    <div v-else-if="!customer" class="error">Customer not found</div>
    <template v-else>
      <div class="profile-header">
        <div class="header-left">
          <router-link to="/customers" class="back-link">← Back to Customers</router-link>
          <h1>{{ customer.firstName }} {{ customer.lastName }}</h1>
          <div class="customer-meta">
            <span class="tier-badge" :class="(customer.loyaltyTier || 'Standard').toLowerCase()">
              {{ customer.loyaltyTier || 'Standard' }} Member
            </span>
            <span class="status-badge" :class="customer.isActive ? 'active' : 'inactive'">
              {{ customer.isActive ? 'Active' : 'Inactive' }}
            </span>
          </div>
        </div>
        <div class="header-actions">
          <router-link :to="`/customers/${customer.id}/edit`" class="btn-secondary">
            Edit Profile
          </router-link>
          <router-link :to="`/customers/${customer.id}/loyalty`" class="btn-primary">
            Manage Loyalty
          </router-link>
        </div>
      </div>

      <div class="profile-content">
        <div class="main-section">
          <!-- Contact Information -->
          <div class="info-card">
            <h2>Contact Information</h2>
            <div class="info-grid">
              <div class="info-item">
                <span class="label">Email</span>
                <span class="value">{{ customer.email }}</span>
              </div>
              <div class="info-item">
                <span class="label">Phone</span>
                <span class="value">{{ customer.phone || 'Not provided' }}</span>
              </div>
              <div class="info-item">
                <span class="label">Date of Birth</span>
                <span class="value">{{ formatDate(customer.dateOfBirth) || 'Not provided' }}</span>
              </div>
            </div>
          </div>

          <!-- Address -->
          <div class="info-card">
            <h2>Address</h2>
            <div v-if="hasAddress" class="address-block">
              <p>{{ customer.address }}</p>
              <p>{{ customer.city }}, {{ customer.state }} {{ customer.zipCode }}</p>
              <p>{{ customer.country }}</p>
            </div>
            <p v-else class="no-data">No address on file</p>
          </div>

          <!-- Account Info -->
          <div class="info-card">
            <h2>Account Information</h2>
            <div class="info-grid">
              <div class="info-item">
                <span class="label">Customer ID</span>
                <span class="value monospace">{{ customer.id }}</span>
              </div>
              <div class="info-item">
                <span class="label">Member Since</span>
                <span class="value">{{ formatDate(customer.createdAt) }}</span>
              </div>
              <div class="info-item">
                <span class="label">Last Updated</span>
                <span class="value">{{ formatDate(customer.updatedAt) }}</span>
              </div>
            </div>
          </div>
        </div>

        <div class="sidebar">
          <!-- Loyalty Summary -->
          <div class="loyalty-card">
            <h2>Loyalty Summary</h2>
            <div class="points-display">
              <span class="points-value">{{ customer.loyaltyPoints }}</span>
              <span class="points-label">Current Points</span>
            </div>
            <div v-if="loyalty" class="loyalty-stats">
              <div class="loyalty-stat">
                <span class="stat-value">{{ loyalty.totalPointsEarned }}</span>
                <span class="stat-label">Total Earned</span>
              </div>
              <div class="loyalty-stat">
                <span class="stat-value">{{ loyalty.totalPointsRedeemed }}</span>
                <span class="stat-label">Redeemed</span>
              </div>
              <div class="loyalty-stat">
                <span class="stat-value">{{ loyalty.transactionsCount }}</span>
                <span class="stat-label">Transactions</span>
              </div>
              <div class="loyalty-stat">
                <span class="stat-value">{{ formatCurrency(loyalty.totalSpent) }}</span>
                <span class="stat-label">Total Spent</span>
              </div>
            </div>
            <router-link :to="`/customers/${customer.id}/loyalty`" class="view-loyalty-link">
              View Full Loyalty Details →
            </router-link>
          </div>

          <!-- Quick Actions -->
          <div class="actions-card">
            <h2>Quick Actions</h2>
            <div class="action-buttons">
              <button class="action-btn" @click="addPoints">Add Points</button>
              <button class="action-btn" @click="createOrder">New Order</button>
              <button class="action-btn secondary" @click="sendEmail">Send Email</button>
              <button 
                class="action-btn" 
                :class="customer.isActive ? 'danger' : 'success'"
                @click="toggleStatus"
              >
                {{ customer.isActive ? 'Deactivate' : 'Activate' }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useCustomerStore } from '@/stores/customerStore'

const route = useRoute()
const router = useRouter()
const customerStore = useCustomerStore()

const loading = ref(true)

const customer = computed(() => customerStore.currentCustomer)
const loyalty = computed(() => customerStore.customerLoyalty)

const hasAddress = computed(() => 
  customer.value?.address || customer.value?.city || customer.value?.state
)

onMounted(async () => {
  const id = route.params.id as string
  try {
    await Promise.all([
      customerStore.fetchCustomer(id),
      customerStore.fetchCustomerLoyalty(id)
    ])
  } catch (error) {
    console.error('Failed to load customer:', error)
  } finally {
    loading.value = false
  }
})

function formatDate(dateString?: string): string {
  if (!dateString) return ''
  return new Date(dateString).toLocaleDateString()
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(value)
}

function addPoints() {
  if (customer.value) {
    router.push(`/customers/${customer.value.id}/loyalty`)
  }
}

function createOrder() {
  // Navigate to POS with customer pre-selected
  router.push(`/sales/pos?customerId=${customer.value?.id}`)
}

function sendEmail() {
  if (customer.value?.email) {
    window.location.href = `mailto:${customer.value.email}`
  }
}

async function toggleStatus() {
  if (!customer.value) return
  
  const action = customer.value.isActive ? 'deactivate' : 'activate'
  if (confirm(`Are you sure you want to ${action} this customer?`)) {
    try {
      await customerStore.updateCustomer(customer.value.id, {
        ...customer.value,
        isActive: !customer.value.isActive
      })
    } catch (error) {
      console.error(`Failed to ${action} customer:`, error)
    }
  }
}
</script>

<style scoped>
.customer-profile {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.loading,
.error {
  text-align: center;
  padding: 60px;
  color: #666;
}

.back-link {
  color: #4a90a4;
  text-decoration: none;
  font-size: 14px;
}

.back-link:hover {
  text-decoration: underline;
}

.profile-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 1px solid #e0e0e0;
}

.header-left h1 {
  margin: 10px 0;
  font-size: 28px;
  color: #1a1a2e;
}

.customer-meta {
  display: flex;
  gap: 10px;
}

.tier-badge,
.status-badge {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 13px;
  font-weight: 500;
}

.tier-badge.standard { background: #e9ecef; color: #495057; }
.tier-badge.silver { background: #e3e3e3; color: #666; }
.tier-badge.gold { background: #fff3cd; color: #856404; }
.tier-badge.platinum { background: #cce5ff; color: #004085; }

.status-badge.active { background: #d4edda; color: #155724; }
.status-badge.inactive { background: #f8d7da; color: #721c24; }

.header-actions {
  display: flex;
  gap: 10px;
}

.btn-primary,
.btn-secondary {
  padding: 10px 20px;
  border-radius: 6px;
  text-decoration: none;
  font-weight: 500;
}

.btn-primary {
  background: #4a90a4;
  color: white;
}

.btn-secondary {
  background: white;
  color: #666;
  border: 1px solid #ddd;
}

.profile-content {
  display: grid;
  grid-template-columns: 1fr 350px;
  gap: 20px;
}

.info-card,
.loyalty-card,
.actions-card {
  background: white;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 20px;
}

.info-card h2,
.loyalty-card h2,
.actions-card h2 {
  margin: 0 0 20px;
  font-size: 18px;
  color: #1a1a2e;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.info-item .label {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
}

.info-item .value {
  font-size: 15px;
  color: #1a1a2e;
}

.info-item .value.monospace {
  font-family: monospace;
  font-size: 13px;
}

.address-block {
  line-height: 1.6;
}

.address-block p {
  margin: 0;
}

.no-data {
  color: #999;
  font-style: italic;
}

.points-display {
  text-align: center;
  padding: 20px;
  background: linear-gradient(135deg, #4a90a4, #357a8c);
  border-radius: 10px;
  margin-bottom: 20px;
}

.points-value {
  display: block;
  font-size: 42px;
  font-weight: bold;
  color: white;
}

.points-label {
  color: rgba(255, 255, 255, 0.8);
  font-size: 14px;
}

.loyalty-stats {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 15px;
  margin-bottom: 20px;
}

.loyalty-stat {
  text-align: center;
  padding: 10px;
  background: #f8f9fa;
  border-radius: 8px;
}

.loyalty-stat .stat-value {
  display: block;
  font-size: 20px;
  font-weight: bold;
  color: #1a1a2e;
}

.loyalty-stat .stat-label {
  font-size: 12px;
  color: #666;
}

.view-loyalty-link {
  display: block;
  text-align: center;
  color: #4a90a4;
  text-decoration: none;
  font-size: 14px;
}

.view-loyalty-link:hover {
  text-decoration: underline;
}

.action-buttons {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.action-btn {
  padding: 10px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  background: #4a90a4;
  color: white;
  border: none;
}

.action-btn:hover {
  background: #3d7a8c;
}

.action-btn.secondary {
  background: white;
  color: #666;
  border: 1px solid #ddd;
}

.action-btn.secondary:hover {
  background: #f8f9fa;
}

.action-btn.danger {
  background: #dc3545;
}

.action-btn.danger:hover {
  background: #c82333;
}

.action-btn.success {
  background: #28a745;
}

.action-btn.success:hover {
  background: #218838;
}

@media (max-width: 900px) {
  .profile-content {
    grid-template-columns: 1fr;
  }
}
</style>

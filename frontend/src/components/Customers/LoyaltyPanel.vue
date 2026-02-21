<template>
  <div class="loyalty-panel">
    <div class="page-header">
      <router-link :to="`/customers/${customerId}`" class="back-link">
        ← Back to Customer Profile
      </router-link>
      <h1>Loyalty Program</h1>
      <p v-if="customer" class="customer-name">
        {{ customer.firstName }} {{ customer.lastName }}
      </p>
    </div>

    <div v-if="loading" class="loading">Loading loyalty information...</div>
    
    <template v-else-if="loyalty">
      <!-- Points Overview -->
      <div class="points-overview">
        <div class="current-points">
          <span class="points-value">{{ loyalty.points }}</span>
          <span class="points-label">Current Points</span>
        </div>
        <div class="tier-info">
          <span class="tier-badge" :class="loyalty.tier.toLowerCase()">
            {{ loyalty.tier }} Member
          </span>
          <p class="tier-benefits">{{ getTierBenefits(loyalty.tier) }}</p>
        </div>
      </div>

      <!-- Stats Cards -->
      <div class="stats-grid">
        <div class="stat-card">
          <span class="stat-icon">📈</span>
          <span class="stat-value">{{ loyalty.totalPointsEarned }}</span>
          <span class="stat-label">Total Earned</span>
        </div>
        <div class="stat-card">
          <span class="stat-icon">🎁</span>
          <span class="stat-value">{{ loyalty.totalPointsRedeemed }}</span>
          <span class="stat-label">Total Redeemed</span>
        </div>
        <div class="stat-card">
          <span class="stat-icon">🛒</span>
          <span class="stat-value">{{ loyalty.transactionsCount }}</span>
          <span class="stat-label">Transactions</span>
        </div>
        <div class="stat-card">
          <span class="stat-icon">💰</span>
          <span class="stat-value">{{ formatCurrency(loyalty.totalSpent) }}</span>
          <span class="stat-label">Total Spent</span>
        </div>
      </div>

      <!-- Actions Section -->
      <div class="actions-section">
        <div class="action-card">
          <h2>Add Points</h2>
          <p>Award bonus points for promotions, referrals, or special occasions.</p>
          <form @submit.prevent="handleAddPoints" class="action-form">
            <div class="form-row">
              <div class="form-group">
                <label>Points to Add</label>
                <input
                  type="number"
                  v-model.number="addPointsForm.points"
                  min="1"
                  required
                  placeholder="100"
                />
              </div>
              <div class="form-group flex-grow">
                <label>Reason</label>
                <input
                  type="text"
                  v-model="addPointsForm.reason"
                  required
                  placeholder="Birthday bonus, referral reward, etc."
                />
              </div>
            </div>
            <button type="submit" class="btn-primary" :disabled="processing">
              {{ processing ? 'Adding...' : 'Add Points' }}
            </button>
          </form>
        </div>

        <div class="action-card">
          <h2>Redeem Points</h2>
          <p>Redeem customer points for discounts or rewards.</p>
          <form @submit.prevent="handleRedeemPoints" class="action-form">
            <div class="form-row">
              <div class="form-group">
                <label>Points to Redeem</label>
                <input
                  type="number"
                  v-model.number="redeemPointsForm.points"
                  min="1"
                  :max="loyalty.points"
                  required
                  placeholder="50"
                />
              </div>
              <div class="form-group flex-grow">
                <label>Reason</label>
                <input
                  type="text"
                  v-model="redeemPointsForm.reason"
                  required
                  placeholder="Discount on purchase, reward claim, etc."
                />
              </div>
            </div>
            <div class="redemption-value">
              Redemption value: {{ formatCurrency(redeemPointsForm.points * 0.01) }}
            </div>
            <button 
              type="submit" 
              class="btn-primary" 
              :disabled="processing || redeemPointsForm.points > loyalty.points"
            >
              {{ processing ? 'Processing...' : 'Redeem Points' }}
            </button>
          </form>
        </div>
      </div>

      <!-- Tier Progress -->
      <div class="tier-progress-section">
        <h2>Tier Progress</h2>
        <div class="tier-ladder">
          <div
            v-for="tier in tiers"
            :key="tier.name"
            class="tier-step"
            :class="{ active: tier.name === loyalty.tier, achieved: isTierAchieved(tier.name) }"
          >
            <div class="tier-icon">{{ tier.icon }}</div>
            <div class="tier-name">{{ tier.name }}</div>
            <div class="tier-threshold">{{ tier.threshold }}+ pts/year</div>
          </div>
        </div>
        <p class="progress-note">
          Tier status is calculated based on points earned in the current calendar year.
        </p>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useCustomerStore } from '@/stores/customerStore'

const route = useRoute()
const customerStore = useCustomerStore()

const loading = ref(true)
const processing = ref(false)

const customerId = computed(() => route.params.id as string)
const customer = computed(() => customerStore.currentCustomer)
const loyalty = computed(() => customerStore.customerLoyalty)

const addPointsForm = ref({
  points: 100,
  reason: ''
})

const redeemPointsForm = ref({
  points: 0,
  reason: ''
})

const tiers = [
  { name: 'Standard', threshold: 0, icon: '🥉' },
  { name: 'Silver', threshold: 500, icon: '🥈' },
  { name: 'Gold', threshold: 1000, icon: '🥇' },
  { name: 'Platinum', threshold: 2500, icon: '💎' }
]

const tierOrder = ['Standard', 'Silver', 'Gold', 'Platinum']

onMounted(async () => {
  try {
    await Promise.all([
      customerStore.fetchCustomer(customerId.value),
      customerStore.fetchCustomerLoyalty(customerId.value)
    ])
  } finally {
    loading.value = false
  }
})

function getTierBenefits(tier: string): string {
  const benefits: Record<string, string> = {
    'Standard': 'Earn 1 point per $1 spent',
    'Silver': 'Earn 1.25 points per $1 spent + Birthday bonus',
    'Gold': 'Earn 1.5 points per $1 spent + Exclusive offers',
    'Platinum': 'Earn 2 points per $1 spent + VIP access + Free shipping'
  }
  return benefits[tier] || ''
}

function isTierAchieved(tierName: string): boolean {
  if (!loyalty.value) return false
  const currentTierIndex = tierOrder.indexOf(loyalty.value.tier)
  const tierIndex = tierOrder.indexOf(tierName)
  return tierIndex <= currentTierIndex
}

async function handleAddPoints() {
  if (!addPointsForm.value.points || !addPointsForm.value.reason) return
  
  processing.value = true
  try {
    await customerStore.addLoyaltyPoints(customerId.value, {
      points: addPointsForm.value.points,
      reason: addPointsForm.value.reason
    })
    addPointsForm.value = { points: 100, reason: '' }
  } catch (error) {
    console.error('Failed to add points:', error)
  } finally {
    processing.value = false
  }
}

async function handleRedeemPoints() {
  if (!redeemPointsForm.value.points || !redeemPointsForm.value.reason) return
  if (loyalty.value && redeemPointsForm.value.points > loyalty.value.points) return
  
  processing.value = true
  try {
    await customerStore.redeemLoyaltyPoints(customerId.value, {
      points: redeemPointsForm.value.points,
      reason: redeemPointsForm.value.reason
    })
    redeemPointsForm.value = { points: 0, reason: '' }
  } catch (error) {
    console.error('Failed to redeem points:', error)
  } finally {
    processing.value = false
  }
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(value)
}
</script>

<style scoped>
.loyalty-panel {
  padding: 20px;
  max-width: 1000px;
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
  margin: 10px 0 5px;
  font-size: 28px;
  color: #1a1a2e;
}

.customer-name {
  color: #666;
  margin: 0 0 30px;
}

.loading {
  text-align: center;
  padding: 60px;
  color: #666;
}

.points-overview {
  display: flex;
  gap: 30px;
  align-items: center;
  background: linear-gradient(135deg, #4a90a4, #357a8c);
  border-radius: 12px;
  padding: 30px;
  margin-bottom: 30px;
  color: white;
}

.current-points {
  text-align: center;
  padding-right: 30px;
  border-right: 1px solid rgba(255, 255, 255, 0.3);
}

.points-value {
  display: block;
  font-size: 48px;
  font-weight: bold;
}

.points-label {
  font-size: 14px;
  opacity: 0.9;
}

.tier-info {
  flex: 1;
}

.tier-badge {
  display: inline-block;
  padding: 6px 16px;
  border-radius: 20px;
  font-weight: 600;
  margin-bottom: 10px;
}

.tier-badge.standard { background: rgba(255, 255, 255, 0.2); }
.tier-badge.silver { background: #c0c0c0; color: #333; }
.tier-badge.gold { background: #ffd700; color: #333; }
.tier-badge.platinum { background: #e5e4e2; color: #333; }

.tier-benefits {
  margin: 0;
  opacity: 0.9;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.stat-card {
  background: white;
  border-radius: 10px;
  padding: 20px;
  text-align: center;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.stat-icon {
  font-size: 28px;
  display: block;
  margin-bottom: 10px;
}

.stat-value {
  display: block;
  font-size: 24px;
  font-weight: bold;
  color: #1a1a2e;
}

.stat-label {
  color: #666;
  font-size: 14px;
}

.actions-section {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.action-card {
  background: white;
  border-radius: 10px;
  padding: 25px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.action-card h2 {
  margin: 0 0 10px;
  font-size: 18px;
  color: #1a1a2e;
}

.action-card p {
  margin: 0 0 20px;
  color: #666;
  font-size: 14px;
}

.action-form .form-row {
  display: flex;
  gap: 15px;
  margin-bottom: 15px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.form-group.flex-grow {
  flex: 1;
}

.form-group label {
  font-size: 13px;
  font-weight: 500;
  color: #666;
}

.form-group input {
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 14px;
}

.redemption-value {
  margin-bottom: 15px;
  font-size: 14px;
  color: #28a745;
  font-weight: 500;
}

.btn-primary {
  background: #4a90a4;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
}

.btn-primary:hover:not(:disabled) {
  background: #3d7a8c;
}

.btn-primary:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.tier-progress-section {
  background: white;
  border-radius: 10px;
  padding: 25px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.tier-progress-section h2 {
  margin: 0 0 20px;
  font-size: 18px;
  color: #1a1a2e;
}

.tier-ladder {
  display: flex;
  justify-content: space-between;
  position: relative;
  padding: 20px 0;
}

.tier-ladder::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  height: 4px;
  background: #e0e0e0;
  z-index: 0;
}

.tier-step {
  position: relative;
  z-index: 1;
  text-align: center;
  padding: 15px;
  background: white;
  border-radius: 10px;
  border: 2px solid #e0e0e0;
  min-width: 100px;
}

.tier-step.achieved {
  border-color: #4a90a4;
  background: #e8f4f8;
}

.tier-step.active {
  border-color: #ffd700;
  background: #fffbeb;
  box-shadow: 0 4px 12px rgba(255, 215, 0, 0.3);
}

.tier-icon {
  font-size: 28px;
  margin-bottom: 5px;
}

.tier-name {
  font-weight: 600;
  color: #1a1a2e;
  margin-bottom: 5px;
}

.tier-threshold {
  font-size: 12px;
  color: #666;
}

.progress-note {
  margin: 20px 0 0;
  font-size: 13px;
  color: #666;
  text-align: center;
}
</style>

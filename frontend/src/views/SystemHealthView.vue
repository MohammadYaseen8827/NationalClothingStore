<template>
  <div class="system-health">
    <div class="page-header">
      <h1>System Health</h1>
      <button @click="refreshHealth" class="btn-refresh" :disabled="loading">
        <i class="fas fa-sync-alt" :class="{ 'fa-spin': loading }"></i>
        Refresh
      </button>
    </div>

    <!-- Health Status Cards -->
    <div class="health-cards">
      <div class="health-card" :class="healthStatusClass">
        <div class="card-icon">
          <i class="fas fa-heartbeat"></i>
        </div>
        <div class="card-content">
          <h3>Status</h3>
          <p class="status-text">{{ healthData?.status || 'Unknown' }}</p>
        </div>
      </div>

      <div class="health-card">
        <div class="card-icon">
          <i class="fas fa-clock"></i>
        </div>
        <div class="card-content">
          <h3>Uptime</h3>
          <p class="status-text">{{ formatUptime(healthData?.uptime) }}</p>
        </div>
      </div>

      <div class="health-card">
        <div class="card-icon">
          <i class="fas fa-code-branch"></i>
        </div>
        <div class="card-content">
          <h3>Version</h3>
          <p class="status-text">{{ healthData?.version || 'N/A' }}</p>
        </div>
      </div>

      <div class="health-card">
        <div class="card-icon">
          <i class="fas fa-calendar-alt"></i>
        </div>
        <div class="card-content">
          <h3>Last Check</h3>
          <p class="status-text">{{ formatDate(healthData?.timestamp) }}</p>
        </div>
      </div>
    </div>

    <!-- Tabs -->
    <div class="tabs">
      <button 
        v-for="tab in tabs" 
        :key="tab.id"
        :class="['tab-btn', { active: activeTab === tab.id }]"
        @click="activeTab = tab.id"
      >
        {{ tab.label }}
      </button>
    </div>

    <!-- Tab Content -->
    <div class="tab-content">
      <!-- Basic Health -->
      <div v-if="activeTab === 'basic'" class="tab-pane">
        <div v-if="loading" class="loading">Loading...</div>
        <div v-else-if="error" class="error-message">{{ error }}</div>
        <div v-else class="health-info">
          <pre>{{ JSON.stringify(healthData, null, 2) }}</pre>
        </div>
      </div>

      <!-- Readiness -->
      <div v-if="activeTab === 'readiness'" class="tab-pane">
        <div v-if="readinessLoading" class="loading">Loading...</div>
        <div v-else-if="readinessError" class="error-message">{{ readinessError }}</div>
        <div v-else class="readiness-checks">
          <div v-for="(value, key) in readinessData?.checks" :key="String(key)" class="check-item">
            <i :class="['fas', value ? 'fa-check-circle' : 'fa-times-circle']"></i>
            <span>{{ formatCheckName(String(key)) }}</span>
            <span class="check-status">{{ value ? 'Ready' : 'Not Ready' }}</span>
          </div>
        </div>
      </div>

      <!-- Detailed -->
      <div v-if="activeTab === 'detailed'" class="tab-pane">
        <div v-if="detailedLoading" class="loading">Loading...</div>
        <div v-else-if="detailedError" class="error-message">{{ detailedError }}</div>
        <div v-else class="detailed-checks">
          <div v-for="(check, category) in detailedData?.checks" :key="String(category)" class="check-category">
            <h4>{{ formatCheckName(String(category)) }}</h4>
            <div class="check-details">
              <span :class="['status-badge', check.status]">{{ check.status }}</span>
              <span v-if="check.responseTime" class="response-time">
                {{ check.responseTime }}ms
              </span>
              <span v-if="check.error" class="error-text">{{ check.error }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import healthService from '@/services/healthService'

const loading = ref(false)
const error = ref('')
const healthData = ref<any>(null)

const readinessLoading = ref(false)
const readinessError = ref('')
const readinessData = ref<any>(null)

const detailedLoading = ref(false)
const detailedError = ref('')
const detailedData = ref<any>(null)

const activeTab = ref('basic')

const tabs = [
  { id: 'basic', label: 'Basic Health' },
  { id: 'readiness', label: 'Readiness' },
  { id: 'detailed', label: 'Detailed' }
]

const healthStatusClass = computed(() => {
  const status = healthData.value?.status
  return {
    'status-healthy': status === 'healthy',
    'status-degraded': status === 'degraded',
    'status-unhealthy': status === 'unhealthy'
  }
})

const refreshHealth = async () => {
  await Promise.all([
    loadHealth(),
    loadReadiness(),
    loadDetailed()
  ])
}

const loadHealth = async () => {
  loading.value = true
  error.value = ''
  try {
    healthData.value = await healthService.getHealth()
  } catch (e: any) {
    error.value = e.message || 'Failed to load health status'
  } finally {
    loading.value = false
  }
}

const loadReadiness = async () => {
  readinessLoading.value = true
  readinessError.value = ''
  try {
    readinessData.value = await healthService.getReadiness()
  } catch (e: any) {
    readinessError.value = e.message || 'Failed to load readiness status'
  } finally {
    readinessLoading.value = false
  }
}

const loadDetailed = async () => {
  detailedLoading.value = true
  detailedError.value = ''
  try {
    detailedData.value = await healthService.getDetailedHealth()
  } catch (e: any) {
    detailedError.value = e.message || 'Failed to load detailed health'
  } finally {
    detailedLoading.value = false
  }
}

const formatUptime = (seconds?: number): string => {
  if (!seconds) return 'N/A'
  const days = Math.floor(seconds / 86400)
  const hours = Math.floor((seconds % 86400) / 3600)
  const mins = Math.floor((seconds % 3600) / 60)
  return `${days}d ${hours}h ${mins}m`
}

const formatDate = (timestamp?: string): string => {
  if (!timestamp) return 'N/A'
  return new Date(timestamp).toLocaleString()
}

const formatCheckName = (key: string): string => {
  return key.replace(/([A-Z])/g, ' $1').replace(/^./, str => str.toUpperCase())
}

onMounted(() => {
  refreshHealth()
})
</script>

<style scoped>
.system-health {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.page-header h1 {
  margin: 0;
  font-size: 24px;
}

.btn-refresh {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  background: #4a90d9;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.btn-refresh:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.health-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  margin-bottom: 24px;
}

.health-card {
  background: white;
  border-radius: 8px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.health-card.status-healthy {
  border-left: 4px solid #28a745;
}

.health-card.status-degraded {
  border-left: 4px solid #ffc107;
}

.health-card.status-unhealthy {
  border-left: 4px solid #dc3545;
}

.card-icon {
  font-size: 32px;
  color: #6c757d;
}

.card-content h3 {
  margin: 0 0 4px 0;
  font-size: 14px;
  color: #6c757d;
}

.status-text {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
  text-transform: capitalize;
}

.tabs {
  display: flex;
  gap: 8px;
  margin-bottom: 16px;
  border-bottom: 1px solid #dee2e6;
}

.tab-btn {
  padding: 8px 16px;
  background: none;
  border: none;
  border-bottom: 2px solid transparent;
  cursor: pointer;
  font-size: 14px;
}

.tab-btn.active {
  border-bottom-color: #4a90d9;
  color: #4a90d9;
}

.tab-content {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.tab-pane {
  min-height: 200px;
}

.loading, .error-message {
  text-align: center;
  padding: 40px;
}

.error-message {
  color: #dc3545;
}

.health-info pre {
  background: #f8f9fa;
  padding: 16px;
  border-radius: 4px;
  overflow-x: auto;
}

.check-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  border-bottom: 1px solid #dee2e6;
}

.check-item i.fa-check-circle {
  color: #28a745;
}

.check-item i.fa-times-circle {
  color: #dc3545;
}

.check-status {
  margin-left: auto;
  font-weight: 500;
}

.check-category {
  margin-bottom: 16px;
}

.check-category h4 {
  margin: 0 0 8px 0;
  text-transform: capitalize;
}

.check-details {
  display: flex;
  align-items: center;
  gap: 12px;
}

.status-badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.status-badge.healthy {
  background: #d4edda;
  color: #155724;
}

.status-badge.degraded {
  background: #fff3cd;
  color: #856404;
}

.status-badge.unhealthy {
  background: #f8d7da;
  color: #721c24;
}

.response-time {
  color: #6c757d;
}

.error-text {
  color: #dc3545;
  font-size: 12px;
}
</style>

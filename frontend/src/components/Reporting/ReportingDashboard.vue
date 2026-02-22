<template>
  <div class="reporting-dashboard">
    <!-- Header -->
    <div class="dashboard-header">
      <h1>Reporting Dashboard</h1>
      <div class="header-controls">
        <div class="date-range-selector">
          <label>Date Range:</label>
          <select v-model="selectedPeriod" @change="updateDateRange">
            <option value="today">Today</option>
            <option value="week">This Week</option>
            <option value="month">This Month</option>
            <option value="quarter">This Quarter</option>
            <option value="year">This Year</option>
            <option value="custom">Custom</option>
          </select>
        </div>
        <div v-if="selectedPeriod === 'custom'" class="custom-dates">
          <input 
            type="date" 
            v-model="customStartDate" 
            @change="updateDateRange"
            :max="customEndDate"
          />
          <span>to</span>
          <input 
            type="date" 
            v-model="customEndDate" 
            @change="updateDateRange"
            :min="customStartDate"
            :max="today"
          />
        </div>
        <div class="location-filter">
          <label>Location:</label>
          <select v-model="selectedLocation" @change="refreshData">
            <option value="all">All Locations</option>
            <option v-for="location in locations" :key="location.id" :value="location.id">
              {{ location.name }}
            </option>
          </select>
        </div>
        <button class="refresh-btn" @click="refreshData" :disabled="loading">
          <span v-if="loading">Refreshing...</span>
          <span v-else>Refresh</span>
        </button>
      </div>
    </div>

    <!-- Key Metrics Cards -->
    <div class="metrics-grid">
      <div class="metric-card sales">
        <div class="metric-icon">💰</div>
        <div class="metric-content">
          <h3>Total Sales</h3>
          <div class="metric-value">{{ formatCurrency(keyMetrics.totalSales) }}</div>
          <div class="metric-change" :class="getChangeClass(keyMetrics.salesGrowth)">
            {{ formatPercent(keyMetrics.salesGrowth) }} vs last period
          </div>
        </div>
      </div>

      <div class="metric-card transactions">
        <div class="metric-icon">📊</div>
        <div class="metric-content">
          <h3>Transactions</h3>
          <div class="metric-value">{{ formatNumber(keyMetrics.totalTransactions) }}</div>
          <div class="metric-change" :class="getChangeClass(keyMetrics.transactionGrowth)">
            {{ formatPercent(keyMetrics.transactionGrowth) }} vs last period
          </div>
        </div>
      </div>

      <div class="metric-card inventory">
        <div class="metric-icon">📦</div>
        <div class="metric-content">
          <h3>Inventory Items</h3>
          <div class="metric-value">{{ formatNumber(keyMetrics.totalInventoryItems) }}</div>
          <div class="metric-subtitle">{{ keyMetrics.lowStockAlerts }} low stock alerts</div>
        </div>
      </div>

      <div class="metric-card customers">
        <div class="metric-icon">👥</div>
        <div class="metric-content">
          <h3>Active Customers</h3>
          <div class="metric-value">{{ formatNumber(keyMetrics.activeCustomers) }}</div>
          <div class="metric-subtitle">{{ keyMetrics.newCustomers }} new this period</div>
        </div>
      </div>

      <div class="metric-card profit">
        <div class="metric-icon">📈</div>
        <div class="metric-content">
          <h3>Gross Profit</h3>
          <div class="metric-value">{{ formatCurrency(keyMetrics.grossProfit) }}</div>
          <div class="metric-change" :class="getChangeClass(keyMetrics.profitMargin)">
            {{ formatPercent(keyMetrics.profitMargin) }} margin
          </div>
        </div>
      </div>

      <div class="metric-card efficiency">
        <div class="metric-icon">⚡</div>
        <div class="metric-content">
          <h3>Avg Transaction</h3>
          <div class="metric-value">{{ formatCurrency(keyMetrics.averageTransactionValue) }}</div>
          <div class="metric-subtitle">Per transaction</div>
        </div>
      </div>
    </div>

    <!-- Charts Section -->
    <div class="charts-section">
      <!-- Sales Trend Chart -->
      <div class="chart-container">
        <h2>Sales Trend</h2>
        <div class="chart-controls">
          <select v-model="salesChartPeriod" @change="updateSalesChart">
            <option value="daily">Daily</option>
            <option value="weekly">Weekly</option>
            <option value="monthly">Monthly</option>
          </select>
        </div>
        <div class="chart-wrapper">
          <canvas ref="salesChart"></canvas>
        </div>
      </div>

      <!-- Top Products Chart -->
      <div class="chart-container">
        <h2>Top Products</h2>
        <div class="chart-controls">
          <select v-model="topProductsMetric" @change="updateTopProducts">
            <option value="revenue">By Revenue</option>
            <option value="quantity">By Quantity</option>
          </select>
        </div>
        <div class="chart-wrapper">
          <canvas ref="topProductsChart"></canvas>
        </div>
      </div>

      <!-- Sales by Payment Method -->
      <div class="chart-container">
        <h2>Sales by Payment Method</h2>
        <div class="chart-wrapper">
          <canvas ref="paymentMethodsChart"></canvas>
        </div>
      </div>

      <!-- Customer Segments -->
      <div class="chart-container">
        <h2>Customer Segments</h2>
        <div class="chart-wrapper">
          <canvas ref="customerSegmentsChart"></canvas>
        </div>
      </div>
    </div>

    <!-- Recent Reports Table -->
    <div class="reports-section">
      <h2>Recent Reports</h2>
      <div class="table-controls">
        <button @click="generateReport" class="generate-btn" :disabled="generatingReport">
          <span v-if="generatingReport">Generating...</span>
          <span v-else>Generate Report</span>
        </button>
        <select v-model="reportType">
          <option value="sales">Sales Report</option>
          <option value="inventory">Inventory Report</option>
          <option value="customers">Customer Report</option>
          <option value="financial">Financial Report</option>
          <option value="procurement">Procurement Report</option>
        </select>
      </div>
      
      <div class="table-container">
        <table class="reports-table">
          <thead>
            <tr>
              <th>Report Name</th>
              <th>Type</th>
              <th>Generated</th>
              <th>Period</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="report in recentReports" :key="report.id">
              <td>{{ report.name }}</td>
              <td>
                <span class="report-type" :class="report.type.toLowerCase()">
                  {{ report.type }}
                </span>
              </td>
              <td>{{ formatDate(report.generatedAt) }}</td>
              <td>{{ report.period }}</td>
              <td>
                <span class="status" :class="report.status.toLowerCase()">
                  {{ report.status }}
                </span>
              </td>
              <td>
                <div class="action-buttons">
                  <button @click="downloadReport(report)" class="download-btn" :disabled="report.status !== 'Completed'">
                    Download
                  </button>
                  <button @click="deleteReport(report)" class="delete-btn">
                    Delete
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Real-time Metrics -->
    <div class="realtime-section">
      <h2>Real-time Metrics</h2>
      <div class="realtime-grid">
        <div class="realtime-card">
          <h3>Current Hour Sales</h3>
          <div class="realtime-value">{{ formatCurrency(realTimeMetrics.currentHourSales) }}</div>
        </div>
        <div class="realtime-card">
          <h3>Today's Sales</h3>
          <div class="realtime-value">{{ formatCurrency(realTimeMetrics.todaySales) }}</div>
        </div>
        <div class="realtime-card">
          <h3>Active Users</h3>
          <div class="realtime-value">{{ realTimeMetrics.activeUsers }}</div>
        </div>
        <div class="realtime-card">
          <h3>Pending Orders</h3>
          <div class="realtime-value">{{ realTimeMetrics.pendingOrders }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted, onUnmounted } from 'vue'
import { useReportingStore } from '@/stores/reportingStore'
import reportingService from '@/services/reportingService'
import { Chart, registerables } from 'chart.js'
import { format } from 'date-fns'

export default {
  name: 'ReportingDashboard',
  setup() {
    const reportingStore = useReportingStore()
    
    // Reactive state
    const loading = ref(false)
    const generatingReport = ref(false)
    const selectedPeriod = ref('month')
    const customStartDate = ref('')
    const customEndDate = ref('')
    const selectedLocation = ref('all')
    const salesChartPeriod = ref('daily')
    const topProductsMetric = ref('revenue')
    const reportType = ref('sales')
    const today = ref(format(new Date(), 'yyyy-MM-dd'))
    
    // Data
    const keyMetrics = reactive({})
    const recentReports = ref([])
    const realTimeMetrics = reactive({})
    const locations = ref([])
    
    // Chart instances
    let salesChart = null
    let topProductsChart = null
    let paymentMethodsChart = null
    let customerSegmentsChart = null
    
    // Real-time update interval
    let realtimeInterval = null

    // Methods
    const updateDateRange = () => {
      let endDate = new Date()
      let startDate = new Date()
      
      switch (selectedPeriod.value) {
        case 'today':
          startDate = new Date()
          break
        case 'week':
          startDate.setDate(endDate.getDate() - 7)
          break
        case 'month':
          startDate.setMonth(endDate.getMonth() - 1)
          break
        case 'quarter':
          startDate.setMonth(endDate.getMonth() - 3)
          break
        case 'year':
          startDate.setFullYear(endDate.getFullYear() - 1)
          break
        case 'custom':
          if (customStartDate.value && customEndDate.value) {
            startDate = new Date(customStartDate.value)
            endDate = new Date(customEndDate.value)
          }
          break
      }
      
      refreshData()
    }

    const refreshData = async () => {
      loading.value = true
      try {
        await Promise.all([
          loadKeyMetrics(),
          loadRecentReports(),
          loadRealTimeMetrics(),
          updateCharts()
        ])
      } catch (error) {
        console.error('Error refreshing dashboard data:', error)
      } finally {
        loading.value = false
      }
    }

    const loadKeyMetrics = async () => {
      try {
        const response = await reportingStore.getDashboardSummary({
          locationId: selectedLocation.value === 'all' ? null : selectedLocation.value
        })
        Object.assign(keyMetrics, response)
      } catch (error) {
        console.error('Error loading key metrics:', error)
      }
    }

    const loadRecentReports = async () => {
      try {
        const response = await reportingStore.getRecentReports()
        recentReports.value = response
      } catch (error) {
        console.error('Error loading recent reports:', error)
      }
    }

    const loadRealTimeMetrics = async () => {
      try {
        const response = await reportingStore.getRealTimeMetrics()
        Object.assign(realTimeMetrics, response)
      } catch (error) {
        console.error('Error loading real-time metrics:', error)
      }
    }

    const updateCharts = async () => {
      await Promise.all([
        updateSalesChart(),
        updateTopProducts(),
        updatePaymentMethods(),
        updateCustomerSegments()
      ])
    }

    const updateSalesChart = async () => {
      try {
        const endDate = new Date()
        let startDate = new Date()
        
        if (salesChartPeriod.value === 'daily') {
          startDate.setDate(endDate.getDate() - 30)
        } else if (salesChartPeriod.value === 'weekly') {
          startDate.setDate(endDate.getDate() - 12 * 7)
        } else {
          startDate.setMonth(endDate.getMonth() - 12)
        }

        const analytics = await reportingStore.getSalesAnalytics({
          startDate,
          endDate,
          period: salesChartPeriod.value.toUpperCase(),
          locationId: selectedLocation.value === 'all' ? null : selectedLocation.value
        })

        const ctx = document.querySelector('[ref="salesChart"]')?.getContext('2d')
        if (ctx) {
          if (salesChart) {
            salesChart.destroy()
          }
          
          salesChart = new Chart(ctx, {
            type: 'line',
            data: {
              labels: analytics.periodData.map(p => p.period),
              datasets: [{
                label: 'Sales',
                data: analytics.periodData.map(p => p.value),
                borderColor: '#3b82f6',
                backgroundColor: 'rgba(59, 130, 246, 0.1)',
                tension: 0.4
              }]
            },
            options: {
              responsive: true,
              plugins: {
                legend: {
                  display: false
                }
              },
              scales: {
                y: {
                  beginAtZero: true,
                  ticks: {
                    callback: (value) => formatCurrency(value)
                  }
                }
              }
            }
          })
        }
      } catch (error) {
        console.error('Error updating sales chart:', error)
      }
    }

    const updateTopProducts = async () => {
      try {
        const endDate = new Date()
        const startDate = new Date()
        startDate.setMonth(endDate.getMonth() - 1)

        const analytics = await reportingStore.getSalesAnalytics({
          startDate,
          endDate,
          period: 'MONTHLY',
          locationId: selectedLocation.value === 'all' ? null : selectedLocation.value
        })

        const topProducts = analytics.topProducts.slice(0, 10)
        
        const ctx = document.querySelector('[ref="topProductsChart"]')?.getContext('2d')
        if (ctx) {
          if (topProductsChart) {
            topProductsChart.destroy()
          }
          
          topProductsChart = new Chart(ctx, {
            type: 'bar',
            data: {
              labels: topProducts.map(p => p.productName),
              datasets: [{
                label: topProductsMetric.value === 'revenue' ? 'Revenue' : 'Quantity',
                data: topProducts.map(p => topProductsMetric.value === 'revenue' ? p.totalRevenue : p.totalQuantity),
                backgroundColor: '#10b981'
              }]
            },
            options: {
              responsive: true,
              plugins: {
                legend: {
                  display: false
                }
              },
              scales: {
                y: {
                  beginAtZero: true
                }
              }
            }
          })
        }
      } catch (error) {
        console.error('Error updating top products chart:', error)
      }
    }

    const updatePaymentMethods = async () => {
      try {
        const endDate = new Date()
        const startDate = new Date()
        startDate.setMonth(endDate.getMonth() - 1)

        const report = await reportingStore.getSalesReport({
          startDate,
          endDate,
          locationId: selectedLocation.value === 'all' ? null : selectedLocation.value
        })

        const ctx = document.querySelector('[ref="paymentMethodsChart"]')?.getContext('2d')
        if (ctx) {
          if (paymentMethodsChart) {
            paymentMethodsChart.destroy()
          }
          
          paymentMethodsChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
              labels: report.salesByPaymentMethod.map(p => p.paymentMethod),
              datasets: [{
                data: report.salesByPaymentMethod.map(p => p.totalAmount),
                backgroundColor: ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6']
              }]
            },
            options: {
              responsive: true,
              plugins: {
                legend: {
                  position: 'right'
                }
              }
            }
          })
        }
      } catch (error) {
        console.error('Error updating payment methods chart:', error)
      }
    }

    const updateCustomerSegments = async () => {
      try {
        const endDate = new Date()
        const startDate = new Date()
        startDate.setMonth(endDate.getMonth() - 1)

        const analytics = await reportingStore.getCustomerAnalytics({
          startDate,
          endDate
        })

        const ctx = document.querySelector('[ref="customerSegmentsChart"]')?.getContext('2d')
        if (ctx) {
          if (customerSegmentsChart) {
            customerSegmentsChart.destroy()
          }
          
          customerSegmentsChart = new Chart(ctx, {
            type: 'pie',
            data: {
              labels: analytics.customerSegments.map(s => s.segment),
              datasets: [{
                data: analytics.customerSegments.map(s => s.count),
                backgroundColor: ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6']
              }]
            },
            options: {
              responsive: true,
              plugins: {
                legend: {
                  position: 'right'
                }
              }
            }
          })
        }
      } catch (error) {
        console.error('Error updating customer segments chart:', error)
      }
    }

    const generateReport = async () => {
      generatingReport.value = true
      try {
        const endDate = new Date()
        const startDate = new Date()
        startDate.setMonth(endDate.getMonth() - 1)

        await reportingService.exportReport({
          reportType: reportType.value,
          startDate: startDate.toISOString(),
          endDate: endDate.toISOString(),
          format: 'PDF'
        })

        await loadRecentReports()
      } catch (error) {
        console.error('Error generating report:', error)
      } finally {
        generatingReport.value = false
      }
    }

    const downloadReport = async (report) => {
      try {
        await reportingStore.downloadReport(report.id)
      } catch (error) {
        console.error('Error downloading report:', error)
      }
    }

    const deleteReport = async (report) => {
      if (confirm(`Are you sure you want to delete ${report.name}?`)) {
        try {
          await reportingStore.deleteReport(report.id)
          await loadRecentReports()
        } catch (error) {
          console.error('Error deleting report:', error)
        }
      }
    }

    // Utility functions
    const formatCurrency = (value) => {
      return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
      }).format(value)
    }

    const formatNumber = (value) => {
      return new Intl.NumberFormat('en-US').format(value)
    }

    const formatPercent = (value) => {
      return `${value.toFixed(1)}%`
    }

    const formatDate = (date) => {
      return format(new Date(date), 'MMM dd, yyyy HH:mm')
    }

    const getChangeClass = (value) => {
      return value > 0 ? 'positive' : value < 0 ? 'negative' : 'neutral'
    }

    // Lifecycle hooks
    onMounted(async () => {
      Chart.register(...registerables)
      
      // Load initial data
      await refreshData()
      
      // Set up real-time updates
      realtimeInterval = setInterval(() => {
        loadRealTimeMetrics()
      }, 30000) // Update every 30 seconds
    })

    onUnmounted(() => {
      if (realtimeInterval) {
        clearInterval(realtimeInterval)
      }
      
      // Destroy charts
      if (salesChart) salesChart.destroy()
      if (topProductsChart) topProductsChart.destroy()
      if (paymentMethodsChart) paymentMethodsChart.destroy()
      if (customerSegmentsChart) customerSegmentsChart.destroy()
    })

    return {
      // State
      loading,
      generatingReport,
      selectedPeriod,
      customStartDate,
      customEndDate,
      selectedLocation,
      salesChartPeriod,
      topProductsMetric,
      reportType,
      today,
      keyMetrics,
      recentReports,
      realTimeMetrics,
      locations,
      
      // Methods
      updateDateRange,
      refreshData,
      generateReport,
      downloadReport,
      deleteReport,
      updateSalesChart,
      updateTopProducts,
      
      // Utility
      formatCurrency,
      formatNumber,
      formatPercent,
      formatDate,
      getChangeClass
    }
  }
}
</script>

<style scoped>
.reporting-dashboard {
  padding: 24px;
  background-color: #f8fafc;
  min-height: 100vh;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 32px;
}

.dashboard-header h1 {
  font-size: 2rem;
  font-weight: 700;
  color: #1e293b;
}

.header-controls {
  display: flex;
  gap: 16px;
  align-items: center;
}

.header-controls label {
  font-weight: 500;
  color: #64748b;
}

.header-controls select,
.header-controls input {
  padding: 8px 12px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  background-color: white;
}

.refresh-btn {
  padding: 8px 16px;
  background-color: #3b82f6;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
}

.refresh-btn:disabled {
  background-color: #9ca3af;
  cursor: not-allowed;
}

.metrics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 24px;
  margin-bottom: 32px;
}

.metric-card {
  background-color: white;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
  gap: 16px;
}

.metric-icon {
  font-size: 2rem;
}

.metric-content h3 {
  font-size: 0.875rem;
  font-weight: 500;
  color: #64748b;
  margin-bottom: 4px;
}

.metric-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 4px;
}

.metric-change {
  font-size: 0.75rem;
  font-weight: 500;
}

.metric-change.positive {
  color: #10b981;
}

.metric-change.negative {
  color: #ef4444;
}

.metric-change.neutral {
  color: #64748b;
}

.metric-subtitle {
  font-size: 0.75rem;
  color: #64748b;
}

.charts-section {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 24px;
  margin-bottom: 32px;
}

.chart-container {
  background-color: white;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.chart-container h2 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 16px;
}

.chart-controls {
  margin-bottom: 16px;
}

.chart-controls select {
  padding: 6px 12px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  background-color: white;
}

.chart-wrapper {
  height: 300px;
}

.reports-section {
  background-color: white;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  margin-bottom: 32px;
}

.reports-section h2 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 16px;
}

.table-controls {
  display: flex;
  gap: 16px;
  margin-bottom: 16px;
}

.generate-btn {
  padding: 8px 16px;
  background-color: #10b981;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
}

.generate-btn:disabled {
  background-color: #9ca3af;
  cursor: not-allowed;
}

.table-controls select {
  padding: 8px 12px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  background-color: white;
}

.reports-table {
  width: 100%;
  border-collapse: collapse;
}

.reports-table th,
.reports-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

.reports-table th {
  font-weight: 600;
  color: #374151;
  background-color: #f9fafb;
}

.report-type {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.75rem;
  font-weight: 500;
}

.report-type.sales {
  background-color: #dbeafe;
  color: #1e40af;
}

.report-type.inventory {
  background-color: #d1fae5;
  color: #065f46;
}

.report-type.customers {
  background-color: #fef3c7;
  color: #92400e;
}

.report-type.financial {
  background-color: #ede9fe;
  color: #5b21b6;
}

.report-type.procurement {
  background-color: #fee2e2;
  color: #991b1b;
}

.status {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.75rem;
  font-weight: 500;
}

.status.completed {
  background-color: #d1fae5;
  color: #065f46;
}

.status.processing {
  background-color: #fef3c7;
  color: #92400e;
}

.status.failed {
  background-color: #fee2e2;
  color: #991b1b;
}

.action-buttons {
  display: flex;
  gap: 8px;
}

.download-btn {
  padding: 4px 8px;
  background-color: #3b82f6;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 0.75rem;
  cursor: pointer;
}

.download-btn:disabled {
  background-color: #9ca3af;
  cursor: not-allowed;
}

.delete-btn {
  padding: 4px 8px;
  background-color: #ef4444;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 0.75rem;
  cursor: pointer;
}

.realtime-section {
  background-color: white;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.realtime-section h2 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 16px;
}

.realtime-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
}

.realtime-card {
  padding: 16px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  text-align: center;
}

.realtime-card h3 {
  font-size: 0.875rem;
  font-weight: 500;
  color: #64748b;
  margin-bottom: 8px;
}

.realtime-value {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1e293b;
}

@media (max-width: 768px) {
  .dashboard-header {
    flex-direction: column;
    gap: 16px;
    align-items: stretch;
  }

  .header-controls {
    flex-direction: column;
    align-items: stretch;
  }

  .metrics-grid {
    grid-template-columns: 1fr;
  }

  .charts-section {
    grid-template-columns: 1fr;
  }

  .table-controls {
    flex-direction: column;
  }

  .realtime-grid {
    grid-template-columns: 1fr;
  }
}
</style>

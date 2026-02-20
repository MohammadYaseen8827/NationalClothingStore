<template>
  <div class="analytics-visualization">
    <!-- Header -->
    <div class="analytics-header">
      <h1>Advanced Analytics</h1>
      <div class="header-controls">
        <div class="analytics-type-selector">
          <label>Analytics Type:</label>
          <select v-model="selectedAnalyticsType" @change="updateAnalytics">
            <option value="sales">Sales Analytics</option>
            <option value="inventory">Inventory Analytics</option>
            <option value="customer">Customer Analytics</option>
            <option value="financial">Financial Analytics</option>
            <option value="predictive">Predictive Analytics</option>
          </select>
        </div>
        <div class="period-selector">
          <label>Period:</label>
          <select v-model="selectedPeriod" @change="updateAnalytics">
            <option value="daily">Daily</option>
            <option value="weekly">Weekly</option>
            <option value="monthly">Monthly</option>
            <option value="quarterly">Quarterly</option>
            <option value="yearly">Yearly</option>
          </select>
        </div>
        <div class="date-range">
          <input 
            type="date" 
            v-model="startDate" 
            @change="updateAnalytics"
            :max="endDate"
          />
          <span>to</span>
          <input 
            type="date" 
            v-model="endDate" 
            @change="updateAnalytics"
            :min="startDate"
          />
        </div>
        <button class="refresh-btn" @click="refreshData" :disabled="loading">
          <span v-if="loading">Loading...</span>
          <span v-else>Refresh</span>
        </button>
      </div>
    </div>

    <!-- Analytics Content -->
    <div class="analytics-content">
      <!-- Sales Analytics -->
      <div v-if="selectedAnalyticsType === 'sales'" class="sales-analytics">
        <!-- Sales Overview -->
        <div class="analytics-overview">
          <div class="overview-card">
            <h3>Total Revenue</h3>
            <div class="metric-value">{{ formatCurrency(salesAnalytics.totalRevenue) }}</div>
            <div class="metric-trend" :class="getTrendClass(salesAnalytics.revenueGrowth)">
              {{ formatPercent(salesAnalytics.revenueGrowth) }} growth
            </div>
          </div>
          <div class="overview-card">
            <h3>Average Order Value</h3>
            <div class="metric-value">{{ formatCurrency(salesAnalytics.averageOrderValue) }}</div>
            <div class="metric-trend" :class="getTrendClass(salesAnalytics.aovGrowth)">
              {{ formatPercent(salesAnalytics.aovGrowth) }} growth
            </div>
          </div>
          <div class="overview-card">
            <h3>Conversion Rate</h3>
            <div class="metric-value">{{ formatPercent(salesAnalytics.conversionRate) }}</div>
            <div class="metric-trend" :class="getTrendClass(salesAnalytics.conversionGrowth)">
              {{ formatPercent(salesAnalytics.conversionGrowth) }} growth
            </div>
          </div>
          <div class="overview-card">
            <h3>Sales Velocity</h3>
            <div class="metric-value">{{ salesAnalytics.salesVelocity.toFixed(1) }}</div>
            <div class="metric-subtitle">Items per hour</div>
          </div>
        </div>

        <!-- Sales Charts -->
        <div class="charts-grid">
          <!-- Revenue Trend -->
          <div class="chart-container">
            <h3>Revenue Trend</h3>
            <div class="chart-controls">
              <select v-model="revenueChartType" @change="updateRevenueChart">
                <option value="line">Line Chart</option>
                <option value="bar">Bar Chart</option>
                <option value="area">Area Chart</option>
              </select>
            </div>
            <div class="chart-wrapper">
              <canvas ref="revenueChart"></canvas>
            </div>
          </div>

          <!-- Product Performance -->
          <div class="chart-container">
            <h3>Product Performance</h3>
            <div class="chart-controls">
              <select v-model="productMetric" @change="updateProductChart">
                <option value="revenue">By Revenue</option>
                <option value="quantity">By Quantity</option>
                <option value="profit">By Profit</option>
              </select>
            </div>
            <div class="chart-wrapper">
              <canvas ref="productChart"></canvas>
            </div>
          </div>

          <!-- Location Performance -->
          <div class="chart-container">
            <h3>Location Performance</h3>
            <div class="chart-wrapper">
              <canvas ref="locationChart"></canvas>
            </div>
          </div>

          <!-- Sales Funnel -->
          <div class="chart-container">
            <h3>Sales Funnel</h3>
            <div class="chart-wrapper">
              <canvas ref="funnelChart"></canvas>
            </div>
          </div>
        </div>

        <!-- Sales Insights -->
        <div class="insights-section">
          <h3>Sales Insights</h3>
          <div class="insights-grid">
            <div class="insight-card" v-for="insight in salesAnalytics.insights" :key="insight.id">
              <div class="insight-type" :class="insight.type.toLowerCase()">
                {{ insight.type }}
              </div>
              <div class="insight-title">{{ insight.title }}</div>
              <div class="insight-description">{{ insight.description }}</div>
              <div class="insight-impact" :class="insight.impact.toLowerCase()">
                {{ insight.impact }} Impact
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Inventory Analytics -->
      <div v-else-if="selectedAnalyticsType === 'inventory'" class="inventory-analytics">
        <!-- Inventory Overview -->
        <div class="analytics-overview">
          <div class="overview-card">
            <h3>Total Inventory Value</h3>
            <div class="metric-value">{{ formatCurrency(inventoryAnalytics.totalValue) }}</div>
            <div class="metric-subtitle">{{ inventoryAnalytics.totalItems }} items</div>
          </div>
          <div class="overview-card">
            <h3>Inventory Turnover</h3>
            <div class="metric-value">{{ inventoryAnalytics.turnoverRate.toFixed(2) }}</div>
            <div class="metric-trend" :class="getTrendClass(inventoryAnalytics.turnoverGrowth)">
              {{ formatPercent(inventoryAnalytics.turnoverGrowth) }} growth
            </div>
          </div>
          <div class="overview-card">
            <h3>Stock Accuracy</h3>
            <div class="metric-value">{{ formatPercent(inventoryAnalytics.stockAccuracy) }}</div>
            <div class="metric-subtitle">{{ inventoryAnalytics.discrepancies }} discrepancies</div>
          </div>
          <div class="overview-card">
            <h3>Carrying Cost</h3>
            <div class="metric-value">{{ formatCurrency(inventoryAnalytics.carryingCost) }}</div>
            <div class="metric-subtitle">{{ formatPercent(inventoryAnalytics.carryingCostPercentage) }} of value</div>
          </div>
        </div>

        <!-- Inventory Charts -->
        <div class="charts-grid">
          <!-- Inventory Levels -->
          <div class="chart-container">
            <h3>Inventory Levels</h3>
            <div class="chart-wrapper">
              <canvas ref="inventoryLevelsChart"></canvas>
            </div>
          </div>

          <!-- Category Performance -->
          <div class="chart-container">
            <h3>Category Performance</h3>
            <div class="chart-wrapper">
              <canvas ref="categoryChart"></canvas>
            </div>
          </div>

          <!-- ABC Analysis -->
          <div class="chart-container">
            <h3>ABC Analysis</h3>
            <div class="chart-wrapper">
              <canvas ref="abcChart"></canvas>
            </div>
          </div>

          <!-- Stock Movement -->
          <div class="chart-container">
            <h3>Stock Movement</h3>
            <div class="chart-wrapper">
              <canvas ref="stockMovementChart"></canvas>
            </div>
          </div>
        </div>

        <!-- Inventory Recommendations -->
        <div class="recommendations-section">
          <h3>Inventory Recommendations</h3>
          <div class="recommendations-grid">
            <div class="recommendation-card" v-for="rec in inventoryAnalytics.recommendations" :key="rec.id">
              <div class="recommendation-priority" :class="rec.priority.toLowerCase()">
                {{ rec.priority }}
              </div>
              <div class="recommendation-title">{{ rec.title }}</div>
              <div class="recommendation-description">{{ rec.description }}</div>
              <div class="recommendation-action">{{ rec.action }}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Customer Analytics -->
      <div v-else-if="selectedAnalyticsType === 'customer'" class="customer-analytics">
        <!-- Customer Overview -->
        <div class="analytics-overview">
          <div class="overview-card">
            <h3>Total Customers</h3>
            <div class="metric-value">{{ formatNumber(customerAnalytics.totalCustomers) }}</div>
            <div class="metric-trend" :class="getTrendClass(customerAnalytics.customerGrowth)">
              {{ formatPercent(customerAnalytics.customerGrowth) }} growth
            </div>
          </div>
          <div class="overview-card">
            <h3>Customer Lifetime Value</h3>
            <div class="metric-value">{{ formatCurrency(customerAnalytics.customerLifetimeValue) }}</div>
            <div class="metric-trend" :class="getTrendClass(customerAnalytics.clvGrowth)">
              {{ formatPercent(customerAnalytics.clvGrowth) }} growth
            </div>
          </div>
          <div class="overview-card">
            <h3>Retention Rate</h3>
            <div class="metric-value">{{ formatPercent(customerAnalytics.retentionRate) }}</div>
            <div class="metric-subtitle">{{ customerAnalytics.churnRate }} churn rate</div>
          </div>
          <div class="overview-card">
            <h3>Customer Satisfaction</h3>
            <div class="metric-value">{{ customerAnalytics.satisfactionScore.toFixed(1) }}/5.0</div>
            <div class="metric-subtitle">{{ customerAnalytics.totalReviews }} reviews</div>
          </div>
        </div>

        <!-- Customer Charts -->
        <div class="charts-grid">
          <!-- Customer Segments -->
          <div class="chart-container">
            <h3>Customer Segments</h3>
            <div class="chart-wrapper">
              <canvas ref="customerSegmentsChart"></canvas>
            </div>
          </div>

          <!-- Cohort Analysis -->
          <div class="chart-container">
            <h3>Cohort Analysis</h3>
            <div class="chart-wrapper">
              <canvas ref="cohortChart"></canvas>
            </div>
          </div>

          <!-- Customer Journey -->
          <div class="chart-container">
            <h3>Customer Journey</h3>
            <div class="chart-wrapper">
              <canvas ref="customerJourneyChart"></canvas>
            </div>
          </div>

          <!-- Loyalty Distribution -->
          <div class="chart-container">
            <h3>Loyalty Distribution</h3>
            <div class="chart-wrapper">
              <canvas ref="loyaltyChart"></canvas>
            </div>
          </div>
        </div>

        <!-- Customer Insights -->
        <div class="insights-section">
          <h3>Customer Insights</h3>
          <div class="insights-grid">
            <div class="insight-card" v-for="insight in customerAnalytics.insights" :key="insight.id">
              <div class="insight-type" :class="insight.type.toLowerCase()">
                {{ insight.type }}
              </div>
              <div class="insight-title">{{ insight.title }}</div>
              <div class="insight-description">{{ insight.description }}</div>
              <div class="insight-impact" :class="insight.impact.toLowerCase()">
                {{ insight.impact }} Impact
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Financial Analytics -->
      <div v-else-if="selectedAnalyticsType === 'financial'" class="financial-analytics">
        <!-- Financial Overview -->
        <div class="analytics-overview">
          <div class="overview-card">
            <h3>Net Revenue</h3>
            <div class="metric-value">{{ formatCurrency(financialAnalytics.netRevenue) }}</div>
            <div class="metric-trend" :class="getTrendClass(financialAnalytics.revenueGrowth)">
              {{ formatPercent(financialAnalytics.revenueGrowth) }} growth
            </div>
          </div>
          <div class="overview-card">
            <h3>Gross Profit</h3>
            <div class="metric-value">{{ formatCurrency(financialAnalytics.grossProfit) }}</div>
            <div class="metric-subtitle">{{ formatPercent(financialAnalytics.grossMargin) }} margin</div>
          </div>
          <div class="overview-card">
            <h3>Operating Expenses</h3>
            <div class="metric-value">{{ formatCurrency(financialAnalytics.operatingExpenses) }}</div>
            <div class="metric-trend" :class="getTrendClass(financialAnalytics.expenseGrowth)">
              {{ formatPercent(financialAnalytics.expenseGrowth) }} growth
            </div>
          </div>
          <div class="overview-card">
            <h3>Net Profit</h3>
            <div class="metric-value">{{ formatCurrency(financialAnalytics.netProfit) }}</div>
            <div class="metric-subtitle">{{ formatPercent(financialAnalytics.netMargin) }} margin</div>
          </div>
        </div>

        <!-- Financial Charts -->
        <div class="charts-grid">
          <!-- Revenue vs Expenses -->
          <div class="chart-container">
            <h3>Revenue vs Expenses</h3>
            <div class="chart-wrapper">
              <canvas ref="revenueVsExpensesChart"></canvas>
            </div>
          </div>

          <!-- Profit Margins -->
          <div class="chart-container">
            <h3>Profit Margins</h3>
            <div class="chart-wrapper">
              <canvas ref="profitMarginsChart"></canvas>
            </div>
          </div>

          <!-- Expense Breakdown -->
          <div class="chart-container">
            <h3>Expense Breakdown</h3>
            <div class="chart-wrapper">
              <canvas ref="expenseBreakdownChart"></canvas>
            </div>
          </div>

          <!-- Cash Flow -->
          <div class="chart-container">
            <h3>Cash Flow</h3>
            <div class="chart-wrapper">
              <canvas ref="cashFlowChart"></canvas>
            </div>
          </div>
        </div>

        <!-- Financial Insights -->
        <div class="insights-section">
          <h3>Financial Insights</h3>
          <div class="insights-grid">
            <div class="insight-card" v-for="insight in financialAnalytics.insights" :key="insight.id">
              <div class="insight-type" :class="insight.type.toLowerCase()">
                {{ insight.type }}
              </div>
              <div class="insight-title">{{ insight.title }}</div>
              <div class="insight-description">{{ insight.description }}</div>
              <div class="insight-impact" :class="insight.impact.toLowerCase()">
                {{ insight.impact }} Impact
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Predictive Analytics -->
      <div v-else-if="selectedAnalyticsType === 'predictive'" class="predictive-analytics">
        <!-- Predictive Overview -->
        <div class="analytics-overview">
          <div class="overview-card">
            <h3>Sales Forecast</h3>
            <div class="metric-value">{{ formatCurrency(predictiveAnalytics.salesForecast) }}</div>
            <div class="metric-subtitle">{{ formatPercent(predictiveAnalytics.forecastAccuracy) }} accuracy</div>
          </div>
          <div class="overview-card">
            <h3>Demand Prediction</h3>
            <div class="metric-value">{{ predictiveAnalytics.demandPrediction.toFixed(0) }}</div>
            <div class="metric-subtitle">units next period</div>
          </div>
          <div class="overview-card">
            <h3>Churn Risk</h3>
            <div class="metric-value">{{ formatPercent(predictiveAnalytics.churnRisk) }}</div>
            <div class="metric-subtitle">{{ predictiveAnalytics.atRiskCustomers }} at risk</div>
          </div>
          <div class="overview-card">
            <h3>Inventory Optimization</h3>
            <div class="metric-value">{{ formatCurrency(predictiveAnalytics.optimizationSavings) }}</div>
            <div class="metric-subtitle">potential savings</div>
          </div>
        </div>

        <!-- Predictive Charts -->
        <div class="charts-grid">
          <!-- Sales Forecast -->
          <div class="chart-container">
            <h3>Sales Forecast</h3>
            <div class="chart-wrapper">
              <canvas ref="salesForecastChart"></canvas>
            </div>
          </div>

          <!-- Demand Prediction -->
          <div class="chart-container">
            <h3>Demand Prediction</h3>
            <div class="chart-wrapper">
              <canvas ref="demandPredictionChart"></canvas>
            </div>
          </div>

          <!-- Churn Prediction -->
          <div class="chart-container">
            <h3>Churn Prediction</h3>
            <div class="chart-wrapper">
              <canvas ref="churnPredictionChart"></canvas>
            </div>
          </div>

          <!-- Optimization Opportunities -->
          <div class="chart-container">
            <h3>Optimization Opportunities</h3>
            <div class="chart-wrapper">
              <canvas ref="optimizationChart"></canvas>
            </div>
          </div>
        </div>

        <!-- Predictive Recommendations -->
        <div class="recommendations-section">
          <h3>Predictive Recommendations</h3>
          <div class="recommendations-grid">
            <div class="recommendation-card" v-for="rec in predictiveAnalytics.recommendations" :key="rec.id">
              <div class="recommendation-confidence">
                Confidence: {{ formatPercent(rec.confidence) }}
              </div>
              <div class="recommendation-title">{{ rec.title }}</div>
              <div class="recommendation-description">{{ rec.description }}</div>
              <div class="recommendation-impact">
                Expected Impact: {{ rec.expectedImpact }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Export Options -->
    <div class="export-section">
      <h3>Export Analytics</h3>
      <div class="export-controls">
        <select v-model="exportFormat">
          <option value="pdf">PDF Report</option>
          <option value="excel">Excel Workbook</option>
          <option value="csv">CSV Data</option>
          <option value="json">JSON Data</option>
        </select>
        <button @click="exportAnalytics" class="export-btn" :disabled="exporting">
          <span v-if="exporting">Exporting...</span>
          <span v-else>Export</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted, onUnmounted } from 'vue'
import { useReportingStore } from '@/stores/reportingStore'
import { Chart, registerables } from 'chart.js'
import { format } from 'date-fns'

export default {
  name: 'AnalyticsVisualization',
  setup() {
    const reportingStore = useReportingStore()
    
    // Reactive state
    const loading = ref(false)
    const exporting = ref(false)
    const selectedAnalyticsType = ref('sales')
    const selectedPeriod = ref('monthly')
    const startDate = ref(format(new Date(Date.now() - 30 * 24 * 60 * 60 * 1000), 'yyyy-MM-dd'))
    const endDate = ref(format(new Date(), 'yyyy-MM-dd'))
    const exportFormat = ref('pdf')
    
    // Chart controls
    const revenueChartType = ref('line')
    const productMetric = ref('revenue')
    
    // Analytics data
    const salesAnalytics = reactive({})
    const inventoryAnalytics = reactive({})
    const customerAnalytics = reactive({})
    const financialAnalytics = reactive({})
    const predictiveAnalytics = reactive({})
    
    // Chart instances
    const charts = reactive({})
    
    // Methods
    const updateAnalytics = async () => {
      loading.value = true
      try {
        switch (selectedAnalyticsType.value) {
          case 'sales':
            await loadSalesAnalytics()
            break
          case 'inventory':
            await loadInventoryAnalytics()
            break
          case 'customer':
            await loadCustomerAnalytics()
            break
          case 'financial':
            await loadFinancialAnalytics()
            break
          case 'predictive':
            await loadPredictiveAnalytics()
            break
        }
      } catch (error) {
        console.error('Error loading analytics:', error)
      } finally {
        loading.value = false
      }
    }

    const loadSalesAnalytics = async () => {
      try {
        const response = await reportingStore.getSalesAnalytics({
          startDate: new Date(startDate.value),
          endDate: new Date(endDate.value),
          period: selectedPeriod.value.toUpperCase()
        })
        Object.assign(salesAnalytics, response)
        await updateSalesCharts()
      } catch (error) {
        console.error('Error loading sales analytics:', error)
      }
    }

    const loadInventoryAnalytics = async () => {
      try {
        const response = await reportingStore.getInventoryAnalytics()
        Object.assign(inventoryAnalytics, response)
        await updateInventoryCharts()
      } catch (error) {
        console.error('Error loading inventory analytics:', error)
      }
    }

    const loadCustomerAnalytics = async () => {
      try {
        const response = await reportingStore.getCustomerAnalytics({
          startDate: new Date(startDate.value),
          endDate: new Date(endDate.value)
        })
        Object.assign(customerAnalytics, response)
        await updateCustomerCharts()
      } catch (error) {
        console.error('Error loading customer analytics:', error)
      }
    }

    const loadFinancialAnalytics = async () => {
      try {
        const response = await reportingStore.getFinancialAnalytics({
          startDate: new Date(startDate.value),
          endDate: new Date(endDate.value),
          period: selectedPeriod.value.toUpperCase()
        })
        Object.assign(financialAnalytics, response)
        await updateFinancialCharts()
      } catch (error) {
        console.error('Error loading financial analytics:', error)
      }
    }

    const loadPredictiveAnalytics = async () => {
      try {
        const response = await reportingStore.getPredictiveAnalytics()
        Object.assign(predictiveAnalytics, response)
        await updatePredictiveCharts()
      } catch (error) {
        console.error('Error loading predictive analytics:', error)
      }
    }

    const updateSalesCharts = async () => {
      // Revenue Trend Chart
      const revenueCtx = document.querySelector('[ref="revenueChart"]')?.getContext('2d')
      if (revenueCtx && salesAnalytics.periodData) {
        if (charts.revenue) charts.revenue.destroy()
        
        charts.revenue = new Chart(revenueCtx, {
          type: revenueChartType.value === 'area' ? 'line' : revenueChartType.value,
          data: {
            labels: salesAnalytics.periodData.map(p => p.period),
            datasets: [{
              label: 'Revenue',
              data: salesAnalytics.periodData.map(p => p.value),
              borderColor: '#3b82f6',
              backgroundColor: revenueChartType.value === 'area' ? 'rgba(59, 130, 246, 0.2)' : '#3b82f6',
              fill: revenueChartType.value === 'area',
              tension: 0.4
            }]
          },
          options: {
            responsive: true,
            plugins: {
              legend: { display: false }
            },
            scales: {
              y: {
                beginAtZero: true,
                ticks: { callback: (value) => formatCurrency(value) }
              }
            }
          }
        })
      }

      // Product Performance Chart
      const productCtx = document.querySelector('[ref="productChart"]')?.getContext('2d')
      if (productCtx && salesAnalytics.topProducts) {
        if (charts.product) charts.product.destroy()
        
        const topProducts = salesAnalytics.topProducts.slice(0, 10)
        charts.product = new Chart(productCtx, {
          type: 'bar',
          data: {
            labels: topProducts.map(p => p.productName),
            datasets: [{
              label: productMetric.value === 'revenue' ? 'Revenue' : 
                     productMetric.value === 'quantity' ? 'Quantity' : 'Profit',
              data: topProducts.map(p => 
                productMetric.value === 'revenue' ? p.totalRevenue :
                productMetric.value === 'quantity' ? p.totalQuantity : p.totalProfit
              ),
              backgroundColor: '#10b981'
            }]
          },
          options: {
            responsive: true,
            plugins: {
              legend: { display: false }
            },
            scales: {
              y: { beginAtZero: true }
            }
          }
        })
      }

      // Location Performance Chart
      const locationCtx = document.querySelector('[ref="locationChart"]')?.getContext('2d')
      if (locationCtx && salesAnalytics.locationPerformance) {
        if (charts.location) charts.location.destroy()
        
        charts.location = new Chart(locationCtx, {
          type: 'doughnut',
          data: {
            labels: salesAnalytics.locationPerformance.map(l => l.locationName),
            datasets: [{
              data: salesAnalytics.locationPerformance.map(l => l.totalRevenue),
              backgroundColor: ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6']
            }]
          },
          options: {
            responsive: true,
            plugins: {
              legend: { position: 'right' }
            }
          }
        })
      }

      // Sales Funnel Chart
      const funnelCtx = document.querySelector('[ref="funnelChart"]')?.getContext('2d')
      if (funnelCtx && salesAnalytics.salesFunnel) {
        if (charts.funnel) charts.funnel.destroy()
        
        charts.funnel = new Chart(funnelCtx, {
          type: 'bar',
          data: {
            labels: salesAnalytics.salesFunnel.map(f => f.stage),
            datasets: [{
              label: 'Count',
              data: salesAnalytics.salesFunnel.map(f => f.count),
              backgroundColor: ['#3b82f6', '#10b981', '#f59e0b', '#ef4444']
            }]
          },
          options: {
            responsive: true,
            plugins: {
              legend: { display: false }
            },
            scales: {
              y: { beginAtZero: true }
            }
          }
        })
      }
    }

    const updateInventoryCharts = async () => {
      // Similar implementation for inventory charts
      // Implementation details omitted for brevity
    }

    const updateCustomerCharts = async () => {
      // Similar implementation for customer charts
      // Implementation details omitted for brevity
    }

    const updateFinancialCharts = async () => {
      // Similar implementation for financial charts
      // Implementation details omitted for brevity
    }

    const updatePredictiveCharts = async () => {
      // Similar implementation for predictive charts
      // Implementation details omitted for brevity
    }

    const refreshData = async () => {
      await updateAnalytics()
    }

    const exportAnalytics = async () => {
      exporting.value = true
      try {
        await reportingStore.exportAnalytics({
          type: selectedAnalyticsType.value,
          format: exportFormat.value,
          startDate: startDate.value,
          endDate: endDate.value,
          period: selectedPeriod.value
        })
      } catch (error) {
        console.error('Error exporting analytics:', error)
      } finally {
        exporting.value = false
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

    const getTrendClass = (value) => {
      return value > 0 ? 'positive' : value < 0 ? 'negative' : 'neutral'
    }

    // Lifecycle hooks
    onMounted(async () => {
      Chart.register(...registerables)
      await updateAnalytics()
    })

    onUnmounted(() => {
      // Destroy all charts
      Object.values(charts).forEach(chart => {
        if (chart) chart.destroy()
      })
    })

    return {
      // State
      loading,
      exporting,
      selectedAnalyticsType,
      selectedPeriod,
      startDate,
      endDate,
      exportFormat,
      revenueChartType,
      productMetric,
      
      // Analytics data
      salesAnalytics,
      inventoryAnalytics,
      customerAnalytics,
      financialAnalytics,
      predictiveAnalytics,
      
      // Methods
      updateAnalytics,
      refreshData,
      exportAnalytics,
      updateSalesCharts,
      updateInventoryCharts,
      updateCustomerCharts,
      updateFinancialCharts,
      updatePredictiveCharts,
      
      // Utility
      formatCurrency,
      formatNumber,
      formatPercent,
      getTrendClass
    }
  }
}
</script>

<style scoped>
.analytics-visualization {
  padding: 24px;
  background-color: #f8fafc;
  min-height: 100vh;
}

.analytics-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 32px;
}

.analytics-header h1 {
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

.analytics-overview {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 24px;
  margin-bottom: 32px;
}

.overview-card {
  background-color: white;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  text-align: center;
}

.overview-card h3 {
  font-size: 0.875rem;
  font-weight: 500;
  color: #64748b;
  margin-bottom: 8px;
}

.metric-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 4px;
}

.metric-trend {
  font-size: 0.75rem;
  font-weight: 500;
}

.metric-trend.positive {
  color: #10b981;
}

.metric-trend.negative {
  color: #ef4444;
}

.metric-trend.neutral {
  color: #64748b;
}

.metric-subtitle {
  font-size: 0.75rem;
  color: #64748b;
}

.charts-grid {
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

.chart-container h3 {
  font-size: 1.125rem;
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

.insights-section,
.recommendations-section {
  background-color: white;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  margin-bottom: 32px;
}

.insights-section h3,
.recommendations-section h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 16px;
}

.insights-grid,
.recommendations-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 16px;
}

.insight-card,
.recommendation-card {
  padding: 16px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
}

.insight-type,
.recommendation-priority,
.recommendation-confidence {
  font-size: 0.75rem;
  font-weight: 500;
  padding: 4px 8px;
  border-radius: 4px;
  display: inline-block;
  margin-bottom: 8px;
}

.insight-type.opportunity,
.recommendation-priority.high {
  background-color: #d1fae5;
  color: #065f46;
}

.insight-type.risk,
.recommendation-priority.low {
  background-color: #fee2e2;
  color: #991b1b;
}

.insight-type.trend,
.recommendation-priority.medium {
  background-color: #fef3c7;
  color: #92400e;
}

.recommendation-confidence {
  background-color: #dbeafe;
  color: #1e40af;
}

.insight-title,
.recommendation-title {
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 4px;
}

.insight-description,
.recommendation-description {
  font-size: 0.875rem;
  color: #64748b;
  margin-bottom: 8px;
}

.insight-impact,
.recommendation-action,
.recommendation-impact {
  font-size: 0.75rem;
  font-weight: 500;
}

.insight-impact.high,
.recommendation-action.urgent {
  color: #ef4444;
}

.insight-impact.medium,
.recommendation-action.recommended {
  color: #f59e0b;
}

.insight-impact.low,
.recommendation-action.optional {
  color: #10b981;
}

.export-section {
  background-color: white;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.export-section h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 16px;
}

.export-controls {
  display: flex;
  gap: 16px;
  align-items: center;
}

.export-controls select {
  padding: 8px 12px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  background-color: white;
}

.export-btn {
  padding: 8px 16px;
  background-color: #8b5cf6;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
}

.export-btn:disabled {
  background-color: #9ca3af;
  cursor: not-allowed;
}

@media (max-width: 768px) {
  .analytics-header {
    flex-direction: column;
    gap: 16px;
    align-items: stretch;
  }

  .header-controls {
    flex-direction: column;
    align-items: stretch;
  }

  .analytics-overview {
    grid-template-columns: 1fr;
  }

  .charts-grid {
    grid-template-columns: 1fr;
  }

  .insights-grid,
  .recommendations-grid {
    grid-template-columns: 1fr;
  }

  .export-controls {
    flex-direction: column;
    align-items: stretch;
  }
}
</style>

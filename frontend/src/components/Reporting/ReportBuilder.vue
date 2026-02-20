<template>
  <div class="report-builder">
    <div class="header">
      <h1>Custom Report Builder</h1>
      <div class="header-actions">
        <button class="btn secondary" @click="reset" :disabled="loading">
          Reset
        </button>
        <button class="btn primary" @click="runReport" :disabled="loading || !isValid">
          <span v-if="loading">Running...</span>
          <span v-else>Run Report</span>
        </button>
      </div>
    </div>

    <div class="builder-grid">
      <div class="panel">
        <h2>Definition</h2>

        <div class="field">
          <label>Report Name</label>
          <input v-model.trim="definition.name" type="text" placeholder="e.g. Monthly Sales by Category" />
        </div>

        <div class="field">
          <label>Dataset</label>
          <select v-model="definition.dataset">
            <option value="sales">Sales</option>
            <option value="inventory">Inventory</option>
            <option value="customers">Customers</option>
            <option value="financial">Financial</option>
            <option value="procurement">Procurement</option>
          </select>
        </div>

        <div class="field">
          <label>Date Range</label>
          <div class="row">
            <input v-model="definition.startDate" type="date" :max="definition.endDate || undefined" />
            <span class="sep">to</span>
            <input v-model="definition.endDate" type="date" :min="definition.startDate || undefined" />
          </div>
        </div>

        <div class="field">
          <label>Filters</label>
          <div class="filters">
            <div class="filter" v-for="(f, idx) in definition.filters" :key="idx">
              <select v-model="f.field">
                <option value="locationId">Location</option>
                <option value="category">Category</option>
                <option value="brand">Brand</option>
                <option value="paymentMethod">Payment Method</option>
                <option value="customerSegment">Customer Segment</option>
                <option value="status">Status</option>
              </select>
              <select v-model="f.op">
                <option value="eq">=</option>
                <option value="neq">!=</option>
                <option value="contains">contains</option>
                <option value="gt">&gt;</option>
                <option value="gte">&gt;=</option>
                <option value="lt">&lt;</option>
                <option value="lte">&lt;=</option>
              </select>
              <input v-model="f.value" type="text" placeholder="Value" />
              <button class="icon-btn danger" @click="removeFilter(idx)" title="Remove">×</button>
            </div>

            <button class="btn tertiary" @click="addFilter">Add filter</button>
          </div>
        </div>

        <div class="field">
          <label>Group By</label>
          <div class="chips">
            <button
              v-for="g in groupByOptions"
              :key="g.value"
              class="chip"
              :class="{ active: definition.groupBy.includes(g.value) }"
              @click="toggleGroupBy(g.value)"
              type="button"
            >
              {{ g.label }}
            </button>
          </div>
        </div>

        <div class="field">
          <label>Metrics</label>
          <div class="chips">
            <button
              v-for="m in metricOptions"
              :key="m.value"
              class="chip"
              :class="{ active: definition.metrics.includes(m.value) }"
              @click="toggleMetric(m.value)"
              type="button"
            >
              {{ m.label }}
            </button>
          </div>
        </div>

        <div class="field">
          <label>Format</label>
          <select v-model="definition.format">
            <option value="json">JSON</option>
            <option value="csv">CSV</option>
            <option value="excel">Excel</option>
            <option value="pdf">PDF</option>
          </select>
        </div>
      </div>

      <div class="panel">
        <h2>Preview</h2>

        <div v-if="error" class="alert error">
          {{ error }}
        </div>

        <div v-if="!result && !error" class="empty">
          Configure the report and click <b>Run Report</b> to see results.
        </div>

        <div v-if="result" class="result">
          <div class="result-actions">
            <button class="btn secondary" @click="download">Download</button>
            <button class="btn secondary" @click="copyJson" v-if="definition.format === 'json'">Copy JSON</button>
          </div>

          <pre class="json" v-if="definition.format === 'json'">{{ prettyJson(result) }}</pre>

          <div v-else class="note">
            Preview is shown as JSON, but download will use the selected format.
            <pre class="json">{{ prettyJson(result) }}</pre>
          </div>
        </div>

        <div class="validation" v-if="!isValid">
          <div class="hint">Missing required fields:</div>
          <ul>
            <li v-if="!definition.name">Report Name</li>
            <li v-if="!definition.startDate || !definition.endDate">Start/End Date</li>
            <li v-if="definition.metrics.length === 0">At least one metric</li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { computed, reactive, ref } from 'vue'
import { useReportingStore } from '@/stores/reportingStore'

export default {
  name: 'ReportBuilder',
  setup() {
    const reportingStore = useReportingStore()

    const loading = ref(false)
    const error = ref('')
    const result = ref(null)

    const groupByOptions = [
      { value: 'date', label: 'Date' },
      { value: 'location', label: 'Location' },
      { value: 'category', label: 'Category' },
      { value: 'product', label: 'Product' },
      { value: 'customer', label: 'Customer' }
    ]

    const metricOptions = [
      { value: 'revenue', label: 'Revenue' },
      { value: 'profit', label: 'Profit' },
      { value: 'quantity', label: 'Quantity' },
      { value: 'transactions', label: 'Transactions' },
      { value: 'inventoryValue', label: 'Inventory Value' },
      { value: 'lowStockCount', label: 'Low Stock Count' }
    ]

    const definition = reactive({
      name: '',
      dataset: 'sales',
      startDate: '',
      endDate: '',
      filters: [],
      groupBy: ['date'],
      metrics: ['revenue'],
      format: 'json'
    })

    const isValid = computed(() => {
      return Boolean(definition.name) &&
        Boolean(definition.startDate) &&
        Boolean(definition.endDate) &&
        definition.metrics.length > 0
    })

    const addFilter = () => {
      definition.filters.push({ field: 'locationId', op: 'eq', value: '' })
    }

    const removeFilter = (idx) => {
      definition.filters.splice(idx, 1)
    }

    const toggleGroupBy = (value) => {
      const idx = definition.groupBy.indexOf(value)
      if (idx >= 0) definition.groupBy.splice(idx, 1)
      else definition.groupBy.push(value)
    }

    const toggleMetric = (value) => {
      const idx = definition.metrics.indexOf(value)
      if (idx >= 0) definition.metrics.splice(idx, 1)
      else definition.metrics.push(value)
    }

    const reset = () => {
      definition.name = ''
      definition.dataset = 'sales'
      definition.startDate = ''
      definition.endDate = ''
      definition.filters = []
      definition.groupBy = ['date']
      definition.metrics = ['revenue']
      definition.format = 'json'
      error.value = ''
      result.value = null
    }

    const runReport = async () => {
      if (!isValid.value) return

      loading.value = true
      error.value = ''

      try {
        // Uses the Pinia store method that will be implemented in T146.
        // For now, this keeps the component wired to the expected app architecture.
        const payload = {
          name: definition.name,
          dataset: definition.dataset,
          startDate: definition.startDate,
          endDate: definition.endDate,
          filters: definition.filters,
          groupBy: definition.groupBy,
          metrics: definition.metrics,
          format: definition.format
        }

        const response = await reportingStore.runCustomReport(payload)
        result.value = response
      } catch (e) {
        error.value = e?.message || 'Failed to run report.'
      } finally {
        loading.value = false
      }
    }

    const download = async () => {
      try {
        const payload = {
          name: definition.name,
          dataset: definition.dataset,
          startDate: definition.startDate,
          endDate: definition.endDate,
          filters: definition.filters,
          groupBy: definition.groupBy,
          metrics: definition.metrics,
          format: definition.format
        }

        await reportingStore.exportCustomReport(payload)
      } catch (e) {
        error.value = e?.message || 'Failed to download report.'
      }
    }

    const prettyJson = (obj) => {
      return JSON.stringify(obj, null, 2)
    }

    const copyJson = async () => {
      if (!result.value) return
      try {
        await navigator.clipboard.writeText(prettyJson(result.value))
      } catch {
        // ignore
      }
    }

    return {
      definition,
      loading,
      error,
      result,
      groupByOptions,
      metricOptions,
      isValid,
      addFilter,
      removeFilter,
      toggleGroupBy,
      toggleMetric,
      reset,
      runReport,
      download,
      prettyJson,
      copyJson
    }
  }
}
</script>

<style scoped>
.report-builder {
  padding: 24px;
  background-color: #f8fafc;
  min-height: 100vh;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 16px;
  margin-bottom: 24px;
}

.header h1 {
  font-size: 2rem;
  font-weight: 700;
  color: #1e293b;
}

.header-actions {
  display: flex;
  gap: 12px;
}

.builder-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 24px;
}

.panel {
  background: #ffffff;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.panel h2 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 16px;
}

.field {
  margin-bottom: 14px;
}

.field label {
  display: block;
  font-weight: 500;
  color: #475569;
  margin-bottom: 6px;
}

.field input,
.field select {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  background: #fff;
}

.row {
  display: flex;
  gap: 10px;
  align-items: center;
}

.sep {
  color: #64748b;
  font-weight: 500;
}

.filters {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.filter {
  display: grid;
  grid-template-columns: 1fr 90px 1fr 36px;
  gap: 8px;
  align-items: center;
}

.chips {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.chip {
  border: 1px solid #cbd5e1;
  background: #fff;
  padding: 8px 10px;
  border-radius: 999px;
  cursor: pointer;
  font-size: 0.875rem;
  color: #334155;
}

.chip.active {
  border-color: #3b82f6;
  background: rgba(59, 130, 246, 0.1);
  color: #1e40af;
}

.btn {
  border: 1px solid transparent;
  border-radius: 8px;
  padding: 10px 14px;
  font-weight: 600;
  cursor: pointer;
}

.btn.primary {
  background: #3b82f6;
  color: #fff;
}

.btn.secondary {
  background: #ffffff;
  border-color: #cbd5e1;
  color: #334155;
}

.btn.tertiary {
  background: #f1f5f9;
  border-color: #e2e8f0;
  color: #334155;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.icon-btn {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
  background: #fff;
  cursor: pointer;
}

.icon-btn.danger {
  color: #991b1b;
  border-color: #fecaca;
  background: #fff5f5;
}

.alert {
  padding: 12px 14px;
  border-radius: 10px;
  margin-bottom: 12px;
}

.alert.error {
  background: #fee2e2;
  border: 1px solid #fecaca;
  color: #991b1b;
}

.empty {
  color: #64748b;
  font-size: 0.95rem;
}

.result-actions {
  display: flex;
  gap: 10px;
  margin-bottom: 10px;
}

.json {
  white-space: pre-wrap;
  word-break: break-word;
  background: #0b1220;
  color: #e2e8f0;
  border-radius: 10px;
  padding: 14px;
  font-size: 0.85rem;
  overflow: auto;
}

.validation {
  margin-top: 14px;
  color: #b45309;
}

.validation .hint {
  font-weight: 600;
  margin-bottom: 6px;
}

@media (max-width: 900px) {
  .builder-grid {
    grid-template-columns: 1fr;
  }

  .header {
    flex-direction: column;
    align-items: stretch;
  }

  .header-actions {
    justify-content: flex-end;
  }

  .filter {
    grid-template-columns: 1fr;
  }
}
</style>

import { log } from './logger'

// Performance metrics interface
export interface PerformanceMetrics {
  operation: string
  startTime: number
  endTime: number
  duration: number
  memoryUsage?: number
  networkRequests?: number
  cacheHits?: number
  cacheMisses?: number
  errorCount?: number
  successRate?: number
  metadata?: Record<string, any>
}

// Performance threshold
export interface PerformanceThreshold {
  operation: string
  warningThreshold: number // in milliseconds
  criticalThreshold: number // in milliseconds
  enabled: boolean
}

// Performance configuration
export interface PerformanceConfig {
  enableMonitoring: boolean
  enableMemoryTracking: boolean
  enableNetworkTracking: boolean
  enableCacheTracking: boolean
  enableAutoOptimization: boolean
  maxMetricsHistory: number
  thresholds: PerformanceThreshold[]
  reportingInterval: number // in milliseconds
}

// Default performance configuration
const DEFAULT_CONFIG: PerformanceConfig = {
  enableMonitoring: true,
  enableMemoryTracking: true,
  enableNetworkTracking: true,
  enableCacheTracking: true,
  enableAutoOptimization: false,
  maxMetricsHistory: 1000,
  thresholds: [
    { operation: 'api_call', warningThreshold: 1000, criticalThreshold: 3000, enabled: true },
    { operation: 'render_component', warningThreshold: 100, criticalThreshold: 300, enabled: true },
    { operation: 'data_fetch', warningThreshold: 2000, criticalThreshold: 5000, enabled: true },
    { operation: 'search', warningThreshold: 500, criticalThreshold: 1500, enabled: true },
    { operation: 'bulk_operation', warningThreshold: 5000, criticalThreshold: 10000, enabled: true }
  ],
  reportingInterval: 30000 // 30 seconds
}

// Performance monitor class
class PerformanceMonitor {
  private config: PerformanceConfig
  private metrics: PerformanceMetrics[] = []
  private activeOperations: Map<string, PerformanceMetrics> = new Map()
  private networkRequestCount = 0
  private cacheHits = 0
  private cacheMisses = 0
  private errorCount = 0
  private reportingTimer?: number

  constructor(config: Partial<PerformanceConfig> = {}) {
    this.config = { ...DEFAULT_CONFIG, ...config }
    this.setupPerformanceObservers()
    this.startReporting()
  }

  // Setup performance observers
  private setupPerformanceObservers(): void {
    if (!this.config.enableMonitoring) return

    // Observer for navigation timing
    if ('performance' in window && 'getEntriesByType' in performance) {
      this.observeNavigationTiming()
    }

    // Observer for resource timing
    if ('PerformanceObserver' in window) {
      this.observeResourceTiming()
    }

    // Observer for long tasks
    if ('PerformanceObserver' in window) {
      this.observeLongTasks()
    }
  }

  // Observe navigation timing
  private observeNavigationTiming(): void {
    const navigationEntries = performance.getEntriesByType('navigation')
    navigationEntries.forEach(entry => {
      const navEntry = entry as PerformanceNavigationTiming
      this.recordMetric('page_load', {
        startTime: navEntry.startTime,
        endTime: navEntry.loadEventEnd,
        duration: navEntry.loadEventEnd - navEntry.startTime,
        metadata: {
          domContentLoaded: navEntry.domContentLoadedEventEnd - navEntry.domContentLoadedEventStart,
          firstPaint: this.getFirstPaintTime(),
          firstContentfulPaint: this.getFirstContentfulPaintTime()
        }
      })
    })
  }

  // Observe resource timing
  private observeResourceTiming(): void {
    try {
      const observer = new PerformanceObserver((list) => {
        list.getEntries().forEach((entry) => {
          if (entry.entryType === 'resource') {
            const resource = entry as PerformanceResourceTiming
            this.networkRequestCount++
            
            this.recordMetric('resource_load', {
              startTime: resource.startTime,
              endTime: resource.responseEnd,
              duration: resource.responseEnd - resource.startTime,
              metadata: {
                name: resource.name,
                type: this.getResourceType(resource.name),
                size: resource.transferSize,
                cached: resource.transferSize === 0 && resource.decodedBodySize > 0
              }
            })
          }
        })
      })
      
      observer.observe({ entryTypes: ['resource'] })
    } catch (error) {
      log.warn('performance', 'Failed to setup resource observer', { error })
    }
  }

  // Observe long tasks
  private observeLongTasks(): void {
    try {
      const observer = new PerformanceObserver((list) => {
        list.getEntries().forEach((entry) => {
          if (entry.entryType === 'longtask') {
            this.recordMetric('long_task', {
              startTime: entry.startTime,
              endTime: entry.startTime + entry.duration,
              duration: entry.duration,
              metadata: {
                attribution: (entry as any).attribution || []
              }
            })
          }
        })
      })
      
      observer.observe({ entryTypes: ['longtask'] })
    } catch (error) {
      log.warn('performance', 'Failed to setup long task observer', { error })
    }
  }

  // Get first paint time
  private getFirstPaintTime(): number {
    const paintEntries = performance.getEntriesByType('paint')
    const firstPaint = paintEntries.find(entry => entry.name === 'first-paint')
    return firstPaint?.startTime || 0
  }

  // Get first contentful paint time
  private getFirstContentfulPaintTime(): number {
    const paintEntries = performance.getEntriesByType('paint')
    const firstContentfulPaint = paintEntries.find(entry => entry.name === 'first-contentful-paint')
    return firstContentfulPaint?.startTime || 0
  }

  // Get resource type
  private getResourceType(url: string): string {
    if (url.includes('.js')) return 'script'
    if (url.includes('.css')) return 'stylesheet'
    if (url.match(/\.(jpg|jpeg|png|gif|webp|svg)$/i)) return 'image'
    if (url.includes('/api/')) return 'api'
    return 'other'
  }

  // Start monitoring an operation
  startOperation(operation: string, metadata?: Record<string, any>): string {
    if (!this.config.enableMonitoring) return ''

    const operationId = `${operation}_${Date.now()}_${Math.random().toString(36).substr(2, 6)}`
    const metric: PerformanceMetrics = {
      operation,
      startTime: performance.now(),
      endTime: 0,
      duration: 0,
      metadata
    }

    this.activeOperations.set(operationId, metric)
    
    log.debug('performance', `Started monitoring: ${operation}`, { operationId, metadata })
    
    return operationId
  }

  // End monitoring an operation
  endOperation(operationId: string, success: boolean = true, additionalMetadata?: Record<string, any>): PerformanceMetrics | null {
    if (!this.config.enableMonitoring) return null

    const metric = this.activeOperations.get(operationId)
    if (!metric) {
      log.warn('performance', `No active operation found for ID: ${operationId}`)
      return null
    }

    metric.endTime = performance.now()
    metric.duration = metric.endTime - metric.startTime

    if (this.config.enableMemoryTracking) {
      metric.memoryUsage = this.getMemoryUsage()
    }

    if (this.config.enableNetworkTracking) {
      metric.networkRequests = this.networkRequestCount
    }

    if (this.config.enableCacheTracking) {
      metric.cacheHits = this.cacheHits
      metric.cacheMisses = this.cacheMisses
    }

    if (!success) {
      this.errorCount++
      metric.errorCount = this.errorCount
    }

    if (additionalMetadata) {
      metric.metadata = { ...metric.metadata, ...additionalMetadata }
    }

    // Move to completed metrics
    this.metrics.push(metric)
    this.activeOperations.delete(operationId)

    // Check thresholds
    this.checkThresholds(metric)

    // Trim history if needed
    if (this.metrics.length > this.config.maxMetricsHistory) {
      this.metrics = this.metrics.slice(-this.config.maxMetricsHistory)
    }

    log.debug('performance', `Completed monitoring: ${metric.operation}`, {
      operationId,
      duration: metric.duration,
      success
    })

    return metric
  }

  // Record a metric directly
  recordMetric(operation: string, metric: Partial<PerformanceMetrics>): void {
    if (!this.config.enableMonitoring) return

    const fullMetric: PerformanceMetrics = {
      operation,
      startTime: metric.startTime || 0,
      endTime: metric.endTime || 0,
      duration: metric.duration || 0,
      memoryUsage: metric.memoryUsage,
      networkRequests: metric.networkRequests,
      cacheHits: metric.cacheHits,
      cacheMisses: metric.cacheMisses,
      errorCount: metric.errorCount,
      metadata: metric.metadata
    }

    this.metrics.push(fullMetric)
    this.checkThresholds(fullMetric)
  }

  // Get memory usage
  private getMemoryUsage(): number {
    if ('memory' in performance && (performance as any).memory) {
      return (performance as any).memory.usedJSHeapSize
    }
    return 0
  }

  // Check performance thresholds
  private checkThresholds(metric: PerformanceMetrics): void {
    const threshold = this.config.thresholds.find(t => t.operation === metric.operation)
    if (!threshold || !threshold.enabled) return

    if (metric.duration > threshold.criticalThreshold) {
      log.error('performance', `Critical performance threshold exceeded`, {
        operation: metric.operation,
        duration: metric.duration,
        threshold: threshold.criticalThreshold
      })
    } else if (metric.duration > threshold.warningThreshold) {
      log.warn('performance', `Performance threshold exceeded`, {
        operation: metric.operation,
        duration: metric.duration,
        threshold: threshold.warningThreshold
      })
    }
  }

  // Increment cache hit
  incrementCacheHit(): void {
    if (this.config.enableCacheTracking) {
      this.cacheHits++
    }
  }

  // Increment cache miss
  incrementCacheMiss(): void {
    if (this.config.enableCacheTracking) {
      this.cacheMisses++
    }
  }

  // Increment network request count
  incrementNetworkRequest(): void {
    if (this.config.enableNetworkTracking) {
      this.networkRequestCount++
    }
  }

  // Increment error count
  incrementError(): void {
    this.errorCount++
  }

  // Get performance statistics
  getStatistics(operation?: string): {
    totalOperations: number
    averageDuration: number
    minDuration: number
    maxDuration: number
    p95Duration: number
    p99Duration: number
    errorRate: number
    cacheHitRate: number
    memoryUsage: number
    networkRequests: number
  } {
    const filteredMetrics = operation 
      ? this.metrics.filter(m => m.operation === operation)
      : this.metrics

    if (filteredMetrics.length === 0) {
      return {
        totalOperations: 0,
        averageDuration: 0,
        minDuration: 0,
        maxDuration: 0,
        p95Duration: 0,
        p99Duration: 0,
        errorRate: 0,
        cacheHitRate: 0,
        memoryUsage: 0,
        networkRequests: 0
      }
    }

    const durations = filteredMetrics.map(m => m.duration).sort((a, b) => a - b)
    const totalErrors = filteredMetrics.reduce((sum, m) => sum + (m.errorCount || 0), 0)
    const totalCacheHits = filteredMetrics.reduce((sum, m) => sum + (m.cacheHits || 0), 0)
    const totalCacheMisses = filteredMetrics.reduce((sum, m) => sum + (m.cacheMisses || 0), 0)
    const totalCacheRequests = totalCacheHits + totalCacheMisses

    return {
      totalOperations: filteredMetrics.length,
      averageDuration: durations.reduce((sum, d) => sum + d, 0) / durations.length,
      minDuration: durations[0] || 0,
      maxDuration: durations[durations.length - 1] || 0,
      p95Duration: durations[Math.floor(durations.length * 0.95)] || 0,
      p99Duration: durations[Math.floor(durations.length * 0.99)] || 0,
      errorRate: totalErrors / filteredMetrics.length,
      cacheHitRate: totalCacheRequests > 0 ? totalCacheHits / totalCacheRequests : 0,
      memoryUsage: filteredMetrics.reduce((sum, m) => sum + (m.memoryUsage || 0), 0) / filteredMetrics.length,
      networkRequests: filteredMetrics.reduce((sum, m) => sum + (m.networkRequests || 0), 0)
    }
  }

  // Get performance report
  getReport(): {
    summary: any
    operations: Record<string, any>
    recommendations: string[]
  } {
    const summary = this.getStatistics()
    const operations: Record<string, any> = {}

    // Group metrics by operation
    const operationGroups = this.metrics.reduce((groups, metric) => {
      if (!groups[metric.operation]) {
        groups[metric.operation] = []
      }
      groups[metric.operation]?.push(metric)
      return groups
    }, {} as Record<string, PerformanceMetrics[]>)

    // Calculate statistics for each operation
    Object.keys(operationGroups).forEach(operation => {
      operations[operation] = this.getStatistics(operation)
    })

    // Generate recommendations
    const recommendations = this.generateRecommendations(summary, operations)

    return {
      summary,
      operations,
      recommendations
    }
  }

  // Generate performance recommendations
  private generateRecommendations(summary: any, operations: Record<string, any>): string[] {
    const recommendations: string[] = []

    // Overall performance recommendations
    if (summary.averageDuration > 1000) {
      recommendations.push('Consider optimizing overall application performance - average duration is high')
    }

    if (summary.errorRate > 0.05) {
      recommendations.push('High error rate detected - review error handling and retry logic')
    }

    if (summary.cacheHitRate < 0.8) {
      recommendations.push('Low cache hit rate - consider optimizing caching strategy')
    }

    // Operation-specific recommendations
    Object.keys(operations).forEach(operation => {
      const stats = operations[operation]
      
      if (stats.averageDuration > 2000) {
        recommendations.push(`${operation} is slow - consider optimization or caching`)
      }

      if (stats.errorRate > 0.1) {
        recommendations.push(`${operation} has high error rate - review implementation`)
      }
    })

    return recommendations
  }

  // Start periodic reporting
  private startReporting(): void {
    if (this.config.reportingInterval > 0) {
      this.reportingTimer = setInterval(() => {
        const report = this.getReport()
        log.info('performance', 'Performance report', report)
      }, this.config.reportingInterval)
    }
  }

  // Stop monitoring
  stop(): void {
    if (this.reportingTimer) {
      clearInterval(this.reportingTimer)
    }
    log.info('performance', 'Performance monitoring stopped')
  }

  // Clear metrics
  clearMetrics(): void {
    this.metrics = []
    this.activeOperations.clear()
    this.networkRequestCount = 0
    this.cacheHits = 0
    this.cacheMisses = 0
    this.errorCount = 0
    log.info('performance', 'Performance metrics cleared')
  }

  // Export metrics
  exportMetrics(format: 'json' | 'csv' = 'json'): string {
    if (format === 'json') {
      return JSON.stringify(this.metrics, null, 2)
    }

    if (format === 'csv') {
      const headers = ['Operation', 'Start Time', 'End Time', 'Duration', 'Memory Usage', 'Network Requests', 'Cache Hits', 'Cache Misses', 'Error Count']
      const csvRows = this.metrics.map(metric => [
        metric.operation,
        metric.startTime,
        metric.endTime,
        metric.duration,
        metric.memoryUsage || 0,
        metric.networkRequests || 0,
        metric.cacheHits || 0,
        metric.cacheMisses || 0,
        metric.errorCount || 0
      ])

      return [headers.join(','), ...csvRows.map(row => row.join(','))].join('\n')
    }

    return ''
  }
}

// Create singleton instance
export const performanceMonitor = new PerformanceMonitor()

// Convenience exports
export const perf = {
  start: (operation: string, metadata?: Record<string, any>) => 
    performanceMonitor.startOperation(operation, metadata),
  end: (operationId: string, success?: boolean, metadata?: Record<string, any>) => 
    performanceMonitor.endOperation(operationId, success, metadata),
  record: (operation: string, metric: Partial<PerformanceMetrics>) => 
    performanceMonitor.recordMetric(operation, metric),
  cache: {
    hit: () => performanceMonitor.incrementCacheHit(),
    miss: () => performanceMonitor.incrementCacheMiss()
  },
  network: {
    request: () => performanceMonitor.incrementNetworkRequest()
  },
  error: () => performanceMonitor.incrementError(),
  stats: (operation?: string) => performanceMonitor.getStatistics(operation),
  report: () => performanceMonitor.getReport(),
  export: (format?: 'json' | 'csv') => performanceMonitor.exportMetrics(format),
  clear: () => performanceMonitor.clearMetrics(),
  stop: () => performanceMonitor.stop()
}

// Performance monitoring decorator
export function monitorPerformance(operation?: string) {
  return function (target: any, propertyKey: string, descriptor: PropertyDescriptor) {
    const originalMethod = descriptor.value
    const operationName = operation || `${target.constructor.name}.${propertyKey}`

    descriptor.value = async function (...args: any[]) {
      const operationId = perf.start(operationName, { args: args.length })
      
      try {
        const result = await originalMethod.apply(this, args)
        perf.end(operationId, true)
        return result
      } catch (error) {
        perf.end(operationId, false, { error: error instanceof Error ? error.message : 'Unknown error' })
        throw error
      }
    }

    return descriptor
  }
}

// Export performance monitor
export { PerformanceMonitor }

// Logging levels
export enum LogLevel {
  DEBUG = 0,
  INFO = 1,
  WARN = 2,
  ERROR = 3,
  FATAL = 4
}

// Log entry interface
export interface LogEntry {
  timestamp: string
  level: LogLevel
  category: string
  message: string
  data?: any
  userId?: string
  sessionId?: string
  requestId?: string
  duration?: number
  stack?: string
}

// Logger configuration
export interface LoggerConfig {
  level: LogLevel
  enableConsole: boolean
  enableStorage: boolean
  maxStorageEntries: number
  enableRemote: boolean
  remoteEndpoint?: string
  categories: string[]
}

// Default logger configuration
const DEFAULT_CONFIG: LoggerConfig = {
  level: LogLevel.INFO,
  enableConsole: true,
  enableStorage: true,
  maxStorageEntries: 1000,
  enableRemote: false,
  categories: ['product-catalog', 'api', 'ui', 'performance', 'security'],
  remoteEndpoint: undefined
}

// Product catalog logger
class ProductCatalogLogger {
  private config: LoggerConfig
  private logs: LogEntry[] = []
  private sessionId: string
  private userId: string | null = null

  constructor(config: Partial<LoggerConfig> = {}) {
    this.config = { ...DEFAULT_CONFIG, ...config }
    this.sessionId = this.generateSessionId()
    this.loadLogsFromStorage()
    this.setupErrorHandlers()
  }

  // Generate unique session ID
  private generateSessionId(): string {
    return `session_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
  }

  // Set current user
  setUserId(userId: string): void {
    this.userId = userId
  }

  // Setup global error handlers
  private setupErrorHandlers(): void {
    // Handle unhandled promise rejections
    window.addEventListener('unhandledrejection', (event) => {
      this.error('Unhandled Promise Rejection', JSON.stringify({
        reason: event.reason,
        stack: event.reason?.stack
      }))
    })

    // Handle uncaught errors
    window.addEventListener('error', (event) => {
      this.error('Uncaught Error', JSON.stringify({
        message: event.message,
        filename: event.filename,
        lineno: event.lineno,
        colno: event.colno,
        stack: event.error?.stack
      }))
    })
  }

  // Load logs from localStorage
  private loadLogsFromStorage(): void {
    try {
      const stored = localStorage.getItem('product_catalog_logs')
      if (stored) {
        this.logs = JSON.parse(stored)
        // Trim logs if they exceed max storage
        if (this.logs.length > this.config.maxStorageEntries) {
          this.logs = this.logs.slice(-this.config.maxStorageEntries)
        }
      }
    } catch (error) {
      console.warn('Failed to load logs from storage:', error)
    }
  }

  // Save logs to localStorage
  private saveLogsToStorage(): void {
    if (!this.config.enableStorage) return

    try {
      const logsToSave = this.logs.slice(-this.config.maxStorageEntries)
      localStorage.setItem('product_catalog_logs', JSON.stringify(logsToSave))
    } catch (error) {
      console.warn('Failed to save logs to storage:', error)
    }
  }

  // Create log entry
  private createLogEntry(level: LogLevel, category: string, message: string, data?: any): LogEntry {
    return {
      timestamp: new Date().toISOString(),
      level,
      category,
      message,
      data,
      userId: this.userId || undefined,
      sessionId: this.sessionId,
      requestId: this.generateRequestId(),
      duration: data?.duration,
      stack: level >= LogLevel.ERROR ? new Error().stack : undefined
    }
  }

  // Generate request ID
  private generateRequestId(): string {
    return `req_${Date.now()}_${Math.random().toString(36).substr(2, 6)}`
  }

  // Core logging method
  private log(level: LogLevel, category: string, message: string, data?: any): void {
    // Check if log level is enabled
    if (level < this.config.level) return

    // Check if category is enabled
    if (!this.config.categories.includes(category)) return

    const entry = this.createLogEntry(level, category, message, data)
    this.logs.push(entry)

    // Console output
    if (this.config.enableConsole) {
      this.outputToConsole(entry)
    }

    // Local storage
    this.saveLogsToStorage()

    // Remote logging (if enabled)
    if (this.config.enableRemote && this.config.remoteEndpoint) {
      this.sendToRemote(entry)
    }
  }

  // Output to console with appropriate styling
  private outputToConsole(entry: LogEntry): void {
    const styles = {
      [LogLevel.DEBUG]: 'color: #6b7280; font-weight: normal;',
      [LogLevel.INFO]: 'color: #3b82f6; font-weight: normal;',
      [LogLevel.WARN]: 'color: #f59e0b; font-weight: bold;',
      [LogLevel.ERROR]: 'color: #ef4444; font-weight: bold;',
      [LogLevel.FATAL]: 'color: #991b1b; font-weight: bold; background: #fee2e2; padding: 2px;'
    }

    const style = styles[entry.level]
    const prefix = `[${LogLevel[entry.level]}] [${entry.category}] [${new Date(entry.timestamp).toLocaleTimeString()}]`

    if (entry.data) {
      console.log(`%c${prefix} ${entry.message}`, style, entry.data)
    } else {
      console.log(`%c${prefix} ${entry.message}`, style)
    }

    // Include stack trace for errors
    if (entry.stack) {
      console.error('Stack trace:', entry.stack)
    }
  }

  // Send to remote endpoint
  private async sendToRemote(entry: LogEntry): Promise<void> {
    if (!this.config.remoteEndpoint) return

    try {
      await fetch(this.config.remoteEndpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(entry)
      })
    } catch (error) {
      console.warn('Failed to send log to remote endpoint:', error)
    }
  }

  // Public logging methods
  debug(category: string, message: string, data?: any): void {
    this.log(LogLevel.DEBUG, category, message, data)
  }

  info(category: string, message: string, data?: any): void {
    this.log(LogLevel.INFO, category, message, data)
  }

  warn(category: string, message: string, data?: any): void {
    this.log(LogLevel.WARN, category, message, data)
  }

  error(category: string, message: string, data?: any): void {
    this.log(LogLevel.ERROR, category, message, data)
  }

  fatal(category: string, message: string, data?: any): void {
    this.log(LogLevel.FATAL, category, message, data)
  }

  // Performance logging
  startPerformanceTimer(operation: string): string {
    const timerId = `perf_${Date.now()}_${Math.random().toString(36).substr(2, 6)}`
    performance.mark(`${timerId}_start`)
    this.debug('performance', `Started: ${operation}`, { timerId })
    return timerId
  }

  endPerformanceTimer(operation: string, timerId: string): void {
    performance.mark(`${timerId}_end`)
    performance.measure(operation, `${timerId}_start`, `${timerId}_end`)

    const measure = performance.getEntriesByName(operation).pop()
    const duration = measure?.duration || 0

    this.info('performance', `Completed: ${operation}`, {
      duration: Math.round(duration),
      timerId
    })
  }

  // API logging
  logApiCall(method: string, url: string, data?: any): string {
    const requestId = this.generateRequestId()
    this.info('api', `${method} ${url}`, {
      requestId,
      method,
      url,
      data: data ? JSON.parse(JSON.stringify(data)) : undefined
    })
    return requestId
  }

  logApiResponse(requestId: string, status: number, response?: any, duration?: number): void {
    const level = status >= 400 ? LogLevel.ERROR : LogLevel.INFO
    const category = status >= 400 ? 'api-error' : 'api'
    
    this.log(level, category, `${status} Response`, {
      requestId,
      status,
      duration,
      response: response ? JSON.parse(JSON.stringify(response)) : undefined
    })
  }

  // User action logging
  logUserAction(action: string, details?: any): void {
    this.info('ui', `User Action: ${action}`, {
      action,
      details,
      timestamp: new Date().toISOString()
    })
  }

  // Form validation logging
  logValidation(formName: string, errors: string[], warnings: string[] = []): void {
    const level = errors.length > 0 ? LogLevel.WARN : LogLevel.INFO
    const hasWarnings = warnings.length > 0

    this.log(level, 'validation', `Form Validation: ${formName}`, {
      formName,
      errorCount: errors.length,
      warningCount: warnings.length,
      errors,
      warnings,
      hasErrors: errors.length > 0,
      hasWarnings
    })
  }

  // State change logging
  logStateChange(entity: string, action: string, details?: any): void {
    this.debug('state', `${entity} ${action}`, {
      entity,
      action,
      details,
      timestamp: new Date().toISOString()
    })
  }

  // Get logs with filtering
  getLogs(filter?: {
    level?: LogLevel
    category?: string
    startDate?: Date
    endDate?: Date
    search?: string
  }): LogEntry[] {
    let filteredLogs = [...this.logs]

    if (filter?.level !== undefined) {
      filteredLogs = filteredLogs.filter(log => log.level >= filter.level!)
    }

    if (filter?.category) {
      filteredLogs = filteredLogs.filter(log => log.category === filter.category)
    }

    if (filter?.startDate) {
      filteredLogs = filteredLogs.filter(log => new Date(log.timestamp) >= filter.startDate!)
    }

    if (filter?.endDate) {
      filteredLogs = filteredLogs.filter(log => new Date(log.timestamp) <= filter.endDate!)
    }

    if (filter?.search) {
      const searchLower = filter.search.toLowerCase()
      filteredLogs = filteredLogs.filter(log => 
        log.message.toLowerCase().includes(searchLower) ||
        log.category.toLowerCase().includes(searchLower)
      )
    }

    return filteredLogs
  }

  // Export logs
  exportLogs(format: 'json' | 'csv' = 'json'): string {
    const logsToExport = this.getLogs()

    if (format === 'json') {
      return JSON.stringify(logsToExport, null, 2)
    }

    if (format === 'csv') {
      const headers = ['Timestamp', 'Level', 'Category', 'Message', 'User ID', 'Session ID', 'Request ID', 'Duration']
      const csvRows = logsToExport.map(log => [
        log.timestamp,
        LogLevel[log.level],
        log.category,
        log.message,
        log.userId || '',
        log.sessionId,
        log.requestId || '',
        log.duration || ''
      ])

      return [headers.join(','), ...csvRows.map(row => row.join(','))].join('\n')
    }

    return ''
  }

  // Clear logs
  clearLogs(): void {
    this.logs = []
    if (this.config.enableStorage) {
      localStorage.removeItem('product_catalog_logs')
    }
    this.info('system', 'Logs cleared')
  }

  // Get statistics
  getStats() {
    const totalLogs = this.logs.length
    const logsByLevel = {
      debug: this.logs.filter(log => log.level === LogLevel.DEBUG).length,
      info: this.logs.filter(log => log.level === LogLevel.INFO).length,
      warn: this.logs.filter(log => log.level === LogLevel.WARN).length,
      error: this.logs.filter(log => log.level === LogLevel.ERROR).length,
      fatal: this.logs.filter(log => log.level === LogLevel.FATAL).length
    }
    const logsByCategory = this.logs.reduce((acc, log) => {
      acc[log.category] = (acc[log.category] || 0) + 1
      return acc
    }, {} as Record<string, number>)

    return {
      totalLogs,
      logsByLevel,
      logsByCategory,
      sessionId: this.sessionId,
      userId: this.userId,
      config: this.config
    }
  }
}

// Create singleton instance
export const logger = new ProductCatalogLogger()

// Convenience exports
export const log = {
  debug: (category: string, message: string, data?: any) => logger.debug(category, message, data),
  info: (category: string, message: string, data?: any) => logger.info(category, message, data),
  warn: (category: string, message: string, data?: any) => logger.warn(category, message, data),
  error: (category: string, message: string, data?: any) => logger.error(category, message, data),
  fatal: (category: string, message: string, data?: any) => logger.fatal(category, message, data),
  performance: {
    start: (operation: string) => logger.startPerformanceTimer(operation),
    end: (operation: string, timerId: string) => logger.endPerformanceTimer(operation, timerId)
  },
  api: {
    call: (method: string, url: string, data?: any) => logger.logApiCall(method, url, data),
    response: (requestId: string, status: number, response?: any, duration?: number) => 
      logger.logApiResponse(requestId, status, response, duration)
  },
  user: {
    action: (action: string, details?: any) => logger.logUserAction(action, details)
  },
  validation: {
    check: (formName: string, errors: string[], warnings?: string[]) => 
      logger.logValidation(formName, errors, warnings)
  },
  state: {
    change: (entity: string, action: string, details?: any) => 
      logger.logStateChange(entity, action, details)
  }
}

// Export logger instance
export { ProductCatalogLogger }

import { log } from './logger'

// Security levels
export enum SecurityLevel {
  PUBLIC = 'public',
  USER = 'user',
  MANAGER = 'manager',
  ADMIN = 'admin',
  SUPER_ADMIN = 'super_admin'
}

// Permission types
export enum Permission {
  // Product permissions
  PRODUCT_READ = 'product:read',
  PRODUCT_CREATE = 'product:create',
  PRODUCT_UPDATE = 'product:update',
  PRODUCT_DELETE = 'product:delete',
  
  // Category permissions
  CATEGORY_READ = 'category:read',
  CATEGORY_CREATE = 'category:create',
  CATEGORY_UPDATE = 'category:update',
  CATEGORY_DELETE = 'category:delete',
  
  // Variation permissions
  VARIATION_READ = 'variation:read',
  VARIATION_CREATE = 'variation:create',
  VARIATION_UPDATE = 'variation:update',
  VARIATION_DELETE = 'variation:delete',
  
  // Image permissions
  IMAGE_READ = 'image:read',
  IMAGE_UPLOAD = 'image:upload',
  IMAGE_DELETE = 'image:delete',
  
  // Bulk operations
  BULK_CREATE = 'bulk:create',
  BULK_UPDATE = 'bulk:update',
  BULK_DELETE = 'bulk:delete',
  
  // System permissions
  SYSTEM_EXPORT = 'system:export',
  SYSTEM_IMPORT = 'system:import',
  SYSTEM_LOGS = 'system:logs'
}

// User interface
export interface User {
  id: string
  username: string
  email: string
  securityLevel: SecurityLevel
  permissions: Permission[]
  roles: string[]
  branchIds: string[]
  isActive: boolean
  lastLogin?: string
}

// Security context
export interface SecurityContext {
  user: User | null
  isAuthenticated: boolean
  sessionId: string
  csrfToken: string
  permissions: Permission[]
}

// Input sanitization rules
export interface SanitizationRule {
  type: 'string' | 'number' | 'email' | 'url' | 'html' | 'sql'
  maxLength?: number
  minLength?: number
  pattern?: RegExp
  allowHtml?: boolean
  stripTags?: boolean
  escapeHtml?: boolean
  normalizeWhitespace?: boolean
}

// Security configuration
export interface SecurityConfig {
  enableCSRF: boolean
  enableXSSProtection: boolean
  enableInputValidation: boolean
  enableRateLimiting: boolean
  maxRequestsPerMinute: number
  sessionTimeout: number
  enableAuditLogging: boolean
  allowedFileTypes: string[]
  maxFileSize: number
}

// Default security configuration
const DEFAULT_CONFIG: SecurityConfig = {
  enableCSRF: true,
  enableXSSProtection: true,
  enableInputValidation: true,
  enableRateLimiting: true,
  maxRequestsPerMinute: 100,
  sessionTimeout: 30 * 60 * 1000, // 30 minutes
  enableAuditLogging: true,
  allowedFileTypes: ['image/jpeg', 'image/png', 'image/webp', 'image/gif'],
  maxFileSize: 5 * 1024 * 1024 // 5MB
}

// Security manager class
class SecurityManager {
  private config: SecurityConfig
  private context: SecurityContext
  private rateLimitMap: Map<string, { count: number; resetTime: number }> = new Map()

  constructor(config: Partial<SecurityConfig> = {}) {
    this.config = { ...DEFAULT_CONFIG, ...config }
    this.context = {
      user: null,
      isAuthenticated: false,
      sessionId: this.generateSessionId(),
      csrfToken: this.generateCSRFToken(),
      permissions: []
    }
    this.setupSecurityHeaders()
  }

  // Generate secure session ID
  private generateSessionId(): string {
    const array = new Uint8Array(32)
    crypto.getRandomValues(array)
    return Array.from(array, byte => byte.toString(16).padStart(2, '0')).join('')
  }

  // Generate CSRF token
  private generateCSRFToken(): string {
    const array = new Uint8Array(32)
    crypto.getRandomValues(array)
    return btoa(String.fromCharCode(...array))
  }

  // Setup security headers
  private setupSecurityHeaders(): void {
    // This would typically be done on the server, but we can log security events
    log.info('security', 'Security headers configured', {
      csrfProtection: this.config.enableCSRF,
      xssProtection: this.config.enableXSSProtection
    })
  }

  // Set user context
  setUser(user: User): void {
    this.context.user = user
    this.context.isAuthenticated = true
    this.context.permissions = user.permissions || []
    
    log.info('security', 'User authenticated', {
      userId: user.id,
      username: user.username,
      securityLevel: user.securityLevel,
      permissions: user.permissions
    })
  }

  // Clear user context
  clearUser(): void {
    log.info('security', 'User logged out', {
      userId: this.context.user?.id,
      sessionId: this.context.sessionId
    })
    
    this.context.user = null
    this.context.isAuthenticated = false
    this.context.permissions = []
  }

  // Get current security context
  getContext(): SecurityContext {
    return { ...this.context }
  }

  // Permission checking
  hasPermission(permission: Permission): boolean {
    if (!this.context.isAuthenticated) {
      return false
    }

    return this.context.permissions.includes(permission)
  }

  // Check multiple permissions
  hasPermissions(permissions: Permission[]): boolean {
    return permissions.every(permission => this.hasPermission(permission))
  }

  // Check if user has any of the specified permissions
  hasAnyPermission(permissions: Permission[]): boolean {
    return permissions.some(permission => this.hasPermission(permission))
  }

  // Role-based authorization
  hasRole(role: string): boolean {
    if (!this.context.user) return false
    return this.context.user.roles.includes(role)
  }

  // Security level check
  hasSecurityLevel(minLevel: SecurityLevel): boolean {
    if (!this.context.user) return false
    
    const levels = [
      SecurityLevel.PUBLIC,
      SecurityLevel.USER,
      SecurityLevel.MANAGER,
      SecurityLevel.ADMIN,
      SecurityLevel.SUPER_ADMIN
    ]
    
    const userLevelIndex = levels.indexOf(this.context.user.securityLevel)
    const requiredLevelIndex = levels.indexOf(minLevel)
    
    return userLevelIndex >= requiredLevelIndex
  }

  // Input sanitization
  sanitizeInput(value: any, rule: SanitizationRule): any {
    if (!this.config.enableInputValidation) {
      return value
    }

    try {
      switch (rule.type) {
        case 'string':
          return this.sanitizeString(value, rule)
        case 'number':
          return this.sanitizeNumber(value, rule)
        case 'email':
          return this.sanitizeEmail(value, rule)
        case 'url':
          return this.sanitizeUrl(value, rule)
        case 'html':
          return this.sanitizeHtml(value, rule)
        case 'sql':
          return this.sanitizeSql(value, rule)
        default:
          return value
      }
    } catch (error) {
      log.error('security', 'Input sanitization failed', { error, value, rule })
      throw new Error('Invalid input format')
    }
  }

  // String sanitization
  private sanitizeString(value: any, rule: SanitizationRule): string {
    let sanitized = String(value || '')

    // Length validation
    if (rule.minLength && sanitized.length < rule.minLength) {
      throw new Error(`String must be at least ${rule.minLength} characters`)
    }
    if (rule.maxLength && sanitized.length > rule.maxLength) {
      sanitized = sanitized.substring(0, rule.maxLength)
    }

    // Pattern validation
    if (rule.pattern && !rule.pattern.test(sanitized)) {
      throw new Error('String format is invalid')
    }

    // HTML processing
    if (rule.stripTags) {
      sanitized = sanitized.replace(/<[^>]*>/g, '')
    }

    if (rule.escapeHtml) {
      sanitized = this.escapeHtml(sanitized)
    }

    if (rule.normalizeWhitespace) {
      sanitized = sanitized.replace(/\s+/g, ' ').trim()
    }

    return sanitized
  }

  // Number sanitization
  private sanitizeNumber(value: any, rule: SanitizationRule): number {
    const num = Number(value)
    
    if (isNaN(num)) {
      throw new Error('Invalid number format')
    }

    if (rule.minLength !== undefined && num < rule.minLength) {
      throw new Error(`Number must be at least ${rule.minLength}`)
    }

    if (rule.maxLength !== undefined && num > rule.maxLength) {
      throw new Error(`Number must not exceed ${rule.maxLength}`)
    }

    return num
  }

  // Email sanitization
  private sanitizeEmail(value: any, rule: SanitizationRule): string {
    const email = String(value || '').toLowerCase().trim()
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    
    if (!emailRegex.test(email)) {
      throw new Error('Invalid email format')
    }

    return this.sanitizeString(email, { ...rule, type: 'string' })
  }

  // URL sanitization
  private sanitizeUrl(value: any, rule: SanitizationRule): string {
    const url = String(value || '').trim()
    
    try {
      const urlObj = new URL(url)
      if (!['http:', 'https:'].includes(urlObj.protocol)) {
        throw new Error('Only HTTP and HTTPS URLs are allowed')
      }
      return urlObj.toString()
    } catch {
      throw new Error('Invalid URL format')
    }
  }

  // HTML sanitization
  private sanitizeHtml(value: any, rule: SanitizationRule): string {
    let html = String(value || '')
    
    if (!rule.allowHtml) {
      html = html.replace(/<[^>]*>/g, '')
    } else {
      // Basic HTML sanitization - remove dangerous tags
      const dangerousTags = ['script', 'iframe', 'object', 'embed', 'form', 'input', 'textarea']
      dangerousTags.forEach(tag => {
        const regex = new RegExp(`<${tag}[^>]*>.*?</${tag}>`, 'gis')
        html = html.replace(regex, '')
      })
      
      // Remove dangerous attributes
      html = html.replace(/on\w+="[^"]*"/gi, '')
      html = html.replace(/javascript:/gi, '')
    }
    
    return html
  }

  // SQL injection prevention
  private sanitizeSql(value: any, rule: SanitizationRule): string {
    const sql = String(value || '')
    
    // Remove common SQL injection patterns
    const dangerousPatterns = [
      /(\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|EXEC|UNION)\b)/gi,
      /(--|\/\*|\*\/|;|'|"|`)/gi,
      /\b(OR|AND)\s+\d+\s*=\s*\d+/gi
    ]
    
    let sanitized = sql
    dangerousPatterns.forEach(pattern => {
      sanitized = sanitized.replace(pattern, '')
    })
    
    return sanitized.trim()
  }

  // HTML escaping
  private escapeHtml(text: string): string {
    const div = document.createElement('div')
    div.textContent = text
    return div.innerHTML
  }

  // Rate limiting
  checkRateLimit(identifier: string = 'default'): boolean {
    if (!this.config.enableRateLimiting) {
      return true
    }

    const now = Date.now()
    const limit = this.rateLimitMap.get(identifier)

    if (!limit || now > limit.resetTime) {
      // Reset or create new limit
      this.rateLimitMap.set(identifier, {
        count: 1,
        resetTime: now + 60000 // 1 minute
      })
      return true
    }

    if (limit.count >= this.config.maxRequestsPerMinute) {
      log.warn('security', 'Rate limit exceeded', {
        identifier,
        count: limit.count,
        resetTime: limit.resetTime
      })
      return false
    }

    limit.count++
    return true
  }

  // CSRF token validation
  validateCSRFToken(token: string): boolean {
    if (!this.config.enableCSRF) {
      return true
    }

    const isValid = token === this.context.csrfToken
    
    if (!isValid) {
      log.error('security', 'CSRF token validation failed', {
        providedToken: token,
        expectedToken: this.context.csrfToken,
        sessionId: this.context.sessionId
      })
    }
    
    return isValid
  }

  // File upload security
  validateFileUpload(file: File): { isValid: boolean; error?: string } {
    // Check file type
    if (!this.config.allowedFileTypes.includes(file.type)) {
      return {
        isValid: false,
        error: `File type ${file.type} is not allowed`
      }
    }

    // Check file size
    if (file.size > this.config.maxFileSize) {
      return {
        isValid: false,
        error: `File size exceeds maximum allowed size of ${this.config.maxFileSize} bytes`
      }
    }

    // Check file name for suspicious patterns
    const suspiciousPatterns = [
      /\.(exe|bat|cmd|scr|pif|com)$/i,
      /\.(php|asp|jsp|cgi|pl|py|rb|sh)$/i,
      /\.\./,
      /[<>:"|?*]/
    ]

    for (const pattern of suspiciousPatterns) {
      if (pattern.test(file.name)) {
        return {
          isValid: false,
          error: 'File name contains suspicious characters or extensions'
        }
      }
    }

    return { isValid: true }
  }

  // Audit logging
  logSecurityEvent(event: string, details: any = {}): void {
    if (this.config.enableAuditLogging) {
      log.info('security', event, {
        ...details,
        userId: this.context.user?.id,
        sessionId: this.context.sessionId,
        timestamp: new Date().toISOString()
      })
    }
  }

  // Security middleware for API calls
  async secureApiCall<T>(
    apiCall: () => Promise<T>,
    requiredPermissions?: Permission[],
    rateLimitKey?: string
  ): Promise<T> {
    // Check authentication
    if (!this.context.isAuthenticated) {
      throw new Error('Authentication required')
    }

    // Check permissions
    if (requiredPermissions && !this.hasPermissions(requiredPermissions)) {
      this.logSecurityEvent('Unauthorized access attempt', {
        requiredPermissions,
        userPermissions: this.context.permissions
      })
      throw new Error('Insufficient permissions')
    }

    // Check rate limiting
    if (rateLimitKey && !this.checkRateLimit(rateLimitKey)) {
      throw new Error('Rate limit exceeded')
    }

    try {
      const result = await apiCall()
      this.logSecurityEvent('API call successful', {
        rateLimitKey
      })
      return result
    } catch (error) {
      this.logSecurityEvent('API call failed', {
        rateLimitKey,
        error: error instanceof Error ? error.message : 'Unknown error'
      })
      throw error
    }
  }

  // Generate secure random string
  generateSecureRandom(length: number = 32): string {
    const array = new Uint8Array(length)
    crypto.getRandomValues(array)
    return Array.from(array, byte => byte.toString(16).padStart(2, '0')).join('')
  }

  // Hash sensitive data
  async hashData(data: string): Promise<string> {
    const encoder = new TextEncoder()
    const dataBuffer = encoder.encode(data)
    const hashBuffer = await crypto.subtle.digest('SHA-256', dataBuffer)
    const hashArray = Array.from(new Uint8Array(hashBuffer))
    return hashArray.map(b => b.toString(16).padStart(2, '0')).join('')
  }

  // Get security statistics
  getSecurityStats() {
    return {
      isAuthenticated: this.context.isAuthenticated,
      user: this.context.user ? {
        id: this.context.user.id,
        username: this.context.user.username,
        securityLevel: this.context.user.securityLevel,
        permissions: this.context.user.permissions
      } : null,
      sessionId: this.context.sessionId,
      permissions: this.context.permissions,
      rateLimitEntries: this.rateLimitMap.size,
      config: this.config
    }
  }
}

// Create singleton instance
export const security = new SecurityManager()

// Convenience exports
export const auth = {
  login: (user: User) => security.setUser(user),
  logout: () => security.clearUser(),
  getContext: () => security.getContext(),
  isAuthenticated: () => security.getContext().isAuthenticated,
  hasPermission: (permission: Permission) => security.hasPermission(permission),
  hasPermissions: (permissions: Permission[]) => security.hasPermissions(permissions),
  hasAnyPermission: (permissions: Permission[]) => security.hasAnyPermission(permissions),
  hasRole: (role: string) => security.hasRole(role),
  hasSecurityLevel: (level: SecurityLevel) => security.hasSecurityLevel(level)
}

export const input = {
  sanitize: (value: any, rule: SanitizationRule) => security.sanitizeInput(value, rule),
  validateFile: (file: File) => security.validateFileUpload(file)
}

export const protection = {
  checkRateLimit: (key?: string) => security.checkRateLimit(key),
  validateCSRF: (token: string) => security.validateCSRFToken(token),
  secureApiCall: <T>(apiCall: () => Promise<T>, permissions?: Permission[], rateLimitKey?: string) => 
    security.secureApiCall(apiCall, permissions, rateLimitKey)
}

export const audit = {
  log: (event: string, details?: any) => security.logSecurityEvent(event, details),
  hash: (data: string) => security.hashData(data),
  generateRandom: (length?: number) => security.generateSecureRandom(length)
}

// Export security manager
export { SecurityManager }

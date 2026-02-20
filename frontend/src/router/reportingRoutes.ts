import type { RouteRecordRaw } from 'vue-router'

// Lazy load reporting components
const ReportingDashboard = () => import('@/components/Reporting/ReportingDashboard.vue')
const AnalyticsVisualization = () => import('@/components/Reporting/AnalyticsVisualization.vue')
const ReportBuilder = () => import('@/components/Reporting/ReportBuilder.vue')

export const reportingRoutes: RouteRecordRaw[] = [
  {
    path: '/reporting',
    name: 'ReportingDashboard',
    component: ReportingDashboard,
    meta: {
      requiresAuth: true,
      roles: ['Manager', 'Admin', 'Analyst']
    }
  },
  {
    path: '/reporting/analytics',
    name: 'AnalyticsVisualization',
    component: AnalyticsVisualization,
    meta: {
      requiresAuth: true,
      roles: ['Manager', 'Admin', 'Analyst']
    }
  },
  {
    path: '/reporting/builder',
    name: 'ReportBuilder',
    component: ReportBuilder,
    meta: {
      requiresAuth: true,
      roles: ['Manager', 'Admin', 'Analyst']
    }
  }
]

export const REPORTING_ROUTE_NAMES = {
  DASHBOARD: 'ReportingDashboard',
  ANALYTICS: 'AnalyticsVisualization',
  BUILDER: 'ReportBuilder'
} as const

export type ReportingRouteName = typeof REPORTING_ROUTE_NAMES[keyof typeof REPORTING_ROUTE_NAMES]

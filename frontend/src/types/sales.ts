export interface CartItem {
  id: string
  productId: string
  productName: string
  sku: string
  unitPrice: number
  quantity: number
  discountPercent: number
  total: number
}

export interface PaymentData {
  method: string
  amount: number
  cashAmount?: number
  change: number
  cardNumber?: string
  cardExpiry?: string
  cardCvc?: string
  giftCardNumber?: string
  transactionId: string
}

export interface ProcessSaleRequest {
  branchId: string
  userId: string
  customerId?: string | null
  items: SaleItemRequest[]
  payments: SalePaymentRequest[]
  notes?: string
}

export interface SaleItemRequest {
  productId: string
  productVariationId?: string | null
  inventoryId: string
  quantity: number
  unitPrice: number
  discountAmount: number
  taxRate: number
}

export interface SalePaymentRequest {
  paymentMethod: string
  amount: number
  currency: string
  referenceNumber?: string
  cardLastFour?: string
  cardType?: string
  giftCardNumber?: string
}

export interface ProcessReturnRequest {
  originalTransactionNumber: string
  branchId: string
  userId: string
  customerId?: string | null
  items: ReturnItemRequest[]
  refundMethod: string
  reason: string
}

export interface ReturnItemRequest {
  originalItemId: string
  productId: string
  quantity: number
  unitPrice: number
  reason: string
}

export interface SalesTransaction {
  id: string
  transactionNumber: string
  branchId: string
  customerId?: string
  userId: string
  transactionType: string
  status: string
  subtotal: number
  taxAmount: number
  discountAmount: number
  totalAmount: number
  amountPaid: number
  changeGiven: number
  loyaltyPointsEarned: number
  loyaltyPointsRedeemed: number
  createdAt: string
  completedAt?: string
  items: SalesTransactionItem[]
  payments: SalesTransactionPayment[]
}

export interface SalesTransactionItem {
  id: string
  productId: string
  productVariationId?: string
  inventoryId: string
  quantity: number
  unitPrice: number
  discountAmount: number
  taxAmount: number
  totalPrice: number
  createdAt: string
}

export interface SalesTransactionPayment {
  id: string
  paymentMethod: string
  amount: number
  currency: string
  referenceNumber?: string
  cardLastFour?: string
  cardType?: string
  giftCardNumber?: string
  authorizationCode?: string
  isApproved: boolean
  processedAt: string
}
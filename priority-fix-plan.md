# Prioritized Fix Plan - Production Risk Order

## Priority 1: CRITICAL (Runtime Failures)
*These will crash the application when triggered*

### 1.1 Implement Payment Service Methods
**Risk:** Complete payment processing failure for card transactions

**Files:**
- `backend/src/Infrastructure/External/PaymentService.cs`

**Fix:** Implement the following methods that currently throw `NotImplementedException`:
```
Lines 158-176:
- ProcessCreditCardPaymentAsync()
- ProcessDebitCardPaymentAsync()  
- ProcessCashPaymentAsync()
- ProcessGiftCardPaymentAsync()
```

**Action:** Either implement actual payment gateway integration or add fallback to existing gift card logic

---

### 1.2 Fix Unit of Work Transaction Commit
**Risk:** All database transactions will fail

**File:** `backend/src/Infrastructure/Data/UnitOfWork.cs`

**Fix:** Line 66-69 - Replace `throw new NotImplementedException()` with actual implementation:
```csharp
public async Task CommitTransactionAsync(CancellationToken cancellationToken)
{
    await CommitAsync(cancellationToken);
    if (_transaction != null)
    {
        await _transaction.CommitAsync(cancellationToken);
        _transaction = null;
    }
}
```

---

## Priority 2: HIGH (Functional Breakage)
*These cause incorrect behavior or missing functionality*

### 2.1 Remove Mock Data from Production Code
**Risk:** Fake transactions and incorrect data in production

**Files:**
- `frontend/src/services/salesService.ts` (lines 180-255)
- `frontend/src/components/Sales/ReturnsExchanges.vue` (lines 23-46)

**Fix:** Either remove the mock service or guard it with environment check:
```typescript
// Add at top of salesService.ts
const USE_MOCK = import.meta.env.VITE_USE_MOCK_DATA === 'true'
```

---

### 2.2 Create Frontend Services for Unused Modules
**Risk:** 50+ backend endpoints are inaccessible from UI

**Missing Services:**
| Module | Backend Endpoints | Priority |
|--------|------------------|----------|
| Customers | 9 endpoints | HIGH |
| Inventory | 25+ endpoints | HIGH |
| Procurement | 18 endpoints | MEDIUM |

**Quick Fix:** Create minimal service wrappers that can be expanded later:
```
frontend/src/services/customerService.ts   (needed for customer management)
frontend/src/services/inventoryService.ts  (needed for stock management)
```

---

## Priority 3: MEDIUM (Integration Issues)
*These cause request failures*

### 3.1 Add Missing Backend Endpoints
**Risk:** Frontend calls fail with 404

**File:** `backend/src/API/Controllers/` - New controller needed

**Add:** ProductVariationsController with:
```
GET    /variations/{id}
GET    /variations/by-sku/{sku}
PATCH  /variations/{id}/stock
PUT    /variations/{id}
DELETE /variations/{id}
```

**Alternative Fix:** Remove these methods from `productCatalogService.ts` if not needed

---

### 3.2 Fix DTO Contract Mismatches
**Risk:** Data loss or validation failures

**Issues to Fix:**

| Frontend File | Backend DTO | Fix |
|---------------|-------------|-----|
| `sales.ts` - ProcessSaleRequest | Add `notes?: string` | Add missing field |
| `sales.ts` - SalePaymentRequest | Add `authorizationCode?: string` | Add missing field |
| `salesService.ts` - getTransactions | Response parsing | Align with PagedResponse structure |

---

## Priority 4: LOW (Code Quality)
*These should be cleaned up but won't cause failures*

### 4.1 Remove Placeholder Code
**Files:**
- `backend/src/Infrastructure/Data/Analytics/001_CreateAnalyticsDataWarehouse.sql` (lines 522-546)
- `backend/src/Infrastructure/Jobs/ReportGenerationJob.cs` (line 292)
- `backend/src/Application/Services/DataArchivalService.cs` (line 24)

**Action:** Either implement or document as TODO with target version

---

## Execution Order

```
PHASE 1 (Immediate - Day 1)
├── 1.1 Fix PaymentService NotImplementedException
├── 1.2 Fix UnitOfWork.CommitTransactionAsync
└── 2.1 Remove mock data

PHASE 2 (Short Term - Week 1)
├── 3.1 Add missing endpoints OR remove frontend calls
└── 3.2 Fix DTO mismatches

PHASE 3 (Medium Term - Week 2)
├── 2.2 Create customerService.ts
└── 2.2 Create inventoryService.ts (minimal)

PHASE 4 (Backlog)
├── 4.1 Clean up placeholders
└── 2.2 Create procurementService.ts (if needed)
```

---

## Verification Checklist

After each fix, verify:
- [ ] Application starts without exceptions
- [ ] Payment flow completes (or fails gracefully with clear message)
- [ ] No mock data in network requests
- [ ] All frontend API calls return 200/expected response

---

*Generated: February 14, 2026*

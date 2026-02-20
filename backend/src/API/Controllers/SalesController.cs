using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NationalClothingStore.API.Filters;
using NationalClothingStore.Application.Common;
using NationalClothingStore.Application.Interfaces;
using NationalClothingStore.Domain.Entities;

namespace NationalClothingStore.API.Controllers;

/// <summary>
/// Controller for sales transaction processing operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class SalesController : ControllerBase
{
    private readonly ISalesProcessingService _salesProcessingService;
    private readonly ILogger<SalesController> _logger;

    public SalesController(
        ISalesProcessingService salesProcessingService,
        ILogger<SalesController> logger)
    {
        _salesProcessingService = salesProcessingService;
        _logger = logger;
    }

    /// <summary>
    /// Process a new sale transaction
    /// </summary>
    /// <param name="request">Sale processing request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processed sales transaction</returns>
    [HttpPost("process-sale")]
    [Authorize(Roles = "Cashier,SalesAssociate,Manager,Admin")]
    [ProducesResponseType(typeof(SalesTransaction), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SalesTransaction>> ProcessSale(
        [FromBody] ProcessSaleRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = await _salesProcessingService.ProcessSaleAsync(request, cancellationToken);
            return Ok(transaction);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error processing sale");
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing sale");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "An error occurred while processing the sale" });
        }
    }

    /// <summary>
    /// Process a return transaction
    /// </summary>
    /// <param name="request">Return processing request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processed return transaction</returns>
    [HttpPost("process-return")]
    [Authorize(Roles = "Cashier,SalesAssociate,Manager,Admin")]
    [ProducesResponseType(typeof(SalesTransaction), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SalesTransaction>> ProcessReturn(
        [FromBody] ProcessReturnRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = await _salesProcessingService.ProcessReturnAsync(request, cancellationToken);
            return Ok(transaction);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error processing return");
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing return");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "An error occurred while processing the return" });
        }
    }

    /// <summary>
    /// Process an exchange transaction
    /// </summary>
    /// <param name="request">Exchange processing request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processed exchange transaction</returns>
    [HttpPost("process-exchange")]
    [Authorize(Roles = "Cashier,SalesAssociate,Manager,Admin")]
    [ProducesResponseType(typeof(SalesTransaction), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SalesTransaction>> ProcessExchange(
        [FromBody] ProcessExchangeRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = await _salesProcessingService.ProcessExchangeAsync(request, cancellationToken);
            return Ok(transaction);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error processing exchange");
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing exchange");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "An error occurred while processing the exchange" });
        }
    }

    /// <summary>
    /// Get sales transaction by ID
    /// </summary>
    /// <param name="id">Transaction ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sales transaction</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Cashier,SalesAssociate,Manager,Admin")]
    [ProducesResponseType(typeof(SalesTransaction), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SalesTransaction>> GetTransaction(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = await _salesProcessingService.GetTransactionByIdAsync(id, cancellationToken);
            if (transaction == null)
            {
                return NotFound(new { Message = "Transaction not found" });
            }
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction {TransactionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "An error occurred while retrieving the transaction" });
        }
    }

    /// <summary>
    /// Get sales transaction by transaction number
    /// </summary>
    /// <param name="transactionNumber">Transaction number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sales transaction</returns>
    [HttpGet("by-number/{transactionNumber}")]
    [Authorize(Roles = "Cashier,SalesAssociate,Manager,Admin")]
    [ProducesResponseType(typeof(SalesTransaction), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SalesTransaction>> GetTransactionByNumber(
        string transactionNumber,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = await _salesProcessingService.GetTransactionByNumberAsync(transactionNumber, cancellationToken);
            if (transaction == null)
            {
                return NotFound(new { Message = "Transaction not found" });
            }
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction {TransactionNumber}", transactionNumber);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "An error occurred while retrieving the transaction" });
        }
    }

    /// <summary>
    /// Get paginated sales transactions
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="branchId">Filter by branch ID</param>
    /// <param name="customerId">Filter by customer ID</param>
    /// <param name="transactionType">Filter by transaction type (SALE, RETURN, EXCHANGE)</param>
    /// <param name="status">Filter by status (PENDING, COMPLETED, CANCELLED, REFUNDED)</param>
    /// <param name="startDate">Filter by start date</param>
    /// <param name="endDate">Filter by end date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of sales transactions</returns>
    [HttpGet]
    [Authorize(Roles = "Manager,Admin")]
    [ProducesResponseType(typeof(PagedResponse<SalesTransaction>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<SalesTransaction>>> GetTransactions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? branchId = null,
        [FromQuery] Guid? customerId = null,
        [FromQuery] string? transactionType = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate pagination parameters
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var (transactions, totalCount) = await _salesProcessingService.GetTransactionsAsync(
                pageNumber, pageSize, branchId, customerId, transactionType, status, startDate, endDate, cancellationToken);

            var response = new PagedResponse<SalesTransaction>
            {
                Items = transactions,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "An error occurred while retrieving transactions" });
        }
    }

    /// <summary>
    /// Get sales statistics for a period
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="branchId">Optional branch filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sales statistics</returns>
    [HttpGet("statistics")]
    [Authorize(Roles = "Manager,Admin")]
    [ProducesResponseType(typeof(SalesStatistics), StatusCodes.Status200OK)]
    public async Task<ActionResult<SalesStatistics>> GetSalesStatistics(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? branchId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var statistics = await _salesProcessingService.GetSalesStatisticsAsync(
                startDate, endDate, branchId, cancellationToken);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sales statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "An error occurred while retrieving sales statistics" });
        }
    }

    /// <summary>
    /// Get top selling products for a period
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="limit">Number of products to return (default: 10)</param>
    /// <param name="branchId">Optional branch filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Top selling products</returns>
    [HttpGet("top-products")]
    [Authorize(Roles = "Manager,Admin")]
    [ProducesResponseType(typeof(IEnumerable<ProductSalesSummary>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductSalesSummary>>> GetTopSellingProducts(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int limit = 10,
        [FromQuery] Guid? branchId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (limit < 1) limit = 1;
            if (limit > 100) limit = 100;

            var products = await _salesProcessingService.GetTopSellingProductsAsync(
                startDate, endDate, limit, branchId, cancellationToken);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top selling products");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "An error occurred while retrieving top selling products" });
        }
    }

    /// <summary>
    /// Cancel a pending transaction
    /// </summary>
    /// <param name="id">Transaction ID</param>
    /// <param name="reason">Reason for cancellation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success indicator</returns>
    [HttpPut("{id:guid}/cancel")]
    [Authorize(Roles = "Manager,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CancelTransaction(
        Guid id,
        [FromBody] CancelTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _salesProcessingService.CancelTransactionAsync(
                id, request.Reason, cancellationToken);
            
            if (!success)
            {
                return NotFound(new { Message = "Transaction not found or cannot be cancelled" });
            }

            return Ok(new { Message = "Transaction cancelled successfully" });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error cancelling transaction {TransactionId}", id);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling transaction {TransactionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "An error occurred while cancelling the transaction" });
        }
    }
}

/// <summary>
/// Request model for cancelling a transaction
/// </summary>
public class CancelTransactionRequest
{
    /// <summary>
    /// Reason for cancellation
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Response model for paged results
/// </summary>
/// <typeparam name="T">Item type</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// Items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();
    
    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; }
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Total number of items
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// Indicates if there's a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
    
    /// <summary>
    /// Indicates if there's a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
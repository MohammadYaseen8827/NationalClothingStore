using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NationalClothingStore.API.Controllers;

namespace NationalClothingStore.API.Filters;

/// <summary>
/// Automatic model validation filter
/// </summary>
public class ValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(kvp => kvp.Value is { Errors.Count: > 0 })
                .SelectMany(kvp => kvp.Value?.Errors
                    .Select(e => new ValidationError
                    {
                        Field = kvp.Key,
                        Message = e.ErrorMessage
                    }) ?? Array.Empty<ValidationError>())
                .ToList();

            var response = new ApiResponse
            {
                Success = false,
                Message = "Validation failed",
                Errors = errors.Select(e => $"{e.Field}: {e.Message}").ToList(),
                Timestamp = DateTime.UtcNow
            };

            context.Result = new BadRequestObjectResult(response);
        }
    }
}

/// <summary>
/// Validation error model
/// </summary>
public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

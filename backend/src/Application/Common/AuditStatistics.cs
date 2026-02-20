namespace NationalClothingStore.Application.Common;

/// <summary>
/// Audit statistics model
/// </summary>
public class AuditStatistics
{
    public int TotalEvents { get; set; }
    public int UserActions { get; set; }
    public int SystemEvents { get; set; }
    public int FailedActions { get; set; }
    public Dictionary<string, int> ActionsByType { get; set; } = new();
    public Dictionary<string, int> EventsByResourceType { get; set; } = new();
    public Dictionary<Guid, int> EventsByUser { get; set; } = new();
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

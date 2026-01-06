using SubscriptionManagement.Domain.Common;

namespace SubscriptionManagement.Domain.ValueObjects;

public sealed class Plan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
}
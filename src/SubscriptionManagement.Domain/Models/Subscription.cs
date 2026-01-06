namespace SubscriptionManagement.Domain.Models;

public class Subscription
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; } = true;
}

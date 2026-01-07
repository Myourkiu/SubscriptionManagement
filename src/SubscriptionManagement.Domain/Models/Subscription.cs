using SubscriptionManagement.Domain.ValueObjects;

namespace SubscriptionManagement.Domain.Models;

public class Subscription
{
    public Guid Id { get; set; }
    public Plan Plan { get; set; } = null!;
    public SubscriptionPeriod Period { get; set; } = null!;
    public SubscriptionStatus Status { get; set; }

    public void Cancel()
    {
        this.Status = SubscriptionStatus.Canceled;
    }

    public void Expire()
    {
        this.Status = SubscriptionStatus.Expired;
    }

    public bool IsActive()
    {
        return Status == SubscriptionStatus.Active;
    }
}

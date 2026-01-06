using System.ComponentModel;

namespace SubscriptionManagement.Domain.Models
{
    public enum SubscriptionStatus
    {
        [Description("Active")]
        Active = 1,
        [Description("Canceled")]
        Canceled = 2,
        [Description("Expired")]
        Expired = 3
    }
}
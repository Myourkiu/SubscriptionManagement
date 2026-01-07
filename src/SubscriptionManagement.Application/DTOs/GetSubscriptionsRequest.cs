using SubscriptionManagement.Domain.Models;

namespace SubscriptionManagement.Application.DTOs
{
    public class GetSubscriptionsRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public SubscriptionStatus? Status { get; set; }
    }
}
namespace SubscriptionManagement.Application.DTOs
{
    public class CreateSubscriptionRequest
    {
        public string PlanName { get; set; } = string.Empty;
        public int DurationInDays { get; set; } = 0;
    }
}
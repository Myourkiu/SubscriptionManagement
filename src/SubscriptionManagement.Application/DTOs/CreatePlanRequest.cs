namespace SubscriptionManagement.Application.DTOs
{
    public class CreatePlanRequest
    {
        public string Name { get; set; } = string.Empty;
        public int DurationInDays { get; set; }
    }
}
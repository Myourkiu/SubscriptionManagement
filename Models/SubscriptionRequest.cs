using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagement.Models;

public class SubscriptionRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
    public decimal Price { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "A duração deve ser maior que zero")]
    public int DurationDays { get; set; }
}

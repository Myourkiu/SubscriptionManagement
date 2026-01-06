using Microsoft.AspNetCore.Mvc;
using SubscriptionManagement.Domain.Models;

namespace SubscriptionManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private static readonly List<Subscription> _subscriptions = new()
    {
        new Subscription { Id = 1, Name = "Plano Básico", Price = 29.90m, DurationDays = 30, IsActive = true },
        new Subscription { Id = 2, Name = "Plano Premium", Price = 99.90m, DurationDays = 30, IsActive = true },
        new Subscription { Id = 3, Name = "Plano Anual", Price = 999.90m, DurationDays = 365, IsActive = true }
    };

    /// <summary>
    /// Obtém todas as assinaturas
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_subscriptions);
    }
}

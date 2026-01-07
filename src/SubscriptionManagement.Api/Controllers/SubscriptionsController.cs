using Microsoft.AspNetCore.Mvc;
using SubscriptionManagement.Application.DTOs;
using SubscriptionManagement.Domain.Common;
using SubscriptionManagement.Infrastructure.Services;

namespace SubscriptionManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly SubscriptionService _subscriptionService;

    public SubscriptionsController(SubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetSubscriptionsRequest? request)
    {
        var subscriptions = await _subscriptionService.GetAllAsync(request);
        return Ok(subscriptions);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionRequest request)
    {
        var result = await _subscriptionService.CreateAsync(request);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _subscriptionService.CancelAsync(id);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

}

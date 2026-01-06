using Microsoft.AspNetCore.Mvc;
using SubscriptionManagement.Models;

namespace SubscriptionManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private static readonly List<Subscription> _subscriptions = new()
    {
        new Subscription
        {
            Id = 1,
            Name = "Plano Básico",
            Description = "Plano básico com recursos essenciais",
            Price = 29.90m,
            DurationDays = 30,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        },
        new Subscription
        {
            Id = 2,
            Name = "Plano Premium",
            Description = "Plano premium com todos os recursos",
            Price = 99.90m,
            DurationDays = 30,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        }
    };

    /// <summary>
    /// Obtém todas as assinaturas
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<Subscription>> GetAll()
    {
        return Ok(_subscriptions.Where(s => s.IsActive));
    }

    /// <summary>
    /// Obtém uma assinatura por ID
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<Subscription> GetById(int id)
    {
        var subscription = _subscriptions.FirstOrDefault(s => s.Id == id && s.IsActive);
        
        if (subscription == null)
        {
            return NotFound(new { message = "Assinatura não encontrada" });
        }

        return Ok(subscription);
    }

    /// <summary>
    /// Cria uma nova assinatura
    /// </summary>
    [HttpPost]
    public ActionResult<Subscription> Create([FromBody] SubscriptionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newSubscription = new Subscription
        {
            Id = _subscriptions.Count > 0 ? _subscriptions.Max(s => s.Id) + 1 : 1,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            DurationDays = request.DurationDays,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _subscriptions.Add(newSubscription);

        return CreatedAtAction(nameof(GetById), new { id = newSubscription.Id }, newSubscription);
    }

    /// <summary>
    /// Atualiza uma assinatura existente
    /// </summary>
    [HttpPut("{id}")]
    public ActionResult<Subscription> Update(int id, [FromBody] SubscriptionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var subscription = _subscriptions.FirstOrDefault(s => s.Id == id && s.IsActive);
        
        if (subscription == null)
        {
            return NotFound(new { message = "Assinatura não encontrada" });
        }

        subscription.Name = request.Name;
        subscription.Description = request.Description;
        subscription.Price = request.Price;
        subscription.DurationDays = request.DurationDays;
        subscription.UpdatedAt = DateTime.UtcNow;

        return Ok(subscription);
    }

    /// <summary>
    /// Desativa uma assinatura (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var subscription = _subscriptions.FirstOrDefault(s => s.Id == id && s.IsActive);
        
        if (subscription == null)
        {
            return NotFound(new { message = "Assinatura não encontrada" });
        }

        subscription.IsActive = false;
        subscription.UpdatedAt = DateTime.UtcNow;

        return NoContent();
    }
}

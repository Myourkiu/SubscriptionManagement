using SubscriptionManagement.Application.DTOs;
using SubscriptionManagement.Domain.Common;
using SubscriptionManagement.Domain.Models;
using SubscriptionManagement.Infrastructure.Repositories;

namespace SubscriptionManagement.Infrastructure.Services;

public class PlanService
{
    private readonly PlanRepository _planRepository;

    public PlanService(PlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    public async Task<List<Plan>> GetAllAsync()
    {
        return await _planRepository.GetAllAsync();
    }

    public async Task<Result<Plan>> CreateAsync(CreatePlanRequest request)
    {
        try
        {
            var plan = new Plan(name: request.Name, durationInDays: request.DurationInDays);
            var createdPlan = await _planRepository.CreateAsync(plan);
            return Result<Plan>.Success(createdPlan);
        }
        catch (Exception ex)
        {
            return Result<Plan>.Failure($"Erro ao criar plano: {ex.Message}");
        }
    }
}

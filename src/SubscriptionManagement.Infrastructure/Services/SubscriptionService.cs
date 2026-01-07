using SubscriptionManagement.Application.DTOs;
using SubscriptionManagement.Domain.Common;
using SubscriptionManagement.Domain.Models;
using SubscriptionManagement.Domain.ValueObjects;
using SubscriptionManagement.Infrastructure.Repositories;

namespace SubscriptionManagement.Infrastructure.Services
{
    public class SubscriptionService
    {
        private readonly SubscriptionRepository _subscriptionRepository;
        private readonly PlanRepository _planRepository;

        public SubscriptionService(SubscriptionRepository subscriptionRepository, PlanRepository planRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
        }

        public async Task<List<Subscription>> GetAllAsync(GetSubscriptionsRequest? filters = null)
        {
            await _subscriptionRepository.ExpireSubscriptionsAsync();
            return await _subscriptionRepository.GetAllAsync(filters);
        }

        public async Task<Result<Subscription>> CreateAsync(CreateSubscriptionRequest request)
        {
            try
            {
                var existingSubscriptions = await _subscriptionRepository.GetAllAsync();
                if (existingSubscriptions.FirstOrDefault(s => s.IsActive()) != null)
                {
                    return Result<Subscription>.Failure("Não é permitido ativar uma nova assinatura quando já existe uma ativa");
                }

                var plan = await _planRepository.GetByIdAsync(request.PlanId);
                if (plan == null)
                {
                    return Result<Subscription>.Failure("Plano não encontrado");
                }

                var period = new SubscriptionPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(plan.DurationInDays));

                var subscription = new Subscription
                {
                    Plan = plan,
                    Period = period,
                    Status = SubscriptionStatus.Active
                };

                var createdSubscription = await _subscriptionRepository.CreateAsync(subscription);
                return Result<Subscription>.Success(createdSubscription);
            }
            catch (Exception ex)
            {
                return Result<Subscription>.Failure($"Erro ao criar assinatura: {ex.Message}");
            }
        }

        public async Task<Result<Subscription>> CancelAsync(Guid id)
        {
            try
            {
                var subscription = await _subscriptionRepository.GetByIdAsync(id);
                if (subscription == null)
                {
                    return Result<Subscription>.Failure("Assinatura não encontrada");
                }

                if (subscription.Status == SubscriptionStatus.Canceled)
                {
                    return Result<Subscription>.Failure("Assinatura já está cancelada");
                }

                subscription.Cancel();
                var updatedSubscription = await _subscriptionRepository.UpdateAsync(subscription);
                return Result<Subscription>.Success(updatedSubscription);
            }
            catch (Exception ex)
            {
                return Result<Subscription>.Failure($"Erro ao cancelar assinatura: {ex.Message}");
            }
        }
    }
}
using SubscriptionManagement.Domain.Exceptions;

namespace SubscriptionManagement.Domain.Models;

public class Plan
{
    public Guid Id { get; set; }
    public string Name { get; private set; } = string.Empty;
    public int DurationInDays { get; private set; } = 0;

    public Plan(string name, int durationInDays)
    {
        ValidatePlan(name, durationInDays);

        Name = name;
        DurationInDays = durationInDays;
    }

    private static void ValidatePlan(string name, int durationInDays)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new DomainException("Nome do plano não pode ser vazio");
        }
        if (durationInDays <= 0)
        {
            throw new DomainException("Duração do plano deve ser maior que 0");
        }
    }
}

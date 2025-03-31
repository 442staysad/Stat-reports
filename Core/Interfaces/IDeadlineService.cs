using Core.Enums;

namespace Core.Interfaces
{
    public interface IDeadlineService
    {
        Task CheckAndUpdateDeadlineAsync(int templateId);
        DateTime CalculateDeadline(DeadlineType deadlineType, int fixedDay, DateTime reportDate);
    }
}
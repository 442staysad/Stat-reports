using Core.Enums;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IDeadlineService
    {
       Task<IEnumerable<SubmissionDeadline>> GetAllAsync();
        Task CheckAndUpdateDeadlineAsync(int templateId);
        DateTime CalculateDeadline(DeadlineType deadlineType, int fixedDay, DateTime reportDate);
    }
}
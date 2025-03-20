// Core/Interfaces/ISubmissionDeadlineRepository.cs
using Core.Entities;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISubmissionDeadlineRepository : IRepository<SubmissionDeadline>
    {
        Task<SubmissionDeadline> GetDeadlineByTemplateIdAsync(int templateId);
    }
}
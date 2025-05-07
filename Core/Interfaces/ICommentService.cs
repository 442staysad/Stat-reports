
using Core.Enums;
using Core.Entities;

namespace Core.Interfaces
{
    public interface ICommentService
    {
        Task<CommentHistory> AddCommentAsync(int deadlineId, string comment, ReportStatus status, int? changedById);
        Task<IEnumerable<CommentHistory>> GetHistoryAsync(int deadlineId);
    }
}

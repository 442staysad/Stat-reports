using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;
using Core.Entities;

namespace Core.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(int deadlineId, string comment, ReportStatus status, int? changedById);
        Task<IEnumerable<CommentHistory>> GetHistoryAsync(int deadlineId);
    }
}

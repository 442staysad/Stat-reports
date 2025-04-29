using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class DeadlineService : IDeadlineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeadlineService> _logger;

        public DeadlineService(IUnitOfWork unitOfWork, ILogger<DeadlineService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<SubmissionDeadline>> GetAllAsync() => await _unitOfWork.SubmissionDeadlines
                .GetAll(q => q.Include(r => r.Template)
                              .Include(b => b.Branch))
                .ToListAsync();

        public async Task CheckAndUpdateDeadlineAsync(int templateId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var repo = _unitOfWork.SubmissionDeadlines;

                var deadline = await repo.FindAsync(d => d.ReportTemplateId == templateId && !d.IsClosed);
                if (deadline == null) return;

                var isAccepted = await repo
                    .AnyAsync(r => r.ReportTemplateId == templateId && r.Status == ReportStatus.Reviewed);

                if (isAccepted)
                {
                    deadline.DeadlineDate = CalculateNextDeadline(deadline);
                    deadline.Status = ReportStatus.InProgress;

                    deadline.Period = deadline.DeadlineType switch
                    {
                        DeadlineType.Monthly => deadline.Period.AddMonths(1),
                        DeadlineType.Quarterly => deadline.Period.AddMonths(3),
                        DeadlineType.HalfYearly => deadline.Period.AddMonths(6),
                        DeadlineType.Yearly => deadline.Period.AddYears(1),
                        _ => deadline.Period
                    };

                    await repo.UpdateAsync(deadline);
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении дедлайна");
                await transaction.RollbackAsync();
                throw;
            }
        }

        private DateTime AdjustDate(DateTime date, int fixedDay)
        {
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            return new DateTime(date.Year, date.Month, Math.Min(fixedDay, daysInMonth - 1));
        }

        public DateTime CalculateDeadline(DeadlineType deadlineType, int fixedDay, DateTime reportDate)
        {
            return deadlineType switch
            {
                DeadlineType.Monthly => AdjustDate(reportDate.AddMonths(1), fixedDay),
                DeadlineType.Quarterly => AdjustDate(reportDate.AddMonths(3 - (reportDate.Month - 1) % 3), fixedDay),
                DeadlineType.HalfYearly => AdjustDate(reportDate.AddMonths(6 - (reportDate.Month - 1) % 6), fixedDay),
                DeadlineType.Yearly => AdjustDate(reportDate.AddYears(1), fixedDay),
                _ => throw new ArgumentOutOfRangeException(nameof(deadlineType))
            };
        }

        private DateTime CalculateNextDeadline(SubmissionDeadline deadline)
        {
            return deadline.DeadlineType switch
            {
                DeadlineType.Monthly => AdjustDate(deadline.DeadlineDate.AddMonths(1), deadline.FixedDay ?? 30),
                DeadlineType.Quarterly => AdjustDate(deadline.DeadlineDate.AddMonths(3 - (deadline.DeadlineDate.Month - 1) % 3), deadline.FixedDay ?? 30),
                DeadlineType.HalfYearly => AdjustDate(deadline.DeadlineDate.AddMonths(6 - (deadline.DeadlineDate.Month - 1) % 6), deadline.FixedDay ?? 30),
                DeadlineType.Yearly => AdjustDate(deadline.DeadlineDate.AddYears(1), deadline.FixedDay ?? 30),
                _ => DateTime.UtcNow
            };
        }
    }
}

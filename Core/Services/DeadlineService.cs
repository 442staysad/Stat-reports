using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class DeadlineService : IDeadlineService
    {
        private readonly IRepository<SubmissionDeadline> _deadlineRepository;
        private readonly IRepository<Report> _reportRepository;
        private readonly ILogger<DeadlineService> _logger;
        public DeadlineService(
            IRepository<SubmissionDeadline> deadlineRepository,
            IRepository<Report> reportRepository, ILogger<DeadlineService> logger)
        {
            _deadlineRepository = deadlineRepository;
            _reportRepository = reportRepository;
            _logger = logger;

        }

        public async Task CheckAndUpdateDeadlineAsync(int templateId)
        {
            using var transaction = await _deadlineRepository.BeginTransactionAsync();

            try
            {
                var deadline = await _deadlineRepository
                    .FindAsync(d => d.ReportTemplateId == templateId && !d.IsClosed);

                if (deadline == null) return;

                // Проверяем, есть ли принятые отчеты
                var isAccepted = await _reportRepository
                    .AnyAsync(r => r.TemplateId == templateId && r.Status == ReportStatus.Reviewed);

                if (isAccepted || deadline.DeadlineDate <= DateTime.UtcNow)
                {
                    deadline.IsClosed = true;
                    await _deadlineRepository.UpdateAsync(deadline);

                    var newDeadline = new SubmissionDeadline
                    {
                        ReportTemplateId = templateId,
                        DeadlineType = deadline.DeadlineType,
                        DeadlineDate = CalculateNextDeadline(deadline),
                        IsClosed = false
                    };

                    await _deadlineRepository.AddAsync(newDeadline);
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private DateTime AdjustDate(DateTime date, int fixedDay)
        {
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            int day = Math.Min(fixedDay, daysInMonth-1);
            return new DateTime(date.Year, date.Month, day);
        }

        public DateTime CalculateDeadline(DeadlineType deadlineType, int fixedDay, DateTime reportDate)
        {
            return deadlineType switch
            {
                // Месячный дедлайн: фиксированный день следующего месяца
                DeadlineType.Monthly => AdjustDate(reportDate.AddMonths(1), fixedDay),
                // Квартальный дедлайн: фиксированный день следующего квартала
                DeadlineType.Quarterly => AdjustDate(reportDate.AddMonths(3 - (reportDate.Month - 1) % 3), fixedDay),
                // Полугодовой дедлайн: фиксированный день через 6 месяцев
                DeadlineType.HalfYearly => AdjustDate(reportDate.AddMonths(6 - (reportDate.Month - 1) % 6), fixedDay),
                // Годовой дедлайн: фиксированный день следующего года
                DeadlineType.Yearly => AdjustDate(reportDate.AddYears(1), fixedDay),
                _ => throw new ArgumentOutOfRangeException(nameof(deadlineType), "Неизвестный тип дедлайна.")
            };
        }

        private DateTime CalculateNextDeadline(SubmissionDeadline deadline)
        {
            var now = DateTime.UtcNow;
            return deadline.DeadlineType switch
            {
                DeadlineType.Monthly => AdjustDate(now.AddMonths(1), deadline.FixedDay ?? 30),
                DeadlineType.Quarterly => AdjustDate(now.AddMonths(3 - (now.Month - 1) % 3), deadline.FixedDay ?? 30),
                DeadlineType.HalfYearly => AdjustDate(now.AddMonths(6 - (now.Month - 1) % 6), deadline.FixedDay ?? 30),
                DeadlineType.Yearly => AdjustDate(now.AddYears(1), deadline.FixedDay ?? 30),
                _ => now
            };
        }
    }
}

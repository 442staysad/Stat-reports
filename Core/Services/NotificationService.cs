using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Report> _reportRepository;
        private readonly IRepository<SubmissionDeadline> _deadlineRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IRepository<Report> reportRepository,
            IRepository<SubmissionDeadline> deadlineRepository,
            ILogger<NotificationService> logger)
        {
            _reportRepository = reportRepository;
            _deadlineRepository = deadlineRepository;
            _logger = logger;
        }

        public async Task CheckOverdueReportsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var overdueReports = (await _reportRepository.GetAllAsync())
                .Where(r => r.Status != Core.Enums.ReportStatus.Проверено)
                .Join(await _deadlineRepository.GetAllAsync(),
                    report => report.TemplateId,
                    deadline => deadline.ReportTemplateId,
                    (report, deadline) => new { Report = report, Deadline = deadline.Deadline })
                .Where(r => r.Deadline < today)
                .ToList();

            foreach (var overdue in overdueReports)
            {
                string subject = $"Просроченный отчет: {overdue.Report.Name}";
                string message = $"Отчет {overdue.Report.Name} не был сдан в срок ({overdue.Deadline:yyyy-MM-dd}). Пожалуйста, проверьте.";
                
            }

            _logger.LogInformation($"Проверено {overdueReports.Count} просроченных отчетов.");
        }
    }
}

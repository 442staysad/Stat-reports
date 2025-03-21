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


    }
}

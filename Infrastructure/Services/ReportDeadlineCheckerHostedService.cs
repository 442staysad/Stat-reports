using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class ReportDeadlineCheckerHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ReportDeadlineCheckerHostedService> _logger;

        public ReportDeadlineCheckerHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<ReportDeadlineCheckerHostedService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        await notificationService.CheckOverdueReportsAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при проверке просроченных отчетов: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Проверка раз в сутки
            }
        }
    }
}
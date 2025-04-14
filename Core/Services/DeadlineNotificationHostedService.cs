using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Services
{
    public class DeadlineNotificationHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DeadlineNotificationHostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var deadlineService = scope.ServiceProvider.GetRequiredService<IDeadlineService>();
                var reportService = scope.ServiceProvider.GetRequiredService<IReportService>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var branchService = scope.ServiceProvider.GetRequiredService<IBranchService>();

                var deadlines = await deadlineService.GetAllAsync();
                var today = DateTime.UtcNow.Date;

                foreach (var deadline in deadlines)
                {
                    var reportTemplateId = deadline.ReportTemplateId;
                    var deadlineDate = deadline.DeadlineDate;

                    var allBranches = await branchService.GetAllBranchesAsync();

                    foreach (var branch in allBranches)
                    {
                        var users = await userService.GetUsersByBranchIdAsync(branch.Id);
                        var report = await reportService.FindByTemplateBranchPeriodAsync(reportTemplateId, branch.Id, deadlineDate.Year, deadlineDate.Month);

                        if (report == null)
                        {
                            var daysLeft = (deadlineDate - today).Days;

                            string? message = daysLeft switch
                            {
                                10 => $"Через 10 дней наступает срок сдачи отчета: {deadline.Template.Name}",
                                0 => $"Сегодня последний день сдачи отчета: {deadline.Template.Name}",
                                < 0 => $"Срок сдачи отчета '{deadline.Template.Name}' истек!",
                                _ => null
                            };

                            if (message != null)
                            {
                                foreach (var user in users)
                                {
                                    await notificationService.AddNotificationAsync(user.Id, message);
                                }
                            }
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Запускаем раз в сутки
            }
        }
    }
}

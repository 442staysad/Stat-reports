using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Enums;

namespace Core.Interfaces
{
    public interface IReportTemplateService
    {
        Task<SubmissionDeadline> CreateSubmissionDeadlineAsync(SubmissionDeadline deadline);
        Task<IEnumerable<ReportTemplate>> GetAllReportTemplatesAsync();
        Task<ReportTemplate> GetReportTemplateByIdAsync(int id);
        Task<ReportTemplate> CreateReportTemplateAsync(ReportTemplate template, DeadlineType deadlineType, int FixedDay, DateTime ReportDate);
        Task<ReportTemplate> UpdateReportTemplateAsync(ReportTemplate template);
        Task<bool> DeleteReportTemplateAsync(int id);
    }
}

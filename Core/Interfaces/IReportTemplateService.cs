using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IReportTemplateService
    {
        Task<IEnumerable<ReportTemplate>> GetAllReportTemplatesAsync();
        Task<ReportTemplate> GetReportTemplateByIdAsync(int id);
        Task<ReportTemplate> CreateReportTemplateAsync(ReportTemplate template);
        Task<ReportTemplate> UpdateReportTemplateAsync(ReportTemplate template);
        Task<bool> DeleteReportTemplateAsync(int id);
    }
}

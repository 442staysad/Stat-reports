using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.Services
{
    public class ReportTemplateService : IReportTemplateService
    {
        private readonly IRepository<ReportTemplate> _reportTemplateRepository;

        public ReportTemplateService(IRepository<ReportTemplate> reportTemplateRepository)
        {
            _reportTemplateRepository = reportTemplateRepository;
        }

        public async Task<IEnumerable<ReportTemplate>> GetAllReportTemplatesAsync()
        {
            return await _reportTemplateRepository.GetAllAsync();
        }

        public async Task<ReportTemplate> GetReportTemplateByIdAsync(int id)
        {
            return await _reportTemplateRepository.FindAsync(r => r.Id == id);
        }

        public async Task<ReportTemplate> CreateReportTemplateAsync(ReportTemplate template)
        {
            await _reportTemplateRepository.AddAsync(template);
            return template;
        }

        public async Task<ReportTemplate> UpdateReportTemplateAsync(ReportTemplate template)
        {
            await _reportTemplateRepository.UpdateAsync(template);
            return template;
        }

        public async Task<bool> DeleteReportTemplateAsync(int id)
        {
            var template = await _reportTemplateRepository.FindAsync(r => r.Id == id);
            if (template == null) return false;
            await _reportTemplateRepository.UpdateAsync(template);
            return true;
        }
    }
}

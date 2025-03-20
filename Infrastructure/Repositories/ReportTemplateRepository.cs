using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReportTemplateRepository: Repository<ReportTemplate>, IReportTemplateRepository
    {
        private readonly ApplicationDbContext _context;
        public ReportTemplateRepository(ApplicationDbContext context) : base(context) { _context = context; }
        public async Task<ReportTemplate> GetTemplateByNameAsync(string name)
        {
            return await _context.ReportTemplates.Where(r => r.Name == name).FirstAsync();
        }
    }
}

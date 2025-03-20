// Infrastructure/Repositories/ReportRepository.cs
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReportRepository : Repository<Report>, IReportRepository
    {
        private readonly ApplicationDbContext _context;
        public ReportRepository(ApplicationDbContext context) : base(context) { _context = context; }

        public async Task<IEnumerable<Report>> GetReportsByTemplateIdAsync(int templateId)
        {
            return await _context.Reports.Where(r => r.TemplateId == templateId).ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetReportsByBranchIdAsync(int branchId)
        {
            return await _context.Reports.Where(r => r.BranchId == branchId).ToListAsync();
        }
    }
}
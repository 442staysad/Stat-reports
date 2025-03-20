using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SummaryReportRepository : Repository<SummaryReport>, ISummaryReportRepository
    {
        private readonly ApplicationDbContext _context;

        public SummaryReportRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SummaryReport>> GetByPeriodAsync(DateTime periodStart, DateTime periodEnd)
        {
            return await _context.SummaryReports
                .Where(sr => sr.PeriodStart >= periodStart && sr.PeriodEnd <= periodEnd)
                .ToListAsync();
        }
    }
}
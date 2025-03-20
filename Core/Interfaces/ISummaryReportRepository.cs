using Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISummaryReportRepository : IRepository<SummaryReport>
    {
        Task<IEnumerable<SummaryReport>> GetByPeriodAsync(DateTime periodStart, DateTime periodEnd);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Interfaces
{
    public interface IReportRepository
    {
        Task<IEnumerable<Report>> GetReportsByTemplateIdAsync(int templateId);

        Task<IEnumerable<Report>> GetReportsByBranchIdAsync(int branchId);
    }
}

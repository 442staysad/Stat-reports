using Core.Entities;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReportTemplateRepository : IRepository<ReportTemplate>
    {
        Task<ReportTemplate> GetTemplateByNameAsync(string name);
    }
}
using Core.Entities;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReportTemplateRepository 
    {
        Task<ReportTemplate> GetTemplateByNameAsync(string name);
    }
}
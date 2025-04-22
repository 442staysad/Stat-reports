using Core.Enums;

namespace Stat_reports.ViewModels
{
    public class ReportFilterViewModel
    {
        public string? Name { get; set; }
        public int? TemplateId { get; set; }
        public int? BranchId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ReportType? Type { get; set; }
    }
}

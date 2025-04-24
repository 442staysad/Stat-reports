using Core.Enums;

namespace Stat_reports.ViewModels
{
    public class ExcelPreviewViewModel
    {
        public string? BranchName { get; set; }
        public string? ReportType { get; set; }
        public int DeadlineId { get; set; }
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public Dictionary<string, Dictionary<string, List<List<string>>>> ExcelData { get; set; } // Исправляем тип
        public string? Comment { get; set; }
        public ReportStatus Status { get; set; }
    }
}

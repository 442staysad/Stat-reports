namespace Stat_reports.ViewModels
{
    public class PendingTemplateViewModel
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public DateTime Deadline { get; set; }
        public string? Status { get; set; }
        public string? Comment { get; set; }
        public int? ReportId { get; set; } // ID загруженного отчета (если есть)
    }
}

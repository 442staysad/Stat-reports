namespace Stat_reports.ViewModels
{
    public class PendingTemplateViewModel
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public DateTime Deadline { get; set; }
        public string? Status { get; set; }
        public string? Comment { get; set; }
        public int? reportId { get; set; }
        public IFormFile? File { get; set; }
    }
}

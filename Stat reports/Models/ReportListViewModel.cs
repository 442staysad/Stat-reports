namespace Stat_reports.Models
{
    public class ReportListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TemplateName { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; }
        public string Status { get; set; } // Статус отчета
        public string BranchName { get; set; } // Название филиала
    }
}

using Core.Entities;
using Core.Enums;

namespace Stat_reports.Models
{
    public class ReportModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TemplateId { get; set; }
        public ReportTemplate Template { get; set; }
        public DateTime UploadDate { get; set; }
        public int UploadedById { get; set; }
        public User UploadedBy { get; set; }

        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public ReportStatus? Status { get; set; }
        public string? FilePath { get; set; } // Путь к файлу

        public ICollection<ReportAccess> Accesses { get; set; } = new List<ReportAccess>();
        public string? Comment { get; set; }
    }
}

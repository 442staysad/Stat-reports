using Core.Entities;

namespace Core.DTO
{
    public class ReportDto:BaseDTO
    {
        public string Name { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int UploadedById { get; set; }
        public int? BranchId { get; set; }  
        public int? TemplateId { get; set; }  
        public ReportStatus Status { get; set; }
        public string? FilePath { get; set; }
        public string? Comment { get; set; }
    }
}
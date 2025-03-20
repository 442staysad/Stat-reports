using Core.Enums;
namespace Core.DTO
{
    public class ReportDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int UploadedById { get; set; }
        public int? BranchId { get; set; }  // Должно быть int?
        public int? TemplateId { get; set; }  // Должно быть int?
        public ReportStatus Status { get; set; }
        public string FilePath { get; set; }
        public string Comment { get; set; }
    }
}
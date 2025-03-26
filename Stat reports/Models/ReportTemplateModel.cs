using Core.Entities;

namespace Stat_reports.Models
{
    public class ReportTemplateModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? FilePath { get; set; } // Путь к файлу шаблона
        public IFormFile? File { get; set; } // Для загрузки файла отчета
        public ICollection<Report>? Reports { get; set; }

        public SubmissionDeadline? SubmissionDeadline { get; set; }
    }
}

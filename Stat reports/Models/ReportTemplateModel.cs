using Core.Entities;

namespace Stat_reports.Models
{
    public class ReportTemplateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Fields { get; set; } // JSON-структура полей
        public string? FilePath { get; set; } // Путь к файлу шаблона
        public ICollection<Report> Reports { get; set; } = new List<Report>();

        public SubmissionDeadline SubmissionDeadline { get; set; }
    }
}

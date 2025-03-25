using Microsoft.AspNetCore.Mvc.Rendering;

namespace Stat_reports.Models
{
    public class ReportCreateEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TemplateId { get; set; }
        public int BranchId { get; set; }
        public string? Fields { get; set; }
        public string? Comment { get; set; }
        public IFormFile? File { get; set; } // Для загрузки файла отчета

        public IEnumerable<SelectListItem> Templates { get; set; } // Список шаблонов
        public IEnumerable<SelectListItem> Branches { get; set; } // Список филиалов
    }
}

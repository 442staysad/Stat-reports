using System.ComponentModel.DataAnnotations;

namespace Core.Enums
{
    public enum ReportStatus
    {
        [Display(Name = "Черновик")]
        Draft,

        [Display(Name = "Нужна корректировка")]
        NeedsCorrection,

        [Display(Name = "Принято")]
        Reviewed,

        [Display(Name = "В работе")]
        InProgress
    }
}

using Core.Entities;

public enum PeriodType { Monthly, Quarterly, HalfYearly, Yearly }

public class SummaryReportGenerationViewModel
{
    public int? SelectedTemplateId { get; set; }
    public PeriodType? PeriodType { get; set; }

    public int? Year { get; set; }
    public int? Month { get; set; }           // для месячного
    public int? Quarter { get; set; }         // для квартального
    public int? HalfYearPeriod { get; set; }  // 1 или 2

    public List<int> SelectedBranchIds { get; set; } = new();

    public List<ReportTemplate> Templates { get; set; } = new();
    public List<Branch> Branches { get; set; } = new();
}
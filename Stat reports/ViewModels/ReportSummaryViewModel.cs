namespace Stat_reports.ViewModels
{
    public class ReportSummaryViewModel
    {
        public List<BranchViewModel> AvailableBranches { get; set; } = new();
        public int[] SelectedBranchIds { get; set; } = Array.Empty<int>();
        public string SelectedReportType { get; set; } = string.Empty;
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }
    }

    public class BranchViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

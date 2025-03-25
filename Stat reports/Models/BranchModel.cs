using System.ComponentModel.DataAnnotations;

namespace Stat_reports.Models
{
    public class BranchModel
    {
        public int Id { get; set; }
        public string? GoverningName { get; set; }
        public string? HeadName { get; set; }
        public string? Name { get; set; }
        public string? Shortname { get; set; }
        [Required]
        public string? UNP { get; set; } // Используется для входа филиала
        public string? OKPO { get; set; }
        public string? OKYLP { get; set; }
        public string? Region { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Supervisor { get; set; }
        public string? ChiefAccountant { get; set; }


    }
}

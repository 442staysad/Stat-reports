using System.ComponentModel.DataAnnotations;

namespace Stat_reports.Models
{
    public class RegisterBranchModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string UNP { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterUserModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

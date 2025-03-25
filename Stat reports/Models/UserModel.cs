using Core.Entities;

namespace Stat_reports.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string? Number { get; set; }
        public string? Email { get; set; }
        public string? Position { get; set; }
        public int? AccessId { get; set; }
        public string PasswordHash { get; set; }
        public string? Role { get; set; }
        public int? BranchId { get; set; } // Для пользователей филиалов

        public Branch Branch { get; set; }
    }
}

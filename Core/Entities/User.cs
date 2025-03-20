using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class User:BaseEntity
    {
        public string UserName { get; set; }
        public int AccessId {  get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public int? BranchId { get; set; } // Для пользователей филиалов
        public Branch Branch { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Employee:BaseEntity
    {
        public string Post {  get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }

    }
}

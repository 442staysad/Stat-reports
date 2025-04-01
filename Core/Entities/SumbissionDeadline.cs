using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;
namespace Core.Entities
{
    public class SubmissionDeadline : BaseEntity
    {
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public int ReportTemplateId { get; set; }
        public ReportTemplate Template { get; set; }
        public DeadlineType DeadlineType { get; set; }
        public DateTime DeadlineDate { get; set; }
        public int? FixedDay { get; set; } // Например, 26-е число (если есть)
        public string? Comment { get; set; }
        public bool IsClosed { get; set; } // Новый флаг
    }
}

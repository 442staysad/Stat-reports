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
        public int? ReportTemplateId { get; set; }
        public ReportTemplate? Template { get; set; }
        public DeadlineType DeadlineType { get; set; } // Тип дедлайна (26-е, квартал, полгода, год)
        public int? FixedDay { get; set; } // Например, 26-е число (если есть)
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SubmissionDeadline : BaseEntity
    {
        public int ReportTemplateId { get; set; }
        public ReportTemplate Template { get; set; }
        public DateTime Deadline { get; set; }
    
        public bool NotificationSent { get; set; }
    }
}

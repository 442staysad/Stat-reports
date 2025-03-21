using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ReportTemplate : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Fields { get; set; } // JSON-структура полей
        public ICollection<Report> Reports { get; set; } = new List<Report>();

        public SubmissionDeadline SubmissionDeadline { get; set; }
    }
}


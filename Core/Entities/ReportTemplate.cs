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
        public string? Description { get; set; }

        public string FilePath { get; set; }//ЭТО ФАЙЛ НАДО БУДЕТ ЗАГРУЖАТЬ  
        public ICollection<Report>? Reports { get; set; }

        public SubmissionDeadline? SubmissionDeadline { get; set; }
    }
}


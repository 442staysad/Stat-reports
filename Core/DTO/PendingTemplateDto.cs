using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Enums;

namespace Core.DTO
{
    public class PendingTemplateDto
    {
        public int TemplateId { get; set; }
        public string? TemplateName { get; set; }
        public DateTime Deadline { get; set; }
        public ReportStatus Status { get; set; }
    }
}

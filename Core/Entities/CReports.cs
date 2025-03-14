using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Entities
{
    class CReports:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }
        public int IdForm { get; set; }
        public int UserId {  get; set; }


    }
}

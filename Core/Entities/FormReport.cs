using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Entities
{
    class FormReport:BaseEntity
    {
        public string Name {  get; set; }
        public string Description { get; set; }
        public string Section { get; set; }
        public string Index { get; set; }
        public int IdAccess { get; set; }
        public int TypeId {  get; set; }
        public bool Obligation { get; set; }

    }
}

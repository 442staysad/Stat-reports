using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    class Branch:BaseEntity
    {
        public string GoverningName { get; set; }
        public string HeadName { get; set; } 
        public string BranchName {  get; set; }
        public string Shortname { get; set; }
        public string UNP {  get; set; }
        public string OKPO { get; set; }
        public string OKYLP { get; set; }
        public string Region { get; set; }
        public string Adres { get; set; }
        public EmailAddressAttribute Email {  get; set; }
        public string Supervisor { get; set; }
        public string ChiefAccountant { get; set; }

    }
}

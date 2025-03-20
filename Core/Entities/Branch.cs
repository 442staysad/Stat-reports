﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Branch:BaseEntity
    {
        public string GoverningName { get; set; }
        public string HeadName { get; set; } 
        public string Name {  get; set; }
        public string Shortname { get; set; }
        public string UNP {  get; set; }
        public string OKPO { get; set; }
        public string OKYLP { get; set; }
        public string Region { get; set; }
        public string Adres { get; set; }
        public string Email {  get; set; }
        public string Supervisor { get; set; }
        public string ChiefAccountant { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Report> Reports { get; set; }

    }
}

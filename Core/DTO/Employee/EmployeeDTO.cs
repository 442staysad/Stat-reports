using Core.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTO.Employee
{
    public class EmployeeDTO : BaseDTO
    {
        public string WorkPhoneNumber { get; set; }
        public string WorkEmailAddress { get; set; }

        public EmployeeShortDTO? PersonalData { get; set; }
        //public DepartmentDTO? DepartmentDTO { get; set; }
        //public IEnumerable<ProjectDTO>? EmployeeProjects { get; set; }

    }
}

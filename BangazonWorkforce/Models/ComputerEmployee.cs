using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class ComputerEmployee
    {
        [Display(Name = "Computer Assigned on:")]
        public DateTime AssignDate { get; set; }
        public DateTime UnassignDate { get; set; }

        public List<Employee> EmployeeList { get; set; }
        public Employee Employee { get; set; }

    }
}

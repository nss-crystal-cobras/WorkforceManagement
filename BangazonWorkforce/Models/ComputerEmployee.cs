using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class ComputerEmployee
    {
        public DateTime AssignDate { get; set; }
        public DateTime UnassignDate { get; set; }

        public List<Employee> EmployeeList { get; set; }
        public Employee Employee { get; set; }

    }
}

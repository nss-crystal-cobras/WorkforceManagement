using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Department
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(55)]
        public string Name { get; set; }
        [Required]
        public int Budget { get; set; }
        public List<Employee> EmployeeList { get; set; } = new List<Employee>();
    }
}

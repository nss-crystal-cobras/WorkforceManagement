using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class DepartmentDetailsViewModel
    {
        Department department { get; set; }
        List<Employee> employees = new List<Employee>();
    }
}

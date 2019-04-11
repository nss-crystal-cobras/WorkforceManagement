using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsSupervisor { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public Computer Computer { get; set; } = new Computer();
        public List<TrainingProgram> TrainingProgramList { get; internal set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(55)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(55)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

        [Required]
        public bool IsSupervisor { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public Computer Computer { get; set; } = new Computer();

        public List<TrainingProgram> TrainingProgramList { get; set; }
        public TrainingProgram TrainingProgram { get; set; }

        public List<TrainingProgram> EmployeeTraining { get; set; }
        //public TrainingProgram TrainingProgram { get; set; } = new TrainingProgram();
    }
}
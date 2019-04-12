using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Employee
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(55)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(55)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Instructor Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
        [Required]
        public bool IsSupervisor { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public Computer Computer { get; set; } = new Computer();
        public TrainingProgram TrainingProgram { get; set; } = new TrainingProgram();
    }
}
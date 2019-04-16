using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace BangazonWorkforce.Models
{

    public class TrainingProgram
    {
        public int Id { get; set; }

        [Display (Name = "Training Program:")]
        public string Name { get; set; }

        [Display (Name = "Training Program Start Date:")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Training Program End Date:")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Program Atendees (Max):")]
        public int MaxAttendees { get; set; }

        public List<Employee> Employees { get; set; }
    }

}






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
        [Display(Name = "Training Program's Name")]
        public string Name { get; set; }
        [Display(Name = "Program Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Program End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Max Attendees")]
        public int MaxAttendees { get; set; }
    }

}






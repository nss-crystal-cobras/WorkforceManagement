using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{

    //JD -- This model will allow the editing of the employee with a dropdown.

    public class EmployeeEditViewModel
    {

        public Employee Employee { get; set; }
        public Department Department { get; set; }
        public TrainingProgram TrainingProgram { get; set; }
        public Computer Computer { get; set; }
        public List<TrainingProgram> TrainingPrograms { get; set; }
        public List<Department> Departments { get; set; }
      
        public List<SelectListItem> DepartmentOptions
        {
            get
            {
                if (Departments == null)
                {
                    return null;
                }

                return Departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();
            }
        }
        public List<SelectListItem> TrainingProgramOptions
        {
            
            get
            {
                DateTime today = DateTime.Now;
                if (TrainingPrograms == null && TrainingProgram.StartDate > today)
                {
                    return null;
                }

                return TrainingPrograms.Select(tp => new SelectListItem
                {
                    Value = tp.Id.ToString(),
                    Text = tp.Name,
                }).ToList();
            }
        }

    }
}

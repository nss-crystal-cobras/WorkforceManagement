﻿using System;
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
        [Display(Name="Department Name")]
        public string Name { get; set; }
        [Required]
        public int Budget { get; set; }
        public List<Employee> EmployeeIdList { get; set; } = new List<Employee>();

        [Display(Name="Department Size")]
        public int EmployeeCount
        {
            get
            {
                return EmployeeIdList.Count();
            }
        }
    }
}

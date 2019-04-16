using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Computer
    {
        //Added display's for the names.-JD

        [Display (Name = "Computer Id")]
        public int Id { get; set; }
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }
        [Display(Name = "Decommission Date")]
        public DateTime? DecommissionDate { get; set; }

        public string Make { get; set; }
        public Employee Employee { get; set; }
        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }
        [Display(Name = "Computer Name")]
        public string ComputerFullName
        {
            get
            {
                return $"{Make} {Manufacturer}";
            }
        }
    }
}

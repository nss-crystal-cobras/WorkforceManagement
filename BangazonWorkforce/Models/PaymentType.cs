using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class PaymentType
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Payment Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Account Number")]
        public int AcctNumber { get; set; }
        [Required]
        [Display(Name = "Customer ID")]
        public int CustomerId { get; set; }
    }
}

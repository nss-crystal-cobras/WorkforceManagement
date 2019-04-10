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
        public string Name { get; set; }
        public int AcctNumber { get; set; }
        public int CustomerId { get; set; }
    }
}

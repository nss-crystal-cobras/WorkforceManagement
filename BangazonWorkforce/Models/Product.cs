using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{

    public class Product
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ProductTypeId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        public int Quantity { get; set; }
    }

}

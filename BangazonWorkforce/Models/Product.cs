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
        [Display (Name = "Product Type ID")]
        public int ProductTypeId { get; set; }

        [Required]
        [Display(Name = "Customer ID")]
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Price")]
        public int Price { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Product Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Product Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Product Quantity")]
        public int Quantity { get; set; }
    }

}

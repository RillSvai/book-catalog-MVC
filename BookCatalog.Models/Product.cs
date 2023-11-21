using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        public string? Title { get;set; }
        public string? Description { get; set; }
        [Required]
        public string? ISBN { get; set; }
        [Required]
        [MinLength(3,ErrorMessage = "Name too short, should be at least 3 letters")]
        [RegularExpression(@"[A-Za-z\s]+")]
        public string? Author { get; set; }
        [Required]
        [Range(0, 1000)]
        [Display(Name = "List Price")]
        public decimal ListPrice { get; set; }
        [Required]
        [Range(0, 1000)]
        [Display (Name = "Price for 1-49")]
        public decimal Price { get; set; }
        [Required]
        [Range(0, 1000)]
        [Display(Name = "Price for 50-99")]
        public decimal Price50 { get; set; }

        [Required]
        [Range(0, 1000)]
        [Display(Name = "Price for 100+")]
        public decimal Price100 { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category? Category { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }


    }
}

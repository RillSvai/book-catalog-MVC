using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookCatalogWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Category Name")]
        [MaxLength(30)]
        [MinLength(3,ErrorMessage = "String too short, should be at least 3 letters")]
		[Required(ErrorMessage = "Name of category required! Ти що здурів?")]
        [RegularExpression(@"[A-Za-z]+", ErrorMessage = "Name of category should contain only letters!")]
		public string? Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1,100,ErrorMessage = "Value should be beetwen 1 and 100! Розбійнику")]
		public int DisplayOrder { get; set; }
    }
}

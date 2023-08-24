using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCartWeb.Models.Dto
{
    public class CategoryMasterDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [RegularExpression(@"^[a-zA-Z '-]{2,20}$", ErrorMessage = "Invalid Category")]
        public string CategoryName { get; set; }



        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCartWeb.Models.Dto
{
    public class RoleMasterDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression(@"^[a-zA-Z '-]{2,20}$", ErrorMessage = "Invalid Role")]
        public string RoleName { get; set; }



        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

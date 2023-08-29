using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ShoppingCartWeb.Models.Dto
{
    public class UserDTO
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int UserId { get; set; }

		[Required(ErrorMessage = "UserName is required")]
		[EmailAddress(ErrorMessage = "Invalid UserName")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public RegistrationDTO Registration { get; set; }

		[Required]
        [ValidateNever]
        [ForeignKey("Registration")]
		public int RegistrationId { get; set; }

		public RoleMasterDTO RoleMaster { get; set; }

		[Required]
		[ValidateNever]
		[ForeignKey("RoleMaster")]
		public int RoleId { get; set; }
		public int CreatedBy { get; set; }
		public DateTime CreatedOn { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime UpdatedOn { get; set; }
		public bool IsDeleted { get; set; } = false;
	}
}

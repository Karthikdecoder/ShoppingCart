using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCartAPI.Models.Dto
{
    public class UserDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        //[Required(ErrorMessage = "Username is required.")]
        //[EmailAddress(ErrorMessage = "Invalid username format.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [ForeignKey("Registration")]
        public int RegistrationId { get; set; }

        [Required]
        [ForeignKey("RoleMaster")]
        public int RoleId { get; set; }



        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

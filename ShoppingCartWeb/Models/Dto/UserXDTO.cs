using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCartWeb.Models.Dto
{
    public class UserXDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

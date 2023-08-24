using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Models.VM
{
    public class UserRegisterationVM
    {
        public UserRegisterationVM()
        {
            User = new UserDTO();
        }
        public UserDTO User { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> RegistrationList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}

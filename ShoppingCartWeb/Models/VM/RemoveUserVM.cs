using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Models.VM
{
    public class RemoveUserVM
    {
        public RemoveUserVM()
        {
            User = new UserDTO();
        }
        public UserDTO User { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> registrationList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> roleList { get; set; }
    }
}

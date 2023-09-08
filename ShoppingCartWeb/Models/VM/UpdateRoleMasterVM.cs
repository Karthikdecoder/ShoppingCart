using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Models.VM
{
    public class UpdateRoleMasterVM
    {
        public RoleMasterDTO RoleMasterDTO { get; set; }

        [ValidateNever]
        public int CurrentPage { get; set; }
    }
}

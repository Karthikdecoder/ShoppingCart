using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ShoppingCartWeb.Models.VM
{
    public class UpdateCategoryMasterVM
    {


        [ValidateNever]
        public int CurrentPage { get; set; }
    }
}

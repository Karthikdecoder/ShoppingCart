using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb.Models.VM
{
	public class CreateRegistrationVM
	{
		public CreateRegistrationVM()
		{
			Registration = new RegistrationDTO();
		}
		public RegistrationDTO Registration { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem> CategoryList { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem> StateList { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem> CountryList { get; set; }
	}
}

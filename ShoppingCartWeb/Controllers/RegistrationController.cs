using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Models.VM;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShoppingCartWeb.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IUserService _authService;
        private readonly IRoleService _roleService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private string _Role;
        public RegistrationController(IUserService authService, IHttpContextAccessor httpContextAccessor, IRoleService roleService, ICategoryService categoryService, IMapper mapper)
        {
            _authService = authService;
            _Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            _roleService = roleService;
            _categoryService = categoryService;
            _mapper = mapper;
        }

		public async Task<IActionResult> IndexRegistration()
		{
			List<RegistrationDTO> list = new();

			var response = await _authService.GetAllUserAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

			if (response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<RegistrationDTO>>(Convert.ToString(response.Result));
			}

			return View(list);
		}

		[HttpGet]
        public async Task<IActionResult> CreateRegistration()
        {
            CategoryMasterVM categoryMaster = new();

            var response = await _categoryService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess) 
            {
                categoryMaster.CategoryList = JsonConvert.DeserializeObject<List<CategoryMasterDTO>>(Convert.ToString(response.Result)).Select(i => new SelectListItem
                    {
                        Text = i.CategoryName,
                        Value = i.CategoryId.ToString()
                    }); 
            }

            return View(categoryMaster);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateRegistration(RoleMasterCreateVM obj)
        //{

        //    APIResponse result = await _authService.RegisterAsync<APIResponse>(obj.Registration, HttpContext.Session.GetString(SD.SessionToken));

        //    if (result != null && result.IsSuccess)
        //    {
        //        return RedirectToAction("Login", "Auth");
        //    }
        //    return View();

        //}

        public async Task<IActionResult> UpdateRegistration(int registrationId)
        {
            UpdateRegistrationVM updateRegistrationVM = new();
            var response = await _authService.GetUserAsync<APIResponse>(registrationId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                RegistrationDTO model = JsonConvert.DeserializeObject<RegistrationDTO>(Convert.ToString(response.Result));
                updateRegistrationVM.Registration = _mapper.Map<RegistrationDTO>(model);
            }

            response = await _categoryService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                updateRegistrationVM.CategoryList = JsonConvert.DeserializeObject<List<CategoryMasterDTO>>(Convert.ToString(response.Result)).Select(i => new SelectListItem
                {
                    Text = i.CategoryName,
                    Value = i.CategoryId.ToString()
                });
                return View(updateRegistrationVM);
            }

            return NotFound();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateRegistration(UpdateRegistrationVM model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var response = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));

        //        if (response != null && response.IsSuccess)
        //        {
        //            return RedirectToAction(nameof(IndexVillaNumber));
        //        }
        //        else
        //        {
        //            if (response.ErrorMessages.Count > 0) // error here!!!!!!!!
        //            {
        //                ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
        //            }
        //        }
        //    }

        //    var resp = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
        //    if (resp != null && resp.IsSuccess)
        //    {
        //        model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
        //            (Convert.ToString(resp.Result)).Select(i => new SelectListItem
        //            {
        //                Text = i.Name,
        //                Value = i.Id.ToString()
        //            }); ;
        //    }

        //    return View(model);
        //}
    }

}

using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Models.VM;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;
using System.Data;
using System.Security.Claims;

namespace ShoppingCartWeb.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IRegistrationService _registrationService;
        private readonly IRoleService _roleService;
        private readonly ICategoryService _categoryService;
		private readonly IStateService _stateService;
		private readonly ICountryService _countryService;
		private readonly IMapper _mapper;
        private string _Role;
        public RegistrationController(IRegistrationService registrationService, IHttpContextAccessor httpContextAccessor, IRoleService roleService, ICategoryService categoryService, IStateService stateService, ICountryService countryService, IMapper mapper)
        {
            _registrationService = registrationService;
            _Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            _roleService = roleService;
            _categoryService = categoryService;
            _stateService = stateService;
            _countryService = countryService;
            _mapper = mapper;
        }

		public async Task<IActionResult> IndexRegistration()
		{
			List<RegistrationDTO> list = new();

			var response = await _registrationService.GetAllRegistrationAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

			if (response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<RegistrationDTO>>(Convert.ToString(response.Result));
			}

			return View(list);
		}

		[HttpGet]
        public async Task<IActionResult> CreateRegistration()
        {
			CreateRegistrationVM createRegisterationVM = new();

            var categoryResponse = await _categoryService.GetAllCategoryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (categoryResponse != null && categoryResponse.IsSuccess) 
            {
                createRegisterationVM.CategoryList = JsonConvert.DeserializeObject<List<CategoryMasterDTO>>(Convert.ToString(categoryResponse.Result)).Select(i => new SelectListItem
                    {
                        Text = i.CategoryName,
                        Value = i.CategoryId.ToString()
                    }); 
            }

            var countryResponse = await _countryService.GetAllCountryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (countryResponse != null && countryResponse.IsSuccess)
            {
                createRegisterationVM.CountryList = JsonConvert.DeserializeObject<List<CountryMasterDTO>>(Convert.ToString(countryResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.CountryName,
                    Value = i.CountryId.ToString()
                });
            }

            var stateResponse = await _stateService.GetAllStateAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

			if (stateResponse != null && stateResponse.IsSuccess)
			{
				createRegisterationVM.StateList = JsonConvert.DeserializeObject<List<StateMasterDTO>>(Convert.ToString(stateResponse.Result)).Select(i => new SelectListItem
				{
					Text = i.StateName,
					Value = i.StateId.ToString()
				});
			}

            var countries = countryResponse.Result;

            var states = new List<StateMasterDTO>();

            //countries.Add(new CountryMasterDTO()
            //{
            //    CountryId = 0,
            //    CountryName = "--Select Country--"
            //});

            //states.Add(new StateMasterDTO()
            //{
            //    StateId = 0,
            //    StateName = "--Select State"
            //});

            ViewBag.Countries = new SelectList(states, "CountryId", "CountryName");
            ViewBag.States = new SelectList(states, "StateId", "StateName");

            return View(createRegisterationVM);
        }

        public JsonResult GetStateByCountryId(int countryId)
        {
            var stateResponse = _stateService.GetAllStateByCountryIdAsync<APIResponse>(countryId, HttpContext.Session.GetString(SD.SessionToken));

            return Json(stateResponse.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRegistration(CreateRegistrationVM obj)
        {

            APIResponse result = await _registrationService.CreateRegistrationAsync<APIResponse>(obj.Registration, HttpContext.Session.GetString(SD.SessionToken));

            if (result != null && result.IsSuccess)
            {
                return RedirectToAction("IndexRegistration", "Registration");
            }
            return View();

        }

		public async Task<IActionResult> UpdateRegistration(int registrationId)
		{
			UpdateRegistrationVM updateRegistrationVM = new();
			var registrationResponse = await _registrationService.GetRegistrationAsync<APIResponse>(registrationId, HttpContext.Session.GetString(SD.SessionToken));

			if (registrationResponse != null && registrationResponse.IsSuccess)
			{
				RegistrationDTO model = JsonConvert.DeserializeObject<RegistrationDTO>(Convert.ToString(registrationResponse.Result));
				updateRegistrationVM.Registration = _mapper.Map<RegistrationDTO>(model);
			}

			var categoryResponse = await _categoryService.GetAllCategoryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

			if (categoryResponse != null && categoryResponse.IsSuccess)
			{
				updateRegistrationVM.CategoryList = JsonConvert.DeserializeObject<List<CategoryMasterDTO>>(Convert.ToString(categoryResponse.Result)).Select(i => new SelectListItem
				{
					Text = i.CategoryName,
					Value = i.CategoryId.ToString()
				});
			}

			var stateResponse = await _stateService.GetAllStateAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

			if (stateResponse != null && stateResponse.IsSuccess)
			{
				updateRegistrationVM.StateList = JsonConvert.DeserializeObject<List<StateMasterDTO>>(Convert.ToString(stateResponse.Result)).Select(i => new SelectListItem
				{
					Text = i.StateName,
					Value = i.StateId.ToString()
				});
			}

			var countryResponse = await _countryService.GetAllCountryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

			if (countryResponse != null && countryResponse.IsSuccess)
			{
				updateRegistrationVM.CountryList = JsonConvert.DeserializeObject<List<CountryMasterDTO>>(Convert.ToString(countryResponse.Result)).Select(i => new SelectListItem
				{
					Text = i.CountryName,
					Value = i.CountryId.ToString()
				});

                return View(updateRegistrationVM);
            }

			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateRegistration(UpdateRegistrationVM model)
		{
			if (ModelState.IsValid)
			{
				var response = await _registrationService.UpdateRegistrationAsync<APIResponse>(model.Registration, HttpContext.Session.GetString(SD.SessionToken));

				if (response != null && response.IsSuccess)
				{
					return RedirectToAction(nameof(IndexRegistration));
				}
				else
				{
					if (response.ErrorMessages.Count > 0) 
					{
						ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
					}
				}
			}

			var updateResponse = await _registrationService.GetAllRegistrationAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (updateResponse != null && updateResponse.IsSuccess)
			{
				model.Registration = (RegistrationDTO)JsonConvert.DeserializeObject<List<RegistrationDTO>>
					(Convert.ToString(updateResponse.Result)).Select(i => new SelectListItem
					{
						Text = i.FirstName,
						Value = i.RegistrationId.ToString()
					});
            }

			return View(model);
		}


		public async Task<IActionResult> RemoveRegistration(int registrationId)
        {
            RemoveRegistrationVM removeRegistrationVM = new();
            var registrationResponse = await _registrationService.GetRegistrationAsync<APIResponse>(registrationId, HttpContext.Session.GetString(SD.SessionToken));

            if (registrationResponse != null && registrationResponse.IsSuccess)
            {
                RegistrationDTO model = JsonConvert.DeserializeObject<RegistrationDTO>(Convert.ToString(registrationResponse.Result));
                removeRegistrationVM.Registration = model;
            }

            var categoryResponse = await _categoryService.GetAllCategoryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (categoryResponse != null && categoryResponse.IsSuccess)
            {
                removeRegistrationVM.CategoryList = JsonConvert.DeserializeObject<List<CategoryMasterDTO>>(Convert.ToString(categoryResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.CategoryName,
                    Value = i.CategoryId.ToString()
                });
            }

            var stateResponse = await _stateService.GetAllStateAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (stateResponse != null && stateResponse.IsSuccess)
            {
                removeRegistrationVM.StateList = JsonConvert.DeserializeObject<List<StateMasterDTO>>(Convert.ToString(stateResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.StateName,
                    Value = i.StateId.ToString()
                });
            }

            var countryResponse = await _countryService.GetAllCountryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (countryResponse != null && countryResponse.IsSuccess)
            {
                removeRegistrationVM.CountryList = JsonConvert.DeserializeObject<List<CountryMasterDTO>>(Convert.ToString(countryResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.CountryName,
                    Value = i.CountryId.ToString()
                });
                return View(removeRegistrationVM);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRegistration(RemoveRegistrationVM removeRegistration)
        {
            var response = await _registrationService.RemoveRegistrationAsync<APIResponse>(removeRegistration.Registration.RegistrationId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexRegistration));
            }
            return View(removeRegistration);
        }
    }

}

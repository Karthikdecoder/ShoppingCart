using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Services.IServices;
using ShoppingCartWeb.Utililty;
using System.Security.Claims;

namespace ShoppingCartWeb.Controllers
{
	public class CountryMasterController : Controller
	{
		private readonly IUserService _userService;
		private readonly IRegistrationService _registrationService;
		private readonly IRoleService _roleService;
		private readonly ICategoryService _categoryService;
		private readonly ICountryService _countryService;
		private readonly IStateService _stateService;
		private readonly IMapper _mapper;
		private string _Role;
		public CountryMasterController(IUserService userService, IHttpContextAccessor httpContextAccessor, IRoleService roleService, ICategoryService categoryService, IMapper mapper, IRegistrationService registrationService, IStateService stateService, ICountryService countryService)
		{
			_userService = userService;
			_Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
			_roleService = roleService;
			_categoryService = categoryService;
			_mapper = mapper;
			_registrationService = registrationService;
			_stateService = stateService;
			_countryService = countryService;
		}

		public async Task<IActionResult> IndexCountryMaster()
		{
			List<CountryMasterDTO> list = new();

			var response = await _countryService.GetAllCountryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

			if (response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<CountryMasterDTO>>(Convert.ToString(response.Result));
			}

			return View(list);
		}

		[HttpGet]
		public async Task<IActionResult> CreateCountryMaster()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateCountryMaster(CountryMasterDTO CountryMasterDTO)
		{

			APIResponse result = await _countryService.CreateCountryAsync<APIResponse>(CountryMasterDTO, HttpContext.Session.GetString(SD.SessionToken));

			if (result != null && result.IsSuccess)
			{
				return RedirectToAction("IndexCountryMaster", "CountryMaster");
			}
			return View();

		}

		public async Task<IActionResult> UpdateCountryMaster(int CountryId)
		{
			var CountryResponse = await _countryService.GetCountryAsync<APIResponse>(CountryId, HttpContext.Session.GetString(SD.SessionToken));

			if (CountryResponse != null && CountryResponse.IsSuccess)
			{
				CountryMasterDTO model = JsonConvert.DeserializeObject<CountryMasterDTO>(Convert.ToString(CountryResponse.Result));

				return View(model);
			}

			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateCountryMaster(CountryMasterDTO model)
		{
			if (ModelState.IsValid)
			{
				var response = await _countryService.UpdateCountryAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));

				if (response != null && response.IsSuccess)
				{
					return RedirectToAction(nameof(IndexCountryMaster));
				}
				else
				{
					if (response.ErrorMessages.Count > 0)
					{
						ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
					}
				}
			}
			return View(model);
		}


		public async Task<IActionResult> RemoveCountryMaster(int CountryId)
		{
			var CountryMasterResponse = await _countryService.GetCountryAsync<APIResponse>(CountryId, HttpContext.Session.GetString(SD.SessionToken));

			if (CountryMasterResponse != null && CountryMasterResponse.IsSuccess)
			{
				CountryMasterDTO model = JsonConvert.DeserializeObject<CountryMasterDTO>(Convert.ToString(CountryMasterResponse.Result));

				return View(model);
			}


			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RemoveCountryMaster(CountryMasterDTO CountryMasterDTO)
		{
			var response = await _countryService.RemoveCountryAsync<APIResponse>(CountryMasterDTO.CountryId, HttpContext.Session.GetString(SD.SessionToken));

			if (response != null && response.IsSuccess)
			{
				return RedirectToAction(nameof(IndexCountryMaster));
			}
			return View(CountryMasterDTO);
		}
	}

}



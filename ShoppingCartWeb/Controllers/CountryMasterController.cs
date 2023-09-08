﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

		public async Task<IActionResult> IndexCountryMaster(string orderBy = "", int currentPage = 1)
		{
            CountryPaginationVM countryPaginationVM = new CountryPaginationVM();


            List<CountryMasterDTO> list = new();

			var response = await _countryService.GetAllCountryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

			if (response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<CountryMasterDTO>>(Convert.ToString(response.Result));
            }

            int totalRecords = list.Count();

            int pageSize = 5;

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            countryPaginationVM.CountryMasterDTO = list;
            countryPaginationVM.CurrentPage = currentPage;
            countryPaginationVM.PageSize = pageSize;
            countryPaginationVM.TotalPages = totalPages;

            return View(countryPaginationVM);
        }

		[HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCountryMaster()
		{
			return View();
		}

		[HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateCountryMaster(CountryMasterDTO CountryMasterDTO)
		{
			if (ModelState.IsValid)
			{
				APIResponse result = await _countryService.CreateCountryAsync<APIResponse>(CountryMasterDTO, HttpContext.Session.GetString(SD.SessionToken));

				if (result != null && result.IsSuccess)
				{
                    TempData["success"] = "Created successfully";
                    return RedirectToAction("IndexCountryMaster", "CountryMaster");
				}

                TempData["error"] = result.ResponseMessage[0].ToString();
                return View(CountryMasterDTO);
            }
			return View();

		}

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateCountryMaster(CountryMasterDTO countryMasterDTO)
		{
			if (ModelState.IsValid)
			{
				var response = await _countryService.UpdateCountryAsync<APIResponse>(countryMasterDTO, HttpContext.Session.GetString(SD.SessionToken));

				if (response != null && response.IsSuccess)
				{
                    TempData["success"] = "Updated successfully";
                    return RedirectToAction(nameof(IndexCountryMaster));
				}

                TempData["error"] = response.ResponseMessage[0].ToString();
                return View(countryMasterDTO);
            }
			return View(countryMasterDTO);
		}

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnableCountry(int countryId)
        {
            if (ModelState.IsValid)
            {
                var response = await _countryService.EnableCountryAsync<APIResponse>(countryId, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Enabled successfully";
                    return RedirectToAction("IndexCountryMaster");
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return RedirectToAction(nameof(IndexCountryMaster));
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveCountryMaster(int countryId)
		{
            var response = await _countryService.RemoveCountryAsync<APIResponse>(countryId, HttpContext.Session.GetString(SD.SessionToken));

			if (response != null && response.IsSuccess)
			{
                TempData["success"] = "Deleted successfully";
                return RedirectToAction(nameof(IndexCountryMaster));
			}

            TempData["success"] = "Error encountered";
            return View();
        }

	}

}



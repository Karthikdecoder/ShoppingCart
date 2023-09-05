using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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

        public async Task<IActionResult> IndexRegistration(string orderBy = "", int currentPage = 1)
        {
            RegistrationVM registrationVM = new();

            List<RegistrationDTO> registrationDTOList = new();

            var response = await _registrationService.GetAllRegistrationAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                registrationDTOList = JsonConvert.DeserializeObject<List<RegistrationDTO>>(Convert.ToString(response.Result));
            }

            int totalRecords = registrationDTOList.Count();

            int pageSize = 5;

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            registrationDTOList = registrationDTOList.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            registrationVM.Registration = registrationDTOList;
            registrationVM.CurrentPage = currentPage;
            registrationVM.PageSize = pageSize;
            registrationVM.TotalPages = totalPages;

            return View(registrationVM);
        }

        public async Task<IActionResult> GetAllDeletedRegistration(string orderBy = "", int currentPage = 1)
        {
            RegistrationVM registrationVM = new();

            List<RegistrationDTO> registrationDTOList = new();

            var response = await _registrationService.GetAllDeletedRegistrationAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                registrationDTOList = JsonConvert.DeserializeObject<List<RegistrationDTO>>(Convert.ToString(response.Result));
            }

            int totalRecords = registrationDTOList.Count();

            int pageSize = 5;

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            registrationDTOList = registrationDTOList.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            registrationVM.Registration = registrationDTOList;
            registrationVM.CurrentPage = currentPage;
            registrationVM.PageSize = pageSize;
            registrationVM.TotalPages = totalPages;

            return View(registrationVM);
        }

        public async Task<IActionResult> ViewRegistration(int registrationId)
        {
            RegistrationDTO registrationDetail = new();

            var registrationResponse = await _registrationService.GetRegistrationAsync<APIResponse>(registrationId, HttpContext.Session.GetString(SD.SessionToken));

            if (registrationResponse != null && registrationResponse.IsSuccess)
            {
                RegistrationDTO model = JsonConvert.DeserializeObject<RegistrationDTO>(Convert.ToString(registrationResponse.Result));
                registrationDetail = _mapper.Map<RegistrationDTO>(model);
            }

            return View(registrationDetail);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
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

            return View(createRegisterationVM);
        }

        public async Task<JsonResult> GetStateByCountryId(int countryId)
        {
            CreateRegistrationVM createRegisterationVM = new();

            var stateList = new List<StateMasterDTO>();

            var stateResponse = await _stateService.GetAllStateByCountryIdAsync<APIResponse>(countryId, HttpContext.Session.GetString(SD.SessionToken));

            if (stateResponse != null && stateResponse.IsSuccess)
            {
                stateList = JsonConvert.DeserializeObject<List<StateMasterDTO>>(Convert.ToString(stateResponse.Result));

            }

            return Json(stateList);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRegistration(CreateRegistrationVM createRegistrationVM)
        {
            if (ModelState.IsValid)
            {
                APIResponse response = await _registrationService.CreateRegistrationAsync<APIResponse>(createRegistrationVM.Registration, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Created successfully";
                    return RedirectToAction("IndexRegistration", "Registration");
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return RedirectToAction("CreateRegistration", "Registration");
            }

            return View();

        }

        [Authorize(Roles = "Admin")]
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

            var countryResponse = await _countryService.GetAllCountryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (countryResponse != null && countryResponse.IsSuccess)
            {
                updateRegistrationVM.CountryList = JsonConvert.DeserializeObject<List<CountryMasterDTO>>(Convert.ToString(countryResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.CountryName,
                    Value = i.CountryId.ToString()
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

                return View(updateRegistrationVM);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRegistration(UpdateRegistrationVM updateRegistrationVM)
        {
            if (ModelState.IsValid)
            {
                var response = await _registrationService.UpdateRegistrationAsync<APIResponse>(updateRegistrationVM.Registration, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Updated successfully";
                    return RedirectToAction(nameof(IndexRegistration));
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return RedirectToAction(nameof(IndexRegistration));
            }

            return View(updateRegistrationVM);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnableRegistration(int registrationId)
        {
            if (ModelState.IsValid)
            {
                var response = await _registrationService.EnableRegistrationAsync<APIResponse>(registrationId, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Enabled successfully";
                    return RedirectToAction(nameof(IndexRegistration));
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return RedirectToAction(nameof(IndexRegistration));
            }

            return View();
        }


        //public async Task<IActionResult> RemoveRegistration(int registrationId)
        //      {
        //          RemoveRegistrationVM removeRegistrationVM = new();

        //          var registrationResponse = await _registrationService.GetRegistrationAsync<APIResponse>(registrationId, HttpContext.Session.GetString(SD.SessionToken));

        //          if (registrationResponse != null && registrationResponse.IsSuccess)
        //          {
        //              RegistrationDTO model = JsonConvert.DeserializeObject<RegistrationDTO>(Convert.ToString(registrationResponse.Result));
        //              removeRegistrationVM.Registration = model;
        //          }

        //          var categoryResponse = await _categoryService.GetAllCategoryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

        //          if (categoryResponse != null && categoryResponse.IsSuccess)
        //          {
        //              removeRegistrationVM.CategoryList = JsonConvert.DeserializeObject<List<CategoryMasterDTO>>(Convert.ToString(categoryResponse.Result)).Select(i => new SelectListItem
        //              {
        //                  Text = i.CategoryName,
        //                  Value = i.CategoryId.ToString()
        //              });
        //          }

        //          var stateResponse = await _stateService.GetAllStateAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

        //          if (stateResponse != null && stateResponse.IsSuccess)
        //          {
        //              removeRegistrationVM.StateList = JsonConvert.DeserializeObject<List<StateMasterDTO>>(Convert.ToString(stateResponse.Result)).Select(i => new SelectListItem
        //              {
        //                  Text = i.StateName,
        //                  Value = i.StateId.ToString()
        //              });
        //          }

        //          var countryResponse = await _countryService.GetAllCountryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

        //          if (countryResponse != null && countryResponse.IsSuccess)
        //          {
        //              removeRegistrationVM.CountryList = JsonConvert.DeserializeObject<List<CountryMasterDTO>>(Convert.ToString(countryResponse.Result)).Select(i => new SelectListItem
        //              {
        //                  Text = i.CountryName,
        //                  Value = i.CountryId.ToString()
        //              });
        //              return View(removeRegistrationVM);
        //          }

        //          return NotFound();
        //      }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRegistration(int registrationId)
        {
            var response = await _registrationService.RemoveRegistrationAsync<APIResponse>(registrationId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Deleted successfully";
                return RedirectToAction(nameof(IndexRegistration));
            }

            TempData["success"] = "Error encountered";
            return View();
        }
    }

}

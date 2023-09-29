using AutoMapper;
using Microsoft.AspNetCore.Authentication;
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
using System.Security.Claims;

namespace ShoppingCartWeb.Controllers
{
    public class StateMasterController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICountryService _countryService;
        private readonly IRegistrationService _registrationService;
        private readonly IRoleService _roleService;
        private readonly ICategoryService _categoryService;
        private readonly IStateService _stateService;
        private readonly IMapper _mapper;
        private string _Role;
        public StateMasterController(IUserService userService, IHttpContextAccessor httpContextAccessor, IRoleService roleService, ICategoryService categoryService, IMapper mapper, IRegistrationService registrationService, IStateService stateService, ICountryService countryService)
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

        [Authorize]
        public async Task<IActionResult> IndexStateMaster(string orderBy = "", int currentPage = 1)
        {
            StateMasterPaginationVM stateMasterPaginationVM = new StateMasterPaginationVM();   

            List<StateMasterDTO> list = new();

            var response = await _stateService.GetAllStateAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<StateMasterDTO>>(Convert.ToString(response.Result));
            }

            int totalRecords = list.Count();

            int pageSize = 5;

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            stateMasterPaginationVM.StateMasterDTO = list;
            stateMasterPaginationVM.CurrentPage = currentPage;
            stateMasterPaginationVM.PageSize = pageSize;
            stateMasterPaginationVM.TotalPages = totalPages;

            return View(stateMasterPaginationVM);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CreateStateMaster()
        {
            StateMasterCreateVM stateMasterCreateVM = new StateMasterCreateVM();

            var countryResponse = await _countryService.GetAllCountryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (countryResponse != null && countryResponse.IsSuccess)
            {
                stateMasterCreateVM.CountryList = JsonConvert.DeserializeObject<List<CountryMasterDTO>>(Convert.ToString(countryResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.CountryName,
                    Value = i.CountryId.ToString()
                });

                return View(stateMasterCreateVM);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStateMaster(StateMasterCreateVM stateMasterCreateVM)
        {
            if(ModelState.IsValid)
            {
                APIResponse result = await _stateService.CreateStateAsync<APIResponse>(stateMasterCreateVM.State, HttpContext.Session.GetString(SD.SessionToken));

                if (result != null && result.IsSuccess)
                {
                    TempData["success"] = "Created successfully";
                    return RedirectToAction("IndexStateMaster", "StateMaster");
                }

                TempData["error"] = result.ResponseMessage[0].ToString();
                return View(stateMasterCreateVM);
            }

            TempData["error"] = "Error encountered";
            return View(stateMasterCreateVM);

        }

        [Authorize]
        public async Task<IActionResult> UpdateStateMaster(int StateId)
        {
            StateMasterCreateVM stateMasterCreateVM = new();

            var stateMasterResponse = await _stateService.GetStateAsync<APIResponse>(StateId, HttpContext.Session.GetString(SD.SessionToken));

            if (stateMasterResponse != null && stateMasterResponse.IsSuccess)
            {
                StateMasterDTO model = JsonConvert.DeserializeObject<StateMasterDTO>(Convert.ToString(stateMasterResponse.Result));
                stateMasterCreateVM.State = _mapper.Map<StateMasterDTO>(model);
            }

            var countryResponse = await _countryService.GetAllCountryAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (countryResponse != null && countryResponse.IsSuccess)
            {
                stateMasterCreateVM.CountryList = JsonConvert.DeserializeObject<List<CountryMasterDTO>>(Convert.ToString(countryResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.CountryName,
                    Value = i.CountryId.ToString()

                });

                return View(stateMasterCreateVM);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStateMaster(StateMasterCreateVM stateMasterCreateVM)
        {
            if (ModelState.IsValid)
            {
                var response = await _stateService.UpdateStateAsync<APIResponse>(stateMasterCreateVM.State, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Updated successfully";
                    return RedirectToAction(nameof(IndexStateMaster));
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return RedirectToAction("UpdateStateMaster", new { StateId = stateMasterCreateVM.State.StateId });
            }

            return View(stateMasterCreateVM);
        }

        [Authorize]
        public async Task<IActionResult> EnableStateMaster(int stateId, int currentPageNo)
        {
            if (ModelState.IsValid)
            {
                var response = await _stateService.EnableStateAsync<APIResponse>(stateId, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Enabled successfully";
                    return RedirectToAction("IndexStateMaster", new { currentPage = currentPageNo });
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return RedirectToAction(nameof(IndexStateMaster));
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> RemoveStateMaster(int stateId)
        {
            var response = await _stateService.RemoveStateAsync<APIResponse>(stateId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Deleted successfully";
                return RedirectToAction(nameof(IndexStateMaster));
            }
            TempData["success"] = "Error encountered";
            return View();
        }
    }

}



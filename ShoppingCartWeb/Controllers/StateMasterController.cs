using AutoMapper;
using Microsoft.AspNetCore.Authentication;
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
        private readonly IRegistrationService _registrationService;
        private readonly IRoleService _roleService;
        private readonly ICategoryService _categoryService;
        private readonly IStateService _stateService;
        private readonly IMapper _mapper;
        private string _Role;
        public StateMasterController(IUserService userService, IHttpContextAccessor httpContextAccessor, IRoleService roleService, ICategoryService categoryService, IMapper mapper, IRegistrationService registrationService, IStateService stateService)
        {
            _userService = userService;
            _Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            _roleService = roleService;
            _categoryService = categoryService;
            _mapper = mapper;
            _registrationService = registrationService;
            _stateService = stateService;
        }

        public async Task<IActionResult> IndexStateMaster()
        {
            List<StateMasterDTO> list = new();

            var response = await _stateService.GetAllStateAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<StateMasterDTO>>(Convert.ToString(response.Result));
            }

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreateStateMaster()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStateMaster(StateMasterDTO StateMasterDTO)
        {

            APIResponse result = await _stateService.CreateStateAsync<APIResponse>(StateMasterDTO, HttpContext.Session.GetString(SD.SessionToken));

            if (result != null && result.IsSuccess)
            {
                return RedirectToAction("IndexStateMaster", "StateMaster");
            }
            return View();

        }

        public async Task<IActionResult> UpdateStateMaster(int StateId)
        {
            var StateResponse = await _stateService.GetStateAsync<APIResponse>(StateId, HttpContext.Session.GetString(SD.SessionToken));

            if (StateResponse != null && StateResponse.IsSuccess)
            {
                StateMasterDTO model = JsonConvert.DeserializeObject<StateMasterDTO>(Convert.ToString(StateResponse.Result));

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStateMaster(StateMasterDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _stateService.UpdateStateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexStateMaster));
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


        public async Task<IActionResult> RemoveStateMaster(int StateId)
        {
            var StateMasterResponse = await _stateService.GetStateAsync<APIResponse>(StateId, HttpContext.Session.GetString(SD.SessionToken));

            if (StateMasterResponse != null && StateMasterResponse.IsSuccess)
            {
                StateMasterDTO model = JsonConvert.DeserializeObject<StateMasterDTO>(Convert.ToString(StateMasterResponse.Result));

                return View(model);
            }


            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStateMaster(StateMasterDTO StateMasterDTO)
        {
            var response = await _stateService.RemoveStateAsync<APIResponse>(StateMasterDTO.StateId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexStateMaster));
            }
            return View(StateMasterDTO);
        }
    }

}



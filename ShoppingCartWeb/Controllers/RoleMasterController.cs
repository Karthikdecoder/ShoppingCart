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
    public class RoleMasterController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRegistrationService _registrationService;
        private readonly IRoleService _roleService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private string _Role;
        public RoleMasterController(IUserService userService, IHttpContextAccessor httpContextAccessor, IRoleService roleService, ICategoryService categoryService, IMapper mapper, IRegistrationService registrationService)
        {
            _userService = userService;
            _Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            _roleService = roleService;
            _categoryService = categoryService;
            _mapper = mapper;
            _registrationService = registrationService;
        }

        public async Task<IActionResult> IndexRoleMaster()
        {
            List<RoleMasterDTO> list = new();

            var response = await _roleService.GetAllRoleAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<RoleMasterDTO>>(Convert.ToString(response.Result));
            }

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreateRoleMaster()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoleMaster(RoleMasterDTO roleMasterDTO)
        {

            APIResponse result = await _roleService.CreateRoleAsync<APIResponse>(roleMasterDTO, HttpContext.Session.GetString(SD.SessionToken));

            if (result != null && result.IsSuccess)
            {
                return RedirectToAction("IndexRoleMaster", "RoleMaster");
            }
            return View();

        }

        public async Task<IActionResult> UpdateRoleMaster(int roleId)
        {
            var roleResponse = await _roleService.GetRoleAsync<APIResponse>(roleId, HttpContext.Session.GetString(SD.SessionToken));

            if (roleResponse != null && roleResponse.IsSuccess)
            {
                RoleMasterDTO model = JsonConvert.DeserializeObject<RoleMasterDTO>(Convert.ToString(roleResponse.Result));

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoleMaster(RoleMasterDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _roleService.UpdateRoleAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexRoleMaster));
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


        public async Task<IActionResult> RemoveRoleMaster(int roleId)
        {
            var roleMasterResponse = await _roleService.GetRoleAsync<APIResponse>(roleId, HttpContext.Session.GetString(SD.SessionToken));

            if (roleMasterResponse != null && roleMasterResponse.IsSuccess)
            {
                RoleMasterDTO model = JsonConvert.DeserializeObject<RoleMasterDTO>(Convert.ToString(roleMasterResponse.Result));

                return View(model);
            }


            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRoleMaster(RoleMasterDTO roleMasterDTO)
        {
            var response = await _roleService.RemoveRoleAsync<APIResponse>(roleMasterDTO.RoleId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexRoleMaster));
            }
            return View(roleMasterDTO);
        }
    }

}



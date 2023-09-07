using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRoleMaster()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoleMaster(RoleMasterDTO roleMasterDTO)
        {
            if (ModelState.IsValid)
            {
                APIResponse result = await _roleService.CreateRoleAsync<APIResponse>(roleMasterDTO, HttpContext.Session.GetString(SD.SessionToken));

                if (result != null && result.IsSuccess)
                {
                    TempData["success"] = "Created successfully";
                    return RedirectToAction("IndexRoleMaster", "RoleMaster");
                }

                TempData["error"] = result.ResponseMessage[0].ToString();
                return View(roleMasterDTO);
            }

            return View();

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoleMaster(int roleId)
        {
            if(roleId == 0)
            {
                return View();
            }

            var roleResponse = await _roleService.GetRoleAsync<APIResponse>(roleId, HttpContext.Session.GetString(SD.SessionToken));

            if (roleResponse != null && roleResponse.IsSuccess)
            {
                RoleMasterDTO model = JsonConvert.DeserializeObject<RoleMasterDTO>(Convert.ToString(roleResponse.Result));

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoleMaster(RoleMasterDTO roleMasterDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _roleService.UpdateRoleAsync<APIResponse>(roleMasterDTO, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Updated successfully";
                    return RedirectToAction(nameof(IndexRoleMaster));
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return View(roleMasterDTO);
            }
            return View(roleMasterDTO);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnableRole(int roleId)
        {
            if (ModelState.IsValid)
            {
                var response = await _roleService.EnableRoleAsync<APIResponse>(roleId, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Enabled successfully";
                    return RedirectToAction("IndexRoleMaster");
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return RedirectToAction(nameof(IndexRoleMaster));
            }

            return View();
        }

        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> RemoveRoleMaster(int roleId)
        //{
        //    var roleMasterResponse = await _roleService.GetRoleAsync<APIResponse>(roleId, HttpContext.Session.GetString(SD.SessionToken));

        //    if (roleMasterResponse != null && roleMasterResponse.IsSuccess)
        //    {
        //        RoleMasterDTO model = JsonConvert.DeserializeObject<RoleMasterDTO>(Convert.ToString(roleMasterResponse.Result));

        //        return View(model);
        //    }


        //    return NotFound();
        //}

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRoleMaster(int roleId)
        {
            var response = await _roleService.RemoveRoleAsync<APIResponse>(roleId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Deleted successfully";
                return RedirectToAction(nameof(IndexRoleMaster));
            }

            TempData["success"] = "Error encountered";
            return View();
        }
    }

}



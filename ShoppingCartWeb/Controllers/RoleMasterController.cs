using AutoMapper;
using Microsoft.AspNetCore.Authentication;
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

        [Authorize]
        public async Task<IActionResult> IndexRoleMaster(string orderBy = "", int currentPage = 1)
        {
            RoleMasterPaginationVM roleMasterPaginationVM = new();

            List<RoleMasterDTO> list = new();

            var response = await _roleService.GetAllRoleAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<RoleMasterDTO>>(Convert.ToString(response.Result));
            }

            int totalRecords = list.Count();

            int pageSize = 5;

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            roleMasterPaginationVM.RoleMasterDTO = list;
            roleMasterPaginationVM.CurrentPage = currentPage;
            roleMasterPaginationVM.PageSize = pageSize;
            roleMasterPaginationVM.TotalPages = totalPages;

            return View(roleMasterPaginationVM);
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
        public async Task<IActionResult> UpdateRoleMaster(int roleId, int currentPageNo)
        {
            if(roleId == 0)
            {
                return View();
            }
            UpdateRoleMasterVM updateRoleMasterVM = new UpdateRoleMasterVM();

            updateRoleMasterVM.CurrentPage = currentPageNo;

            var roleResponse = await _roleService.GetRoleAsync<APIResponse>(roleId, HttpContext.Session.GetString(SD.SessionToken));

            if (roleResponse != null && roleResponse.IsSuccess)
            {
                updateRoleMasterVM.RoleMasterDTO = JsonConvert.DeserializeObject<RoleMasterDTO>(Convert.ToString(roleResponse.Result));

                return View(updateRoleMasterVM);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoleMaster(UpdateRoleMasterVM updateRoleMasterVM)
        {
            if (ModelState.IsValid)
            {
                var response = await _roleService.UpdateRoleAsync<APIResponse>(updateRoleMasterVM.RoleMasterDTO, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Updated successfully";
                    return RedirectToAction("IndexRoleMaster", new { currentPage = updateRoleMasterVM.CurrentPage });
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return View(updateRoleMasterVM);
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnableRole(int roleId, int currentPageNo)
        {
            if (ModelState.IsValid)
            {
                var response = await _roleService.EnableRoleAsync<APIResponse>(roleId, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Enabled successfully";
                    return RedirectToAction("IndexRoleMaster", new { currentPage = currentPageNo });
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return RedirectToAction(nameof(IndexRoleMaster));
            }

            return View();
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRoleMaster(int roleId, int currentPageNo)
        {
            var response = await _roleService.RemoveRoleAsync<APIResponse>(roleId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Deleted successfully";
                return RedirectToAction("IndexRoleMaster", new { currentPage = currentPageNo });
            }

            TempData["success"] = "Error encountered";
            return View();
        }
    }

}



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
    public class MenuRoleMappingController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRegistrationService _registrationService;
        private readonly IRoleService _roleService;
        private readonly IMenuRoleMappingService _MenuRoleMappingService;
        private readonly IMapper _mapper;
        private string _Role;
        public MenuRoleMappingController(IUserService userService, IHttpContextAccessor httpContextAccessor, IRoleService roleService, IMenuRoleMappingService MenuRoleMappingService, IMapper mapper, IRegistrationService registrationService)
        {
            _userService = userService;
            _Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            _roleService = roleService;
            _MenuRoleMappingService = MenuRoleMappingService;
            _mapper = mapper;
            _registrationService = registrationService;
        }

        [Authorize]
        public async Task<IActionResult> IndexMenuRoleMapping(string orderBy = "", int currentPage = 1)
        {
            //MenuRoleMappingPaginationVM MenuRoleMappingPaginationVM = new();

            List<MenuRoleMappingDTO> list = new();

            var response = await _MenuRoleMappingService.GetAllMenuRoleMappingAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<MenuRoleMappingDTO>>(Convert.ToString(response.Result));
            }

            int totalRecords = list.Count();

            int pageSize = 5;

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            //list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            //MenuRoleMappingPaginationVM.MenuRoleMappingDTO = list;
            //MenuRoleMappingPaginationVM.CurrentPage = currentPage;
            //MenuRoleMappingPaginationVM.PageSize = pageSize;
            //MenuRoleMappingPaginationVM.TotalPages = totalPages;

            return View(list);
        }

        public async Task<IActionResult> ViewMenuRoleMapping(int MenuRoleMappingId)
        {
            MenuRoleMappingDTO MenuRoleMappingDetail = new();

            var MenuRoleMappingResponse = await _MenuRoleMappingService.GetMenuRoleMappingAsync<APIResponse>(MenuRoleMappingId, HttpContext.Session.GetString(SD.SessionToken));

            if (MenuRoleMappingResponse != null && MenuRoleMappingResponse.IsSuccess)
            {
                MenuRoleMappingDTO model = JsonConvert.DeserializeObject<MenuRoleMappingDTO>(Convert.ToString(MenuRoleMappingResponse.Result));
                MenuRoleMappingDetail = _mapper.Map<MenuRoleMappingDTO>(model);
            }

            return View(MenuRoleMappingDetail);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMenuRoleMapping()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuRoleMapping(MenuRoleMappingDTO MenuRoleMappingDTO)
        {
            if (ModelState.IsValid)
            {
                APIResponse response = await _MenuRoleMappingService.CreateMenuRoleMappingAsync<APIResponse>(MenuRoleMappingDTO, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Created successfully";
                    return RedirectToAction("IndexMenuRoleMapping", "MenuRoleMapping");
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return View(MenuRoleMappingDTO);
            }
            return View();

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMenuRoleMapping(int MenuRoleMappingId, int currentPageNo)
        {
            MenuRoleMappingDTO model = new();

            //model.CurrentPage = currentPageNo;

            var MenuRoleMappingResponse = await _MenuRoleMappingService.GetMenuRoleMappingAsync<APIResponse>(MenuRoleMappingId, HttpContext.Session.GetString(SD.SessionToken));

            if (MenuRoleMappingResponse != null && MenuRoleMappingResponse.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<MenuRoleMappingDTO>(Convert.ToString(MenuRoleMappingResponse.Result));

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMenuRoleMapping(MenuRoleMappingDTO MenuRoleMappingDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _MenuRoleMappingService.UpdateMenuRoleMappingAsync<APIResponse>(MenuRoleMappingDTO, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Updated successfully";
                    return RedirectToAction("IndexMenuRoleMapping");
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return View(MenuRoleMappingDTO);
            }

            return View(MenuRoleMappingDTO);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnableMenuRoleMapping(int MenuRoleMappingId, int currentPageNo)
        {
            if (ModelState.IsValid)
            {
                var response = await _MenuRoleMappingService.EnableMenuRoleMappingAsync<APIResponse>(MenuRoleMappingId, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Enabled successfully";
                    return RedirectToAction("IndexMenuRoleMapping", new { currentPage = currentPageNo });
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return RedirectToAction(nameof(IndexMenuRoleMapping));
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveMenuRoleMapping(int MenuRoleMappingId, int currentPageNo)
        {
            var response = await _MenuRoleMappingService.RemoveMenuRoleMappingAsync<APIResponse>(MenuRoleMappingId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Deleted successfully";
                return RedirectToAction("IndexMenuRoleMapping", new { currentPage = currentPageNo });
            }

            TempData["success"] = "Error encountered";
            return View();
        }
    }

}



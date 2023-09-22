using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ShoppingCartWeb.Models;
using ShoppingCartWeb.Models.Dto;
using ShoppingCartWeb.Models.VM;
using ShoppingCartWeb.Services;
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
        private readonly IMenuService _menuService;
        private readonly IMapper _mapper;
        private string _Role;
        public MenuRoleMappingController(IUserService userService, IHttpContextAccessor httpContextAccessor, IRoleService roleService, IMenuRoleMappingService MenuRoleMappingService, IMapper mapper, IRegistrationService registrationService, IMenuService menuService)
        {
            _userService = userService;
            _Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            _roleService = roleService;
            _MenuRoleMappingService = MenuRoleMappingService;
            _mapper = mapper;
            _registrationService = registrationService;
            _menuService = menuService;
        }

        [Authorize]
        public async Task<IActionResult> IndexMenuRoleMapping(string orderBy = "", int currentPage = 1)
        {
            MenuRoleMappingPaginationVM MenuRoleMappingPaginationVM = new();

            List<MenuRoleMappingDTO> list = new();

            var response = await _MenuRoleMappingService.GetAllMenuRoleMappingAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<MenuRoleMappingDTO>>(Convert.ToString(response.Result));
            }

            int totalRecords = list.Count();

            int pageSize = 5;

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            MenuRoleMappingPaginationVM.MenuRoleMappingDTO = list;
            MenuRoleMappingPaginationVM.CurrentPage = currentPage;
            MenuRoleMappingPaginationVM.PageSize = pageSize;
            MenuRoleMappingPaginationVM.TotalPages = totalPages;

            return View(MenuRoleMappingPaginationVM);
        }

        [Authorize]
        public async Task<IActionResult> MenuRoleMapping()
        {
            List<MenuRoleMappingDTO> list = new();

            var roleIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);

            int roleId = int.Parse(roleIdClaim.Value);

            var menuResponse = await _MenuRoleMappingService.GetAllMenuByRoleIdMappingAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (menuResponse != null && menuResponse.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<MenuRoleMappingDTO>>(Convert.ToString(menuResponse.Result));
            }

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
            MenuRoleMappingVM menuRoleMappingVM = new();

            var menuResponse = await _menuService.GetAllMenuAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            // Getting all the menus

            if (menuResponse != null && menuResponse.IsSuccess)
            {
                menuRoleMappingVM.MenuList = JsonConvert.DeserializeObject<List<MenuDTO>>(Convert.ToString(menuResponse.Result)).Select(i => new CustomSelectListItem
                {
                    Text = i.MenuName,
                    Value = i.MenuId.ToString(),
                    ParentId = i.ParentId.ToString(),
                    Selected = false
                });
            }

            var roleResponse = await _roleService.GetAllRoleAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (roleResponse != null && roleResponse.IsSuccess)
            {
                menuRoleMappingVM.RoleList = JsonConvert.DeserializeObject<List<RoleMasterDTO>>(Convert.ToString(roleResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.RoleName,
                    Value = i.RoleId.ToString()
                });
            }

            return View(menuRoleMappingVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuRoleMapping(MenuRoleMappingVM MenuRoleMappingVM)
        {
            if (ModelState.IsValid)
            {
                string[] selectedMenuIds = (string[])MenuRoleMappingVM.MenuRoleMapping.SelectedMenuIds;

                APIResponse response = await _MenuRoleMappingService.CreateMenuRoleMappingAsync<APIResponse>(MenuRoleMappingVM.MenuRoleMapping, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Created successfully";
                    return RedirectToAction("IndexMenuRoleMapping", "MenuRoleMapping");
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return View(MenuRoleMappingVM);
            }
            return View();

        }

        public async Task<IActionResult> GetAllMenuByRoleId(int roleId)
        {
            var menuListByRoleId = new List<MenuRoleMappingDTO>();

            var menuByRoleIdResponse = await _MenuRoleMappingService.GetMenuIdByRoleIdAsync<APIResponse>(roleId, HttpContext.Session.GetString(SD.SessionToken));

            if (menuByRoleIdResponse != null && menuByRoleIdResponse.IsSuccess)
            {
                //menuList = (List<MenuRoleMappingDTO>)JsonConvert.DeserializeObject<List<MenuRoleMappingDTO>>(Convert.ToString(menuByRoleIdResponse)).Select(i => new SelectListItem
                //{
                //    Text = i.Menu.MenuName,
                //    Value = i.MenuId.ToString()
                //});

                menuListByRoleId = JsonConvert.DeserializeObject<List<MenuRoleMappingDTO>>(Convert.ToString(menuByRoleIdResponse.Result));
            }

            ViewBag.MenuRoleMappingList = menuListByRoleId;
            ViewBag.RoleId = roleId;

            return View("CreateMenuRoleMapping", menuListByRoleId);

            //return Json(menuListByRoleId);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMenuRoleMapping(int menuRoleMappingId)
        {
            MenuRoleMappingVM menuRoleMappingVM = new();

            var menuRoleMappingResponse = await _MenuRoleMappingService.GetMenuRoleMappingAsync<APIResponse>(menuRoleMappingId, HttpContext.Session.GetString(SD.SessionToken));

            if (menuRoleMappingResponse != null && menuRoleMappingResponse.IsSuccess)
            {
                MenuRoleMappingDTO model = JsonConvert.DeserializeObject<MenuRoleMappingDTO>(Convert.ToString(menuRoleMappingResponse.Result));
                menuRoleMappingVM.MenuRoleMapping = _mapper.Map<MenuRoleMappingDTO>(model);
            }

            var menuResponse = await _menuService.GetAllMenuAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (menuResponse != null && menuResponse.IsSuccess)
            {
                menuRoleMappingVM.MenuList = JsonConvert.DeserializeObject<List<MenuDTO>>(Convert.ToString(menuResponse.Result)).Select(i => new CustomSelectListItem
                {
                    Text = i.MenuName,
                    Value = i.MenuId.ToString()
                });
            }

            var roleResponse = await _roleService.GetAllRoleAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (roleResponse != null && roleResponse.IsSuccess)
            {
                menuRoleMappingVM.RoleList = JsonConvert.DeserializeObject<List<RoleMasterDTO>>(Convert.ToString(roleResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.RoleName,
                    Value = i.RoleId.ToString()
                });
            }

            return View(menuRoleMappingVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMenuRoleMapping(MenuRoleMappingVM menuRoleMappingVM)
        {
            if (ModelState.IsValid)
            {
                var response = await _MenuRoleMappingService.UpdateMenuRoleMappingAsync<APIResponse>(menuRoleMappingVM.MenuRoleMapping, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Updated successfully";
                    return RedirectToAction("IndexMenuRoleMapping");
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return View(menuRoleMappingVM);
            }

            return View(menuRoleMappingVM);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnableMenuRoleMapping(int menuRoleMappingId)
        {
            if (ModelState.IsValid)
            {
                var response = await _MenuRoleMappingService.EnableMenuRoleMappingAsync<APIResponse>(menuRoleMappingId, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Enabled successfully";
                    return RedirectToAction("IndexMenuRoleMapping");
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return RedirectToAction(nameof(IndexMenuRoleMapping));
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveMenuRoleMapping(int menuRoleMappingId)
        {
            var response = await _MenuRoleMappingService.RemoveMenuRoleMappingAsync<APIResponse>(menuRoleMappingId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Disabled successfully";
                return RedirectToAction("IndexMenuRoleMapping");
            }

            TempData["success"] = "Error encountered";
            return View();
        }
    }

}



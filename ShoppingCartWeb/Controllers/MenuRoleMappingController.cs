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


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMenuRoleMapping(int selectedRole = 0)
        {
            MenuRoleMappingVM menuRoleMappingVM = new();

            var menuResponse = await _menuService.GetAllMenuAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (menuResponse != null && menuResponse.IsSuccess)
            {
                menuRoleMappingVM.MenuList = JsonConvert.DeserializeObject<List<MenuDTO>>(Convert.ToString(menuResponse.Result)).Select(i => new CustomSelectListItem
                {
                    Text = i.MenuName,
                    Value = i.MenuId,
                    ParentId = (int)i.ParentId,
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

            // Set the selected role in the view model

            menuRoleMappingVM.MenuRoleMapping.RoleId = selectedRole;

            //// Populate the MenuList based on the selected role
            //if (selectedRole != 0)
            //{
            //    var selectedMenuIds = await _MenuRoleMappingService.GetSelectedMenuIdsForRoleAsync<APIResponse>(selectedRole, HttpContext.Session.GetString(SD.SessionToken));

            //    List<int> menuListByRoleId = JsonConvert.DeserializeObject<List<MenuRoleMappingDTO>>(selectedMenuIds.Result.ToString()).Select(c => c.MenuId).ToList();

            //    List<string> menuListByRoleIdls = JsonConvert.DeserializeObject<List<MenuRoleMappingDTO>>(selectedMenuIds.Result.ToString()).Select(c => c.MenuId.ToString()).ToList();

            //    if (selectedMenuIds != null)
            //    {
            //        menuRoleMappingVM.MenuRoleMapping.SelectedMenuIdsDTO = menuListByRoleId;
            //        menuRoleMappingVM.SelectedMenuIds = menuListByRoleIdls;
            //    }
            //}

            return View(menuRoleMappingVM);
        }

        public async Task<IActionResult> GetMenuIds(int roleId)
        {
            var selectedMenuIds = await _MenuRoleMappingService.GetSelectedMenuIdsForRoleAsync<APIResponse>(roleId, HttpContext.Session.GetString(SD.SessionToken));

            var menuListByRoleId = JsonConvert.DeserializeObject<List<MenuRoleMappingDTO>>(selectedMenuIds.Result.ToString()).Select(c => c.MenuId).ToList();


            var menuListByRole = JsonConvert.DeserializeObject<List<MenuRoleMappingDTO>>(Convert.ToString(selectedMenuIds.Result)).Select(i => new CustomSelectListItem
            {
                MenuRoleMappingId = i.MenuRoleMappingId,
                MenuId = i.MenuId,
                RoleId = i.RoleId
            });

            var menuResponse = await _menuService.GetAllMenuAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            List<CustomSelectListItem> menuList = null;

            if (menuResponse != null && menuResponse.IsSuccess)
            {
                menuList = JsonConvert.DeserializeObject<List<MenuDTO>>(Convert.ToString(menuResponse.Result)).Select(i => new CustomSelectListItem
                {
                    Text = i.MenuName,
                    Value = i.MenuId,
                    ParentId = (int)i.ParentId,
                    Selected = menuListByRoleId.Contains(i.MenuId) // Set the "Selected" property based on menuListByRoleId
                }).ToList();
            }

            return Json(menuListByRoleId);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuRoleMapping(MenuRoleMappingVM menuRoleMappingVM)
        {
            APIResponse response = await _MenuRoleMappingService.UpdateMenuRoleMappingAsync<APIResponse>(menuRoleMappingVM.MenuRoleMapping, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("CreateMenuRoleMapping");
            }

            // Handle the form submission and any other logic here

            // Redirect back to the GET action to refresh the page with the selected role

            return RedirectToAction("CreateMenuRoleMapping");
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
                    Value = i.MenuId
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


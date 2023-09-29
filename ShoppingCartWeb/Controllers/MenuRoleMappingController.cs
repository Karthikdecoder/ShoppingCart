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
        public async Task<IActionResult> CreateMenuRoleMapping()
        {

            MenuRoleMappingVM menuRoleMappingVM = new();

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

      
        [HttpGet]
        public async Task<IActionResult> GetMenusByRole(int roleId)
        {
            var menuByRoleId = await _MenuRoleMappingService.GetSelectedMenuIdsForRoleAsync<APIResponse>(roleId, HttpContext.Session.GetString(SD.SessionToken));

            List<MenuDTO> menuByRoleIdList = JsonConvert.DeserializeObject<List<MenuDTO>>(Convert.ToString(menuByRoleId.Result));

            MenuRoleMappingVM menuRoleMappingVM = new();

            var menuList = await _menuService.GetAllMenuAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (menuList != null)
            {
                menuRoleMappingVM.MenuList = JsonConvert.DeserializeObject<List<MenuDTO>>(Convert.ToString(menuList.Result)).Select(i => new CustomSelectListItem
                {
                    Text = i.MenuName,
                    Value = i.MenuId.ToString(),
                    Selected = false,
                    ParentId = i.ParentId ?? 0,
                    Description = i.Description

                }).ToList();

                foreach (var fromMenuList in menuRoleMappingVM.MenuList)
                {
                    foreach (var fromMenuByRoleList in menuByRoleIdList)
                    {
                        if (fromMenuByRoleList.MenuId.ToString() == fromMenuList.Value)
                        {
                            fromMenuList.Selected = true;
                        }
                    }
                }
            }

            List<CustomSelectListItem> roleMenuMappingList = new();

            foreach (var parentMenu in menuRoleMappingVM.MenuList)
            {
                if (parentMenu.ParentId == 0)
                {
                    roleMenuMappingList.Add(parentMenu);
                }
            }

            foreach (var parentMenu in menuRoleMappingVM.MenuList)
            {
                if (parentMenu.ParentId == 0)
                {
                    foreach (var childMenu in menuRoleMappingVM.MenuList)
                    {
                        if (childMenu.ParentId.ToString() == parentMenu.Value)
                        {
                            roleMenuMappingList.Add(childMenu);
                        }
                    }
                }
            }

            return Json(roleMenuMappingList);
        }

       
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuRoleMapping(MenuRoleMappingVM menuRoleMappingVM, string selectedMenuIdsList)
        {
            try
            {
                List<MenuRoleMappingDTO> menuRoleMappingList = new();

                List<CustomSelectListItem> selectedMenuList = JsonConvert.DeserializeObject<List<CustomSelectListItem>>(selectedMenuIdsList);

                var response = await _MenuRoleMappingService.GetAllMenuRoleMappingAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    menuRoleMappingList = JsonConvert.DeserializeObject<List<MenuRoleMappingDTO>>(Convert.ToString(response.Result));

                }
                if (ModelState.IsValid)
                {
                    foreach (var selectedItem in selectedMenuList)
                    {

                        var matchingItem = menuRoleMappingList.FirstOrDefault(item =>
                            item.MenuId.ToString() == selectedItem.Value &&
                            item.RoleId == menuRoleMappingVM.MenuRoleMapping.RoleId
                        );

                        if (matchingItem != null)
                        {
                            if (selectedItem.Selected && matchingItem.IsDeleted == true)
                            {
                                var enableMapping = await _MenuRoleMappingService.EnableMenuRoleMappingAsync<APIResponse>(matchingItem.MenuRoleMappingId, HttpContext.Session.GetString(SD.SessionToken));
                            }
                            else if (selectedItem.Selected == false && matchingItem.IsDeleted == false)
                            {
                                var disableMapping = await _MenuRoleMappingService.RemoveMenuRoleMappingAsync<APIResponse>(matchingItem.MenuRoleMappingId, HttpContext.Session.GetString(SD.SessionToken));
                            }
                        }
                        else
                        {
                            if (selectedItem.Selected)
                            {
                                menuRoleMappingVM.MenuRoleMapping.MenuId = int.Parse(selectedItem.Value);

                                APIResponse createMapping = await _MenuRoleMappingService.CreateMenuRoleMappingAsync<APIResponse>(menuRoleMappingVM.MenuRoleMapping, HttpContext.Session.GetString(SD.SessionToken));
                            }
                        }
                    }

                    TempData["success"] = "Saved successfully";
                    return RedirectToAction(nameof(CreateMenuRoleMapping));

                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            TempData["error"] = "Error encountered";

            return View(menuRoleMappingVM);
        }

    }

}
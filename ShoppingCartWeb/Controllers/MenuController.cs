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
    public class MenuController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRegistrationService _registrationService;
        private readonly IRoleService _roleService;
        private readonly IMenuService _menuService;
        private readonly IMapper _mapper;
        private string _Role;
        public MenuController(IUserService userService, IHttpContextAccessor httpContextAccessor, IRoleService roleService, IMenuService menuService, IMapper mapper, IRegistrationService registrationService)
        {
            _userService = userService;
            _Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            _roleService = roleService;
            _menuService = menuService;
            _mapper = mapper;
            _registrationService = registrationService;
        }

        [Authorize]
        public async Task<IActionResult> IndexMenu(string orderBy = "", int currentPage = 1)
        {
            MenuPaginationVM MenuPaginationVM = new();

            List<MenuDTO> list = new();

            var response = await _menuService.GetAllMenuAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<MenuDTO>>(Convert.ToString(response.Result));
            }

            int totalRecords = list.Count();

            int pageSize = 5;

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            MenuPaginationVM.MenuDTO = list;
            MenuPaginationVM.CurrentPage = currentPage;
            MenuPaginationVM.PageSize = pageSize;
            MenuPaginationVM.TotalPages = totalPages;

            return View(MenuPaginationVM);
        }

        public async Task<IActionResult> ViewMenu(int MenuId)
        {
            MenuDTO MenuDetail = new();

            var MenuResponse = await _menuService.GetMenuAsync<APIResponse>(MenuId, HttpContext.Session.GetString(SD.SessionToken));

            if (MenuResponse != null && MenuResponse.IsSuccess)
            {
                MenuDTO model = JsonConvert.DeserializeObject<MenuDTO>(Convert.ToString(MenuResponse.Result));
                MenuDetail = _mapper.Map<MenuDTO>(model);
            }

            return View(MenuDetail);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CreateMenu()
        {
            MenuVM MenuVM = new MenuVM();

            var parentIdResponse = await _menuService.GetParentIdAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (parentIdResponse != null && parentIdResponse.IsSuccess)
            {
                MenuVM.ParentList = JsonConvert.DeserializeObject<List<MenuDTO>>(Convert.ToString(parentIdResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.MenuName,
                    Value = i.MenuId.ToString()
                });

                return View(MenuVM);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenu(MenuVM MenuVM)
        {
            if (ModelState.IsValid)
            {
                if (MenuVM.Menu.ParentId == null)
                {
                    MenuVM.Menu.ParentId = 0;
                }

                APIResponse response = await _menuService.CreateMenuAsync<APIResponse>(MenuVM.Menu, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Created successfully";
                    return RedirectToAction("IndexMenu", "Menu");
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return View(MenuVM);
            }
            return View();

        }

        [Authorize]
        public async Task<IActionResult> UpdateMenu(int MenuId, int currentPageNo)
        {
            MenuVM MenuVM = new MenuVM();

            //model.CurrentPage = currentPageNo;

            var MenuResponse = await _menuService.GetMenuAsync<APIResponse>(MenuId, HttpContext.Session.GetString(SD.SessionToken));

            if (MenuResponse != null && MenuResponse.IsSuccess)
            {
                MenuDTO model = JsonConvert.DeserializeObject<MenuDTO>(Convert.ToString(MenuResponse.Result));
                MenuVM.Menu = _mapper.Map<MenuDTO>(model);
            }

            var parentIdResponse = await _menuService.GetParentIdAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (parentIdResponse != null && parentIdResponse.IsSuccess)
            {
                MenuVM.ParentList = JsonConvert.DeserializeObject<List<MenuDTO>>(Convert.ToString(parentIdResponse.Result)).Select(i => new SelectListItem
                {
                    Text = i.MenuName,
                    Value = i.MenuId.ToString()

                });

                return View(MenuVM);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMenu(MenuVM MenuVM)
        {
            if (ModelState.IsValid)
            {
                if (MenuVM.Menu.ParentId == null)
                {
                    MenuVM.Menu.ParentId = 0;
                }

                var response = await _menuService.UpdateMenuAsync<APIResponse>(MenuVM.Menu, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Updated successfully";
                    return RedirectToAction("IndexMenu");
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return View(MenuVM);
            }

            return View(MenuVM);
        }

        [Authorize]
        public async Task<IActionResult> EnableMenu(int MenuId, int currentPageNo)
        {
            if (ModelState.IsValid)
            {
                var response = await _menuService.EnableMenuAsync<APIResponse>(MenuId, HttpContext.Session.GetString(SD.SessionToken));

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Enabled successfully";
                    return RedirectToAction("IndexMenu", new { currentPage = currentPageNo });
                }

                TempData["error"] = response.ResponseMessage[0].ToString();
                return RedirectToAction(nameof(IndexMenu));
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> RemoveMenu(int MenuId, int currentPageNo)
        {
            var response = await _menuService.RemoveMenuAsync<APIResponse>(MenuId, HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Deleted successfully";
                return RedirectToAction("IndexMenu", new { currentPage = currentPageNo });
            }

            TempData["success"] = "Error encountered";
            return View();
        }
    }

}


